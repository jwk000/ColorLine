using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorLine
{
    public partial class ColorLine : Form
    {

        class VecPoint
        {
            public Point _pos;
            public int _speedX;
            public int _speedY;
        }

        //直线的条数
        const int mLineNum = 10;
        //顶点数
        const int mVecNum = 30;
        //线间距
        const int mLineMargen = 10;
        //场景大小
        int mSceneLeftX = 0;
        int mSceneRightX = 1600;
        int mSceneUpY = 0;
        int mSceneDownY = 900;
        int mSceneMargen = 200;
        //变换速度
        const int mRunSpeed = 10;
        const int mColorSpeed = 1;
        const int mColorChangeCircle = 500;
        int mCurrentColorCircle = 0;
        int mCurrentColorSpeed = mColorSpeed;
        int mCurrentChangeColor = 0;//0=r 1=g 2=b

        //线的颜色
        Color mLineColor;
        int r, g, b;
        //直线是由一系列定点构成的
        VecPoint[,] mVecs = new VecPoint[mVecNum, mLineNum];
        //随机数
        Random mRandGen = new Random();

        public ColorLine()
        {
            InitializeComponent();
            InitVecs();
            InitColor();
            InitForm();
        }

        //初始化窗口
        void InitForm()
        {
            //窗口全屏
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.ClientSize = Screen.PrimaryScreen.Bounds.Size;

            mSceneLeftX = mSceneMargen;
            mSceneUpY = mSceneMargen;
            mSceneRightX = ClientSize.Width - mSceneMargen;
            mSceneDownY = ClientSize.Height - mSceneMargen;

            //窗口背景
            this.BackColor = Color.Black;
            //隐藏鼠标
            Cursor.Hide();
            this.ShowInTaskbar = false;
            //双帧缓冲打开
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            //定时器
            UITimer.Enabled = true;
            UITimer.Interval = mRunSpeed;
            UITimer.Tick += UITimer_Tick;
            UITimer.Start();
        }

        //自然渐变颜色分量
        int CalcNextColorValue(int x)
        {
            if (x + mCurrentColorSpeed > 255 || x + mCurrentColorSpeed < 0)
            {
                mCurrentColorSpeed = -mCurrentColorSpeed;
            }
            return x + mCurrentColorSpeed;
        }

        private void UITimer_Tick(object sender, EventArgs e)
        {
            //计算下个颜色,rgb轮流变换一段时间
            if(++mCurrentColorCircle >= mColorChangeCircle)
            {
                mCurrentColorCircle = 0;
                mCurrentChangeColor = mRandGen.Next(0, 3);
            }
            if (mCurrentChangeColor == 0) r = CalcNextColorValue(r);
            if (mCurrentChangeColor == 1) g = CalcNextColorValue(g);
            if (mCurrentChangeColor == 2) b = CalcNextColorValue(b);

            mLineColor = Color.FromArgb(r,g,b);
            //计算下个位置
            foreach (VecPoint v in mVecs)
            {
                int x = v._pos.X + v._speedX;
                if (x < mSceneLeftX)
                {
                    x = mSceneLeftX;
                    v._speedX = -v._speedX;
                }else if(x > mSceneRightX)
                {
                    x = mSceneRightX;
                    v._speedX = -v._speedX;
                }
                int y = v._pos.Y + v._speedY;
                if (y < mSceneUpY)
                {
                    y = mSceneUpY;
                    v._speedY = -v._speedY;
                }
                else if(y>mSceneDownY)
                {
                    y = mSceneDownY;
                    v._speedY = -v._speedY;
                }
                v._pos = new Point(x , y);
            }
            //重绘
            this.Invalidate(new Rectangle(0, 0, this.Size.Width, this.Size.Height));
        }

        //初始化定点
        void InitVecs()
        {
            for(int i=0;i<mVecNum;i++)
            {
                int x = mRandGen.Next(mSceneLeftX + 5, mSceneRightX - 5);
                int y = mRandGen.Next(mSceneUpY + 5, mSceneDownY - 5);
                int speedx = mRandGen.Next(1, 8);
                int speedy = mRandGen.Next(1, 8);
                for(int j = 0; j < mLineNum; j++)
                {
                    mVecs[i, j] = new VecPoint()
                    {
                        _pos = new Point(x + j * mLineMargen, y + j * mLineMargen),
                        _speedX = speedx,
                        _speedY = speedy
                    };
                }
            }
        }
        //初始化颜色
        void InitColor()
        {
            r = mRandGen.Next(10, 255);
            g = mRandGen.Next(10, 255);
            b = mRandGen.Next(10, 255);
            mLineColor = Color.FromArgb(r, g, b);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Pen _pen = new Pen(mLineColor, 1.5f);

            for(int j=0;j<mLineNum;j++)
            {
                Point[] _vecs = new Point[mVecNum+1];
                for(int i=0;i<mVecNum;i++)
                {
                    _vecs[i] = mVecs[i, j]._pos;
                }
                _vecs[mVecNum] = mVecs[0, j]._pos;
                //g.DrawLines(_pen, _vecs);
                g.DrawClosedCurve(_pen, _vecs);
            }
        }

        //按键关闭
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            Keys key = e.KeyCode;
            if(key == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
