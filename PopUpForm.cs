using OllamaSharp.Models.Chat;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Speedie
{
    public partial class PopUpForm : Form
    {
        private RichTextBox inputTextBox;
        private string response;
        private string placeholderText;
        private Features features;
        private string execOutput;
        public event Action<string> ResponseReady;
        public PopUpForm(string message)
        {
            this.Load += (s, e) =>
            {
                CreateRoundedEdges();
                placeholderText = message;
                InitializeTextBox(message);
                features = new Features();
            };
        }
        private void CreateRoundedEdges()
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            int radius = 30; // Radius for rounded corners
            path.AddArc(0, 0, radius, radius, 180, 90); // Top-left corner
            path.AddArc(Width - radius, 0, radius, radius, 270, 90); // Top-right corner
            path.AddArc(Width - radius, Height - radius, radius, radius, 0, 90); // Bottom-right corner
            path.AddArc(0, Height - radius, radius, radius, 90, 90); // Bottom-left corner
            path.CloseFigure();

            this.Region = new System.Drawing.Region(path);
            this.Invalidate();
        }
        private void InitializeTextBox(string message)
        {
            // Initialize the input TextBox for user input
            inputTextBox = new RichTextBox
            {
                Size = new Size(600, 90),
                Location = new Point(50, this.ClientSize.Height - 115),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.Gray,
                BorderStyle = BorderStyle.None,
                Font = new Font("Calibri Light", 14),
                Multiline = true,
                WordWrap = true,
            };

            inputTextBox.AppendText("\n" + message + ", how can I help?");
            inputTextBox.SelectionStart = 0;
            inputTextBox.SelectionLength = inputTextBox.Text.Length;
            inputTextBox.SelectionAlignment = HorizontalAlignment.Center;
            inputTextBox.GotFocus += InputTextBox_GotFocus;
            inputTextBox.LostFocus += InputTextBox_LostFocus;

            // Create a rounded rectangle region for the input TextBox
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            int radius = 30; // Radius for rounded edges
            path.AddArc(0, 0, radius, radius, 180, 90); // Top-left corner
            path.AddArc(inputTextBox.Width - radius, 0, radius, radius, 270, 90); // Top-right corner
            path.AddArc(inputTextBox.Width - radius, inputTextBox.Height - radius, radius, radius, 0, 90); // Bottom-right corner
            path.AddArc(0, inputTextBox.Height - radius, radius, radius, 90, 90); // Bottom-left corner
            path.CloseFigure();

            inputTextBox.Region = new System.Drawing.Region(path);

            // Add the input TextBox to the form
            this.Controls.Add(inputTextBox);

            // Add event handler for when the user presses Enter
            inputTextBox.KeyDown += InputTextBox_KeyDown;
        }
        private void InputTextBox_GotFocus(object sender, EventArgs e)
        {
            if (inputTextBox.Text == "\n" + placeholderText + ", how can I help?")
            {
                inputTextBox.Text = "";
                inputTextBox.ForeColor = Color.Gray;
            }
        }
        private void InputTextBox_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputTextBox.Text))
            {
                inputTextBox.Text = "\n" + placeholderText + ", how can I help?";
                inputTextBox.SelectionStart = 0;
                inputTextBox.SelectionLength = inputTextBox.Text.Length;
                inputTextBox.SelectionAlignment = HorizontalAlignment.Center;
                inputTextBox.ForeColor = Color.Gray;
            }
        }
        private async void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                inputTextBox.Enabled = false;
                string userInput = inputTextBox.Text;
                inputTextBox.Text = "\nGetting my thoughts together...";
                inputTextBox.SelectionStart = 0;
                inputTextBox.SelectionLength = inputTextBox.Text.Length;
                inputTextBox.SelectionAlignment = HorizontalAlignment.Center;
                inputTextBox.ForeColor = Color.Blue;

                try
                {
                    OllamaConnector ollama = new OllamaConnector();
                    if (userInput.ToLower().Contains("run"))
                    {
                        execOutput = await Task.Run(() => features.RunPortScanAndGraph("127.0.0.1"));
                        string buildQuery = userInput+"\nHere are the results:\n"+execOutput;
                        buildQuery = buildQuery + "\ntest = [80, 22, 443, 444, 21, 8888, 79, 4040, 495, 4859]";
                        response = await Task.Run(() => ollama.GetResponseAsync(buildQuery));
                        ResponseReady?.Invoke(response);
                    }else
                    {
                        response = await Task.Run(() => ollama.GetResponseAsync(userInput));
                        ResponseReady?.Invoke(response);
                    }
                }
                catch (Exception)
                {
                    response = "Hmm, I was unable to process that. ";
                    ResponseReady?.Invoke(response);
                }
                finally
                {
                    inputTextBox.Enabled = true;
                    inputTextBox.Text = "\n" + placeholderText + ", how can I help?";
                    inputTextBox.SelectionStart = 0;
                    inputTextBox.SelectionLength = inputTextBox.Text.Length;
                    inputTextBox.SelectionAlignment = HorizontalAlignment.Center;
                    inputTextBox.ForeColor = Color.Gray;
                }
            }
        }

        public string Response => response;
        public string GetResponse()
        {
            return response;
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTCLIENT = 1;

            // Prevent dragging and any movement
            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)HTCLIENT;
                return;
            }

            base.WndProc(ref m);
        }
    }
}
