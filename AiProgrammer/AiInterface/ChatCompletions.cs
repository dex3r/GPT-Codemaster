using System.Diagnostics;
using System.Net;
using AiProgrammer.Logging;
using AiProgrammer.Solving;
using OpenAI_API;
using OpenAI_API.Chat;

namespace AiProgrammer.AiInterface;

public class ChatCompletions : ICompletions, IChat
{
    private readonly ISolverProgressLogger? _solverProgressLogger;

    public ChatCompletions(ISolverProgressLogger solverProgressLogger)
    {
        _solverProgressLogger = solverProgressLogger;
    }
    
    public async Task<string> GetCompletion(string systemMessage, string messageToComplete)
    {
        return await GetResponseForChat(systemMessage, messageToComplete);
    }
    
    public async Task<string> GetResponseForChat(string systemMessage, string chatMessage)
    {
        return await GetResponseFromChatbot(systemMessage, chatMessage);
    }

    private async Task<string> GetResponseFromChatbot(string systemMessage, string userMessage)
    {
        Exception? lastException = null;

        for (int i = 0; i < 10; i++)
        {
            try
            {
                Console.WriteLine($"Waiting for Chat API response, try #{i+1}...");
                Stopwatch sw = Stopwatch.StartNew();

                string result = await GetResponseFromChatbotUnsafe(systemMessage, userMessage);

                sw.Stop();
                Console.WriteLine($"Got Chat API response in {sw.Elapsed.TotalSeconds:0.0} seconds");
                return result;
            }
            catch (TimeoutException timeoutException)
            {
                lastException = timeoutException;
                // Continue
            }
            catch (HttpRequestException httpRequestException)
            {
                if (httpRequestException.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    TimeSpan tooManyRequestsDelay = TimeSpan.FromSeconds(50);
                    Console.WriteLine($"Got 'TooManyRequests' response, waiting {tooManyRequestsDelay.TotalSeconds:0.} seconds before retry...");
                    await Task.Delay(tooManyRequestsDelay);
                    continue;
                }

                throw;
            }
            catch (TaskCanceledException taskCanceledException)
            {
                if (taskCanceledException.InnerException is TimeoutException)
                {
                    lastException = taskCanceledException;
                    continue;
                }

                throw;
            }

            await Task.Delay(TimeSpan.FromSeconds(10));
        }

        throw lastException ?? new Exception("Something went really wrong while trying to send Chat request");
    }
    
    private async Task<string> GetResponseFromChatbotUnsafe(string systemMessage, string userMessage)
    {
        OpenAIAPI api = new OpenAIAPI(APIAuthentication.LoadFromEnv(), CreateHttpClient);
        
        Conversation chat = api.Chat.CreateConversation();

        chat.RequestParameters.TopP = 1;
        chat.RequestParameters.Temperature = 0;
        chat.RequestParameters.MaxTokens = 4096; //TODO: Change for bigger models, like 32k model.
        chat.RequestParameters.Model = "gpt-4";

        chat.AppendSystemMessage(systemMessage);
        chat.AppendUserInput(userMessage);

        await (_solverProgressLogger?.LogChatRequest(chat) ?? Task.CompletedTask);

        string response = await chat.GetResponseFromChatbot();
        await (_solverProgressLogger?.LogChatResponse(response) ?? Task.CompletedTask);

        return response;
    }

    private HttpClient CreateHttpClient()
    {
        return new HttpClient
        {
            Timeout = TimeSpan.FromMinutes(10)
        };
    }
}
