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
        private float angle = 0f; // Angle for rotation
        private Timer timer;
        private int diameter; // Diameter of the loader circle
        private bool animate; // Animation flag

        public LoaderControl(int diameter, bool animate)
        {
            this.diameter = diameter;
            this.animate = animate;

            this.DoubleBuffered = true; // Reduce flickering
            this.Size = new Size(diameter, diameter); // Set the size of the loader

            // Start the timer for animation if animate is true
            if (animate)
            {
                timer = new Timer();
                timer.Interval = 50; // Update every 50ms
                timer.Tick += (s, e) =>
                {
                    angle += 5; // Increment the angle for rotation
                    this.Invalidate(); // Redraw control
                };
                timer.Start();
            }
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

            // Draw the pill shape in the center if animate is true
            if (animate)
            {
                DrawPillShape(g, center);
                DrawMouth(g, center);
            }
        }

        private void DrawPillShape(Graphics g, PointF center)
        {
            // Pill shape dimensions
            float pillWidth = 20;
            float pillHeight = 40;
            float pillRadius = pillHeight / 2;

            // Draw the pill shape (rectangle with rounded ends)
            using (Brush brush = new SolidBrush(Color.Blue))
            {
                // Create a path for the pill shape
                GraphicsPath path = new GraphicsPath();
                path.AddArc(center.X - pillRadius, center.Y - pillWidth / 2, pillRadius * 2, pillWidth, 180, 180); // Left end
                path.AddArc(center.X - pillRadius, center.Y - pillWidth / 2 - pillHeight + pillWidth, pillRadius * 2, pillWidth, 0, 180); // Right end
                path.CloseFigure();
                g.FillPath(brush, path);
            }
        }

        private void DrawMouth(Graphics g, PointF center)
        {
            // Mouth dimensions
            float mouthWidth = 30;
            float mouthHeight = 15;
            float mouthY = center.Y + 10; // Position of the mouth below the pill

            // Draw the semi-ring (arc)
            using (Pen pen = new Pen(Color.Blue, 4))
            {
                g.DrawArc(pen, center.X - mouthWidth / 2, mouthY - mouthHeight / 2, mouthWidth, mouthHeight, 0, 180); // Draw the semi-ring
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
