using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_Hex
{
    public partial class Form1 : Form
    {
        Graphics graphics;
        Pen pen;
        List<float> x;
        List<float> y;      

        public Form1()
        {
            InitializeComponent();
            Board.InitialBoard();
            graphics = Board.CreateGraphics();
            Board.Paint += Board_Paint;           
        }   
        
        private void Board_Paint(object sender, PaintEventArgs e)
        {           
            Board.GetGraphicData(out x, out y);
            Graphics g = e.Graphics;
            pen = new Pen(Color.Blue, 10);
            for(int i = 0; i < x.Count; i++)
            {
                if (i < x.Count / 4)
                    g.DrawLine(pen, x[i], y[i], x[i + 1], y[i + 1]);
                else if (i < x.Count / 2 - 1)
                {
                    pen.Color = Color.Red;
                    g.DrawLine(pen, x[i], y[i], x[i + 1], y[i + 1]);
                }
                else if (i < x.Count / 4 * 3 - 1)
                {
                    pen.Color = Color.Blue;
                    g.DrawLine(pen, x[i], y[i], x[i + 1], y[i + 1]);
                }
                else if(i < x.Count - 2)
                {
                   pen.Color = Color.Red;
                   g.DrawLine(pen, x[i], y[i], x[i + 1], y[i + 1]);
                }
            }          
        }
    }
}
