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
    public enum ImageOperation
    {
        OpAdd,
        OpSubtract,
        OpMultiply,
        OpDivide,
        OpMax,
        OpMin,
        OpAbs,
        OpAbDiff,
        and,
        or,
        xor,
        not,
        compare,
    }

    public enum ImageFilter
    {
        FilterBlur,
        FilterBoxFilter,
        FilterMedianBlur,
        FilterGaussianBlur,
        FilterBilateral,
        FilterSobel,
        FilterScharr,
        FilterLaplacian,
        FilterCanny,

    }

    public partial class ImageDlg : Form
    {
        public ImageDlg()
        {
            InitializeComponent();
        }
        private void edToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageDlg dlg = new ImageDlg();
            dlg.ShowDialog();
        }


        private bool isImageLoaded = false; //불러온이미지없이 연산시 예외처리
        private bool isImageOp = false;     //저장할 연산이미지없을시 예외처리
        Mat blur = new Mat();
        Mat dst = new Mat();

        private string loadFilePath; // 로드된 이미지 파일 경로 저장

        private void rabbitToolStripMenuItem_Click(object sender, EventArgs e)
        {

            loadFilePath = "rabbit.JPG"; // 파일 경로 저장
            Mat src = Cv2.ImRead("rabbit.JPG");
            SrcPictureBox.Image = BitmapConverter.ToBitmap(src);
            isImageLoaded = true; //이미지가 로드되었음을 표시
        }

        private void spongebobSqurepantsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadFilePath = "spongebob.JPG"; // 파일 경로 저장
            Mat src = Cv2.ImRead("spongebob.JPG");
            SrcPictureBox.Image = BitmapConverter.ToBitmap(src);
            isImageLoaded = true;
        }

        private void cakeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadFilePath = "cake.JPG"; // 파일 경로 저장
            Mat src = Cv2.ImRead("cake.JPG");
            SrcPictureBox.Image = BitmapConverter.ToBitmap(src);
            isImageLoaded = true;
        }

        private void choonsikToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadFilePath = "choonsik.GIF";
            Image gifImage = Image.FromFile(loadFilePath);

            // PictureBox에 GIF 표시
            SrcPictureBox.Image = gifImage;


            isImageLoaded = true;
        }
        private void LoadBTN_Click(object sender, EventArgs e)
        {
            DstPictureBox.Image = SrcPictureBox.Image;
        }





        private void SaveBTN_Click(object sender, EventArgs e)
        {
            if (!isImageOp || dst.Empty()) // dst가 비었는지 확인
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
                AddExtension = true // 사용자가 확장자를 입력하지 않으면 기본 확장자를 자동 추가 ???????????????????????????????
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

                        default:
                            MessageBox.Show("지원하지 않는 파일 형식입니다. JPG, PNG, BMP만 저장 가능합니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void OpcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isImageLoaded)
            {
                MessageBox.Show("먼저 이미지를 불러오라츄~", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 이미지 로드
                if (string.IsNullOrEmpty(openFileDialog1.FileName))
                {
                    MessageBox.Show("유효한 이미지 파일을 선택하세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Mat src1 = Cv2.ImRead(openFileDialog1.FileName);
                Mat src2 = new Mat(src1.Size(), MatType.CV_8UC3, new Scalar(0, 0, 30));
                Mat dst = new Mat(); //연산 결과 저장

                // OpcomboBox에서 선택된 연산 확인
                if (!Enum.IsDefined(typeof(ImageOperation), OpcomboBox.SelectedIndex))
                {
                    MessageBox.Show("올바르지 않은 연산이 선택되었습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ImageOperation selType = (ImageOperation)OpcomboBox.SelectedIndex;


            switch (selType)
            {
                case ImageOperation.OpAdd:
                    Cv2.Add(src1, src2, dst);
                    break;
                case ImageOperation.OpSubtract:
                    Cv2.Subtract(src1, src2, dst);
                    break;
                case ImageOperation.OpMultiply: // 두 이미지의 각 픽셀 값을 곱함
                    Cv2.Multiply(src1, src2, dst);
                    break;
                case ImageOperation.OpDivide:
                    Cv2.Divide(src1, src2, dst);
                    break;
                case ImageOperation.OpMax:
                    Cv2.Max(src1, src2, dst);
                    break;
                case ImageOperation.OpMin:
                    Cv2.Min(src1, src2, dst);
                    break;
                case ImageOperation.OpAbs:
                    Cv2.Multiply(src1, src2, dst);
                    Cv2.Abs(dst);
                    break;
                // 두 이미지 간 절대 차이를 계산
                case ImageOperation.OpAbDiff:
                    Mat wingKa = new Mat(); // 임시 Mat 객체 생성
                    Cv2.Multiply(src1, src2, wingKa); // src1과 src2를 곱한 결과를 wingKa에 저장
                    Cv2.Absdiff(src1, wingKa, dst); // src1과 wingKa 간의 절대 차이를 dst에 저장
                    break;
                // 두 이미지의 비트 AND 연산 (각 픽셀의 비트 AND 결과)
                case ImageOperation.and:
                    Cv2.BitwiseAnd(src1, src2, dst);
                    break;
                // 두 이미지의 비트 OR 연산 (각 픽셀의 비트 OR 결과)
                case ImageOperation.or:
                    Cv2.BitwiseOr(src1, src2, dst);
                    break;
                // 두 이미지의 비트 XOR 연산 (각 픽셀의 비트 XOR 결과)
                case ImageOperation.xor:
                    Cv2.BitwiseXor(src1, src2, dst);
                    break;
                // src1의 비트를 반전시킴 (NOT 연산)
                case ImageOperation.not:
                    Cv2.BitwiseNot(src1, src2, dst);
                    break;
                // 두 이미지를 비교 (픽셀 값이 같은 경우를 비교)
                case ImageOperation.compare:
                    Cv2.Compare(src1, src2, dst, CmpType.EQ); //두 이미지의 픽셀 값이 같은 경우를 비교하고 결과를 반환.
                    break;
                default:
                    MessageBox.Show("지원되지 않는 연산입니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;

                }

            // OpenCvSharp의 Mat을 Bitmap으로 변환하여 PictureBox에 출력
            DstPictureBox.Image = BitmapConverter.ToBitmap(dst);
            DstPictureBox.Refresh(); //UI 업데이트 강제 실행
                
            isImageOp = true;

            // 연산 결과를 TextBox에 출력 (예: Mat의 픽셀 평균값)
            Scalar mean = Cv2.Mean(dst); // Mat의 평균값 계산
            textBox1.Text = $"Mean: {mean.Val0:F2}, {mean.Val1:F2}, {mean.Val2:F2}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"연산 중 오류가 발생했습니다:\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }




        private void Filtercmbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isImageLoaded)
            {
                MessageBox.Show("먼저 이미지를 불러오라츄~", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 이미 로드된 이미지를 다시 로드하지 않음
                Mat src = Cv2.ImRead(loadFilePath, ImreadModes.Color);
                Mat dst = new Mat(); // 필터 결과 저장용 Mat
                Mat blur = new Mat();

                Cv2.GaussianBlur(src, blur, new OpenCvSharp.Size(3, 3), 1, 0, BorderTypes.Default);

                ImageFilter selectedFilter = (ImageFilter)Filtercmbx.SelectedItem;

                switch (selectedFilter)
                {
                    case ImageFilter.FilterBlur:
                        Cv2.Blur(src, dst, new OpenCvSharp.Size(9, 9), new OpenCvSharp.Point(-1, -1), BorderTypes.Default);
                        break;
                    case ImageFilter.FilterBoxFilter:
                        Cv2.BoxFilter(src, dst, MatType.CV_8UC3, new OpenCvSharp.Size(9, 9), new OpenCvSharp.Point(-1, -1), true, BorderTypes.Default);
                        break;
                    case ImageFilter.FilterMedianBlur:
                        Cv2.MedianBlur(src, dst, 9);
                        break;
                    case ImageFilter.FilterGaussianBlur:
                        Cv2.GaussianBlur(src, dst, new OpenCvSharp.Size(9, 9), 1, 1, BorderTypes.Default);
                        break;
                    case ImageFilter.FilterBilateral:
                        Cv2.BilateralFilter(src, dst, 9, 3, 3, BorderTypes.Default);
                        break;
                    case ImageFilter.FilterScharr:
                        Cv2.Scharr(blur, dst, MatType.CV_32F, 1, 0, scale: 1, delta: 0, BorderTypes.Default);
                        dst.ConvertTo(dst, MatType.CV_8UC1);
                        break;
                    case ImageFilter.FilterLaplacian:
                        Cv2.Laplacian(blur, dst, MatType.CV_32F, ksize: 3, scale: 1, delta: 0, BorderTypes.Default);
                        dst.ConvertTo(dst, MatType.CV_8UC1);
                        break;
                    case ImageFilter.FilterCanny:
                        Cv2.Canny(blur, dst, 100, 200, 3, true);
                        break;


                }

                // OpenCvSharp의 Mat을 Bitmap으로 변환하여 PictureBox에 출력
                DstPictureBox.Image = BitmapConverter.ToBitmap(dst);
                isImageOp = true;

                // 연산 결과를 TextBox에 출력 (예: Mat의 픽셀 평균값)
                Scalar mean = Cv2.Mean(dst); // Mat의 평균값 계산
                textBox1.Text = $"Mean: {mean.Val0:F2}, {mean.Val1:F2}, {mean.Val2:F2}";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"필터 적용 중 오류가 발생했습니다:\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}







