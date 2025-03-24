using System.Threading.Tasks;
using AgentTools.Llm.Models;

namespace AgentTools.Llm.Interfaces
{
    public interface ILlmProvider
    {
        /// <summary>
        /// Gets the name of the LLM provider
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Gets the model name being used
        /// </summary>
        string ModelName { get; }

        /// <summary>
        /// Generates a completion based on the provided prompt
        /// </summary>
        /// <param name="prompt">The input prompt</param>
        /// <param name="options">Optional parameters for the completion</param>
        /// <returns>The generated completion</returns>
        Task<string> GenerateCompletionAsync(string prompt, CompletionOptions? options = null);

        /// <summary>
        /// Generates a chat completion based on the provided messages
        /// </summary>
        /// <param name="messages">The chat messages</param>
        /// <param name="options">Optional parameters for the completion</param>
        /// <returns>The generated chat completion</returns>
        Task<string> GenerateChatCompletionAsync(ChatMessage[] messages, CompletionOptions? options = null);
    }
} 