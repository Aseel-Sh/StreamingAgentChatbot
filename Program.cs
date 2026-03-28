using StreamingAgentChatbot;

const int maxMessages = 20;


var apiKey = Environment.GetEnvironmentVariable("OPENROUTER_API_KEY");

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Missing OPENROUTER_API_KEY env variable");
    return;
}

var chatService = new ChatService(apiKey);

var messages = new List<ChatMessage>();

messages.Add(new ChatMessage(
    "system",
    "You are a helpful AI tutor. Explain concepts clearly, step by step, using simple language. Keep answers concise but informative."
));

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


    if (messages.Count > maxMessages)
    {
        messages.RemoveAt(1);
    }
}