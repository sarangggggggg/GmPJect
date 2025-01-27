using System;
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


//콤보박스를 넣어서 칼라를 모노로 바꾸는 필터추가

namespace GmPJect
{
    // 이미지 연산(Enum으로 정의)
    enum ImageOperation
    {
        OpAdd = 0,         // 이미지 더하기
        OpSubtract,        // 이미지 빼기
        OpMultiply,        // 이미지 곱하기
        OpDivide,          // 이미지 나누기
        OpMax,             // 두 이미지 중 큰 값
        OpMin,             // 두 이미지 중 작은 값
        OpAbs,             // 절대값 연산
        OpAbDiff,          // 절대 차 연산
        and,               // 비트 연산 AND
        or,                // 비트 연산 OR
        xor,               // 비트 연산 XOR
        not,               // 비트 연산 NOT
        compare,           // 두 이미지 비교
    }

    // 이미지 필터(Enum으로 정의)
    enum ImageFilter
    {
        FilterBlur = 0,          // 평균 블러 필터
        FilterBoxFilter,         // 박스 필터
        FilterMedianBlur,        // 중간값 필터
        FilterGaussianBlur,      // 가우시안 블러 필터
        FilterBilateral,         // 양방향 필터
        FilterSobel,             // 소벨 필터
        FilterScharr,            // 샤르 필터
        FilterLaplacian,         // 라플라시안 필터
        FilterCanny,             // 캐니 엣지 검출
    }

    public partial class ImageDlg : Form
    {
        public ImageDlg()
        {
            InitializeComponent(); // 폼 초기화
        }
        private void edToolStripMenuItem_Click(object sender, EventArgs e)
        { //Form1 먼저 띄우고 그 다음에 ImageDlg 띄우기
            ImageDlg dlg = new ImageDlg();
            dlg.ShowDialog();
        }


        private bool isImageLoaded = false; //불러온이미지없이 연산시 예외처리
        private bool isImageOp = false;     //저장할 연산이미지없을시 예외처리
        Mat blur = new Mat();  // OpenCV Mat 객체 선언
        Mat dst = new Mat();  // OpenCV Mat 객체 선언

        private string loadFilePath; // 로드된 이미지 파일 경로 저장

        private void rabbitToolStripMenuItem_Click(object sender, EventArgs e)
        {  // "토끼" 버튼 클릭 시 동작

            loadFilePath = "rabbit.JPG"; // 파일 경로 저장
            Mat src1 = Cv2.ImRead("rabbit.JPG"); // OpenCV로 이미지 읽기
            SrcPictureBox.Image = BitmapConverter.ToBitmap(src1); // Mat 객체를 Bitmap으로 변환하여 PictureBox에 표시
            isImageLoaded = true; //이미지가 성공적으로 로드되었음을 표시
        }

        private void spongebobSqurepantsToolStripMenuItem_Click(object sender, EventArgs e)
        {  // "스폰지밥" 버튼 클릭 시 동작
            loadFilePath = "spongebob.JPG"; // 파일 경로 저장
            Mat src1 = Cv2.ImRead("spongebob.JPG"); //OpenCV로 이미지 읽기
            SrcPictureBox.Image = BitmapConverter.ToBitmap(src1); // Mat 객체를 Bitmap으로 변환하여 PictureBox에 표시
            isImageLoaded = true; //이미지가 성공적으로 로드되었음을 표시
        }

        private void cakeToolStripMenuItem_Click(object sender, EventArgs e)
        { // "케이크" 버튼 클릭 시 동작
            loadFilePath = "cake.JPG"; // 파일 경로 저장
            Mat src1 = Cv2.ImRead("cake.JPG"); //OpenCV로 이미지 읽기
            SrcPictureBox.Image = BitmapConverter.ToBitmap(src1); // Mat 객체를 Bitmap으로 변환하여 PictureBox에 표시
            isImageLoaded = true; //이미지가 성공적으로 로드되었음을 표시
        }

        private void choonsikToolStripMenuItem_Click(object sender, EventArgs e)
        { //"춘식이"버튼 클릭 시 동작
            loadFilePath = "choonsik.GIF"; //파일 경로 저장
            Image gifImage = Image.FromFile(loadFilePath); //OpenCV로 이미지 읽기
            SrcPictureBox.Image = gifImage;  // PictureBox에 GIF 표시
            isImageLoaded = true; //이미지가 성공적으로 로드되었음을 표시
        }
        private void applyBTN_Click(object sender, EventArgs e)
        {
            if (!isImageOp || dst.Empty())
            {
                MessageBox.Show("적용할 이미지가 없습니다. 먼저 필터 또는 연산을 선택하세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 현재 dst(Mat) 내용을 원본 이미지 Mat(src)에 적용
                Mat src = dst.Clone(); // dst를 src으로 복사하여 적용
                DstPictureBox.Image = BitmapConverter.ToBitmap(src); // DstPictureBox에 업데이트

                // 상태 초기화
                isImageLoaded = true;
                isImageOp = false; // 이미 적용했으므로 연산 상태 리셋
                MessageBox.Show("이미지 변환이 성공적으로 적용되었습니다.", "적용 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"이미지를 적용하는 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void 열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {  //DialogResult는 windows forms 라이브러리에서 제공하는 열거형.
                //대화상자(Dialog)에서 반환되는 값 표현
                SrcPictureBox.Load(openFileDialog1.FileName);

                Mat src = Cv2.ImRead(openFileDialog1.FileName);
                SrcPictureBox.Image = BitmapConverter.ToBitmap(src);

                isImageLoaded = true;

            }
        }




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
                DefaultExt = "jpg", // 기본 파일 확장자를 JPEG로 설정
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
            Mat src = Cv2.ImRead(loadFilePath, ImreadModes.Color);
            Mat src2 = new Mat(src.Size(), MatType.CV_8UC3, new Scalar(50, 50, 50)); // 연산용 두 번째 이미지

            // 선택된 연산 타입 가져오기
            ImageOperation selType = (ImageOperation)OpcomboBox.SelectedIndex;

            // 연산 적용
            switch (selType)
            {
                case ImageOperation.OpAdd:
                    Cv2.Add(src, src2, dst);
                    break;
                case ImageOperation.OpSubtract:
                    Cv2.Subtract(src, src2, dst);
                    break;
                case ImageOperation.OpMultiply:
                    Cv2.Multiply(src, src2, dst);
                    break;
                case ImageOperation.OpDivide:
                    Cv2.Divide(src, src2, dst);
                    break;
                case ImageOperation.OpMax:
                    Cv2.Max(src, src2, dst);
                    break;
                case ImageOperation.OpMin:
                    Cv2.Min(src, src2, dst);
                    break;
                case ImageOperation.OpAbDiff:
                    Cv2.Absdiff(src, src2, dst);
                    break;
                case ImageOperation.and:
                    Cv2.BitwiseAnd(src, src2, dst);
                    break;
                case ImageOperation.or:
                    Cv2.BitwiseOr(src, src2, dst);
                    break;
                case ImageOperation.xor:
                    Cv2.BitwiseXor(src, src2, dst);
                    break;
                case ImageOperation.not:
                    Cv2.BitwiseNot(src, dst);
                    break;
                case ImageOperation.compare:
                    Cv2.Compare(src, src2, dst, CmpType.EQ);
                    break;
            }

            // 결과 업데이트
            DstPictureBox.Image = BitmapConverter.ToBitmap(dst);
            isImageOp = true;

            // 텍스트박스에 연산 결과 표시
            Scalar mean = Cv2.Mean(dst);
            textBox1.Text = $"Mean: {mean.Val0:F2}, {mean.Val1:F2}, {mean.Val2:F2}";
        }

        private void Filtercmbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isImageLoaded)
            {
                MessageBox.Show("먼저 이미지를 열어주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

                isImageOp = true;
            }

            // 원본 이미지 읽기
            Mat src = Cv2.ImRead(loadFilePath, ImreadModes.Color);

            // 선택된 필터 타입 가져오기
            ImageFilter selType = (ImageFilter)Filtercmbx.SelectedIndex;

            // 필터 적용
            switch (selType)
            {
                case ImageFilter.FilterBlur:
                    Cv2.Blur(src, dst, new OpenCvSharp.Size(9, 9));
                    break;
                case ImageFilter.FilterBoxFilter:
                    Cv2.BoxFilter(src, dst, MatType.CV_8UC3, new OpenCvSharp.Size(9, 9));
                    break;
                case ImageFilter.FilterMedianBlur:
                    Cv2.MedianBlur(src, dst, 9);
                    break;
                case ImageFilter.FilterGaussianBlur:
                    Cv2.GaussianBlur(src, dst, new OpenCvSharp.Size(9, 9), 1.5, 1.5);
                    break;
                case ImageFilter.FilterBilateral:
                    Cv2.BilateralFilter(src, dst, 9, 75, 75);
                    break;
                case ImageFilter.FilterSobel:
                    Cv2.Sobel(src, dst, MatType.CV_8U, 1, 0, ksize: 3);
                    break;
                case ImageFilter.FilterScharr:
                    Cv2.Scharr(src, dst, MatType.CV_8U, 1, 0);
                    break;
                case ImageFilter.FilterLaplacian:
                    Cv2.Laplacian(src, dst, MatType.CV_8U);
                    break;
                case ImageFilter.FilterCanny:
                    Cv2.Canny(src, dst, 100, 200);
                    break;
            }

            // 결과 업데이트
            DstPictureBox.Image = BitmapConverter.ToBitmap(dst);
            isImageOp = true;

            // 텍스트박스에 필터 결과 표시
            Scalar mean = Cv2.Mean(dst);
            textBox1.Text = $"Mean: {mean.Val0:F2}, {mean.Val1:F2}, {mean.Val2:F2}";
        }


    }
}









