namespace AgentTools.Llm.Models
{
    public class ChatMessage
    {
        /// <summary>
        /// The role of the message sender (e.g., "system", "user", "assistant")
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// The content of the message
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Optional name of the message sender
        /// </summary>
        public string? Name { get; set; }

        public ChatMessage(string role, string content, string? name = null)
        {
            Role = role;
            Content = content;
            Name = name;
        }
    }
} 