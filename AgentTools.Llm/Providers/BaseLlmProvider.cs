using System.Threading.Tasks;
using AgentTools.Llm.Interfaces;
using AgentTools.Llm.Models;

namespace AgentTools.Llm.Providers
{
    public abstract class BaseLlmProvider : ILlmProvider
    {
        protected readonly string _modelName;
        protected readonly string _providerId;
        protected readonly CompletionOptions _defaultOptions;

        protected BaseLlmProvider(string providerId, string modelName, CompletionOptions? defaultOptions = null)
        {
            _providerId = providerId;
            _modelName = modelName;
            _defaultOptions = defaultOptions ?? new CompletionOptions();
        }

        public abstract string ProviderName { get; }

        public string ProviderId => _providerId;

        public string ModelName => _modelName;

        public virtual async Task<string> GenerateCompletionAsync(string prompt, CompletionOptions? options = null)
        {
            var mergedOptions = MergeOptions(options);
            return await GenerateCompletionInternalAsync(prompt, mergedOptions);
        }

        public virtual async Task<string> GenerateChatCompletionAsync(ChatMessage[] messages, CompletionOptions? options = null)
        {
            var mergedOptions = MergeOptions(options);
            return await GenerateChatCompletionInternalAsync(messages, mergedOptions);
        }

        protected abstract Task<string> GenerateCompletionInternalAsync(string prompt, CompletionOptions options);
        protected abstract Task<string> GenerateChatCompletionInternalAsync(ChatMessage[] messages, CompletionOptions options);

        protected CompletionOptions MergeOptions(CompletionOptions? options)
        {
            if (options == null)
                return _defaultOptions;

            return new CompletionOptions
            {
                Temperature = options.Temperature,
                MaxTokens = options.MaxTokens,
                TopP = options.TopP,
                FrequencyPenalty = options.FrequencyPenalty,
                PresencePenalty = options.PresencePenalty,
                Stream = options.Stream,
                StopSequences = options.StopSequences,
                Instructions = options.Instructions
            };
        }
    }
} 