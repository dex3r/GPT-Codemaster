# GPT-Codemaster

Automatic programming by creating Pull Requests from Issues using LLMs.

An experimental project to automate programming. It uses a task-based approach to move a software project development forward by reading task descriptions from Github Issues and implementing them in a Pull Request.

Example Pull Requests created with this tool are available [here](https://github.com/dex3r/GPT-Codemaster/pulls?q=is%3Apr+label%3A%22GPT-Codemaster+example%22+)

## Features
 - [x] Modifying files 
 - [x] Creating new files
 - [x] Automatic reacting to Issues with the specified label
 - [x] Creating Pull Requests
 - [ ] Released as Github Action in the marketplace
 - [ ] Automatic creation of tests for it's implementation
 - [ ] Reacting to Pull Requests checks
 - [ ] Back-and-forth conversation and reacting to human feedback in a Pull Request
 - [ ] Implement Reflection (https://arxiv.org/pdf/2303.11366.pdf and https://nanothoughts.substack.com/p/reflecting-on-reflexion)
 - [ ] Being able to go though massive projects
 - [ ] Read .ai/project_description_short.json of every project and include it in the prompt

## Tests
 - [x] Comlpete a simple Issue, [example](https://github.com/dex3r/GPT-Codemaster/pull/2)
 - [ ] Complete a more complex issue

## FAQ 

### Does it work?
Sometimes, and only for small projects with small files, like this one. The biggest limitation right now is the token length for LLMs.

### Does it _really_ work?
It's an experiment, not an actual product.

### What do I need to use it?
1. GPT-4 API access and token
1. Github repository
1. This project as a GitHub Action
1. Enable "Allow Github Actions to create pull requests" in settings
