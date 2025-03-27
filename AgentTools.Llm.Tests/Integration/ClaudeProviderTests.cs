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
    }
}