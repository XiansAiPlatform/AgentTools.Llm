namespace AgentTools.Llm.Configuration
{
    public class ClaudeConfiguration
    {
        /// <summary>
        /// The Claude API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The default model to use (e.g., "claude-2")
        /// </summary>
        public string DefaultModel { get; set; }

        public ClaudeConfiguration(string apiKey, string defaultModel)
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            DefaultModel = defaultModel ?? throw new ArgumentNullException(nameof(defaultModel));
        }
    }
}