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
        private TextBox inputTextBox;
        public PopUpForm(string message)
        {
            this.Load += (s, e) =>
            {
                CreateRoundedEdges();
                InitializeTextBox(message);
            };
        }
        private void CreateRoundedEdges()
        {
            // Create a rounded rectangle region based on the current size of the form
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            int radius = 30; // Radius for rounded corners

            path.AddArc(0, 0, radius, radius, 180, 90); // Top-left corner
            path.AddArc(Width - radius, 0, radius, radius, 270, 90); // Top-right corner
            path.AddArc(Width - radius, Height - radius, radius, radius, 0, 90); // Bottom-right corner
            path.AddArc(0, Height - radius, radius, radius, 90, 90); // Bottom-left corner
            path.CloseFigure();

            this.Region = new System.Drawing.Region(path);
        }

        private void InitializeTextBox(string message)
        {

            inputTextBox = new PaddingTextBox(message)
            {
                Size = new System.Drawing.Size(600, 40), // Width adjusted for padding
                Location = new System.Drawing.Point(45, this.ClientSize.Height - 80), 
            };

            // Create a rounded rectangle region for the TextBox
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            int radius = 30; // Radius for rounded edges
            path.AddArc(0, 0, radius, radius, 180, 90); // Top-left corner
            path.AddArc(inputTextBox.Width - radius, 0, radius, radius, 270, 90); // Top-right corner
            path.AddArc(inputTextBox.Width - radius, inputTextBox.Height - radius, radius, radius, 0, 90); // Bottom-right corner
            path.AddArc(0, inputTextBox.Height - radius, radius, radius, 90, 90); // Bottom-left corner
            path.CloseFigure();

            inputTextBox.Region = new System.Drawing.Region(path);

            // Add the TextBox to the form
            this.Controls.Add(inputTextBox);
        }

        protected override void WndProc(ref Message m)
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
