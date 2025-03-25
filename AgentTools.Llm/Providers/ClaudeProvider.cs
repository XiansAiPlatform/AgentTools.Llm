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
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public override string ProviderName => "Anthropic-Claude";

        protected override async Task<string> GenerateCompletionInternalAsync(string prompt, CompletionOptions options)
        {
            var request = new
            {
                model = ModelName,
                prompt = prompt,
                max_tokens_to_sample = options.MaxTokens,
                temperature = options.Temperature,
                stop_sequences = options.StopSequences
            };

            var response = await SendRequestAsync($"{BaseUrl}/complete", request);
            return response;
        }

        protected override async Task<string> GenerateChatCompletionInternalAsync(ChatMessage[] messages, CompletionOptions options)
        {
            // For Claude, chat messages are concatenated into a single prompt
            var prompt = string.Join("\n", messages.Select(m => $"{m.Role}: {m.Content}"));

            var request = new
            {
                model = ModelName,
                prompt = prompt,
                max_tokens_to_sample = options.MaxTokens,
                temperature = options.Temperature,
                stop_sequences = options.StopSequences
            };

            var response = await SendRequestAsync($"{BaseUrl}/complete", request);
            return response;
        }

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

            // Handle Claude's response format
            if (responseObj.TryGetProperty("completion", out var completion))
            {
                return completion.GetString() ?? string.Empty;
            }

            throw new Exception($"Unexpected response format: {responseContent}");
        }
    }
}