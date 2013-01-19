using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WaterClock
{
    public partial class Form1 : Form
    {
        private Graphics gGraphics;
        private Pen gPen;
        private SolidBrush gBrush;

        static readonly int HourBarX = 0;
        static readonly int HourBarWidth = 72;

        static readonly int MinuteBarX = 158;// 206;
        static readonly int MinuteBarWidth = 96;

        static readonly int SecondBarX = 258;
        static readonly int SecondBarWidth = 96;


        static readonly int RightPipeX = 158;
        static readonly int RightPipeY = 40;
        static readonly int RightPipeWidth = 156;
        static readonly int RightPipeHeight = 33;

        static readonly int LeftPipeX = 8;
        static readonly int LeftPipeY = 40;
        static readonly int LeftPipeWidth = 156;
        static readonly int LeftPipeHeight = 33;

        static readonly int BASE_WIDTH = 360;
        static readonly int BASE_HEIGHT = 640;


        static readonly int BAR_Y_OFFSET = 74;
        static readonly int BAR_HEIGHT = 466;

        static readonly int PENDULUM_X = 160;
        static readonly int PENDULUM_Y = 40;
        static readonly int PENDULUM_POINT_X = 32;
        static readonly int PENDULUM_POINT_Y = 30;

        
        private int gPendulumWidth;
        private int gPendulumHeight ;
            
        private float xaspect;
        private float yaspect;
        private int TimeTimes;
        private float gHour;
        private float gMinute;
        private float gSecond;
        private float gMilSecond;

        Image gImage = null;
        private Image gPendulumImage;
        private float gAngle;
        private float gAngleDif;

        public Form1()
        {
            InitializeComponent();
            TimeTimes = 1;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            long tick = DateTime.Now.Ticks ;
            tick /= 10000000;
            tick *= TimeTimes;
            gSecond = (tick % 60);
            tick /= 60;
            gMinute = (tick % 60);
            tick /= 60;
            gHour = (tick % 12);
            gMilSecond = (gSecond * 1000 + DateTime.Now.Millisecond) * TimeTimes % (60*1000);

            dateTimePicker1.Value = DateTime.Now;
            pictureBox1.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = bmp;
            gGraphics = Graphics.FromImage(pictureBox1.Image);
            gPen = new Pen(Color.Blue);
            gBrush = new SolidBrush(Color.Cyan);


            gImage = new Bitmap(GetType(), "timer_image.png");
            gPendulumImage = new Bitmap(GetType(), "huriko.png"); 

            gPendulumWidth = gPendulumImage.Width;
            gPendulumHeight = gPendulumImage.Height;

            long tick = DateTime.Now.Ticks;
            tick /= 10000000;
            tick *= TimeTimes;
            gSecond = (tick % 60);
            tick /= 60;
            gMinute = (tick % 60);
            tick /= 60;
            gHour = (tick % 12);
            gMilSecond = (gSecond * 1000 + DateTime.Now.Millisecond) * TimeTimes % (60 * 1000);

            gAngle = 0;
            gAngleDif = 5;
            dateTimePicker1.Value = DateTime.Now;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            gGraphics.Clear(Color.Black);
            xaspect = (float)pictureBox1.Size.Width / BASE_WIDTH;
            yaspect = (float)pictureBox1.Size.Height / BASE_HEIGHT;

            gGraphics.ResetTransform();

            Rectangle hourrect = CalcRect(HourBarX, HourBarWidth, gHour + gMinute /60, 12);
            gGraphics.FillRectangle(gBrush, hourrect);
            Rectangle minrect = CalcRect(MinuteBarX, MinuteBarWidth, gMinute + gMilSecond / 60000, 60);
            gGraphics.FillRectangle(gBrush, minrect);
            Rectangle secondrect = CalcRect(SecondBarX, SecondBarWidth, gMilSecond, 60 * 1000 - 1);
            gGraphics.FillRectangle(gBrush, secondrect);

            if (gAngle > 13)
            {
                Rectangle rpiperect = new Rectangle();
                rpiperect.X = (int)(RightPipeX * xaspect);
                rpiperect.Y = (int)(RightPipeY * yaspect);
                rpiperect.Width = (int)(RightPipeWidth * xaspect);
                rpiperect.Height = (int)(RightPipeHeight * yaspect);
                gGraphics.FillRectangle(gBrush, rpiperect);
            }
            if (gAngle < -13)
            {
                Rectangle lpiperect = new Rectangle();
                lpiperect.X = (int)(LeftPipeX * xaspect);
                lpiperect.Y = (int)(LeftPipeY * yaspect);
                lpiperect.Width = (int)(LeftPipeWidth * xaspect);
                lpiperect.Height = (int)(LeftPipeHeight * yaspect);
                gGraphics.FillRectangle(gBrush, lpiperect);
            }

            gGraphics.DrawImage(gImage, 0, 0, pictureBox1.Size.Width, pictureBox1.Size.Height);

            float pendwidth = gPendulumWidth * pictureBox1.Size.Width / BASE_WIDTH;
            gGraphics.TranslateTransform((PENDULUM_X * xaspect - pendwidth / 2)
                , (PENDULUM_Y * yaspect));
            gGraphics.RotateTransform(gAngle);
            gAngle += gAngleDif;
            if (Math.Abs( gAngle) > 15)
            {
                gAngleDif = -gAngleDif;
            }
            // 振り子描画
            gGraphics.DrawImage(gPendulumImage, -pendwidth / 2, -PENDULUM_POINT_Y * yaspect,
              gPendulumWidth *  pictureBox1.Size.Width / BASE_WIDTH,
              gPendulumHeight *  pictureBox1.Size.Height/ BASE_HEIGHT);

        }

        private Rectangle CalcRect(int x, int width, float n, int max)
        {
            Rectangle rect = new Rectangle();
            rect.X = (int)(x * xaspect);
            rect.Width = (int)(width * xaspect);
            float dy = (float)n/max;
            rect.Y = (int)((BAR_Y_OFFSET + BAR_HEIGHT * (1 - dy)) * yaspect);
            rect.Height = (int)((BAR_HEIGHT+100) * dy * yaspect);
            return rect;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (pictureBox1.Size.Width == 0 || pictureBox1.Size.Height == 0)
            {
                return;
            }
            Bitmap bmp = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);
            pictureBox1.Image = bmp;
            gGraphics = Graphics.FromImage(pictureBox1.Image);
            pictureBox1.Invalidate();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = "x" + trackBar1.Value;
            TimeTimes = trackBar1.Value;
        }
    }
}
