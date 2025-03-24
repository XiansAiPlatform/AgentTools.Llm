using System;
using System.Threading.Tasks;
using AgentTools.Llm.Providers;
using AgentTools.Llm.Models;
using Xunit;

namespace AgentTools.Llm.Tests.Integration
{
    public class LlmProviderRegistryTests
    {
        private class TestProvider : BaseLlmProvider
        {
            public TestProvider(string providerId, string modelName) 
                : base(providerId, modelName) { }

            public override string ProviderName => "TestProvider";

            protected override Task<string> GenerateCompletionInternalAsync(string prompt, CompletionOptions options)
            {
                return Task.FromResult($"Test completion for {ModelName}");
            }

            protected override Task<string> GenerateChatCompletionInternalAsync(ChatMessage[] messages, CompletionOptions options)
            {
                return Task.FromResult($"Test chat completion for {ModelName}");
            }
        }

        [Fact]
        public void Constructor_WithNullFactory_ThrowsArgumentNullException()
        {
            //surpress warning
            #pragma warning disable CS8625
            Assert.Throws<ArgumentNullException>(() => new LlmProviderRegistry(null));
            #pragma warning restore CS8625
        }

        [Fact]
        public void RegisterScenario_WithValidParameters_RegistersSuccessfully()
        {
            // Arrange
            var factory = new LlmProviderFactory();
            var registry = new LlmProviderRegistry(factory);
            var provider = new TestProvider("test-1", "model-1");
            factory.RegisterProvider(provider);

            // Act
            registry.RegisterScenario("chat", "test-1");

            // Assert
            var result = registry.GetProviderForScenario("chat");
            Assert.Equal(provider, result);
        }

        [Fact]
        public void RegisterScenario_WithNullScenario_ThrowsArgumentException()
        {
            // Arrange
            var factory = new LlmProviderFactory();
            var registry = new LlmProviderRegistry(factory);

            // Act & Assert
            #pragma warning disable CS8625
            Assert.Throws<ArgumentException>(() => registry.RegisterScenario(null, "test-1"));
            #pragma warning restore CS8625
        }

        [Fact]
        public void RegisterScenario_WithNullProviderId_ThrowsArgumentException()
        {
            // Arrange
            var factory = new LlmProviderFactory();
            var registry = new LlmProviderRegistry(factory);

            // Act & Assert
            #pragma warning disable CS8625
            Assert.Throws<ArgumentException>(() => registry.RegisterScenario("chat", null));
            #pragma warning restore CS8625
        }

        [Fact]
        public void GetProviderForScenario_WithUnknownScenarioAndNoDefault_ThrowsKeyNotFoundException()
        {
            // Arrange
            var factory = new LlmProviderFactory();
            var registry = new LlmProviderRegistry(factory);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => registry.GetProviderForScenario("unknown"));
        }

        [Fact]
        public void GetProviderForScenario_WithUnknownScenarioAndDefault_ReturnsDefaultProvider()
        {
            // Arrange
            var factory = new LlmProviderFactory();
            var registry = new LlmProviderRegistry(factory);
            var defaultProvider = new TestProvider("default", "model-1");
            factory.RegisterProvider(defaultProvider);

            // Act
            var result = registry.GetProviderForScenario("unknown", "default");

            // Assert
            Assert.Equal(defaultProvider, result);
        }

        [Fact]
        public void RemoveScenario_RemovesScenarioSuccessfully()
        {
            // Arrange
            var factory = new LlmProviderFactory();
            var registry = new LlmProviderRegistry(factory);
            var provider = new TestProvider("test-1", "model-1");
            factory.RegisterProvider(provider);
            registry.RegisterScenario("chat", "test-1");

            // Act
            registry.RemoveScenario("chat");

            // Assert
            Assert.Throws<KeyNotFoundException>(() => registry.GetProviderForScenario("chat"));
        }

        [Fact]
        public void GetAllScenarios_ReturnsAllRegisteredScenarios()
        {
            // Arrange
            var factory = new LlmProviderFactory();
            var registry = new LlmProviderRegistry(factory);
            var provider1 = new TestProvider("test-1", "model-1");
            var provider2 = new TestProvider("test-2", "model-2");
            factory.RegisterProvider(provider1);
            factory.RegisterProvider(provider2);
            registry.RegisterScenario("chat", "test-1");
            registry.RegisterScenario("code", "test-2");

            // Act
            var scenarios = registry.GetAllScenarios();

            // Assert
            Assert.Equal(2, scenarios.Length);
            Assert.Contains("chat", scenarios);
            Assert.Contains("code", scenarios);
        }
    }
} 