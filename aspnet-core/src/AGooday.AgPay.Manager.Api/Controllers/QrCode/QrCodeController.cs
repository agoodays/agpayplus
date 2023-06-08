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

        [HttpGet, Route("shell/nostyle.png")]
        public IActionResult NoStyle()
        {
            // 创建二维码对象
            Bitmap qrCodeImage = QrCodeBuilder.Generate(icon: new Bitmap(Path.Combine(_env.WebRootPath, "images", "avatar.png")));

            // 将位图对象转换为 PNG 格式并输出到响应流
            MemoryStream ms = new MemoryStream();
            qrCodeImage.Save(ms, ImageFormat.Png);
            qrCodeImage.Dispose();
            return File(ms.ToArray(), "image/png");
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

            Color color = ColorTranslator.FromHtml("#ff0000");
            g.Clear(color);

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
            int logoWidth = logo.Width > (width - (width * 0.1)) ? (int)(width - (width * 0.1)) : logo.Width;
            int logoHeight = logo.Height > (topMargin - (topMargin * 0.1)) ? (int)(topMargin - (topMargin * 0.1)) : logo.Height;
            int logoLeft = (width - logoWidth) / 2;
            int logoTop = (topMargin - logoHeight) / 2;

            // 在画布上绘制logo
            g.DrawImage(logo, logoLeft, logoTop, logoWidth, logoHeight);

            g.SmoothingMode = SmoothingMode.AntiAlias;
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

            Color newColor = ColorAdjuster.LightenColor(color, 0);
            SolidBrush brush = new SolidBrush(newColor);

            g.FillPath(brush, bottomPath);

            // 在画布上绘制二维码
            Bitmap qrCode = QrCodeBuilder.Generate(icon: new Bitmap(Path.Combine(_env.WebRootPath, "images", "avatar.png")));
            // 计算二维码位置和大小
            int qrSize = width - (leftMargin * 2) - cornerRadius;
            int qrLeft = (width - qrSize) / 2;
            int qrTop = topRect.Top + (topRect.Height - qrSize) / 2;
            g.DrawImage(qrCode, qrLeft, qrTop, qrSize, qrSize);

            //// 创建二维码对象
            //QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //QRCodeData qrCodeData = qrGenerator.CreateQrCode($"https://www.example.com", QRCodeGenerator.ECCLevel.Q);
            //QRCode qrCode = new QRCode(qrCodeData);

            //// 将二维码绘制到画布上
            //int qrModuleSize = qrSize / qrCodeData.ModuleMatrix.Count;
            //for (int x = 0; x < qrCodeData.ModuleMatrix.Count; x++)
            //{
            //    for (int y = 0; y < qrCodeData.ModuleMatrix.Count; y++)
            //    {
            //        if (qrCodeData.ModuleMatrix[x][y])
            //        {
            //            Rectangle qrRect = new Rectangle(qrLeft + x * qrModuleSize, qrTop + y * qrModuleSize, qrModuleSize, qrModuleSize);
            //            g.FillRectangle(Brushes.Black, qrRect);
            //        }
            //    }
            //}

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

            int payLogoWidth = (int)(width * 0.1);
            int payLogoHeight = (int)(width * 0.1);
            int payTypeCount = payTypes.Count;
            int padding = ((width - leftMargin * 2) - (int)(payTypeCount * (width * 0.1))) / (payTypeCount + 1); // 每个LOGO之间间距
            int index = 0;
            foreach (var item in payTypes)
            {
                string payLogoPath = item.ImgUrl;
                Image payLogo = Image.FromFile(payLogoPath);

                int payLogoLeft = (leftMargin + padding) + (payLogoWidth + padding) * index;

                int payLogoTop = (int)(topMargin + topPath.GetBounds().Height) + (int)(height - (topMargin + topPath.GetBounds().Height + bottomMargin) - payLogoHeight) / 2;

                g.DrawImage(payLogo, payLogoLeft, payLogoTop, payLogoWidth, payLogoHeight);

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
            g.DrawString(title, font, Brushes.White, textPos);

            g.SmoothingMode = SmoothingMode.AntiAlias;
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

            // 在画布上绘制二维码
            Bitmap qrCode = QrCodeBuilder.Generate(icon: new Bitmap(Path.Combine(_env.WebRootPath, "images", "avatar.png")));
            // 计算二维码位置和大小
            int qrSize = width - (leftMargin * 2) - cornerRadius;
            int qrLeft = (width - qrSize) / 2;
            int qrTop = topRect.Top + (topRect.Height - qrSize) / 2;
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

            return File(ms.GetBuffer(), "image/png");
        }

        [HttpGet, Route("shell/stylec.png")]
        public IActionResult StyleC()
        {
            Color backgroundColor = Color.Red;
            int cornerRadius = 50;
            var logoPath = Path.Combine(_env.WebRootPath, "images", "jeepay.png");

            var payTypes = new List<PayType>() {
                new PayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "unionpay.png"), Alias = "银联", Name="unionpay"  },
                new PayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "ysfpay.png"), Alias = "云闪付", Name="ysfpay" },
                new PayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "wxpay.png"), Alias = "微信", Name="wxpay"  },
                new PayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "alipay.png"), Alias = "支付宝", Name="alipay" },
            };

            var bitmap = DrawQrCode.GenerateImage(1190, 1684, backgroundColor, cornerRadius, logoPath, icon: new Bitmap(Path.Combine(_env.WebRootPath, "images", "avatar.png")), payTypes: payTypes);
            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            bitmap.Dispose();
            return File(ms.GetBuffer(), "image/png");
        }

        [HttpGet, Route("shell/styled.png")]
        public IActionResult StyleD()
        {
            // 设置图片宽高和边距
            int width = 1190;
            int height = 1684;
            int leftMargin = (int)(width * 0.1);
            int topMargin = (int)(width * 0.1);

            // 创建位图对象
            Bitmap bmp = new Bitmap(width, height);

            // 创建画布对象
            Graphics g = Graphics.FromImage(bmp);

            // 设置画布背景色为红色
            g.Clear(Color.Red);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            int cornerRadius = 50;
            int diameter = cornerRadius * 2;
            // 分割成上下两部分
            Rectangle topRect = new Rectangle(leftMargin, topMargin, width - leftMargin * 2, (int)((height - topMargin * 2) * 0.8));
            // 创建下面部分圆角矩形路径
            GraphicsPath topPath = new GraphicsPath();
            Rectangle arcRect = new Rectangle(topRect.X, topRect.Y, diameter, diameter);
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

            Rectangle bottomRect = new Rectangle(leftMargin, topRect.Bottom + ((int)(leftMargin / 2)), width - leftMargin * 2, (int)(height - topMargin - topRect.Bottom - ((int)(leftMargin / 2))));
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

            // 在画布上绘制二维码
            Bitmap qrCode = QrCodeBuilder.Generate(icon: new Bitmap(Path.Combine(_env.WebRootPath, "images", "avatar.png")));
            // 计算二维码位置和大小
            int qrSize = width - (leftMargin * 2) - cornerRadius;
            int qrLeft = (width - qrSize) / 2;
            int qrTop = diameter + topRect.Top + (topRect.Height - qrSize) / 2;
            g.DrawImage(qrCode, qrLeft, qrTop, qrSize, qrSize);

            // 释放二维码资源
            qrCode.Dispose();

            // 绘制大标题文本
            string title = "这里是大标题";
            Font font = new Font("Arial", 48, FontStyle.Bold);
            SizeF titleSize = g.MeasureString(title, font);
            PointF textPos = new PointF(width / 2 - titleSize.Width / 2, topMargin + diameter);
            g.DrawString(title, font, Brushes.Black, textPos);

            // 在画布上绘制文字
            string text = "No.220101000001";
            SizeF textSize = g.MeasureString(text, new Font("Arial", 36));
            int textLeft = (int)((bottomPath.GetBounds().Width - textSize.Width) / 2 + bottomPath.GetBounds().X);
            int textTop = (int)(topRect.Bottom - diameter);
            g.DrawString(text, new Font("Arial", 36), Brushes.Black, textLeft, textTop);

            var payTypes = new List<PayType>() {
                new PayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "unionpay.png"), Alias = "银联", Name="unionpay"  },
                new PayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "ysfpay.png"), Alias = "云闪付", Name="ysfpay" },
                new PayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "wxpay.png"), Alias = "微信", Name="wxpay"  },
                new PayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "alipay.png"), Alias = "支付宝", Name="alipay" },
            };

            int payLogoWidth = (int)(width * 0.1);
            int payLogoHeight = (int)(width * 0.1);
            int payTypeCount = payTypes.Count;
            int padding = ((width - leftMargin * 2) - (int)(payTypeCount * (width * 0.1))) / (payTypeCount + 1); // 每个LOGO之间间距
            int index = 0;
            foreach (var item in payTypes)
            {
                string payLogoPath = item.ImgUrl;
                Image payLogo = Image.FromFile(payLogoPath);

                int payLogoLeft = (leftMargin + padding) + (payLogoWidth + padding) * index;

                int payLogoTop = (int)(((int)(leftMargin / 2)) + topMargin + topPath.GetBounds().Height) + (int)(height - (((int)(leftMargin / 2)) + topMargin + topPath.GetBounds().Height + topMargin) - payLogoHeight) / 2 ;

                g.DrawImage(payLogo, payLogoLeft, payLogoTop, payLogoWidth, payLogoHeight);

                index++;
            }

            var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            g.Dispose();
            bmp.Dispose();

            return File(ms.GetBuffer(), "image/png");
        }
    }

    public static class ColorAdjuster
    {
        public static Color LightenColor(Color color, float hueShift)
        {
            // 将给定颜色转换为 HSL 颜色模型
            float hue, saturation, lightness;
            ColorToHSL(color, out hue, out saturation, out lightness);

            // 降低色相
            hue -= hueShift;
            if (hue < 0)
            {
                hue += 360;
            }

            // 提高亮度
            lightness = Math.Min(lightness + 0.45f, 1.0f);

            // 将 HSL 颜色模型转换回 RGB 颜色模型
            Color newColor = HSLToColor(hue, saturation, lightness);

            return newColor;
        }

        // 将 RGB 颜色模型转换为 HSL 颜色模型
        public static void ColorToHSL(Color color, out float hue, out float saturation, out float lightness)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            // 找出 RGB 三原色中的最大值和最小值
            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));

            // 计算亮度
            hue = 0;
            saturation = 0;
            lightness = (max + min) / 2;

            // 如果最大值和最小值相等，表示颜色为灰色
            if (max == min)
            {
                hue = 0;
                saturation = 0;
            }
            else
            {
                // 计算饱和度
                float d = max - min;
                saturation = lightness > 0.5f ? d / (2 - max - min) : d / (max + min);

                // 计算色相
                switch (max)
                {
                    case var _ when max == r:
                        hue = (g - b) / d + (g < b ? 6 : 0);
                        break;

                    case var _ when max == g:
                        hue = (b - r) / d + 2;
                        break;

                    case var _ when max == b:
                        hue = (r - g) / d + 4;
                        break;
                }
                hue /= 6;
            }
        }

        // 将 HSL 颜色模型转换为 RGB 颜色模型
        public static Color HSLToColor(float hue, float saturation, float lightness)
        {
            float r, g, b;

            // 如果饱和度为 0，颜色为灰色
            if (saturation == 0)
            {
                r = g = b = lightness;
            }
            else
            {
                float q = lightness < 0.5f ? lightness * (1 + saturation) : lightness + saturation - lightness * saturation;
                float p = 2 * lightness - q;
                r = HueToRGB(p, q, hue + 1f / 3f);
                g = HueToRGB(p, q, hue);
                b = HueToRGB(p, q, hue - 1f / 3f);
            }

            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        // 辅助函数，将色相值转换为 RGB 颜色模型中的值
        private static float HueToRGB(float p, float q, float t)
        {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1f / 6f) return p + (q - p) * 6f * t;
            if (t < 1f / 2f) return q;
            if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
            return p;
        }
    }

    public static class QrCodeBuilder
    {
        public static Bitmap Generate(string content = "https://www.example.com", Bitmap icon = null)
        {
            // 创建二维码对象
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap qrCodeImage = qrCode.GetGraphic(30, Color.Black, Color.White, icon);
            return qrCodeImage;
        }
    }

    public class PayType
    {
        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
    }

    public static class DrawQrCode
    {
        public static Bitmap GenerateImage(int width, int height, Color backgroundColor, int cornerRadius, string logoPath = null, string title = null, Bitmap icon = null, string text = "No.220101000001", List<PayType> payTypes = null)
        {
            int leftMargin = (int)(width * 0.1);
            int topMargin = (int)(width * 0.3);
            int bottomMargin = (int)(width * 0.1);

            // Create the image
            Bitmap image = new Bitmap(width, height);

            // Create the graphics object
            Graphics graphics = Graphics.FromImage(image);
            graphics.Clear(backgroundColor);

            if (string.IsNullOrWhiteSpace(logoPath) && !string.IsNullOrWhiteSpace(title))
            {
                graphics.DrawTitleText(width, title, topMargin);
            }

            if (!string.IsNullOrWhiteSpace(logoPath))
            {
                graphics.DrawMainLogo(width, logoPath, topMargin);
            }

            // 创建中间的白色圆角矩形
            int middleWidth = width - leftMargin * 2;
            int middleHeight = height - topMargin - bottomMargin;
            int middleX = leftMargin;
            int middleY = topMargin;
            Rectangle middleRectangle = new Rectangle(x: middleX, middleY, middleWidth, middleHeight);
            // 绘制中间白色圆角矩形
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillRoundedRectangle(Brushes.White, middleRectangle, cornerRadius);
            //graphics.DrawRoundedRectangle(Pens.White, middleRectangle, cornerRadius);

            // 在画布上绘制二维码
            Bitmap qrCode = QrCodeBuilder.Generate(icon: icon);
            // 计算二维码位置和大小
            int qrSize = width - (leftMargin * 2) - cornerRadius;
            int qrLeft = (width - qrSize) / 2;
            int qrTop = middleRectangle.Top + (middleRectangle.Width - qrSize) / 2;
            graphics.DrawImage(qrCode, qrLeft, qrTop, qrSize, qrSize);

            Rectangle bottomRect = new Rectangle(leftMargin, (topMargin + middleRectangle.Width), width - leftMargin * 2, height - (topMargin + middleRectangle.Width) - bottomMargin);
            GraphicsPath bottomPath = new GraphicsPath();
            int diameter = cornerRadius * 2;
            Rectangle arcRect = new Rectangle(bottomRect.X, bottomRect.Y, diameter, diameter);
            bottomPath.AddLine(bottomRect.Left, bottomRect.Top, bottomRect.Right, bottomRect.Top); // 直线段
            arcRect.X = bottomRect.Right - diameter;
            bottomPath.AddLine(bottomRect.Right, bottomRect.Top, bottomRect.Right, bottomRect.Bottom); // 直线段 
            arcRect.Y = bottomRect.Bottom - diameter;
            bottomPath.AddArc(arcRect, 0, 90); // 右下角
            arcRect.X = bottomRect.X;
            bottomPath.AddArc(arcRect, 90, 90); // 左下角
            bottomPath.CloseFigure();
            Color color = ColorAdjuster.LightenColor(backgroundColor, 0);
            SolidBrush brush = new SolidBrush(color);
            graphics.FillPath(brush, bottomPath);

            // 在画布上绘制文字
            SizeF textSize = graphics.MeasureString(text, new Font("Arial", 36));
            int textLeft = (int)((bottomPath.GetBounds().Width - textSize.Width) / 2 + bottomPath.GetBounds().X);
            int textTop = (int)(bottomPath.GetBounds().Bottom + (middleRectangle.Width - bottomPath.GetBounds().Bottom - textSize.Height) / 2);
            graphics.DrawString(text, new Font("Arial", 36), Brushes.Black, textLeft, textTop);

            int payLogoWidth = (int)(width * 0.1);
            int payLogoHeight = (int)(width * 0.1);
            int payTypeCount = payTypes.Count;
            int padding = ((width - leftMargin * 2) - (int)(payTypeCount * (width * 0.1))) / (payTypeCount + 1); // 每个LOGO之间间距
            int index = 0;
            foreach (var item in payTypes)
            {
                string payLogoPath = item.ImgUrl;
                Image payLogo = Image.FromFile(payLogoPath);

                int payLogoLeft = (leftMargin + padding) + (payLogoWidth + padding) * index;

                int payLogoTop = (int)(topMargin + middleRectangle.Width) + (int)(height - (topMargin + middleRectangle.Width + bottomMargin) - payLogoHeight) / 2;

                graphics.DrawImage(payLogo, payLogoLeft, payLogoTop, payLogoWidth, payLogoHeight);

                index++;
            }

            graphics.Dispose();

            // 将图像保存到文件
            //image.Save(outputPath, ImageFormat.Png);
            return image;
        }

        private static void DrawTitleText(this Graphics graphics, int width, string title, int topMargin)
        {
            // 绘制大标题文本
            Font font = new Font("Arial", 48, FontStyle.Bold);
            SizeF titleSize = graphics.MeasureString(title, font);
            PointF textPos = new PointF(width / 2 - titleSize.Width / 2, topMargin / 2 - titleSize.Height / 2);
            graphics.DrawString(title, font, Brushes.White, textPos);
        }

        private static void DrawMainLogo(this Graphics graphics, int width, string logoPath, int topMargin)
        {
            // 加载logo图片
            Image logo = Image.FromFile(logoPath);
            // 计算logo的位置和大小
            int logoWidth = logo.Width > (width - (width * 0.1)) ? (int)(width - (width * 0.1)) : logo.Width;
            int logoHeight = logo.Height > (topMargin - (topMargin * 0.1)) ? (int)(topMargin - (topMargin * 0.1)) : logo.Height;
            int logoLeft = (width - logoWidth) / 2;
            int logoTop = (topMargin - logoHeight) / 2;
            // 在画布上绘制logo
            graphics.DrawImage(logo, logoLeft, logoTop, logoWidth, logoHeight);
        }

        private static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle rectangle, int cornerRadius)
        {
            GraphicsPath path = CreateRoundedRectangle(rectangle, cornerRadius);
            graphics.FillPath(brush, path);
        }

        private static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle rectangle, int cornerRadius)
        {
            GraphicsPath path = CreateRoundedRectangle(rectangle, cornerRadius);
            graphics.DrawPath(pen, path);
        }

        private static GraphicsPath CreateRoundedRectangle(Rectangle rectangle, int cornerRadius)
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
