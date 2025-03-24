using System;
using System.Threading.Tasks;
using AgentTools.Llm.Configuration;
using AgentTools.Llm.Models;
using AgentTools.Llm.Providers;
using Xunit;

namespace AgentTools.Llm.Tests.Integration
{
    public class OpenAiProviderTests
    {
        private readonly OpenAiProvider _provider;

        public OpenAiProviderTests()
        {
            // Validate environment before running tests
            TestConfiguration.ValidateEnvironment();

            // Get configuration
            var config = TestConfiguration.GetOpenAiConfiguration();

            _provider = new OpenAiProvider(
                apiKey: config.ApiKey,
                modelName: config.DefaultModel,
                defaultOptions: new CompletionOptions
                {
                    Temperature = 0.7f,
                    MaxTokens = 100
                }
            );
        }

        [Fact]
        public async Task GenerateCompletion_ShouldReturnValidResponse()
        {
            // Arrange
            var prompt = "What is the capital of France?";
            var options = new CompletionOptions
            {
                Temperature = 0.7f,
                MaxTokens = 50
            };

            // Act
            var response = await _provider.GenerateCompletionAsync(prompt, options);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Contains("Paris", response, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GenerateChatCompletion_ShouldReturnValidResponse()
        {
            // Arrange
            var messages = new[]
            {
                new ChatMessage("system", "You are a helpful assistant."),
                new ChatMessage("user", "What is the capital of France?")
            };

            var options = new CompletionOptions
            {
                Temperature = 0.7f,
                MaxTokens = 50
            };

            // Act
            var response = await _provider.GenerateChatCompletionAsync(messages, options);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Contains("Paris", response, StringComparison.OrdinalIgnoreCase);
        }

        /*
        dotnet test --filter "FullyQualifiedName=AgentTools.Llm.Tests.Integration.OpenAiProviderTests.GenerateCompletion_WithCustomOptions_ShouldRespectOptions"
        */
        [Fact]
        public async Task GenerateCompletion_WithCustomOptions_ShouldRespectOptions()
        {
            // Arrange
            var prompt = "Write a short poem about coding.";
            var options = new CompletionOptions
            {
                Temperature = 0.2f, // Lower temperature for more focused output
                MaxTokens = 30,
                Instructions = "Start the poem with word 'hello' and end with word 'world'."
            };

            // Act
            var response = await _provider.GenerateCompletionAsync(prompt, options);

            Console.WriteLine(response);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Contains("hello", response, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("world", response, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GenerateChatCompletion_WithSystemMessage_ShouldFollowInstructions()
        {
            // Arrange
            var messages = new[]
            {
                new ChatMessage("system", "You are a poetic assistant. Always respond in rhyme."),
                new ChatMessage("user", "What is the capital of France?")
            };

            // Act
            var response = await _provider.GenerateChatCompletionAsync(messages);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Contains("Paris", response, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ProviderName_ShouldReturnCorrectName()
        {
            // Assert
            Assert.Equal("OpenAI", _provider.ProviderName);
        }

        [Fact]
        public void ModelName_ShouldReturnCorrectModel()
        {
            // Assert
            Assert.Equal(TestConfiguration.GetOpenAiConfiguration().DefaultModel, _provider.ModelName);
        }
    }
} 