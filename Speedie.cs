using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speedie
{
    public class SpeedieService
    {
        private readonly IChatCompletionService _chatService;
        private readonly ChatHistory _history;
        private readonly SpeechRecognitionEngine _recognizer;
        private readonly SpeechSynthesizer _synthesizer;

        public SpeedieService()
        {
            var builder = Kernel.CreateBuilder();
            builder.AddOllamaChatCompletion("llama3.1:latest", new Uri("http://localhost:11434"));
            var kernel = builder.Build();
            _chatService = kernel.GetRequiredService<IChatCompletionService>();
            _history = new ChatHistory();
            _history.AddSystemMessage("Your name is Speedie.");

            _recognizer = new SpeechRecognitionEngine();
            _synthesizer = new SpeechSynthesizer();
            ConfigureSpeechRecognition();
        }

        private void ConfigureSpeechRecognition()
        {
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("Speedie"))); // Listen for the keyword
            _recognizer.SpeechRecognized += OnKeywordRecognized;
            _recognizer.SetInputToDefaultAudioDevice();
        }

        public void StartListening()
        {
            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // Start continuous recognition
        }

        private async void OnKeywordRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Stop listening for the keyword
            _recognizer.RecognizeAsyncStop();

            // Start listening for user input
            var userMessage = await CaptureUserInputAsync();

            // Get response from AI
            var response = await GetResponseAsync(userMessage);

            // Read response out loud
            SpeakResponse(response);

            // Restart listening for the keyword
            StartListening();
        }

        private async Task<string> CaptureUserInputAsync()
        {
            // Configure the recognizer to listen for continuous speech input
            _recognizer.LoadGrammar(new DictationGrammar());
            _recognizer.RecognizeAsync(RecognizeMode.Single); // Listen for a single input

            string capturedText = string.Empty;

            // Event handler for recognized speech
            _recognizer.SpeechRecognized += (s, e) =>
            {
                capturedText = e.Result.Text;
                _recognizer.RecognizeAsyncStop(); // Stop after capturing the input
            };

            // Wait until the user stops talking
            await Task.Delay(5000); // Adjust the delay as necessary

            return capturedText;
        }

        public async Task<string> GetResponseAsync(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return "Empty user input";
            }

            _history.AddUserMessage(userMessage);
            var response = await _chatService.GetChatMessageContentAsync(_history);
            _history.AddMessage(response.Role, response.Content ?? string.Empty);

            return response.Content ?? "No response";
        }

        private void SpeakResponse(string response)
        {
            _synthesizer.Speak(response); // Read the response out loud
        }
    }
}
