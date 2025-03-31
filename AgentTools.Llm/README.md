# AgentTools.Llm

A flexible .NET library for integrating with various Large Language Model (LLM) providers. This library provides a unified interface for working with different LLM providers while maintaining the ability to use provider-specific features.

## Features

- Unified interface for different LLM providers
- Support for both text completion and chat completion
- Configurable completion options
- Easy provider registration and management
- Extensible architecture for adding new providers

## Installation

Add the package to your project using NuGet:

```bash
dotnet add package AgentTools.Llm
```

## Usage

### Basic Usage

```csharp
using AgentTools.Llm.Providers;
using AgentTools.Llm.Models;

// Create a provider factory
var factory = new LlmProviderFactory();

// Create and register OpenAI providers with different models
var gpt35Provider = new OpenAiProvider(
    apiKey: "your-api-key-here",
    modelName: "gpt-3.5-turbo",
    providerId: "gpt-35" // Unique identifier for this provider instance
);
var gpt4Provider = new OpenAiProvider(
    apiKey: "your-api-key-here",
    modelName: "gpt-4",
    providerId: "gpt-4"
);
factory.RegisterProvider(gpt35Provider);
factory.RegisterProvider(gpt4Provider);

// Get the specific provider instance you want to use
var provider = factory.GetProvider("gpt-35"); // or "gpt-4"

// Generate a completion
var response = await provider.GenerateCompletionAsync(
    "What is the capital of France?",
    new CompletionOptions { Temperature = 0.7f }
);
```

### Using Different Models for Different Scenarios

```csharp
// Create a provider registry to manage different models
public class LlmProviderRegistry
{
    private readonly LlmProviderFactory _factory;
    
    public LlmProviderRegistry(LlmProviderFactory factory)
    {
        _factory = factory;
    }
    
    public ILlmProvider GetProviderForScenario(string scenario)
    {
        return scenario switch
        {
            "chat" => _factory.GetProvider("gpt-35"),
            "code" => _factory.GetProvider("gpt-4"),
            "analysis" => _factory.GetProvider("gpt-4"),
            _ => _factory.GetProvider("gpt-35") // Default fallback
        };
    }
}

// Usage
var registry = new LlmProviderRegistry(factory);
var chatProvider = registry.GetProviderForScenario("chat");
var codeProvider = registry.GetProviderForScenario("code");
```

### Chat Completion

```csharp
var messages = new[]
{
    new ChatMessage("system", "You are a helpful assistant."),
    new ChatMessage("user", "What is the capital of France?")
};

var response = await provider.GenerateChatCompletionAsync(messages);
```

### Using Custom Options

```csharp
var options = new CompletionOptions
{
    Temperature = 0.2f,
    MaxTokens = 100,
    TopP = 0.9f,
    FrequencyPenalty = 0.5f,
    PresencePenalty = 0.5f,
    StopSequences = new[] { "\n" }
};

var response = await provider.GenerateCompletionAsync("Write a short poem.", options);
```

These parameters control how a Large Language Model (LLM) generates text. Here’s what each one does:

Temperature = 0.2f
- Controls randomness in the output.
- A lower value (close to 0) makes the model more deterministic, meaning it picks the most probable words.
- A higher value (closer to 1) makes responses more diverse and creative.
- 0.2 means the model will generate more predictable and focused outputs.

MaxTokens = 100
- Sets the maximum number of tokens (words, punctuation, or parts of words) the model can generate in a response.
- 100 tokens means the response length will be limited to around 100 words or shorter.

TopP = 0.9f (Nucleus Sampling)
- Controls diversity by sampling from the top p probability mass instead of selecting from all possible words.
- When TopP = 0.9, the model considers only the most probable 90% of words and ignores the least likely 10%.
- This balances diversity and coherence.
  
FrequencyPenalty = 0.5f
- Reduces the likelihood of the model repeating words.
- A higher value discourages repetition.
- 0.5 applies a moderate penalty, making outputs less redundant.

PresencePenalty = 0.5f
- Encourages the model to introduce new words/concepts.
- A higher value makes it more likely to explore new topics.
- 0.5 gives a slight push for diverse wording.

StopSequences = new[] { “\n” }
- Defines a sequence that stops the response generation.
- ”\n” (newline) means the model will stop generating when it reaches a line break.
- Useful for ensuring short, structured responses.
 
## Configuration

### OpenAI Configuration

```csharp
var config = new OpenAiConfiguration(
    apiKey: "your-api-key-here",
    defaultModel: "gpt-3.5-turbo",
    organizationId: "optional-org-id"
);
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.
