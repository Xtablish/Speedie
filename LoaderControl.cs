using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Speedie
{
    public class LoaderControl : Control
    {
        private float angle = 0f;
        private Timer timer;
        private int diameter; 

        public LoaderControl(int diameter)
        {
            this.diameter = diameter;

            this.DoubleBuffered = true; // Reduce flickering
            this.Size = new Size(diameter, diameter);

            timer = new Timer();
            timer.Interval = 50; 
            timer.Tick += (s, e) =>
            {
                angle += 5; // Increment the angle for rotation
                this.Invalidate(); // Redraw control
            };
            timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Center point for rotation
            PointF center = new PointF(this.Width / 2f, this.Height / 2f);
            float radius = diameter / 2f - 20; // Radius for the semi-circles, adjusting for padding
            float thickness = 8; // Thickness of the semi-circles

            // Colors for the loader
            Color[] colors = {
            Color.FromArgb(234, 67, 53), // Red
            Color.FromArgb(66, 133, 244), // Blue
            Color.FromArgb(251, 188, 5), // Yellow
            Color.FromArgb(52, 168, 83) // Green
        };

            // Draw each color in a semi-circle
            for (int i = 0; i < colors.Length; i++)
            {
                using (Pen pen = new Pen(colors[i], thickness))
                {
                    float startAngle = angle + (i * 90); // Adjust the angle for each section
                    g.DrawArc(pen, center.X - radius, center.Y - radius, radius * 2, radius * 2, startAngle, 90); // Draw each quarter arc
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
