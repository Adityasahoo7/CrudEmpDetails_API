using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CrudDemoPratice.Models.DTOs.AISearch;
using CrudDemoPratice.Service.AI.Interface;
using Microsoft.Extensions.Configuration;

namespace CrudDemoPratice.Service.AI.Implementation
{
    public class AIQueryInterpreter : IAIQueryInterpreter
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AIQueryInterpreter(HttpClient httpClient,IConfiguration config)
        {
            _apiKey = config["Groq:ApiKey"] ?? throw new Exception("Groq API key not found in configuration.");
            _httpClient = httpClient;
        }

        public async Task<AIQueryMetadataDto> InterpretAsync(string naturalLanguageQuery)
        {
            var requestBody = new
            {
                //model = "llama3-70b-8192", // Recommended Groq model
                model = "llama-3.1-8b-instant",
                temperature = 0,
                messages = new object[]
                {
                    new
                    {
                        role = "system",
                        content = @"You are a strict JSON generator.
Convert user natural language into VALID JSON only.
Do NOT explain anything.
Do NOT add text before or after JSON.
Return ONLY JSON."
                    },
                    new
                    {
                        role = "user",
                        content = $@"
Database Table: Employee
Columns: Id, Name, Salary, Phone, Email, Age, Department
Allowed Operators: >, <, =, contains

Return JSON format:
{{
  ""filters"": [
    {{ ""column"": ""Salary"", ""operator"": "">"", ""value"": ""50000"" }}
  ],
  ""sortBy"": ""Salary"",
  ""sortDescending"": true
}}

User Query: {naturalLanguageQuery}
"
                    }
                }
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://api.groq.com/openai/v1/chat/completions"

            );

            request.Headers.Add("Authorization", $"Bearer {_apiKey}");

            request.Content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            Console.WriteLine("➡ Sending request to Groq...");

            var response = await _httpClient.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Groq ERROR {(int)response.StatusCode}: {responseText}");

            using var doc = JsonDocument.Parse(responseText);

            var content = doc.RootElement
                             .GetProperty("choices")[0]
                             .GetProperty("message")
                             .GetProperty("content")
                             .GetString();

            if (string.IsNullOrWhiteSpace(content))
                throw new Exception("Empty response from AI");

            return JsonSerializer.Deserialize<AIQueryMetadataDto>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }
            );
        }
    }
}
