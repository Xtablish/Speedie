using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Speedie
{
    public class PaddingTextBox : TextBox
    {
        private int paddingSize = 10; 

        public int PaddingSize
        {
            get { return paddingSize; }
            set { paddingSize = value; this.Invalidate(); } 
        }

        public PaddingTextBox(string message)
        {
            this.Multiline = true; 
            this.BorderStyle = BorderStyle.None; 
            this.MinimumSize = new Size(this.Height, 30);
            this.Padding = new Padding(paddingSize);
            this.ForeColor = Color.White;
            this.TextAlign = HorizontalAlignment.Center;
            this.BackColor = Color.FromArgb(45, 45, 45);
            this.Font = new Font("Calibri Light", 20);
            this.Text = message;
            this.ReadOnly = true;
            
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        private static extern bool HideCaret(IntPtr hWnd);
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Call base class's OnPaint method
            e.Graphics.Clear(this.BackColor); // Clear the background
            // Draw the text
            TextRenderer.DrawText(e.Graphics, this.Text, this.Font,
                new Point(paddingSize, paddingSize), this.ForeColor);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            HideCaret(this.Handle); 
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            HideCaret(this.Handle); 
        }
    }
}
