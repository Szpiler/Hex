using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_Hex
{
    class HexagonButton : Button
    {
        public int color = 0;
        protected override void OnPaint(PaintEventArgs pevent)
        {
            // Create an array of pionts
            Point[] myArray =
            {               
                new Point(ClientSize.Width/2, 0),
                new Point(0, ClientSize.Height/4),
                new Point(0, (ClientSize.Height/4)*3),
                new Point(ClientSize.Width/2, ClientSize.Height),
                new Point(ClientSize.Width, (ClientSize.Height/4)*3),
                new Point(ClientSize.Width, ClientSize.Height/4),
                new Point(ClientSize.Width/2, 0)
            };
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddPolygon(myArray);

            this.Region = new System.Drawing.Region(myPath);
            base.OnPaint(pevent);
        }
    }
}
