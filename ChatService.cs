using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StreamingAgentChatbot;

public class ChatService
{
    private readonly HttpClient _httpClient;

    public ChatService(string apiKey)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<string> StreamChatAsync(List<ChatMessage> messages)
    {
        var requestBody = new
        {
            model = "stepfun/step-3.5-flash:free",
            stream = true,
            messages = messages
        };

        var json = JsonSerializer.Serialize(requestBody);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://openrouter.ai/api/v1/chat/completions")
        {
            Content = content
        };

        var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead
        );

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: {(int)response.StatusCode}");
            return "";
        }

        var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        var fullResponse = new StringBuilder();

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (!line.StartsWith("data:"))
                continue;

            var jsonChunk = line.Substring("data:".Length).Trim();

            if (jsonChunk == "[DONE]")
                break;

            using var doc = JsonDocument.Parse(jsonChunk);

            var root = doc.RootElement;

            if (!root.TryGetProperty("choices", out var choices))
                continue;

            var firstChoice = choices[0];

            if (!firstChoice.TryGetProperty("delta", out var delta))
                continue;

            if (delta.TryGetProperty("content", out var chunk))
            {
                var text = chunk.GetString();
                Console.Write(text);
                fullResponse.Append(text);
            }
        }

        Console.WriteLine();

        return fullResponse.ToString();
    }
}
