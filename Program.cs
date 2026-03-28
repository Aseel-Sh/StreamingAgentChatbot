using StreamingAgentChatbot;


var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Missing OPENROUTER_API_KEY env variable");
    return;
}

var chatService = new ChatService(apiKey);

var messages = new List<ChatMessage>();

while (true)
{
    Console.Write("\nYou: ");
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput))
        continue;

    if (userInput.ToLower() == "exit")
        break;

    messages.Add(new ChatMessage("user", userInput));

    Console.Write("Assistant: ");

    var response = await chatService.StreamChatAsync(messages);

    messages.Add(new ChatMessage("assistant", response));
}