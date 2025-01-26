using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp. Extensions;


namespace GmPJect
{
    //그리기 타입
    enum DrawType
    {
        DrawNone = 0,
        DrawLine,
        DrawRectangle,
        DrawEllipse
    }

    //라인 칼라
    enum DrawColor : int
    {
        ColorBlack = 0,
        ColorRed,
        ColorOrange,
        ColorYellow,
        ColorGreen,
        ColorBlue,
        ColorPurple,
        ColorWhite
    }

    public partial class Form1 : Form

    {
        

        //시작점
        private System.Drawing.Point _startPos;
        //현재 위치
        private System.Drawing.Point _currentPos;
        //그리기 모드
        private bool _isDrawing = false;
        //도형 형태
        private DrawType _drawType;
        //라인 두께
        private int _lineThickness = 2;
        public Form1()
        {
            _drawType = DrawType.DrawNone;

            InitializeComponent();
        }
        //PictureBox 초기화
        private void 새로만들기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }

        //이미지 로딩
        private void 열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Mat src = Cv2.ImRead(openFileDialog1.FileName);
                pictureBox1.Image = BitmapConverter.ToBitmap(src);
                //pictureBox1.Load(openFileDialog1.FileName);
            }
        }

        //이미지 저장
        private void 저장ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }



        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (_drawType != DrawType.DrawNone)
                {
                    _startPos = e.Location;
                    _isDrawing = true;
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                _currentPos = e.Location;
                pictureBox1.Invalidate();  // 화면을 다시 그리도록 요청
            }
        }

        //마우스 버튼 Up 이벤트
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                
                    
            _isDrawing = false;
                
            }
        }

        //직선 그리기 버튼 이벤트
        private void btnDrawLine_Click(object sender, EventArgs e)
        {
            _drawType = DrawType.DrawLine;
        }

        //사각형 그리기 버튼 이벤트
        private void btnDrawSquare_Click(object sender, EventArgs e)
        {
            _drawType = DrawType.DrawRectangle;
        }

        //원형 그리기 버튼 이벤트
        private void btnDrawEllipse_Click(object sender, EventArgs e)
        {
            _drawType = DrawType.DrawEllipse;
        }

        // 사각형 계산 유틸리티
        private Rectangle GetRectangle(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            //x,y,width, height
            return new Rectangle(
                Math.Min(p1.X, p2.X),
                Math.Min(p1.Y, p2.X),
                Math.Abs(p1.X - p2.X),
                Math.Abs(p1.X - p2.Y));

        }
        //선두께 선택시 두께값 저장
        private void cbLineThickness_SelectedIndexChanged(object sender, EventArgs e)
        {
            _lineThickness = Convert.ToInt32(cbLineThickness.SelectedItem);
        }
        private Color GetSelColor()
        {
            Color clrLine = new Color();

            DrawColor drawColor = (DrawColor)cbLineColor.SelectedIndex;

            switch (drawColor)
            {
                case DrawColor.ColorBlack:
                    clrLine = Color.Black;
                    break;
                case DrawColor.ColorRed:
                    clrLine = Color.Red;
                    break;
                case DrawColor.ColorOrange:
                    clrLine = Color.Orange;
                    break;
                case DrawColor.ColorYellow:
                    clrLine = Color.Yellow;
                    break;
                case DrawColor.ColorGreen:
                    clrLine = Color.Green;
                    break;
                case DrawColor.ColorBlue:
                    clrLine = Color.Blue;
                    break;
                case DrawColor.ColorPurple:
                    clrLine = Color.Purple;
                    break;
                case DrawColor.ColorWhite:
                    clrLine = Color.White;
                    break;
                default:
                    clrLine = Color.Black;
                    break;
            }

            return clrLine;
        }

        //PictureBox 그래픽 업데이트 이벤트
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (_isDrawing)
            {
                Graphics grp = e.Graphics;

                Color clrLine = GetSelColor();

                Pen pen = new Pen(clrLine, _lineThickness); //선 색상 및 굵기 설정

                switch (_drawType)
                {
                    case DrawType.DrawLine:
                        //직선 그리기
                        grp.DrawLine(pen, _startPos.X, _startPos.Y, _currentPos.X, _currentPos.Y);
                        break;
                    case DrawType.DrawRectangle:
                        //사각형 그리기
                        var rect = GetRectangle(_startPos, _currentPos);
                        grp.DrawRectangle(pen, rect);
                        break;
                   case DrawType.DrawEllipse:
                        //타원 그리기
                        var ellipse = GetRectangle(_startPos, _currentPos);
                        grp.DrawEllipse(pen, ellipse);
                            break;
                }

                //Pen 메모리 해제
                pen.Dispose();
            }
        }

        private void edToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageDlg dlg = new ImageDlg();
            dlg.ShowDialog();
        }
    }
}

      
 







