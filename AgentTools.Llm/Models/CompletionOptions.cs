namespace AgentTools.Llm.Models
{
    public class CompletionOptions
    {
        /// <summary>
        /// The temperature to use for sampling (0.0 to 1.0)
        /// </summary>
        public float Temperature { get; set; } = 0.7f;

        /// <summary>
        /// The maximum number of tokens to generate
        /// </summary>
        public int MaxTokens { get; set; } = 2000;

        /// <summary>
        /// The top-p value for nucleus sampling (0.0 to 1.0)
        /// </summary>
        public float TopP { get; set; } = 1.0f;

        /// <summary>
        /// The frequency penalty to apply (-2.0 to 2.0)
        /// </summary>
        public float FrequencyPenalty { get; set; } = 0.0f;

        /// <summary>
        /// The presence penalty to apply (-2.0 to 2.0)
        /// </summary>
        public float PresencePenalty { get; set; } = 0.0f;

        /// <summary>
        /// Whether to stream the response
        /// </summary>
        public bool Stream { get; set; } = false;

        /// <summary>
        /// Stop sequences to use for generation
        /// </summary>
        public string[]? StopSequences { get; set; }

        /// <summary>
        /// Instructions for how the model should behave
        /// </summary>
        public string? Instructions { get; set; }
    }
} 