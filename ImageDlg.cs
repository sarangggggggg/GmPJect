﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp.Extensions;
using OpenCvSharp;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Runtime.Hosting;


//콤보박스를 넣어서 칼라를 모노로 바꾸는 필터추가

namespace GmPJect
{
    // 이미지 연산(Enum)
    enum ImageOperation
    {
        opAdd = 0,          
        opSubtract,      
        opMultiply,        
        opDivide,          
        opMax,             
        opMin,             
        opAbs,            
        opAbsDiff,         
        and,               
        or,                
        xor,               
        not,               
        compare        
    }

    // 이미지 필터(Enum)
    enum ImageFilter
    {
        FilterBlur = 0,         
        FilterBoxFilter,        
        FilterMedianBlur,       
        FilterGaussianBlur,     
        FilterBilateral, 
        FilterSobel,             
        FilterScharr,           
        FilterLaplacian,         
        FilterCanny            
    }

    public partial class ImageDlg : Form
    {
        //*************************************예외처리
        private bool isImageLoaded = false;
        private bool isImageOp = false;
        
        //*************************************Mat객체선언
        Mat blur = new Mat();
        Mat dst = new Mat();

        public ImageDlg()
        { 
            InitializeComponent();
        }

       
        //이미지 초기화
        private void 새로만들기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SrcPictureBox.Image = null;
            DstPictureBox.Image = null;
        }


        //이미지 열기
        private void 열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
               
                Mat src1 = Cv2.ImRead(openFileDialog1.FileName);
                SrcPictureBox.Image = BitmapConverter.ToBitmap(src1);
                isImageLoaded = true; //이미지가 성공적으로 열렸을 때 isImageLoaded 값을 true로 설정해야 이미지를 불러온 상태임을 표시할 수 있습니다.!!!!!!
            }
        }
        
        //연산 적용 버튼
        private void ApplyBTN_Click(object sender, EventArgs e)
        {
          

            //이미지를 불러오지 않았을 경우
            if (!isImageLoaded)
            
            {
                MessageBox.Show("먼저 이미지를 열어주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 이미지를 불러왔지만 필터나 연산이 적용되지 않은 경우
            if (!isImageOp)
            {               
                MessageBox.Show("이미지에 적용할 필터나 연산이 없습니다.\n필터를 선택하거나 연산을 적용해주세요.",
                                "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 현재 dst(Mat) 내용을 원본 이미지 Mat(src)에 적용
                Mat src1 = dst.Clone(); // dst를 src으로 복사하여 적용
                DstPictureBox.Image = BitmapConverter.ToBitmap(src1); // DstPictureBox에 업데이트
                isImageOp = true; // 연산 성공 후 상태 업데이트

                
                // 연산 적용 상태 초기화
                isImageLoaded = true;
                
                MessageBox.Show("이미지 변환이 성공적으로 적용되었습니다.", "적용 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지를 적용하는 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            


            // 텍스트박스에 연산 결과 표시
            Scalar mean = Cv2.Mean(dst); // dst에서 평균값 계산
            textBox1.Text = $"Mean: {mean.Val0:F2}, {mean.Val1:F2}, {mean.Val2:F2}";
            



        }

           


        //저장 버튼
        private void SaveBTN_Click(object sender, EventArgs e)
        {
            if (!isImageOp) // 연산된 이미지가 없을 경우 예외 처리
            {
                MessageBox.Show("저장할 이미지가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // SaveFileDialog 설정
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "이미지 저장",
                Filter = "JPEG Files|*.jpg|PNG Files|*.png|Bitmap Files|*.bmp|All Files|*.*", // 저장 가능한 파일 형식 필터
                DefaultExt = "jpg", 
                AddExtension = true // 사용자가 확장자를 입력하지 않으면 기본 확장자를 자동 추가 
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                string fileExtension = System.IO.Path.GetExtension(filePath).ToLower(); // 확장자 추출

                // 저장할 Mat 객체 (예: dst에 연산된 결과가 있다고 가정)
                Mat imageToSave = dst;

                try
                {
                    // 확장자에 따른 저장 형식 설정
                    switch (fileExtension)
                    {
                        case ".jpg":
                        case ".jpeg":
                            Cv2.ImWrite(filePath, imageToSave, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90)); // JPEG 품질 설정
                            break;

                        case ".png":
                            Cv2.ImWrite(filePath, imageToSave, new ImageEncodingParam(ImwriteFlags.PngCompression, 3)); // PNG 압축 설정
                            break;

                        case ".bmp":
                            Cv2.ImWrite(filePath, imageToSave);
                            break;

                        case ".gif":
                            Cv2.ImWrite(filePath, imageToSave);
                            break;

                        default:
                            MessageBox.Show("지원하지 않는 파일 형식입니다. JPG, PNG, BMP, GIF만 저장 가능합니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                    }

                    MessageBox.Show("이미지가 성공적으로 저장되었습니다.", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"이미지를 저장하는 동안 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        // 연산 콤보박스에서 선택 시 동작
        private void OpcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isImageLoaded)
            {
                MessageBox.Show("먼저 이미지를 열어주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

                
            }


            // 원본 이미지 읽기
            Mat src1 = Cv2.ImRead(openFileDialog1.FileName);
            Mat src2 = new Mat(src1.Size(), MatType.CV_8UC3, new Scalar(50, 50, 50)); // 연산용 두 번째 이미지

            // 선택된 연산 타입 가져오기
            ImageOperation selType = (ImageOperation)OpcomboBox.SelectedIndex;

            // 연산 적용
            switch (selType)
            {
                case ImageOperation.opAdd:
                    Cv2.Add(src1, src2, dst);
                    break;
                case ImageOperation.opSubtract:
                    Cv2.Subtract(src1, src2, dst);
                    break;
                case ImageOperation.opMultiply:
                    Cv2.Multiply(src1, src2, dst);
                    break;
                case ImageOperation.opDivide:
                    Cv2.Divide(src1, src2, dst);
                    break;
                case ImageOperation.opMax:
                    Cv2.Max(src1, src2, dst);
                    break;
                case ImageOperation.opMin:
                    Cv2.Min(src1, src2, dst);
                    break;
                case ImageOperation.opAbs:
                    Cv2.Multiply(src1, src2, dst);
                    Cv2.Abs(dst);
                    break;
                case ImageOperation.opAbsDiff:
                    Mat matMul = new Mat();
                    Cv2.Multiply(src1, src2, matMul);
                    Cv2.Absdiff(src1, matMul, dst);
                    break;
                case ImageOperation.and:
                    Cv2.BitwiseAnd(src1, src2, dst);
                    break;
                case ImageOperation.or:
                    Cv2.BitwiseOr(src1, src2, dst);
                    break;
                case ImageOperation.xor:
                    Cv2.BitwiseXor(src1, src2, dst);
                    break;
                case ImageOperation.not:
                    Cv2.BitwiseNot(src1, dst);
                    break;
                case ImageOperation.compare:
                    Cv2.Compare(src1, src2, dst, CmpType.EQ);
                    break;
            }

            if (dst.Empty())
            {
                MessageBox.Show("연산이 실패했습니다. 다시 시도해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isImageOp = false;
                return;
            }

            isImageOp = true;

        }

        private void Filtercmbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isImageLoaded)
            {
                MessageBox.Show("먼저 이미지를 열어주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

               
            }
     

            // 원본 이미지 읽기
            Mat src1 = Cv2.ImRead(openFileDialog1.FileName);
            //Mat src = Cv2.ImRead("sparkler.png", ImreadModes.ReducedColor2);
           // Mat src2 = new Mat(src1.Size(), MatType.CV_8UC3, new Scalar(0, 0, 30));

            Cv2.GaussianBlur(src1, blur, new OpenCvSharp.Size(3, 3), 1, 0, BorderTypes.Default);

            // 선택된 필터 타입 가져오기
            ImageFilter selType = (ImageFilter)Filtercmbx.SelectedIndex;

            // 필터 적용
            switch (selType)
            {
                case ImageFilter.FilterBlur:
                    Cv2.Blur(src1, dst, new OpenCvSharp.Size(9, 9));
                    break;
                case ImageFilter.FilterBoxFilter:
                    Cv2.BoxFilter(src1, dst, MatType.CV_8UC3, new OpenCvSharp.Size(9, 9));
                    break;
                case ImageFilter.FilterMedianBlur:
                    Cv2.MedianBlur(src1, dst, 9);
                    break;
                case ImageFilter.FilterGaussianBlur:
                    Cv2.GaussianBlur(src1, dst, new OpenCvSharp.Size(9, 9), 1.5, 1.5);
                    break;
                case ImageFilter.FilterBilateral:
                    Cv2.BilateralFilter(src1, dst, 9, 75, 75);
                    break;
                case ImageFilter.FilterSobel:
                    Cv2.Sobel(src1, dst, MatType.CV_8U, 1, 0, ksize: 3);
                    break;
                case ImageFilter.FilterScharr:
                    Cv2.Scharr(src1, dst, MatType.CV_8U, 1, 0);
                    break;
                case ImageFilter.FilterLaplacian:
                    Cv2.Laplacian(src1, dst, MatType.CV_8U);
                    break;
                case ImageFilter.FilterCanny:
                    Cv2.Canny(src1, dst, 100, 200);
                    break;
            }

            if (dst.Empty())
            {
                MessageBox.Show("연산이 실패했습니다. 다시 시도해주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                isImageOp = false;
                return;
            }

            isImageOp = true;

        }

        
    }
}









