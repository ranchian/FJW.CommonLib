using System;
using System.IO;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;

namespace FJW.CommonLib.Utils
{
    /// <summary>
    /// 水印位置枚举
    /// </summary>
    public enum WaterMarkStatus
    {
        /// <summary>
        /// 左上
        /// </summary>
        LeftTop = 1,
        /// <summary>
        /// 中上
        /// </summary>
        CenterTop = 2,
        /// <summary>
        /// 右上
        /// </summary>
        RightTop = 3,
        /// <summary>
        /// 左中
        /// </summary>
        LeftCenter = 4,
        /// <summary>
        /// 中间
        /// </summary>
        CenterCenter = 5,
        /// <summary>
        /// 右中
        /// </summary>
        RightCenter = 6,
        /// <summary>
        /// 左下
        /// </summary>
        LeftBottom = 7,
        /// <summary>
        /// 下中
        /// </summary>
        CenterBottom = 8,
        /// <summary>
        /// 右下
        /// </summary>
        RightBottom = 9
    }

    /// <summary>
    /// 图片帮助类
    /// </summary>
    public static class ImageHelper
    {
        #region 图片格式转换
        /// <summary>
        /// 图片格式转换
        /// </summary>
        /// <param name="oriFilename">原始文件相对路径</param>
        /// <param name="desiredFilename">生成目标文件相对路径</param>
        /// <returns></returns>
        ///  JPG采用的是有损压缩所以JPG图像有可能会降低图像清晰度，而像素是不会降低的   
        ///  GIF采用的是无损压缩所以GIF图像是不会降低原图图像清晰度和像素的，但是GIF格式只支持256色图像。
        public static bool ConvertImage(string oriFilename, string desiredFilename)
        {
            var extname = desiredFilename.Substring(desiredFilename.LastIndexOf('.') + 1).ToLower();
            ImageFormat desiredFormat;
            //根据扩张名，指定ImageFormat
            switch (extname)
            {
                case "bmp":// Windows系统下的标准位图格式，使用很普遍。其结构简单，未经过压缩，一般图像文件会比较大。它最大的好处就是能被大多数软件“接受”，可称为通用格式
                    desiredFormat = ImageFormat.Bmp;
                    break;
                case "gif":// 分为静态GIF和动画GIF两种，支持透明背景图像，适用于多种操作系统，“体型”很小，网上很多小动画都是GIF格式。其实GIF是将多幅图像保存为一个图像文件，从而形成动画,所以归根到底GIF仍然是图片文件格式。 GIF格式的图片可以用Photoshop的配套软件ImageReady制作.转换可以使用ps或者fireworks.
                    desiredFormat = ImageFormat.Gif;
                    break;
                case "jpeg":// 应用最广泛的图片格式之一，它采用一种特殊的有损压缩算法，将不易被人眼察觉的图像颜色删除，从而达到较大的压缩比(可达到2:1甚至40:1)，所以“身材娇小，容貌姣好”，特别受网络青睐,很实用! 
                    desiredFormat = ImageFormat.Jpeg;
                    break;
                case "ico":// Windows的图标文件格式的一种，可以存储单个图案、多尺寸、多色板的图标文件
                    desiredFormat = ImageFormat.Icon;
                    break;
                case "png":// 与JPG格式类似,网页中有很多图片都是这种格式，压缩比高于GIF，支持图像透明，可以利用Alpha通道调节图像的透明度,文件会比JPEG格式大一些
                    desiredFormat = ImageFormat.Png;
                    break;
                default:
                    desiredFormat = ImageFormat.Jpeg;
                    break;
            }
            try
            {
                var imgFile = Image.FromFile(WebPathTran(oriFilename));
                imgFile.Save(WebPathTran(desiredFilename), desiredFormat);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 图片缩放
        /// <summary>
        /// 图片固定大小缩放
        /// </summary>
        /// <param name="oriFileName">源文件相对地址</param>
        /// <param name="desiredFilename">目标文件相对地址</param>
        /// <param name="intWidth">目标文件宽</param>
        /// <param name="intHeight">目标文件高</param>
        /// <param name="imageFormat">图片文件格式</param>
        public static bool ChangeImageSize(string oriFileName, string desiredFilename, int intWidth, int intHeight, ImageFormat imageFormat)
        {
            //目的图片名称路径
            var transferFileNameStr = WebPathTran(desiredFilename);
            FileStream myOutput = null;
            try
            {
                var myAbort = new Image.GetThumbnailImageAbort(ImageAbort);
                //来源图片定义
                var sourceImage = Image.FromFile(oriFileName);
                //目的图片定义
                var targetImage = sourceImage.GetThumbnailImage(intWidth, intHeight, myAbort, IntPtr.Zero);
                //将TargetFileNameStr的图片放宽为IntWidth，高为IntHeight 
                myOutput = new FileStream(transferFileNameStr, FileMode.Create, FileAccess.Write, FileShare.Write);
                targetImage.Save(myOutput, imageFormat);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (myOutput != null) myOutput.Close();
            }
        }

        private static bool ImageAbort()
        {
            return false;
        }
        #endregion

        #region 文字水印
        /// <summary>
        /// 文字水印
        /// </summary>
        /// <param name="wtext">水印文字</param>
        /// <param name="oriFilename">原图片物理文件名</param>
        /// <param name="desiredFilename">生成图片物理文件名</param>
        /// <param name="fontname">字体名称</param>
        /// <param name="fontsize">字体大小</param>
        /// <param name="quality">图片像素质量</param>
        /// <param name="watermarkStatus">水印位置</param>
        public static bool ImageWaterText(string wtext, string oriFilename, string desiredFilename, WaterMarkStatus watermarkStatus, int quality = 80, string fontname = "Verdana", int fontsize = 12)
        {
            var img = Image.FromFile(oriFilename);
            var graphics = Graphics.FromImage(img);
            Font drawFont = new Font(fontname, fontsize, FontStyle.Regular, GraphicsUnit.Pixel);
            SizeF crSize = graphics.MeasureString(wtext, drawFont);
            float xpos = 0;
            float ypos = 0;

            #region
            switch (watermarkStatus)
            {
                case WaterMarkStatus.LeftTop:
                    xpos = (float)img.Width * (float).01;
                    ypos = (float)img.Height * (float).01;
                    break;
                case WaterMarkStatus.CenterTop:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (float)img.Height * (float).01;
                    break;
                case WaterMarkStatus.RightTop:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = (float)img.Height * (float).01;
                    break;
                case WaterMarkStatus.LeftCenter:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case WaterMarkStatus.CenterCenter:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case WaterMarkStatus.RightCenter:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case WaterMarkStatus.LeftBottom:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case WaterMarkStatus.CenterBottom:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case WaterMarkStatus.RightBottom:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
            }
            #endregion

            try
            {
                graphics.DrawString(wtext, drawFont, new SolidBrush(Color.White), xpos + 1, ypos + 1);
                graphics.DrawString(wtext, drawFont, new SolidBrush(Color.Black), xpos, ypos);
                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo ici = null;
                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.MimeType.IndexOf("jpeg") > -1)
                        ici = codec;
                }
                EncoderParameters encoderParams = new EncoderParameters();
                long[] qualityParam = new long[1];
                if (quality < 0 || quality > 100)
                    quality = 80;

                qualityParam[0] = quality;

                EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
                encoderParams.Param[0] = encoderParam;

                if (ici != null)
                    img.Save(desiredFilename, ici, encoderParams);
                else
                    img.Save(desiredFilename);
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                graphics.Dispose();
                img.Dispose();
            }
        }

        /// <summary>
        /// 文字水印
        /// </summary>
        /// <param name="wtext">水印文字</param>
        /// <param name="buffer">原图片字节流</param>
        /// <param name="fontname">字体名称</param>
        /// <param name="fontsize">字体大小</param>
        /// <param name="quality">图片像素质量</param>
        /// <param name="watermarkStatus">水印位置</param>
        /// <returns></returns>
        public static byte[] ImageWaterText(string wtext, byte[] buffer, WaterMarkStatus watermarkStatus, int quality = 80, string fontname = "Verdana", int fontsize = 12)
        {
            MemoryStream ms = new MemoryStream(buffer);
            var img = Image.FromStream(ms);
            var graphics = Graphics.FromImage(img);
            Font drawFont = new Font(fontname, fontsize, FontStyle.Regular, GraphicsUnit.Pixel);
            SizeF crSize = graphics.MeasureString(wtext, drawFont);
            float xpos = 0;
            float ypos = 0;

            #region
            switch (watermarkStatus)
            {
                case WaterMarkStatus.LeftTop:
                    xpos = (float)img.Width * (float).01;
                    ypos = (float)img.Height * (float).01;
                    break;
                case WaterMarkStatus.CenterTop:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (float)img.Height * (float).01;
                    break;
                case WaterMarkStatus.RightTop:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = (float)img.Height * (float).01;
                    break;
                case WaterMarkStatus.LeftCenter:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case WaterMarkStatus.CenterCenter:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case WaterMarkStatus.RightCenter:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case WaterMarkStatus.LeftBottom:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case WaterMarkStatus.CenterBottom:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case WaterMarkStatus.RightBottom:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
            }
            #endregion

            try
            {
                graphics.DrawString(wtext, drawFont, new SolidBrush(Color.White), xpos + 1, ypos + 1);
                graphics.DrawString(wtext, drawFont, new SolidBrush(Color.Black), xpos, ypos);
                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo ici = null;
                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.MimeType.IndexOf("jpeg") > -1)
                        ici = codec;
                }
                EncoderParameters encoderParams = new EncoderParameters();
                long[] qualityParam = new long[1];
                if (quality < 0 || quality > 100)
                    quality = 80;

                qualityParam[0] = quality;

                EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
                encoderParams.Param[0] = encoderParam;

                using (MemoryStream saveSr = new MemoryStream())
                {
                    if (ici != null)
                        img.Save(saveSr, ici, encoderParams);
                    else
                        img.Save(saveSr, img.RawFormat);

                    byte[] result = new byte[saveSr.Length];
                    saveSr.Seek(0, SeekOrigin.Begin);
                    saveSr.Read(result, 0, result.Length);
                    return result;
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                ms.Dispose();
                graphics.Dispose();
                img.Dispose();
            }
        }
        #endregion

        #region 图片水印
        /// <summary>
        /// 在图片上生成图片水印
        /// </summary>
        /// <param name="oriFilename">原图片物理文件名</param>
        /// <param name="desiredFilename">生成图片物理文件名</param>
        /// <param name="waterPicSource">水印图片物理文件名</param>
        /// <param name="watermarkStatus">水印图片的位置</param>
        /// <param name="quality">图片像素质量</param>
        public static bool ImageWaterPic(string oriFilename, string desiredFilename, string waterPicSource, WaterMarkStatus watermarkStatus, int quality = 80)
        {
            var sourceimage = Image.FromFile(oriFilename);
            var sourcegraphics = Graphics.FromImage(sourceimage);
            var waterPicSourceImage = Image.FromFile(waterPicSource);

            float xpos = 0;
            float ypos = 0;

            #region
            switch (watermarkStatus)
            {
                case WaterMarkStatus.LeftTop:
                    xpos = (float)sourceimage.Width * (float).01;
                    ypos = (float)sourceimage.Height * (float).01;
                    break;
                case WaterMarkStatus.CenterTop:
                    xpos = ((float)sourceimage.Width * (float).50) - (waterPicSourceImage.Width / 2);
                    ypos = (float)sourceimage.Height * (float).01;
                    break;
                case WaterMarkStatus.RightTop:
                    xpos = ((float)sourceimage.Width * (float).99) - waterPicSourceImage.Width;
                    ypos = (float)sourceimage.Height * (float).01;
                    break;
                case WaterMarkStatus.LeftCenter:
                    xpos = (float)sourceimage.Width * (float).01;
                    ypos = ((float)sourceimage.Height * (float).50) - (waterPicSourceImage.Height / 2);
                    break;
                case WaterMarkStatus.CenterCenter:
                    xpos = ((float)sourceimage.Width * (float).50) - (waterPicSourceImage.Width / 2);
                    ypos = ((float)sourceimage.Height * (float).50) - (waterPicSourceImage.Height / 2);
                    break;
                case WaterMarkStatus.RightCenter:
                    xpos = ((float)sourceimage.Width * (float).99) - waterPicSourceImage.Width;
                    ypos = ((float)sourceimage.Height * (float).50) - (waterPicSourceImage.Height / 2);
                    break;
                case WaterMarkStatus.LeftBottom:
                    xpos = (float)sourceimage.Width * (float).01;
                    ypos = ((float)sourceimage.Height * (float).99) - waterPicSourceImage.Height;
                    break;
                case WaterMarkStatus.CenterBottom:
                    xpos = ((float)sourceimage.Width * (float).50) - (waterPicSourceImage.Width / 2);
                    ypos = ((float)sourceimage.Height * (float).99) - waterPicSourceImage.Height;
                    break;
                case WaterMarkStatus.RightBottom:
                    xpos = ((float)sourceimage.Width * (float).99) - waterPicSourceImage.Width;
                    ypos = ((float)sourceimage.Height * (float).99) - waterPicSourceImage.Height;
                    break;
            }
            #endregion

            try
            {
                sourcegraphics.DrawImage(waterPicSourceImage, new Rectangle((int)xpos, (int)ypos, waterPicSourceImage.Width, waterPicSourceImage.Height), 0, 0, waterPicSourceImage.Width, waterPicSourceImage.Height, GraphicsUnit.Pixel);

                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo ici = null;
                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.MimeType.IndexOf("jpeg") > -1)
                        ici = codec;
                }
                EncoderParameters encoderParams = new EncoderParameters();
                long[] qualityParam = new long[1];
                if (quality < 0 || quality > 100)
                    quality = 80;

                qualityParam[0] = quality;

                EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
                encoderParams.Param[0] = encoderParam;

                if (ici != null)
                    sourceimage.Save(desiredFilename, ici, encoderParams);
                else
                    sourceimage.Save(desiredFilename);

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                sourcegraphics.Dispose();
                sourceimage.Dispose();
                waterPicSourceImage.Dispose();
            }
        }

        /// <summary>
        /// 在图片上生成图片水印
        /// </summary>
        /// <param name="oriPicBuf">原图片字节流</param>
        /// <param name="waterPicBuf">水印图片字节流</param>
        /// <param name="watermarkStatus">水印图片的位置</param>
        /// <param name="quality">图片像素质量</param>
        /// <returns></returns>
        public static byte[] ImageWaterPic(byte[] oriPicBuf, byte[] waterPicBuf, WaterMarkStatus watermarkStatus, int quality = 80)
        {
            MemoryStream oriMs = new MemoryStream(oriPicBuf);
            var sourceimage = Image.FromStream(oriMs);
            var sourcegraphics = Graphics.FromImage(sourceimage);
            MemoryStream waterMs = new MemoryStream(waterPicBuf);
            var waterPicSourceImage = Image.FromStream(waterMs);

            float xpos = 0;
            float ypos = 0;

            #region
            switch (watermarkStatus)
            {
                case WaterMarkStatus.LeftTop:
                    xpos = (float)sourceimage.Width * (float).01;
                    ypos = (float)sourceimage.Height * (float).01;
                    break;
                case WaterMarkStatus.CenterTop:
                    xpos = ((float)sourceimage.Width * (float).50) - (waterPicSourceImage.Width / 2);
                    ypos = (float)sourceimage.Height * (float).01;
                    break;
                case WaterMarkStatus.RightTop:
                    xpos = ((float)sourceimage.Width * (float).99) - waterPicSourceImage.Width;
                    ypos = (float)sourceimage.Height * (float).01;
                    break;
                case WaterMarkStatus.LeftCenter:
                    xpos = (float)sourceimage.Width * (float).01;
                    ypos = ((float)sourceimage.Height * (float).50) - (waterPicSourceImage.Height / 2);
                    break;
                case WaterMarkStatus.CenterCenter:
                    xpos = ((float)sourceimage.Width * (float).50) - (waterPicSourceImage.Width / 2);
                    ypos = ((float)sourceimage.Height * (float).50) - (waterPicSourceImage.Height / 2);
                    break;
                case WaterMarkStatus.RightCenter:
                    xpos = ((float)sourceimage.Width * (float).99) - waterPicSourceImage.Width;
                    ypos = ((float)sourceimage.Height * (float).50) - (waterPicSourceImage.Height / 2);
                    break;
                case WaterMarkStatus.LeftBottom:
                    xpos = (float)sourceimage.Width * (float).01;
                    ypos = ((float)sourceimage.Height * (float).99) - waterPicSourceImage.Height;
                    break;
                case WaterMarkStatus.CenterBottom:
                    xpos = ((float)sourceimage.Width * (float).50) - (waterPicSourceImage.Width / 2);
                    ypos = ((float)sourceimage.Height * (float).99) - waterPicSourceImage.Height;
                    break;
                case WaterMarkStatus.RightBottom:
                    xpos = ((float)sourceimage.Width * (float).99) - waterPicSourceImage.Width;
                    ypos = ((float)sourceimage.Height * (float).99) - waterPicSourceImage.Height;
                    break;
            }
            #endregion

            try
            {
                sourcegraphics.DrawImage(waterPicSourceImage, new Rectangle((int)xpos, (int)ypos, waterPicSourceImage.Width, waterPicSourceImage.Height), 0, 0, waterPicSourceImage.Width, waterPicSourceImage.Height, GraphicsUnit.Pixel);

                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo ici = null;
                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.MimeType.IndexOf("jpeg") > -1)
                        ici = codec;
                }
                EncoderParameters encoderParams = new EncoderParameters();
                long[] qualityParam = new long[1];
                if (quality < 0 || quality > 100)
                    quality = 80;

                qualityParam[0] = quality;

                EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
                encoderParams.Param[0] = encoderParam;

                using (MemoryStream saveSr = new MemoryStream())
                {
                    if (ici != null)
                        sourceimage.Save(saveSr, ici, encoderParams);
                    else
                        sourceimage.Save(saveSr, sourceimage.RawFormat);
                    byte[] result = new byte[saveSr.Length];
                    saveSr.Seek(0, SeekOrigin.Begin);
                    saveSr.Read(result, 0, result.Length);
                    return result;
                }
            }
            catch
            {
                return null;
            }
            finally
            {
                oriMs.Dispose();
                waterMs.Dispose();
                sourcegraphics.Dispose();
                sourceimage.Dispose();
                waterPicSourceImage.Dispose();
            }
        }
        #endregion

        #region 路径转换
        /// <summary>
        /// 路径转换（转换成绝对路径）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string WebPathTran(string path)
        {
            try
            {
                return HttpContext.Current.Server.MapPath(path);
            }
            catch
            {
                return path;
            }
        }
        #endregion
    }
}