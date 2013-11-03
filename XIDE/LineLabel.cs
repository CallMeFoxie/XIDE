using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace XIDE
{
    class LineLabel : Control
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(this.BackColor);
            //Font f = this.Font;
            Font f = new Font(FontFamily.GenericMonospace, 8);

            Brush b = new SolidBrush(this.ForeColor);

            SizeF size = g.MeasureString("A", f);
            float pos = 0.0f;

            int nMin = minNumber;
            //if (nMin != 1) nMin++;

            for (int i = nMin; i <= maxNumber; ++i)
            {
                SizeF width = g.MeasureString(i.ToString(), f);
                
                // middle position calculation
                float center = LineSpacing / 2;
                center -= width.Height / 2;

                g.DrawString(i.ToString(), f, b, this.Width - width.Width - Padding.Right, pos);
                pos += LineSpacing;
            }
        }

        public float LineSpacing = 14f;
        public int minNumber = 1;
        public int maxNumber = 1;
    }
}
