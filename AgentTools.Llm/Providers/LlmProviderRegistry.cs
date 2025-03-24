using System;
using AgentTools.Llm.Interfaces;

namespace AgentTools.Llm.Providers
{
    /// <summary>
    /// A registry for managing different LLM providers based on scenarios
    /// </summary>
    public class LlmProviderRegistry
    {
        private readonly LlmProviderFactory _factory;
        private readonly Dictionary<string, string> _scenarioMappings;

        public LlmProviderRegistry(LlmProviderFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _scenarioMappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Registers a mapping between a scenario and a provider ID
        /// </summary>
        /// <param name="scenario">The scenario name (e.g., "chat", "code", "analysis")</param>
        /// <param name="providerId">The ID of the provider to use for this scenario</param>
        public void RegisterScenario(string scenario, string providerId)
        {
            if (string.IsNullOrEmpty(scenario))
                throw new ArgumentException("Scenario cannot be null or empty", nameof(scenario));
            if (string.IsNullOrEmpty(providerId))
                throw new ArgumentException("ProviderId cannot be null or empty", nameof(providerId));

            _scenarioMappings[scenario] = providerId;
        }

        /// <summary>
        /// Gets the appropriate provider for a given scenario
        /// </summary>
        /// <param name="scenario">The scenario to get a provider for</param>
        /// <param name="defaultProviderId">Optional default provider ID if scenario is not found</param>
        /// <returns>The provider instance for the scenario</returns>
        /// <exception cref="KeyNotFoundException">Thrown when scenario is not found and no default is provided</exception>
        public ILlmProvider GetProviderForScenario(string scenario, string? defaultProviderId = null)
        {
            if (string.IsNullOrEmpty(scenario))
                throw new ArgumentException("Scenario cannot be null or empty", nameof(scenario));

            if (_scenarioMappings.TryGetValue(scenario, out var providerId))
                return _factory.GetProvider(providerId);

            if (!string.IsNullOrEmpty(defaultProviderId))
                return _factory.GetProvider(defaultProviderId);

            throw new KeyNotFoundException($"No provider mapping found for scenario '{scenario}' and no default provider specified");
        }

        /// <summary>
        /// Removes a scenario mapping
        /// </summary>
        /// <param name="scenario">The scenario to remove</param>
        public void RemoveScenario(string scenario)
        {
            _scenarioMappings.Remove(scenario);
        }

        /// <summary>
        /// Gets all registered scenarios
        /// </summary>
        /// <returns>An array of all registered scenario names</returns>
        public string[] GetAllScenarios()
        {
            return _scenarioMappings.Keys.ToArray();
        }
    }
} 