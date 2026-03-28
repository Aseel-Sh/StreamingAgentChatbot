using System.Net.Http.Headers;
using System.IO;
using System.Text;
using System.Text.Json;


var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Missing OPENROUTER_API_KEY env variable");
    return;
}

using var httpClient = new HttpClient();

httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

while (true)
{
    Console.Write("\nYou: ");
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput))
        continue;

    if (userInput.ToLower() == "exit")
        break;

    var requestBody = new
    {
        model = "stepfun/step-3.5-flash:free",
        stream = true,
        messages = new[]
    {
            new
            {
                role = "user",
                content = userInput
            }

        }
    };

    var json = JsonSerializer.Serialize(requestBody);

    using var content = new StringContent(json, Encoding.UTF8, "application/json");

    var request = new HttpRequestMessage(HttpMethod.Post, "https://openrouter.ai/api/v1/chat/completions")
    {
        Content = content
    };

    var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

    var stream = await response.Content.ReadAsStreamAsync();
    using var reader = new StreamReader(stream);

    Console.WriteLine("Assistant:");

    while (!reader.EndOfStream)
    {
        var line = await reader.ReadLineAsync();

        if (string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        if (!line.StartsWith("data:"))
        {
            continue;
        }

        var jsonChunk = line.Substring("data:".Length).Trim();

        if (jsonChunk == "[DONE]")
        {
            break;
        }

        using var doc = JsonDocument.Parse(jsonChunk);

        var root = doc.RootElement;

        if (!root.TryGetProperty("choices", out var choices))
            continue;

        var firstChoice = choices[0];

        if (!firstChoice.TryGetProperty("delta", out var delta))
            continue;

        if (delta.TryGetProperty("content", out var chunk))
        {
            Console.Write(chunk.GetString());
        }

    }

    //Console.WriteLine();
}