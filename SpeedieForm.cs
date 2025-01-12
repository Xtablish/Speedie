using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Windows.Forms;
using System.Speech.Synthesis;
using static System.Net.Mime.MediaTypeNames;

namespace Speedie
{
    public partial class SpeedieForm : Form
    {
        public SpeechRecognitionEngine recognizer;
        private Form _popUpForm;
        private bool standbyMode;
        private string greeting = GreetingProvider.GetGreeting();
        private TextBox dynamicTextBox;
        private readonly SpeechSynthesizer synthesizer;
        private Dictionary<int, string> recognizedTexts = new Dictionary<int, string>();
        private bool isRecognitionRunning;
        private int entryCounter;

        private static readonly string[] greetingPhrases = 
        {
        "Hi there! How can I help you today?",
        "Hello! What can I do for you?",
        "Hey! What’s on your mind?",
        "Hi! How can I assist you?",
        "Hello! I’m here to assist you.",
        "Hey! How can I support you today?",
        "Hi! Feel free to ask me anything.",
        "Hi! What’s up? How can I help?",
        "Hi there! What can I help you with?",
        };

        public SpeedieForm()
        {
            InitializeComponent();
            entryCounter = 0;
            isRecognitionRunning = true;
            this.Hide();
            
            InitializeSpeechRecognition();

            synthesizer = new SpeechSynthesizer
            {
                Volume = 100,
                Rate = 0
            };
            synthesizer.SelectVoice("Microsoft Zira Desktop");
        }

        public void InitializeSpeechRecognition()
        {
            recognizer = new SpeechRecognitionEngine();
            recognizer.SetInputToDefaultAudioDevice();

            Grammar grammar = new DictationGrammar();
            recognizer.LoadGrammar(grammar);
            recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(VoiceRecognizer);
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void VoiceRecognizer(object sender, SpeechRecognizedEventArgs e)
        {
            string recognizedText = e.Result.Text;
            Console.WriteLine($"User command: {recognizedText}");
            recognizedTexts.Add(entryCounter++, recognizedText);
            ProcessRecognizedTexts();
        }

        private async void ProcessRecognizedTexts()
        {
            foreach (var text in recognizedTexts.ToList()) 
            {
                string lowerText = text.Value.ToLower();
                string userInput = string.Empty;

                if (lowerText.Contains("speedie") || lowerText.Contains("speedy"))
                {
                    if(isRecognitionRunning)
                    {
                        isRecognitionRunning = false;
                        recognizer.RecognizeAsyncCancel();  

                        int index = lowerText.IndexOf("speedie") >= 0 ? lowerText.IndexOf("speedie") : lowerText.IndexOf("speedy");
                        index += (lowerText.IndexOf("speedie") >= 0 ? "speedie".Length : "speedy".Length);
                        userInput = lowerText.Substring(index).Trim();

                        Console.WriteLine($"User command: {userInput}");
                        recognizedTexts.Remove(text.Key);

                        // Create an instance of SpeedieService
                        SpeedieService speedieService = new SpeedieService();
                        string response = await speedieService.GetResponseAsync(userInput);

                        // Speak the response
                        standbyMode = false;
                        SpeakResponse(response);
                        ShowPopUp(response);
                        recognizer.RecognizeAsync(RecognizeMode.Multiple);
                        break;
                    }
                }
            }
        }

        private async void SpeakResponse(string response)
        {
            await Task.Run(() =>
            {
                try
                {
                    synthesizer.Speak(response);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in text-to-speech: {ex.Message}");
                }
            });
        }

        public void ShowPopUp(string response)
        {
            if (_popUpForm != null && !_popUpForm.IsDisposed)
            {
                return;
            }

            _popUpForm = new PopUpForm(greeting)
            {
                Size = new Size(700, 800),
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Text = "Speedie AI",
                BackColor = Color.FromArgb(35, 35, 35),
                MaximizeBox = false,
                MinimizeBox = false
            };
            PositionPopUpAboveTaskbar();

            if (standbyMode)
            {
                AddGreetingLabel();
                var loader = new LoaderControl(130, true)
                {
                    Location = new Point((_popUpForm.ClientSize.Width - 100) / 2, (_popUpForm.ClientSize.Height - 100) / 2)
                };
                _popUpForm.Controls.Add(loader);
            }
            else
            {
                var loader = new LoaderControl(70, false)
                {
                    Location = new Point(30, 20)
                };
                _popUpForm.Controls.Add(loader);
                InitializeDynamicTextBox(_popUpForm, response);
            }

            _popUpForm.Show();
        }

        private void AddGreetingLabel()
        {
            var greetingLabel = new GradientLabel
            {
                Text = "Speedie",
                Font = new Font("Calibri Light", 20, FontStyle.Bold),
                Size = new Size(100, 50),
                Location = new Point(18, 15)
            };
            _popUpForm.Controls.Add(greetingLabel);
        }

        private void InitializeDynamicTextBox(Form popUpForm, string message)
        {
            dynamicTextBox = new TextBox
            {
                Location = new Point(50, 100),
                Width = 600,
                Height = 600,
                BackColor = popUpForm.BackColor,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                TextAlign = HorizontalAlignment.Left,
                ReadOnly = true,
                Font = new Font("Calibri Light", 14),
                Multiline = true,
                WordWrap = true,
            };

            dynamicTextBox.Text = message;
            popUpForm.Controls.Add(dynamicTextBox);
        }

        private void PositionPopUpAboveTaskbar()
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;

            _popUpForm.Location = new Point(
                (screenWidth - _popUpForm.Width) / 2,
                workingArea.Bottom - _popUpForm.Height - 6
            );
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            recognizer?.Dispose(); 
            base.OnFormClosing(e);
        }
    }
}
