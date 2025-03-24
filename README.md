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

// Create and register an OpenAI provider
var openAiProvider = new OpenAiProvider(
    apiKey: "your-api-key-here",
    modelName: "gpt-3.5-turbo"
);
factory.RegisterProvider(openAiProvider);

// Get the provider
var provider = factory.GetProvider("OpenAI");

// Generate a completion
var response = await provider.GenerateCompletionAsync(
    "What is the capital of France?",
    new CompletionOptions { Temperature = 0.7f }
);
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

## Configuration

### OpenAI Configuration

```csharp
var config = new OpenAiConfiguration(
    apiKey: "your-api-key-here",
    defaultModel: "gpt-3.5-turbo",
    organizationId: "optional-org-id"
);
```

## Running Tests

### Prerequisites

1. Make sure you have .NET SDK installed
2. Create a `.env` file in the `AgentTools.Llm.Tests` directory with the following variables:

```env
# Required
OPENAI_API_KEY=your-api-key-here

# Optional
OPENAI_TEST_MODEL=gpt-3.5-turbo  # Defaults to "gpt-3.5-turbo"
OPENAI_ORG_ID=your-org-id
```

> **Note**: Make sure to add `.env` to your `.gitignore` file to prevent committing sensitive information.

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test AgentTools.Llm.Tests

# Run tests with specific filter
dotnet test --filter "FullyQualifiedName~OpenAiProviderTests"
```

## Adding New Providers

To add a new LLM provider:

1. Create a new class that inherits from `BaseLlmProvider`
2. Implement the required methods:
   - `ProviderName`
   - `GenerateCompletionInternalAsync`
   - `GenerateChatCompletionInternalAsync`
3. Register the provider with the factory

Example:

```csharp
public class CustomProvider : BaseLlmProvider
{
    public CustomProvider(string modelName) : base(modelName) { }
    
    public override string ProviderName => "CustomProvider";
    
    protected override async Task<string> GenerateCompletionInternalAsync(
        string prompt, 
        CompletionOptions options)
    {
        // Implement provider-specific logic
    }
    
    protected override async Task<string> GenerateChatCompletionInternalAsync(
        ChatMessage[] messages, 
        CompletionOptions options)
    {
        // Implement provider-specific logic
    }
}
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
