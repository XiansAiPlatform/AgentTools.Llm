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

        protected override async Task<string> GenerateCompletionInternalAsync(string prompt, CompletionOptions options)
        {
            var request = new
            {
                model = ModelName,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                max_tokens = options.MaxTokens,
                temperature = options.Temperature
            };

            var response = await SendRequestAsync($"{BaseUrl}/messages", request);
            return response;
        }

        protected override async Task<string> GenerateChatCompletionInternalAsync(ChatMessage[] messages, CompletionOptions options)
        {
            var request = new
            {
                model = ModelName,
                messages = messages.Select(m => new
                {
                    role = m.Role.ToLower(), // must be: user, assistant, or system
                    content = m.Content
                }),
                max_tokens = options.MaxTokens,
                temperature = options.Temperature
            };

            var response = await SendRequestAsync($"{BaseUrl}/messages", request);
            return response;
        }

        private async Task<string> SendRequestAsync<T>(string url, T request)
        {
            var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            var requestContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

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
