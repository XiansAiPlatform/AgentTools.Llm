# AgentTools.Llm

A flexible .NET library for integrating with various Large Language Model (LLM) providers. This library provides a unified interface for working with different LLM providers while maintaining the ability to use provider-specific features.

## Library Usage

[Usage Documentation](./AgentTools.Llm/README.md)

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

## Publishing to NuGet

To publish a new version of the package to NuGet:

1. Get a NuGet API key from [NuGet.org](https://www.nuget.org/account/apikeys)
2. Set your API key as an environment variable:

   ```bash
   export NUGET_API_KEY='your-api-key'
   ```

3. Update the version in `AgentTools.Llm.csproj` to match the version you want to publish
4. Run the publish script with the version number:

   ```bash
   ./publish.sh 1.0.0
   ```

The script will:

- Build the solution
- Run tests
- Create the NuGet package
- Publish it to NuGet.org

> **Note**: Make sure to update the version number in both `AgentTools.Llm.csproj` and the publish command to match.

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
