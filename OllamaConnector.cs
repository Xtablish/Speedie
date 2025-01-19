using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedie
{
    public class OllamaConnector
    {
        private readonly IChatCompletionService _chatService;
        private readonly ChatHistory _history;

        public OllamaConnector()
        {
            var builder = Kernel.CreateBuilder();
#pragma warning disable SKEXP0070
            builder.AddOllamaChatCompletion("phi3:medium", new Uri("http://localhost:11434"));
#pragma warning restore SKEXP0070
            var kernel = builder.Build();
            _chatService = kernel.GetRequiredService<IChatCompletionService>();
            _history = new ChatHistory();
            _history.AddSystemMessage("You are Speedie., a helpful assistant. Keep responses short and efficient.");
        }

        public async Task<string> GetResponseAsync(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return null;
            }

            _history.AddUserMessage(userMessage);
            var response = await _chatService.GetChatMessageContentAsync(_history);
            _history.AddMessage(response.Role, response.Content ?? string.Empty);

            return response.Content ?? "Hmm, I was able to process that. ";
        }
    }
}
