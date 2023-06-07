using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AGooday.AgPay.Manager.Api.Controllers.QrCode
{
    [ApiController, AllowAnonymous]
    [Route("api/qrc")]
    public class QrCodeController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public QrCodeController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet, Route("shell/stylea.png")]
        public IActionResult StyleA()
        {
            // 设置图片宽高和边距
            int width = 1190;
            int height = 1684;
            int leftMargin = (int)(width * 0.1);
            int topMargin = (int)(width * 0.3);
            int bottomMargin = (int)(width * 0.1);

            // 创建位图对象
            Bitmap bmp = new Bitmap(width, height);

            // 创建画布对象
            Graphics g = Graphics.FromImage(bmp);

            // 设置画布背景色为红色
            g.Clear(Color.Red);

            //// 绘制大标题文本
            //string title = "这里是大标题";
            //Font font = new Font("Arial", 48, FontStyle.Bold);
            //SizeF titleSize = g.MeasureString(title, font);
            //PointF textPos = new PointF(width / 2 - titleSize.Width / 2, topMargin / 2 - titleSize.Height / 2);
            //g.DrawString(title, font, Brushes.White, textPos);

            string logoPath = Path.Combine(_env.WebRootPath, "images", "jeepay.png");
            // 加载logo图片
            Image logo = Image.FromFile(logoPath);
            // 计算logo的位置和大小
            int logoWidth = logo.Width;
            int logoHeight = logo.Height;
            int logoLeft = (width - logoWidth) / 2;
            int logoTop = (topMargin - logoHeight) / 2;

            // 在画布上绘制logo
            g.DrawImage(logo, logoLeft, logoTop, logoWidth, logoHeight);

            // 创建白色圆角矩形路径
            GraphicsPath path = new GraphicsPath();
            Rectangle rect = new Rectangle(leftMargin, topMargin, width - leftMargin * 2, height - topMargin - bottomMargin);
            int cornerRadius = 50;
            int diameter = cornerRadius * 2;
            Rectangle arcRect = new Rectangle(rect.X, rect.Y, diameter, diameter);
            path.AddArc(arcRect, 180, 90); // 左上角
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90); // 右上角
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90);   // 右下角
            arcRect.X = rect.X;
            path.AddArc(arcRect, 90, 90);  // 左下角
            path.CloseFigure();

            // 填充白色圆角矩形路径
            g.FillPath(Brushes.White, path);

            // 分割成上下两部分
            Rectangle topRect = new Rectangle(leftMargin, topMargin, width - leftMargin * 2, width - leftMargin * 2);
            // 创建下面部分圆角矩形路径
            GraphicsPath topPath = new GraphicsPath();
            arcRect = new Rectangle(topRect.X, topRect.Y, diameter, diameter);

            // 圆角
            //topPath.AddArc(arcRect, 180, 90);// 左上角
            //arcRect.X = topRect.Right - diameter;
            //topPath.AddArc(arcRect, 270, 90);// 右上角
            //arcRect.Y = topRect.Bottom - diameter;
            //topPath.AddArc(arcRect, 0, 90); // 右下角
            //arcRect.X = topRect.X;
            //topPath.AddArc(arcRect, 90, 90);// 左下角

            // 左上角和右上角圆角，右下角和左下角直角
            topPath.AddArc(arcRect, 180, 90); // 左上角 
            arcRect.X = topRect.Right - diameter;
            topPath.AddArc(arcRect, 270, 90); // 右上角
            topPath.AddLine(topRect.Right, topRect.Top, topRect.Right, topRect.Bottom); // 直线段
            arcRect.Y = topRect.Bottom - diameter;
            topPath.AddLine(topRect.Right, topRect.Bottom, topRect.Left, topRect.Bottom); // 直线段
            arcRect.X = topRect.X;
            topPath.AddLine(topRect.Left, topRect.Bottom, topRect.Left, topRect.Top); // 直线段

            //// 左上角圆角
            //topPath.AddArc(arcRect, 180, 90);
            //// 右上角圆角
            //arcRect.X = topRect.Right - diameter;
            //topPath.AddArc(arcRect, 270, 90);
            //// 右下角直角
            //topPath.AddLine(topRect.Right, topRect.Bottom - diameter, topRect.Right, topRect.Bottom);
            //topPath.AddLine(topRect.Right, topRect.Bottom, topRect.Left + diameter, topRect.Bottom);
            //// 左下角直角
            //topPath.AddLine(topRect.Left + diameter, topRect.Bottom, topRect.Left, topRect.Bottom);
            //topPath.AddLine(topRect.Left, topRect.Bottom, topRect.Left, topRect.Y + diameter);

            topPath.CloseFigure();
            g.FillPath(Brushes.White, topPath);

            Rectangle bottomRect = new Rectangle(leftMargin, topRect.Bottom, width - leftMargin * 2, height - topRect.Bottom - bottomMargin);
            GraphicsPath bottomPath = new GraphicsPath();
            arcRect = new Rectangle(bottomRect.X, bottomRect.Y, diameter, diameter);

            // 圆角
            //bottomPath.AddArc(arcRect, 180, 90);
            //arcRect.X = bottomRect.Right - diameter;
            //bottomPath.AddArc(arcRect, 270, 90);
            //arcRect.Y = bottomRect.Bottom - diameter;
            //bottomPath.AddArc(arcRect, 0, 90);
            //arcRect.X = bottomRect.X;
            //bottomPath.AddArc(arcRect, 90, 90);

            // 左上角和右上角直角，右下角和左下角圆角
            //bottomPath.AddArc(arcRect, 180, 0); // 左上角
            //bottomPath.AddLine(bottomRect.Left, bottomRect.Top, bottomRect.Right, bottomRect.Top); // 直线段
            //arcRect.X = bottomRect.Right - diameter;
            //bottomPath.AddArc(arcRect, 270, 0); // 右上角
            //bottomPath.AddLine(bottomRect.Right, bottomRect.Top, bottomRect.Right, bottomRect.Bottom); // 直线段
            //arcRect.Y = bottomRect.Bottom - diameter;
            //bottomPath.AddArc(arcRect, 0, 90); // 右下角
            //arcRect.X = bottomRect.X;
            //bottomPath.AddArc(arcRect, 90, 90); // 左下角

            bottomPath.AddLine(bottomRect.Left, bottomRect.Top, bottomRect.Right, bottomRect.Top); // 直线段
            arcRect.X = bottomRect.Right - diameter;
            bottomPath.AddLine(bottomRect.Right, bottomRect.Top, bottomRect.Right, bottomRect.Bottom); // 直线段 
            arcRect.Y = bottomRect.Bottom - diameter;
            bottomPath.AddArc(arcRect, 0, 90); // 右下角
            arcRect.X = bottomRect.X;
            bottomPath.AddArc(arcRect, 90, 90); // 左下角

            bottomPath.CloseFigure();
            g.FillPath(Brushes.LightPink, bottomPath);

            // 计算二维码位置和大小
            int qrSize = 900;
            int qrLeft = (width - qrSize) / 2;
            int qrTop = topMargin + (width - leftMargin * 2 - qrSize) / 2;

            // 创建二维码对象
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode($"https://www.example.com", QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            // 将二维码绘制到画布上
            int qrModuleSize = qrSize / qrCodeData.ModuleMatrix.Count;
            for (int x = 0; x < qrCodeData.ModuleMatrix.Count; x++)
            {
                for (int y = 0; y < qrCodeData.ModuleMatrix.Count; y++)
                {
                    if (qrCodeData.ModuleMatrix[x][y])
                    {
                        Rectangle qrRect = new Rectangle(qrLeft + x * qrModuleSize, qrTop + y * qrModuleSize, qrModuleSize, qrModuleSize);
                        g.FillRectangle(Brushes.Black, qrRect);
                    }
                }
            }

            // 在画布上绘制文字
            string text = "No.220101000001";
            SizeF textSize = g.MeasureString(text, new Font("Arial", 36));
            int textLeft = (int)((bottomPath.GetBounds().Width - textSize.Width) / 2 + bottomPath.GetBounds().X);
            int textTop = (int)(bottomPath.GetBounds().Bottom + (topRect.Height - bottomPath.GetBounds().Bottom - textSize.Height) / 2);
            g.DrawString(text, new Font("Arial", 36), Brushes.Black, textLeft, textTop);

            var payTypes = new List<PayType>() {
                new PayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "unionpay.png"), Alias = "银联", Name="unionpay"  },
                new PayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "ysfpay.png"), Alias = "云闪付", Name="ysfpay" },
                new PayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "wxpay.png"), Alias = "微信", Name="wxpay"  },
                new PayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "alipay.png"), Alias = "支付宝", Name="alipay" },
            };

            int payTypeWidth = 100; // 每个大小
            int padding = 50; // 每个LOGO之间间距
            int payTypeCount = payTypes.Count;
            int freeWidth = (int)bottomPath.GetBounds().Width - (payTypeWidth + padding) * (payTypeCount - 1);
            int index = 0;
            foreach (var item in payTypes)
            {
                logoPath = item.ImgUrl;
                logo = Image.FromFile(logoPath);

                logoWidth = logo.Width;
                logoHeight = logo.Height;

                // 计算每个LOGO的位置
                if (index == 0)
                {
                    logoLeft = (freeWidth - logoWidth) / 2;
                }
                else
                {
                    logoLeft = (int)(bottomPath.GetBounds().Width - (payTypeWidth + padding) * (payTypeCount - index - 1) - logoWidth) / 2 + (payTypeWidth + padding) * index;
                }

                logoTop = (int)(bottomPath.GetBounds().Y - logoHeight) / 2;

                g.DrawImage(logo, logoLeft, logoTop + 780, logoWidth, logoHeight);

                index++;
            }

            // 释放资源
            g.Dispose();

            // 将位图对象转换为 PNG 格式并输出到响应流
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            bmp.Dispose();
            return File(ms.ToArray(), "image/png");
        }

        [HttpGet, Route("shell/styleb.png")]
        public IActionResult StyleB()
        {
            // 设置图片宽高和边距
            int width = 1190;
            int height = 1684;
            int leftMargin = (int)(width * 0.1);
            int topMargin = (int)(width * 0.3);
            int bottomMargin = (int)(width * 0.1);

            // 创建位图对象
            Bitmap bmp = new Bitmap(width, height);

            // 创建画布对象
            Graphics g = Graphics.FromImage(bmp);

            // 设置画布背景色为红色
            g.Clear(Color.Red);

            // 绘制大标题文本
            string title = "这里是大标题";
            Font font = new Font("Arial", 48, FontStyle.Bold);
            SizeF titleSize = g.MeasureString(title, font);
            PointF textPos = new PointF(width / 2 - titleSize.Width / 2, topMargin / 2 - titleSize.Height / 2);
            g.DrawString(title, font, Brushes.Black, textPos);

            // 创建白色圆角矩形路径
            GraphicsPath path = new GraphicsPath();
            Rectangle rect = new Rectangle(leftMargin, topMargin, width - leftMargin * 2, height - topMargin - bottomMargin);
            int cornerRadius = 50;
            int diameter = cornerRadius * 2;
            Rectangle arcRect = new Rectangle(rect.X, rect.Y, diameter, diameter);
            path.AddArc(arcRect, 180, 90);// 左上角
            arcRect.X = rect.Right - diameter;
            path.AddArc(arcRect, 270, 90);// 右上角
            arcRect.Y = rect.Bottom - diameter;
            path.AddArc(arcRect, 0, 90); // 右下角
            arcRect.X = rect.X;
            path.AddArc(arcRect, 90, 90);// 左下角
            path.CloseFigure();

            // 填充白色圆角矩形路径
            g.FillPath(Brushes.White, path);

            // 分割成上下两部分
            // Rectangle topRect = new Rectangle(leftMargin + cornerRadius, topMargin, width - leftMargin * 2 - cornerRadius * 2, width - leftMargin * 2 - cornerRadius * 2);
            Rectangle topRect = new Rectangle(leftMargin, topMargin, width - leftMargin * 2, width - leftMargin * 2);
            // 创建下面部分圆角矩形路径
            GraphicsPath topPath = new GraphicsPath();
            arcRect = new Rectangle(topRect.X, topRect.Y, diameter, diameter);
            topPath.AddArc(arcRect, 180, 90);// 左上角
            arcRect.X = topRect.Right - diameter;
            topPath.AddArc(arcRect, 270, 90);// 右上角
            arcRect.Y = topRect.Bottom - diameter;
            topPath.AddArc(arcRect, 0, 90); // 右下角
            arcRect.X = topRect.X;
            topPath.AddArc(arcRect, 90, 90);// 左下角
            topPath.CloseFigure();
            // g.FillRectangle(Brushes.Yellow, topRect);
            g.FillPath(Brushes.Yellow, topPath);

            Rectangle bottomRect = new Rectangle(leftMargin, topRect.Bottom, width - leftMargin * 2, height - topRect.Bottom - bottomMargin);
            // g.FillRectangle(Brushes.LightPink, bottomRect);
            // 创建下面部分圆角矩形路径
            GraphicsPath bottomPath = new GraphicsPath();
            arcRect = new Rectangle(bottomRect.X, bottomRect.Y, diameter, diameter);
            bottomPath.AddArc(arcRect, 180, 90);
            arcRect.X = bottomRect.Right - diameter;
            bottomPath.AddArc(arcRect, 270, 90);
            arcRect.Y = bottomRect.Bottom - diameter;
            bottomPath.AddArc(arcRect, 0, 90);
            arcRect.X = bottomRect.X;
            bottomPath.AddArc(arcRect, 90, 90);
            bottomPath.CloseFigure();
            // 填充上下两部分的背景
            g.FillPath(Brushes.LightPink, bottomPath);

            // 计算二维码位置和大小
            int qrSize = 700;
            int qrLeft = (width - qrSize) / 2;
            int qrTop = topRect.Top + (topRect.Height - qrSize) / 2;

            // 在画布上绘制二维码
            Bitmap qrCode = GenerateQRCode("http://www.example.com/", qrSize);
            g.DrawImage(qrCode, qrLeft, qrTop, qrSize, qrSize);

            // 释放二维码资源
            qrCode.Dispose();
            
            // 在画布上绘制文字
            string text = "No.220101000001";
            SizeF textSize = g.MeasureString(text, new Font("Arial", 36));
            int textLeft = (int)((bottomPath.GetBounds().Width - textSize.Width) / 2 + bottomPath.GetBounds().X);
            int textTop = (int)(bottomPath.GetBounds().Bottom + (topRect.Height - bottomPath.GetBounds().Bottom - textSize.Height) / 2);
            g.DrawString(text, new Font("Arial", 36), Brushes.Black, textLeft, textTop);

            //// 在画布上绘制文字
            //string text = "扫码关注公众号";
            //SizeF textSize = g.MeasureString(text, new Font("Arial", 24));
            //int textLeft = (int)((width - textSize.Width) / 2);
            //int textTop = topRect.Bottom + (int)((bottomMargin - textSize.Height) / 2);
            //g.DrawString(text, new Font("Arial", 24), Brushes.Black, textLeft, textTop);

            //// 在画布上绘制文字
            //string text = "扫码关注公众号";
            //SizeF textSize = g.MeasureString(text, new Font("Arial", 24));
            //int textLeft = (int)((width - textSize.Width) / 2);
            //int textTop = topRect.Bottom + (int)((height - topRect.Bottom - textSize.Height) / 2);
            //g.DrawString(text, new Font("Arial", 24), Brushes.Black, textLeft, textTop);

            //// 保存图片
            //bmp.Save("output.png", ImageFormat.Png);

            //// 释放资源
            //g.Dispose();
            //bmp.Dispose();

            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            g.Dispose();
            bmp.Dispose();

            //Color backgroundColor = Color.Red;
            //int cornerRadius = 50;

            //var bitmap = QrDraw.GenerateImage(1190, 1684, backgroundColor, cornerRadius);
            //var ms = new MemoryStream();
            //bitmap.Save(ms, ImageFormat.Png);

            return File(ms.GetBuffer(), "image/png");
        }

        private Bitmap GenerateQRCode(string content, int size)
        {
            // TODO: 使用第三方二维码库或微信支付官方接口生成二维码
            // 这里使用纯黑色填充一个正方形作为示例
            Bitmap bmp = new Bitmap(size, size);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            g.FillRectangle(Brushes.Black, new Rectangle(0, 0, size, size));
            g.Dispose();
            return bmp;
        }
    }

    public class PayType
    {
        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
    }

    public static class QrDraw
    {
        public static Bitmap GenerateImage(int width, int height, Color backgroundColor, int cornerRadius)
        {
            // Create the image
            Bitmap image = new Bitmap(width, height);

            // Create the graphics object
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(backgroundColor);


            // Create the rectangle for the white rounded rectangle in the middle
            int middleWidth = width - 238;
            int middleHeight = height - 238;
            int middleX = (width - middleWidth) / 2;
            int middleY = (height - middleHeight) / 2 + 238;
            Rectangle middleRectangle = new Rectangle(x: middleX, middleY, middleWidth, middleHeight - 238);

            // Draw the white rounded rectangle in the middle
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillRoundedRectangle(Brushes.White, middleRectangle, cornerRadius);
            graphics.DrawRoundedRectangle(Pens.White, middleRectangle, cornerRadius);

            // Save the image to a file
            //image.Save(outputPath, ImageFormat.Png);
            return image;
        }

        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle rectangle, int cornerRadius)
        {
            GraphicsPath path = CreateRoundedRectangle(rectangle, cornerRadius);
            graphics.FillPath(brush, path);
        }

        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle rectangle, int cornerRadius)
        {
            GraphicsPath path = CreateRoundedRectangle(rectangle, cornerRadius);
            graphics.DrawPath(pen, path);
        }

        public static GraphicsPath CreateRoundedRectangle(Rectangle rectangle, int cornerRadius)
        {
            GraphicsPath path = new GraphicsPath();

            int diameter = cornerRadius * 2;
            Rectangle arcRectangle = new Rectangle(rectangle.X, rectangle.Y, diameter, diameter);

            // Top-left corner
            path.AddArc(arcRectangle, 180, 90);

            // Top-right corner
            arcRectangle.X = rectangle.Right - diameter;
            path.AddArc(arcRectangle, 270, 90);

            // Bottom-right corner
            arcRectangle.Y = rectangle.Bottom - diameter;
            path.AddArc(arcRectangle, 0, 90);

            // Bottom-left corner
            arcRectangle.X = rectangle.Left;
            path.AddArc(arcRectangle, 90, 90);

            path.CloseFigure();

            return path;
        }
    }
}
