using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;
using System.Text;


namespace StreamingAgentChatbot;

public class AgentService
{
    private readonly AIAgent _agent;
    private AgentSession? _session;

    public AgentService(string apiKey)
    {
        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri("https://openrouter.ai/api/v1")
        };

        var client = new OpenAIClient(new ApiKeyCredential(apiKey), options);

        var chatClient = client
            .GetChatClient("stepfun/step-3.5-flash:free")
            .AsIChatClient();

        _agent = chatClient.AsAIAgent(
            instructions: "You are a helpful AI tutor. Explain things simply. Use tools when needed.",
            name: "Assistant",
            tools: [AIFunctionFactory.Create(GetTime, name: nameof(GetTime))]
        );
    }

    public async Task<string> SendAsync(string input)
    {
        _session ??= await _agent.CreateSessionAsync();

        var fullResponse = new StringBuilder();

        await foreach (var update in _agent.RunStreamingAsync(input, _session))
        {
            if (update.Text is not null)
            {
                Console.Write(update.Text);
                fullResponse.Append(update.Text);
            }
        }

        Console.WriteLine();

        return fullResponse.ToString();
    }

    private string GetTime()
    {
        return DateTime.Now.ToString("HH:mm:ss");
    }
}