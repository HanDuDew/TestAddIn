using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;

namespace AutoBeau.Services
{
    public class AIChatService
    {
        private readonly OpenAIClient _openAIClient;
        private readonly string _systemPrompt;

        public AIChatService(string apiKey)
        {
            _openAIClient = new OpenAIClient(apiKey);
            _systemPrompt = "You are an AI assistant specialized in helping with Autodesk Inventor projects and general questions." +
                            "Your primary focus is to assist users with drawing operations in Inventor." +
                            "You should not answer questions that have nothing to do with Inventor or Engineer drawing operations. In those cases, respond with 'Sorry, that is out of my knowledge domain. Please ask me anything about Inventor'. " +
                            "Greetings are allowed." +
                            "Keep your responses concise and practical.";
        }

        public async Task<string> SendMessageAsync(string userMessage)
        {
            try
            {
                var chatMessages = new List<ChatMessage>
                {
                    new SystemChatMessage(_systemPrompt),
                    new UserChatMessage(userMessage)
                };

                var chatCompletion = await _openAIClient.GetChatClient("gpt-4o-mini").CompleteChatAsync(chatMessages);
                
                return chatCompletion.Value.Content[0].Text;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        public async Task<string> SendMessageWithContextAsync(string userMessage, string[] previousMessages)
        {
            try
            {
                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage(_systemPrompt)
                };

                // Add previous messages for context (last 10 messages to stay within token limits)
                var recentMessages = previousMessages.Skip(Math.Max(0, previousMessages.Length - 10));
                foreach (var message in recentMessages)
                {
                    if (message.StartsWith("User: "))
                    {
                        messages.Add(new UserChatMessage(message.Substring(6)));
                    }
                    else if (message.StartsWith("AI: "))
                    {
                        messages.Add(new AssistantChatMessage(message.Substring(4)));
                    }
                }

                messages.Add(new UserChatMessage(userMessage));

                // Uses gpt-4o-mini for testing purposes
                var chatCompletion = await _openAIClient.GetChatClient("gpt-4o-mini").CompleteChatAsync(messages);
                
                return chatCompletion.Value.Content[0].Text;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}