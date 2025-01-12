using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;

namespace Speedie
{
    public class GradientLabel : Label
    {
        public GradientLabel()
        {
            this.TextAlign = ContentAlignment.MiddleCenter; // Center the text
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, Color.Blue, Color.Red, 90f))
            {
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                // Center the text
                SizeF textSize = e.Graphics.MeasureString(Text, Font);
                float x = (ClientSize.Width - textSize.Width) / 2;
                float y = (ClientSize.Height - textSize.Height) / 2;
                e.Graphics.DrawString(Text, Font, brush, new PointF(x, y));
            }
        }
    }
}
