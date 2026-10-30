using System.Text;
using System.Text.Json;
using CrudDemoPratice.Models.DTOs.AISearch;
using CrudDemoPratice.Service.AI.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CrudDemoPratice.Service.AI.Implementation
{
    public class AIQueryInterpreter : IAIQueryInterpreter
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<AIQueryInterpreter> _logger;

        private static readonly Dictionary<string, string> NumericColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Id"] = "Id",
            ["Salary"] = "Salary",
            ["Age"] = "Age"
        };
        private static readonly Dictionary<string, string> TextColumns = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Name"] = "Name",
            ["Phone"] = "Phone",
            ["Email"] = "Email",
            ["Department"] = "Department"
        };
        private static readonly HashSet<string> Operators = new() { ">", "<", "=", "contains" };

        private const string SystemPrompt = """
You convert natural-language employee searches into JSON filters. Output JSON only.

Table: Employee
Columns and types:
- Id (number), Salary (number), Age (number)
- Name (text), Phone (text), Email (text), Department (text)

Allowed operators: ">", "<", "=", "contains"

Rules:
1. "column" must be EXACTLY one of: Id, Name, Salary, Phone, Email, Age, Department.
2. Use "=" for exact matches on numbers (id, age, salary equal to).
3. Use "contains" only for text columns (Name, Email, Phone, Department).
4. "more than / greater than / above / over" => ">". "less than / below / under" => "<".
5. "between X and Y" => two filters: ">" X and "<" Y.
6. "value" is always a string containing only the value (no commas, no currency symbols).
7. If the user mentions an id / employee id / emp id, ALWAYS use column "Id" with operator "=".
8. If the query has no filter at all (e.g. "get all employees"), return an empty filters array.
9. sortBy is null unless the user asks for sorting. sortDescending is false unless they say highest/descending/top.

Examples:
Query: get id 421
{"filters":[{"column":"Id","operator":"=","value":"421"}],"sortBy":null,"sortDescending":false}

Query: salary more than 40000
{"filters":[{"column":"Salary","operator":">","value":"40000"}],"sortBy":null,"sortDescending":false}

Query: employees in IT department with age below 30 sorted by salary highest first
{"filters":[{"column":"Department","operator":"contains","value":"IT"},{"column":"Age","operator":"<","value":"30"}],"sortBy":"Salary","sortDescending":true}

Query: name contains ravi
{"filters":[{"column":"Name","operator":"contains","value":"ravi"}],"sortBy":null,"sortDescending":false}
""";

        public AIQueryInterpreter(HttpClient httpClient, IConfiguration config, ILogger<AIQueryInterpreter> logger)
        {
            _apiKey = config["groq-apikey"] ?? throw new Exception("Groq API key not found in configuration.");
            _httpClient = httpClient;
            _logger = logger;
        }
            
        public async Task<AIQueryMetadataDto> InterpretAsync(string naturalLanguageQuery)
        {
            var requestBody = new
            {
                model = "openai/gpt-oss-120b",
                temperature = 0,
                response_format = new { type = "json_object" }, // forces valid JSON
                messages = new object[]
                {
                    new { role = "system", content = SystemPrompt },
                    new { role = "user", content = $"Query: {naturalLanguageQuery}" }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.groq.com/openai/v1/chat/completions");
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Groq ERROR {(int)response.StatusCode}: {responseText}");

            using var doc = JsonDocument.Parse(responseText);
            var content = doc.RootElement.GetProperty("choices")[0]
                             .GetProperty("message").GetProperty("content").GetString();

            if (string.IsNullOrWhiteSpace(content))
                throw new Exception("Empty response from AI");

            _logger.LogInformation("AI raw output for '{Query}': {Content}", naturalLanguageQuery, content);

            var result = JsonSerializer.Deserialize<AIQueryMetadataDto>(
                content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new Exception("Could not parse AI response");

            return Validate(result, naturalLanguageQuery);
        }

        // Never trust the model: normalize and verify everything before it reaches the DB layer
        private AIQueryMetadataDto Validate(AIQueryMetadataDto dto, string originalQuery)
        {
            dto.Filters ??= new();

            foreach (var f in dto.Filters)
            {
                var op = f.Operator?.Trim().ToLowerInvariant() ?? "";
                if (!Operators.Contains(op))
                    throw new InvalidOperationException($"Invalid operator '{f.Operator}' from AI.");
                f.Operator = op;

                if (NumericColumns.TryGetValue(f.Column ?? "", out var numCol))
                {
                    f.Column = numCol;
                    if (op == "contains")
                        throw new InvalidOperationException($"'contains' is not valid for numeric column {numCol}.");
                    if (!decimal.TryParse(f.Value, out _))
                        throw new InvalidOperationException($"Value '{f.Value}' is not numeric for {numCol}.");
                }
                else if (TextColumns.TryGetValue(f.Column ?? "", out var txtCol))
                {
                    f.Column = txtCol;
                }
                else
                {
                    throw new InvalidOperationException($"Unknown column '{f.Column}' from AI.");
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.SortBy))
            {
                var all = NumericColumns.Concat(TextColumns).ToDictionary(k => k.Key, v => v.Value, StringComparer.OrdinalIgnoreCase);
                dto.SortBy = all.TryGetValue(dto.SortBy, out var s) ? s : null;
            }

            return dto;
        }
    }
}