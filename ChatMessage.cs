using System;
using System.Collections.Generic;
using System.Text;

namespace StreamingAgentChatbot;

public class ChatMessage
{
    public string role { get; set; }
    public string content { get; set; }

    public ChatMessage(string role, string content)
    {
        this.role = role;
        this.content = content;
    }
}
