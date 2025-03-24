using System;
using System.Collections.Generic;
using AgentTools.Llm.Interfaces;

namespace AgentTools.Llm.Providers
{
    public class LlmProviderFactory
    {
        private readonly Dictionary<string, ILlmProvider> _providers;

        public LlmProviderFactory()
        {
            _providers = new Dictionary<string, ILlmProvider>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Registers a new LLM provider
        /// </summary>
        /// <param name="provider">The provider instance to register</param>
        public void RegisterProvider(ILlmProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _providers[provider.ProviderId] = provider;
        }

        /// <summary>
        /// Gets a provider by its unique identifier
        /// </summary>
        /// <param name="providerId">The unique identifier of the provider to retrieve</param>
        /// <returns>The provider instance</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the provider is not found</exception>
        public ILlmProvider GetProvider(string providerId)
        {
            if (!_providers.TryGetValue(providerId, out var provider))
                throw new KeyNotFoundException($"Provider with ID '{providerId}' not found. Available providers: {string.Join(", ", _providers.Keys)}");

            return provider;
        }

        /// <summary>
        /// Gets all registered providers
        /// </summary>
        /// <returns>An array of all registered providers</returns>
        public ILlmProvider[] GetAllProviders()
        {
            return _providers.Values.ToArray();
        }

        /// <summary>
        /// Removes a provider by its unique identifier
        /// </summary>
        /// <param name="providerId">The unique identifier of the provider to remove</param>
        public void RemoveProvider(string providerId)
        {
            _providers.Remove(providerId);
        }
    }
} 