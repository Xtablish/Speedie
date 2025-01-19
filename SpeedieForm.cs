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
        private PopUpForm _popUpForm;
        private string greeting = GreetingProvider.GetGreeting();
        private readonly SpeechSynthesizer synthesizer;
        private TextBox dynamicTextBox;
        private string response;

        public SpeedieForm()
        {
            InitializeComponent();
            ShowPopUp();
            synthesizer = new SpeechSynthesizer
            {
                Volume = 100,
                Rate = 0
            };
            synthesizer.SelectVoice("Microsoft Zira Desktop");
        }

        public void ShowPopUp()
        {
            if (_popUpForm == null || _popUpForm.IsDisposed)
            {
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

                AddGreetingLabel();
                var loader = new LoaderControl(130)
                {
                    Location = new Point((_popUpForm.ClientSize.Width - 100) / 2, (_popUpForm.ClientSize.Height - 100) / 2)
                };
                _popUpForm.Controls.Add(loader);
                _popUpForm.ResponseReady += OnResponseReady;
                _popUpForm.Show();
            }
            else
            {
                _popUpForm.BringToFront();
            }
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

        private async Task InitializeDynamicTextBox(PopUpForm popUpForm)
        {
            dynamicTextBox = new TextBox
            {
                Location = new Point(50, 100),
                Width = 600,
                Height = 500,
                BackColor = popUpForm.BackColor,
                ForeColor = Color.White,
                BorderStyle = BorderStyle.None,
                TextAlign = HorizontalAlignment.Left,
                ReadOnly = true,
                Font = new Font("Calibri Light", 14),
                Multiline = true,
                WordWrap = true,
            };

            await Task.Run(() =>
            {
                dynamicTextBox.Text = popUpForm.GetResponse();
            });

            var loader = new LoaderControl(70)
            {
                Location = new Point(30, 20)
            };
            popUpForm.Controls.Add(loader);
            popUpForm.Controls.Add(loader);
            popUpForm.Controls.Add(dynamicTextBox);
            SpeakResponse(dynamicTextBox.Text);
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
            base.OnFormClosing(e);
        }
        private async void OnResponseReady(string response)
        {
            var loader = _popUpForm.Controls.OfType<LoaderControl>().FirstOrDefault();
            var label  = _popUpForm.Controls.OfType<GradientLabel>().FirstOrDefault();
            if (loader != null && label != null)
            {
                _popUpForm.Controls.Remove(loader);
                _popUpForm.Controls.Remove(label);
            }
            await InitializeDynamicTextBox(_popUpForm);
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
    }
}
