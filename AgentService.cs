using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using System.ClientModel;


namespace StreamingAgentChatbot;

public class AgentService
{
    private readonly AIAgent _agent;

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

        _agent = new ChatClientAgent(
            chatClient,
            new ChatClientAgentOptions
            {
                Name = "Assistant",
                ChatOptions = new ChatOptions
                {
                    Instructions = "You are a helpful AI tutor. Explain things simply."
                }
            });
    }

    public async Task<string> SendAsync(string input)
    {
        var response = await _agent.RunAsync(input);

        return response.Text;
    }
}