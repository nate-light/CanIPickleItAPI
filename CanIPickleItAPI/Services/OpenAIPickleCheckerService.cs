using System.Text;
using System.Text.Json;
using CanIPickleItAPI.Services;

namespace CanIPickleItAPI.Services
{
    public class OpenAIPickleCheckerService : IPickleCheckerService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<OpenAIPickleCheckerService> _logger;

        public OpenAIPickleCheckerService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAIPickleCheckerService> logger)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key not configured");
            _logger = logger;
            
            _httpClient.BaseAddress = new Uri("https://api.openai.com/");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<PickleCheckResult> CanPickleAsync(string item)
        {
            try
            {
                var prompt = $"Can '{item}' be pickled? Answer with 'Yes' or 'No' followed by a brief explanation why. Keep the response under 100 words.";

                var request = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                        new { role = "system", content = "You are an expert on pickling foods and preserving items. You know what can and cannot be safely pickled." },
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 150,
                    temperature = 0.3
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("v1/chat/completions", content);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("OpenAI API request failed with status code: {StatusCode}", response.StatusCode);
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error response: {ErrorContent}", errorContent);
                    
                    return new PickleCheckResult
                    {
                        CanPickle = false,
                        Reason = "Unable to determine if item can be pickled due to API error."
                    };
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JsonSerializer.Deserialize<JsonElement>(responseContent);

                var assistantMessage = responseData
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? "No response received.";

                // Parse the response to determine if it can be pickled
                var canPickle = assistantMessage.ToLower().StartsWith("yes");
                
                return new PickleCheckResult
                {
                    CanPickle = canPickle,
                    Reason = assistantMessage
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if {Item} can be pickled", item);
                return new PickleCheckResult
                {
                    CanPickle = false,
                    Reason = "An error occurred while checking if the item can be pickled."
                };
            }
        }
    }
}