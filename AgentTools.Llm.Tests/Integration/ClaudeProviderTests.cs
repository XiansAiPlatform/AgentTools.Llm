using System;
using System.Threading.Tasks;
using AgentTools.Llm.Configuration;
using AgentTools.Llm.Models;
using AgentTools.Llm.Providers;
using Xunit;

namespace AgentTools.Llm.Tests.Integration
{
    public class ClaudeProviderTests
    {
        private readonly ClaudeProvider _provider;

        public ClaudeProviderTests()
        {
            // Validate environment before running tests
            TestConfiguration.ValidateEnvironment();

            // Get configuration
            var config = TestConfiguration.GetClaudeConfiguration();

            _provider = new ClaudeProvider(
                providerId: "claude-3",
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

            // Additional Assertions
            // Ensure the system message is handled correctly
            Assert.DoesNotContain("system", response, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GenerateChatCompletion_WithInstructionsAsSystemMessage_ShouldAffectResponse()
        {
            // Arrange
            var prompt = "Write a short poem about coding.";
            var options = new CompletionOptions
            {
                Temperature = 0.2f,
                MaxTokens = 100,
                Instructions = "The poems that you write should always start with the word 'hello' and end with the word 'world'. Never skip this rule."
            };

            var messages = new[]
            {
                new ChatMessage("system", options.Instructions!),
                new ChatMessage("user", prompt)
            };

            // Act
            var response = await _provider.GenerateChatCompletionAsync(messages, options);

            Console.WriteLine("Claude Response:\n" + response);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Contains("hello", response, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("world", response, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GenerateChatCompletion_WithBehavioralSystemMessage_ShouldGuideToneOrStyle()
        {
            // Arrange
            var messages = new[]
            {
                new ChatMessage("system", "You are a poetic assistant. Always respond in rhyme."),
                new ChatMessage("user", "Describe the ocean.")
            };

            // Act
            var response = await _provider.GenerateChatCompletionAsync(messages);

            Console.WriteLine("Claude Response:\n" + response);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Contains("ocean", response, StringComparison.OrdinalIgnoreCase);
            // Optionally: test for rhyming pattern using regex or token count
        }

        [Fact]
        public void ProviderName_ShouldReturnCorrectName()
        {
            // Assert
            Assert.Equal("Anthropic-Claude", _provider.ProviderName);
        }

        [Fact]
        public void ModelName_ShouldReturnCorrectModel()
        {
            // Assert
            var expectedModel = TestConfiguration.GetClaudeConfiguration().DefaultModel;
            Assert.Equal(expectedModel, _provider.ModelName);
        }

        [Fact]
        public void ProviderId_ShouldReturnCorrectId()
        {
            // Assert
            Assert.Equal("claude-3", _provider.ProviderId);
        }
    }
}