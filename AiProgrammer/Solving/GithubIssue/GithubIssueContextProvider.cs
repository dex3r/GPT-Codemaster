using AiProgrammer.Solving.Model;

namespace AiProgrammer.Solving.GithubIssue;

public class GithubIssueContextProvider : IGithubIssueContextProvider, IDisposable
{
    private GithubIssueContext? _context;
    private bool _isDisposed;
    
    public GithubIssueContext Get()
    {
        if (_isDisposed)
        {
            throw new Exception("Tried to get Github Issue context after the scope has been disposed");
        }
        
        if (_context == null)
        {
            throw new Exception("Github Issue Context is not set. Are you trying to access functionality exclusive to Github Issue " +
                                "from a context without Github Issue?");
        }

        return _context;
    }

    public void Set(GithubIssueContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public void Dispose()
    {
        _context = null;
        _isDisposed = true;
    }
}
