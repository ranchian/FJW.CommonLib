using System;
using System.Text;
using System.Drawing;

namespace FJW.CommonLib.Utils
{
    /// <summary>
    /// 验证码帮助类
    /// </summary>
    public class VCodeHelper
    {
        /// <summary>
        /// 产生验证码
        /// </summary>
        /// <param name="codeLength">产生验证码长度位数</param>       
        /// <param name="types">验证码类型 0(0-9之间的验证码) 1(a-z之间验证码) 2(A-Z之间验证码) 3(0-9 a-z之间验证码) 4(0-9 A-Z之间验证码) 5(0-9 a-z A-Z之间验证码) 6 (中文验证码) 7(数值加中文) 8(a-z加中文) 9(A-Z加中文) 10(a-z Z-Z加中文) 11(a-z 0-9加中文) 12(A-Z 0-9加中文) 13(所有混合模式验证码)</param>
        /// <param name="isSinusoid">是否为图片添加 正弦曲线Wave扭曲 true添加</param>
        /// <param name="isBorder">是否为图片添加边框</param>
        /// <returns></returns>
        public static string CreateVerifyCodeImage(int codeLength, int types, bool isSinusoid = true, bool isBorder = true)
        {
            //返回中文字符集 中间用逗号隔开
            string outArray;

            var tmpCode = CreateCode(codeLength, types, out outArray);
            if (types == 6)
            {
                //产生中文字符
                CreateCheckCodeImage(outArray.Split(','));
            }
            else
            {
                CreateImage(tmpCode, isSinusoid, isBorder);
            }
            return tmpCode;
        }

        /// <summary>
        /// 高级方法输出验证码
        /// </summary>
        /// <param name="checkCode">验证码</param>
        /// <param name="isSinusoid">是否为图片添加 正弦曲线Wave扭曲 true添加</param>
        /// <param name="isBorder">是否为图片添加边框</param>
        /// <returns></returns>
        public static void CreateImages(string checkCode, bool isSinusoid = true, bool isBorder = true)
        {
            int iwidth = checkCode.Length * 15;

            Bitmap image = new Bitmap(iwidth, 25);

            Graphics g = Graphics.FromImage(image);

            g.Clear(Color.White);

            //定义颜色
            Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };

            string[] font = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };

            Brush backBrush = Brushes.DimGray;

            #region 随机输出噪音线

            int PenWidth1;

            for (int i = 0; i < 3; i++)
            {
                float x, y, x1, y1;

                Random rnd = new Random();

                x = image.Width * (float)rnd.NextDouble();

                y = image.Height * (float)rnd.NextDouble();

                x1 = image.Width * (float)rnd.NextDouble();

                y1 = image.Height * (float)rnd.NextDouble();

                PenWidth1 = Convert.ToInt32(2 * rnd.NextDouble()); //修改参数可获得不同的效果

                g.DrawLine(new Pen(backBrush, PenWidth1), x, y, x1, y1);

                //Random random = new Random();

                //int x1 = random.Next(image.Width);

                //int x2 = random.Next(image.Width);

                //int y1 = random.Next(image.Height);

                //int y2 = random.Next(image.Height);                

                //g.DrawLine(new Pen(Color.LightGray), x1, y1, x2, y2);
            }

            #endregion

            #region 随机输出噪点

            Random rand = new Random();

            for (int i = 0; i < 10; i++)
            {
                int x = rand.Next(image.Width);

                int y = rand.Next(image.Height);

                g.DrawRectangle(new Pen(Color.LightGray, 0), x, y, 1, 1);
            }

            #endregion

            #region 输出不同字体和颜色的验证码字符

            for (int i = 0; i < checkCode.Length; i++)
            {
                int cindex = rand.Next(7);

                int findex = rand.Next(5);

                Font f = new Font(font[findex], 10, FontStyle.Bold);

                Brush b = new SolidBrush(c[cindex]);

                int ii = 4;

                if ((i + 1) % 2 == 0)
                {
                    ii = 2;
                }

                g.DrawString(checkCode.Substring(i, 1), f, b, 3 + (i * 12), ii);
            }

            #endregion

            #region 扭曲图形

            if (isSinusoid)
            {
                Random rnd3d = new Random();

                double S3d = rnd3d.NextDouble();

                int Twist1, Twist2;

                if (S3d > 0.9)
                {
                    Twist1 = 0;

                    Twist2 = 0;
                }
                else
                {
                    Random rnd4 = new Random();

                    Twist1 = Convert.ToInt32(rnd4.NextDouble() * 3);  //扭曲参数随机生成

                    Twist2 = Convert.ToInt32(rnd4.NextDouble() * 2); //扭曲参数随机生成
                }

                image = TwistImage(image, true, -Twist1, -Twist2);

                image = TwistImage(image, true, Twist1, Twist2); //多扭曲几次也没关系，只是消耗服务器资源多些  

                //image = TwistImage(image, true, 3, 0.4);
            }

            #endregion

            #region 画一个边框

            if (isBorder)
            {
                g.DrawRectangle(new Pen(Color.Silver, 0), 0, 0, image.Width - 1, image.Height - 1);
            }

            #endregion

            #region 输出到浏览器

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

            System.Web.HttpContext.Current.Response.ClearContent();

            System.Web.HttpContext.Current.Response.ContentType = "image/Jpeg";

            System.Web.HttpContext.Current.Response.BinaryWrite(ms.ToArray());

            g.Dispose();

            image.Dispose();

            #endregion
        }

        /// <summary>
        /// 产生验证码
        /// </summary>
        /// <param name="codeLength">验证码长度</param>
        /// <param name="types">验证码类型 0(0-9之间的验证码) 1(a-z之间验证码) 2(A-Z之间验证码) 3(0-9 a-z之间验证码) 4(0-9 A-Z之间验证码) 5(0-9 a-z A-Z之间验证码) 6 (中文验证码) 7(数值加中文) 8(a-z加中文) 9(A-Z加中文) 10(a-z Z-Z加中文) 11(a-z 0-9加中文) 12(A-Z 0-9加中文) 13(所有混合模式验证码)</param>
        /// <param name="outArray">返回out函数值</param>
        /// <returns></returns>
        private static string CreateCode(int codeLength, int types, out string outArray)
        {
            var codeStr = "";

            switch (types)
            {
                case 0://(0-9之间的验证码)
                    codeStr = "1,2,3,4,5,6,7,8,9,0";
                    break;
                case 1://(a-z之间验证码)
                    codeStr = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
                    break;
                case 2://(A-Z之间验证码)
                    codeStr = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
                    break;
                case 3://(0-9 a-z之间验证码)
                    codeStr = "1,2,3,4,5,6,7,8,9,0,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
                    break;
                case 4://(0-9 A-Z之间验证码)
                    codeStr = "1,2,3,4,5,6,7,8,9,0,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
                    break;
                case 5://(0-9 a-z A-Z之间验证码)
                    codeStr = "1,2,3,4,5,6,7,8,9,0,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
                    break;
                case 6://中文验证码
                    codeStr = ChineseCodes(codeLength);
                    break;
                case 7://数值加中文
                    codeStr = "1,2,3,4,5,6,7,8,9,0," + ChineseCodes(codeLength);
                    break;
                case 8://a-z加中文
                    codeStr = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z," + ChineseCodes(codeLength);
                    break;
                case 9://A-Z加中文
                    codeStr = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z," + ChineseCodes(codeLength);
                    break;
                case 10://a-z Z-Z加中文
                    codeStr = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z," + ChineseCodes(codeLength);
                    break;
                case 11://a-z 0-9加中文
                    codeStr = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,1,2,3,4,5,6,7,8,9,0," + ChineseCodes(codeLength);
                    break;
                case 12://A-Z 0-9加中文
                    codeStr = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,1,2,3,4,5,6,7,8,9,0," + ChineseCodes(codeLength);
                    break;
                case 13://所有混合模式验证码
                    codeStr = "1,2,3,4,5,6,7,8,9,0,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z," + ChineseCodes(codeLength);
                    break;
                default:
                    codeStr = "1,2,3,4,5,6,7,8,9,0,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
                    break;
            }

            outArray = codeStr;//返回输出函数

            var strArr = codeStr.Split(',');

            var codeStrReturn = "";

            var rand = new Random();

            for (var i = 0; i < codeLength; i++)
            {
                if (types == 6)
                {
                    codeStrReturn += strArr[i];
                }
                else
                {
                    codeStrReturn += strArr[rand.Next(0, strArr.Length)];
                }
            }

            return codeStrReturn;
        }

        /// <summary>
        /// 产生中文验证码
        /// </summary>
        /// <param name="codeLength">长度</param>
        /// <returns></returns>
        private static string ChineseCodes(int codeLength)
        {
            var codeStrs = "";

            //获取GB2312编码页（表） 
            var gb = Encoding.GetEncoding("gb2312");

            //调用函数产生4个随机中文汉字编码 
            var bytes = CreateRegionCode(codeLength);

            var str = new string[codeLength];

            for (var i = 0; i < codeLength; i++)
            {
                str[i] = gb.GetString((byte[])Convert.ChangeType(bytes[i], typeof(byte[])));

                codeStrs += str[i] + ",";
            }

            if (codeStrs.Substring(codeStrs.Length - 1, 1) == ",")
            {
                codeStrs = codeStrs.Substring(0, codeStrs.Length - 1);
            }

            return codeStrs;
        }

        /// <summary>
        /// 生成中文字符
        /// </summary>
        /// <param name="checkCode"></param>
        private static void CreateCheckCodeImage(string[] checkCode)
        {
            if (checkCode == null || checkCode.Length <= 0)
                return;

            var image = new Bitmap((int)Math.Ceiling((checkCode.Length * 32.5)), 60);

            var g = Graphics.FromImage(image);

            try
            {
                //生成随机生成器 
                var random = new Random();

                //清空图片背景色 
                g.Clear(Color.White);

                //画图片的背景噪音线 
                for (var i = 0; i < 20; i++)
                {
                    var x1 = random.Next(image.Width);

                    var x2 = random.Next(image.Width);

                    var y1 = random.Next(image.Height);

                    var y2 = random.Next(image.Height);

                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                //定义颜色
                Color[] c = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
                //定义字体
                string[] f = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };

                for (var k = 0; k <= checkCode.Length - 1; k++)
                {
                    var cindex = random.Next(7);

                    var findex = random.Next(5);

                    var drawFont = new Font(f[findex], 16, (FontStyle.Bold));

                    var drawBrush = new SolidBrush(c[cindex]);

                    float x = 5.0F;

                    float y = 0.0F;

                    float width = 20.0F;

                    float height = 25.0F;

                    int sjx = random.Next(10);

                    int sjy = random.Next(image.Height - (int)height);

                    RectangleF drawRect = new RectangleF(x + sjx + (k * 25), y + sjy, width, height);

                    StringFormat drawFormat = new StringFormat();

                    drawFormat.Alignment = StringAlignment.Center;

                    g.DrawString(checkCode[k], drawFont, drawBrush, drawRect, drawFormat);
                }

                //画图片的前景噪音点 
                for (int i = 0; i < 100; i++)
                {
                    int x = random.Next(image.Width);

                    int y = random.Next(image.Height);

                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                //画图片的边框线 
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                System.Web.HttpContext.Current.Response.ClearContent();

                System.Web.HttpContext.Current.Response.ContentType = "image/Jpeg";

                System.Web.HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }

        /// <summary>
        /// 普通方法产生验证图片
        /// </summary>
        /// <param name="checkCode">验证码</param>
        /// <param name="IsSinusoid">是否为图片添加 正弦曲线Wave扭曲 true添加</param>
        /// <param name="IsBorder">是否为图片添加边框</param>
        /// <returns></returns>
        private static void CreateImage(string checkCode, bool IsSinusoid, bool IsBorder)
        {
            int iwidth = (int)(checkCode.Length * 15);

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(iwidth, 25);

            Graphics g = Graphics.FromImage(image);

            //WebColorConverter ww = new WebColorConverter();

            //g.Clear((Color)ww.ConvertFromString("#ffffff"));
            g.Clear(Color.White);

            //g.Clear((Color)ww.ConvertFromString("#FAE264"));

            Random random = new Random();

            #region 画图片的背景噪音线

            for (int i = 0; i < 12; i++)
            {
                int x1 = random.Next(image.Width);

                int x2 = random.Next(image.Width);

                int y1 = random.Next(image.Height);

                int y2 = random.Next(image.Height);

                g.DrawLine(new Pen(Color.LightGray), x1, y1, x2, y2);
            }

            #endregion

            Font font = new Font("Arial", 15, FontStyle.Bold | FontStyle.Italic);

            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.Gray, 1.2f, true);

            g.DrawString(checkCode, font, brush, 0, 0);

            #region 画图片的前景噪音点

            for (int i = 0; i < 10; i++)
            {
                int x = random.Next(image.Width);

                int y = random.Next(image.Height);

                image.SetPixel(x, y, Color.White);
            }

            #endregion

            #region 画图片的边框线

            if (IsBorder)
            {
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
            }

            #endregion

            #region 扭曲图形

            if (IsSinusoid)
            {
                Random rnd3d = new Random();

                double S3d = rnd3d.NextDouble();

                int Twist1, Twist2;

                if (S3d > 0.9)
                {
                    Twist1 = 0;

                    Twist2 = 0;
                }
                else
                {
                    Random rnd4 = new Random();

                    Twist1 = Convert.ToInt32(rnd4.NextDouble() * 3);  //扭曲参数随机生成

                    Twist2 = Convert.ToInt32(rnd4.NextDouble() * 2); //扭曲参数随机生成
                }

                image = TwistImage(image, true, -Twist1, -Twist2);

                image = TwistImage(image, true, Twist1, Twist2); //多扭曲几次也没关系，只是消耗服务器资源多些  

                //image = TwistImage(image, true, 3, 0.4);
            }

            #endregion

            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);

            System.Web.HttpContext.Current.Response.ClearContent();

            System.Web.HttpContext.Current.Response.ContentType = "image/Gif";

            System.Web.HttpContext.Current.Response.BinaryWrite(ms.ToArray());

            g.Dispose();

            image.Dispose();
        }


        /// <summary> 
        /// 正弦曲线Wave扭曲图片
        /// </summary>
        /// <param name="srcBmp">图片路径</param> 
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="nMultValue">波形的幅度倍数，越大扭曲的程度越高，一般为3</param>
        /// <param name="dPhase">波形的起始相位，取值区间[0-2*PI)</param> 
        /// <returns></returns>
        private static Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            double PI2 = 6.28318530717959;

            System.Drawing.Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);

            // 将位图背景填充为白色
            System.Drawing.Graphics graph = System.Drawing.Graphics.FromImage(destBmp);

            graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), 0, 0, destBmp.Width, destBmp.Height);

            graph.Dispose();

            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;

            for (int i = 1; i < destBmp.Width - 1; i++)
            {
                for (int j = 1; j < destBmp.Height - 1; j++)
                {
                    double dx = 0;

                    dx = bXDir ? (PI2 * (double)j) / dBaseAxisLen : (PI2 * (double)i) / dBaseAxisLen;

                    dx += dPhase;

                    double dy = Math.Sin(dx);

                    // 取得当前点的颜色  
                    int nOldX = 0, nOldY = 0;

                    nOldX = bXDir ? i + (int)(dy * dMultValue) : i;

                    nOldY = bXDir ? j : j + (int)(dy * dMultValue);

                    System.Drawing.Color color = srcBmp.GetPixel(i, j);

                    if (nOldX >= 0 && nOldX < destBmp.Width && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }

            return destBmp;
        }

        /// <summary>
        /// 此函数在汉字编码范围内随机创建含两个元素的十六进制字节数组，每个字节数组代表一个汉字，并将 四个字节数组存储在object数组中。 
        /// </summary>
        /// <param name="strlength">代表需要产生的汉字个数 </param>
        /// <returns></returns>
        private static object[] CreateRegionCode(int strlength)
        {
            //定义一个字符串数组储存汉字编码的组成元素 
            string[] rBase = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };

            //string blist = "1,2,3,4,5,6,7,8,9,0,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";

            Random rnd = new Random();

            //定义一个object数组用来 
            object[] bytes = new object[strlength];

            /**/
            /*每循环一次产生一个含两个元素的十六进制字节数组，并将其放入bject数组中 
             每个汉字有四个区位码组成 
             区位码第1位和区位码第2位作为字节数组第一个元素 
             区位码第3位和区位码第4位作为字节数组第二个元素 
            */
            for (int i = 0; i < strlength; i++)
            {
                //区位码第1位 
                int r1 = rnd.Next(11, 14);
                string str_r1 = rBase[r1].Trim();

                //区位码第2位 
                rnd = new Random(r1 * unchecked((int)DateTime.Now.Ticks) + i);//更换随机数发生器的种子避免产生重复值 
                int r2;
                if (r1 == 13)
                {
                    r2 = rnd.Next(0, 7);
                }
                else
                {
                    r2 = rnd.Next(0, 16);
                }
                string str_r2 = rBase[r2].Trim();

                //区位码第3位 
                rnd = new Random(r2 * unchecked((int)DateTime.Now.Ticks) + i);
                int r3 = rnd.Next(10, 16);
                string str_r3 = rBase[r3].Trim();

                //区位码第4位 
                rnd = new Random(r3 * unchecked((int)DateTime.Now.Ticks) + i);
                int r4;
                if (r3 == 10)
                {
                    r4 = rnd.Next(1, 16);
                }
                else if (r3 == 15)
                {
                    r4 = rnd.Next(0, 15);
                }
                else
                {
                    r4 = rnd.Next(0, 16);
                }
                string str_r4 = rBase[r4].Trim();

                //定义两个字节变量存储产生的随机汉字区位码 
                byte byte1 = Convert.ToByte(str_r1 + str_r2, 16);
                byte byte2 = Convert.ToByte(str_r3 + str_r4, 16);
                //将两个字节变量存储在字节数组中 
                byte[] strR = new byte[] { byte1, byte2 };

                //将产生的一个汉字的字节数组放入object数组中 
                bytes.SetValue(strR, i);

            }

            return bytes;
        }
    }
}