namespace AgentTools.Llm.Configuration
{
    public class OpenAiConfiguration
    {
        /// <summary>
        /// The OpenAI API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The default model to use (e.g., "gpt-4", "gpt-3.5-turbo")
        /// </summary>
        public string DefaultModel { get; set; }

        /// <summary>
        /// The organization ID (optional)
        /// </summary>
        public string? OrganizationId { get; set; }

        public OpenAiConfiguration(string apiKey, string defaultModel, string? organizationId = null)
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            DefaultModel = defaultModel ?? throw new ArgumentNullException(nameof(defaultModel));
            OrganizationId = organizationId;
        }
    }
} 