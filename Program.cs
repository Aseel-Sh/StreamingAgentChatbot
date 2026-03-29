using StreamingAgentChatbot;

var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Missing OPENROUTER_API_KEY env variable");
    return;
}

var agentService = new AgentService(apiKey);

while (true)
{
    Console.Write("\nYou: ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    if (input.ToLower() == "exit")
        break;

    var response = await agentService.SendAsync(input);

    Console.WriteLine($"Assistant: {response}");
}