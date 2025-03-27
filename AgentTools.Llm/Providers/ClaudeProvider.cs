using System.Text;
using System.Text.Json;
using AgentTools.Llm.Models;

namespace AgentTools.Llm.Providers
{
    public class ClaudeProvider : BaseLlmProvider
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.anthropic.com/v1";

        public ClaudeProvider(string providerId, string apiKey, string modelName, CompletionOptions? defaultOptions = null)
            : base(providerId, modelName, defaultOptions)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
            _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
        }

        public override string ProviderName => "Anthropic-Claude";

        // Claude uses two different endpoints based on model version:
        // - For claude-1.x: /complete with prompt-based input
        // - For claude-2.x and claude-3.x: /messages with structured messages[]
        // These helpers automatically select the correct endpoint.
        private bool UseMessagesEndpoint =>
            ModelName.StartsWith("claude-2") || ModelName.StartsWith("claude-3");

        private string CompletionEndpoint =>
            UseMessagesEndpoint ? $"{BaseUrl}/messages" : $"{BaseUrl}/complete";

        protected override async Task<string> GenerateCompletionInternalAsync(string prompt, CompletionOptions options)
        {
            var request = UseMessagesEndpoint
                ? BuildMessagesRequestFromPrompt(prompt, options)
                : BuildPromptRequest(prompt, options);

            var response = await SendRequestAsync(CompletionEndpoint, request);
            return response;
        }

        protected override async Task<string> GenerateChatCompletionInternalAsync(ChatMessage[] messages, CompletionOptions options)
        {
            var request = UseMessagesEndpoint
                ? BuildMessagesRequest(messages, options)
                : BuildPromptRequestFromMessages(messages, options);

            var response = await SendRequestAsync(CompletionEndpoint, request);
            return response;
        }

        private object BuildPromptRequest(string prompt, CompletionOptions options) => new
        {
            model = ModelName,
            prompt = prompt,
            max_tokens_to_sample = options.MaxTokens,
            temperature = options.Temperature,
            stop_sequences = options.StopSequences
        };

        private object BuildPromptRequestFromMessages(ChatMessage[] messages, CompletionOptions options)
        {
            var promptBuilder = new StringBuilder();

            foreach (var message in messages)
            {
                var role = message.Role.ToLower() switch
                {
                    "user" => "Human",
                    "assistant" => "Assistant",
                    _ => message.Role
                };
                promptBuilder.AppendLine($"{role}: {message.Content}");
            }

            promptBuilder.Append("Assistant:");

            return BuildPromptRequest(promptBuilder.ToString(), options);
        }

        private object BuildMessagesRequest(ChatMessage[] messages, CompletionOptions options) => new
        {
            model = ModelName,
            messages = messages.Select(m => new
            {
                role = m.Role.ToLower(), // must be: user, assistant, system
                content = m.Content
            }),
            max_tokens = options.MaxTokens,
            temperature = options.Temperature
        };

        private object BuildMessagesRequestFromPrompt(string prompt, CompletionOptions options) => new
        {
            model = ModelName,
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            max_tokens = options.MaxTokens,
            temperature = options.Temperature
        };

        private async Task<string> SendRequestAsync<T>(string url, T request)
        {
            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"Sending request to: {url}");
            Console.WriteLine($"Request body: {json}");

            var response = await _httpClient.PostAsync(url, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error response: {responseContent}");
                throw new HttpRequestException(
                    $"API request failed with status code {response.StatusCode}. Response: {responseContent}");
            }

            var responseObj = JsonSerializer.Deserialize<JsonElement>(responseContent);

            if (responseObj.TryGetProperty("completion", out var completion))
            {
                return completion.GetString() ?? string.Empty;
            }

            if (responseObj.TryGetProperty("content", out var contentArray) &&
                contentArray.ValueKind == JsonValueKind.Array &&
                contentArray.GetArrayLength() > 0 &&
                contentArray[0].TryGetProperty("text", out var text))
            {
                return text.GetString() ?? string.Empty;
            }

            throw new Exception($"Unexpected response format: {responseContent}");
        }
    }
}
