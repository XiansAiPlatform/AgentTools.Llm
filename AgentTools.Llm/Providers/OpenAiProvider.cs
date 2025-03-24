using System.Text;
using System.Text.Json;
using AgentTools.Llm.Models;

namespace AgentTools.Llm.Providers
{
    public class OpenAiProvider : BaseLlmProvider
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.openai.com/v1";

        public OpenAiProvider(string providerId, string apiKey, string modelName, CompletionOptions? defaultOptions = null)
            : base(providerId, modelName, defaultOptions)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public override string ProviderName => "OpenAI";

        protected override async Task<string> GenerateCompletionInternalAsync(string prompt, CompletionOptions options)
        {
            var request = new
            {
                model = ModelName,
                input = prompt,
                instructions = options.Instructions
            };

            var response = await SendRequestAsync($"{BaseUrl}/responses", request);
            return response;
        }

        protected override async Task<string> GenerateChatCompletionInternalAsync(ChatMessage[] messages, CompletionOptions options)
        {
            // For chat completion, we use the messages array as input
            var request = new
            {
                model = ModelName,
                input = messages.Select(m => new
                {
                    role = m.Role,
                    content = m.Content
                }).ToArray(),
                instructions = options.Instructions
            };

            var response = await SendRequestAsync($"{BaseUrl}/responses", request);
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

            // Handle the new response format
            if (responseObj.TryGetProperty("output", out var output) && output.GetArrayLength() > 0)
            {
                var firstOutput = output[0];
                if (firstOutput.TryGetProperty("content", out var messageContent) && messageContent.GetArrayLength() > 0)
                {
                    var firstContent = messageContent[0];
                    if (firstContent.TryGetProperty("text", out var text))
                    {
                        return text.GetString() ?? string.Empty;
                    }
                }
            }

            throw new Exception($"Unexpected response format: {responseContent}");
        }
    }
} 