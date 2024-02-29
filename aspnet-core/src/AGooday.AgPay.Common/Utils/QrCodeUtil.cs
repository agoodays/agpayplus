using AGooday.AgPay.Common.Models;
using QRCoder;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AGooday.AgPay.Common.Utils
{
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

        public static Bitmap ChangeColor(string imgPath, string originColor, string targetColor)
        {
            // 加载图片
            Bitmap bitmap = new Bitmap(Image.FromFile(imgPath));
            Color color = ColorTranslator.FromHtml(originColor);

            // 遍历图片的每一个像素
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    // 获取当前像素的颜色
                    Color pixelColor = bitmap.GetPixel(x, y);

                    // 如果当前像素是白色，将其替换为蓝色
                    if (pixelColor.R == color.R && pixelColor.G == color.G && pixelColor.B == color.B)
                    {
                        //Color newColor = Color.FromArgb(pixelColor.A, 0, 0, 255);
                        Color newColor = ColorTranslator.FromHtml(targetColor);
                        bitmap.SetPixel(x, y, newColor);
                    }
                }
            }

            return bitmap;
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
        public static Bitmap Generate(string plainText, int pixel)
        {
            var generator = new QRCodeGenerator();
            var qrCodeData = generator.CreateQrCode(plainText, QRCodeGenerator.ECCLevel.L);
            var qrCode = new QRCode(qrCodeData);

            var bitmap = qrCode.GetGraphic(pixel);
            //var bitmap = qrCode.GetGraphic(pixel, Color.Black, Color.White, null, 15, 6, false);

            return bitmap;
        }

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

    /// <summary>
    /// https://learn.microsoft.com/zh-cn/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only
    /// </summary>
    public static class DrawQrCode
    {
        public static Bitmap GenerateStyleAImage(int width = 1190, int height = 1684, string backgroundColor = "#ff0000", int cornerRadius = 50, string logoPath = null, string title = null, string content = "https://www.example.com", string iconPath = null, string text = "No.220101000001", List<QrCodePayType> payTypes = null)
        {
            int leftMargin = (int)(width * 0.1);
            int topMargin = (int)(width * 0.3);
            int bottomMargin = (int)(width * 0.1);

            // 创建位图对象
            Bitmap image = new Bitmap(width, height);

            // 创建画布对象
            Graphics graphics = Graphics.FromImage(image);
            Color bgColor = ColorTranslator.FromHtml(backgroundColor);
            graphics.Clear(bgColor);

            if (string.IsNullOrWhiteSpace(logoPath) && !string.IsNullOrWhiteSpace(title))
            {
                graphics.DrawTitleText(title, Brushes.White, width, topMargin);
            }

            if (!string.IsNullOrWhiteSpace(logoPath))
            {
                graphics.DrawMainLogo(logoPath, width, topMargin);
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
            Bitmap icon = null;
            if (iconPath != null)
            {
                icon= new Bitmap(GetImageAsync(iconPath).Result);
            }
            Bitmap qrCode = QrCodeBuilder.Generate(content, icon);
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
            Color color = ColorAdjuster.LightenColor(bgColor, 0);
            SolidBrush brush = new SolidBrush(color);
            graphics.FillPath(brush, bottomPath);

            if (!string.IsNullOrWhiteSpace(text))
            {
                // 在画布上绘制文字
                SizeF textSize = graphics.MeasureString(text, new Font("Arial", 36));
                int textLeft = (int)((bottomPath.GetBounds().Width - textSize.Width) / 2 + bottomPath.GetBounds().X);
                int textTop = (int)(bottomPath.GetBounds().Bottom + (middleRectangle.Width - bottomPath.GetBounds().Bottom - textSize.Height) / 2);
                graphics.DrawString(text, new Font("Arial", 36), Brushes.Black, textLeft, textTop);
            }

            payTypes = payTypes.Where(w => !string.IsNullOrWhiteSpace(w.ImgUrl)).ToList();
            int payLogoWidth = (int)(width * 0.1);
            int payLogoHeight = (int)(width * 0.1);
            int payTypeCount = payTypes.Count;
            int padding = ((width - leftMargin * 2) - (int)(payTypeCount * (width * 0.1))) / (payTypeCount + 1); // 每个LOGO之间间距
            int index = 0;
            foreach (var item in payTypes)
            {
                string payLogoPath = item.ImgUrl;
                Image payLogo = GetImageAsync(payLogoPath).Result;

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

        public static Bitmap GenerateStyleBImage(int width = 1190, int height = 1684, string backgroundColor = "#ff0000", int cornerRadius = 50, string logoPath = null, string title = null, string content = "https://www.example.com", string iconPath = null, string text = "No.220101000001", List<QrCodePayType> payTypes = null)
        {
            int leftMargin = (int)(width * 0.1);
            int topMargin = (int)(width * 0.1);

            // 创建位图对象
            Bitmap image = new Bitmap(width, height);

            // 创建画布对象
            Graphics graphics = Graphics.FromImage(image);

            // 设置画布背景色为红色
            Color bgColor = ColorTranslator.FromHtml(backgroundColor);
            graphics.Clear(bgColor);

            Color color = ColorAdjuster.LightenColor(bgColor, 0);
            SolidBrush brush = new SolidBrush(color);

            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int diameter = cornerRadius * 2;
            // 分割成上下两部分
            Rectangle topRect = new Rectangle(leftMargin, topMargin, width - leftMargin * 2, (int)((height - topMargin * 2) * 0.8));
            graphics.FillRoundedRectangle(Brushes.White, topRect, cornerRadius);

            Rectangle bottomRect = new Rectangle(leftMargin, topRect.Bottom + ((int)(leftMargin / 2)), width - leftMargin * 2, (int)(height - (topRect.Bottom + ((int)(leftMargin / 2)) + topMargin)));
            graphics.FillRoundedRectangle(brush, bottomRect, cornerRadius);

            // 在画布上绘制二维码
            Bitmap icon = null;
            if (iconPath != null)
            {
                icon = new Bitmap(GetImageAsync(iconPath).Result);
            }
            Bitmap qrCode = QrCodeBuilder.Generate(content, icon);
            // 计算二维码位置和大小
            int qrSize = width - (leftMargin * 2) - cornerRadius;
            int qrLeft = (width - qrSize) / 2;
            int qrTop = diameter + topRect.Top + (topRect.Height - qrSize) / 2;
            graphics.DrawImage(qrCode, qrLeft, qrTop, qrSize, qrSize);

            // 释放二维码资源
            qrCode.Dispose();


            if (string.IsNullOrWhiteSpace(logoPath) && !string.IsNullOrWhiteSpace(title))
            {
                graphics.DrawTitleText(title, Brushes.Black, width, qrTop - topMargin + cornerRadius, topMargin);
            }

            if (!string.IsNullOrWhiteSpace(logoPath))
            {
                graphics.DrawMainLogo(logoPath, width, qrTop - topMargin + cornerRadius, topMargin);
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                // 在画布上绘制文字
                SizeF textSize = graphics.MeasureString(text, new Font("Arial", 36));
                int textLeft = (int)((bottomRect.Width - textSize.Width) / 2 + bottomRect.X);
                int textTop = (int)(topRect.Bottom - diameter);
                graphics.DrawString(text, new Font("Arial", 36), Brushes.Black, textLeft, textTop);
            }

            payTypes = payTypes.Where(w => !string.IsNullOrWhiteSpace(w.ImgUrl)).ToList();
            int payLogoWidth = (int)(width * 0.1);
            int payLogoHeight = (int)(width * 0.1);
            int payTypeCount = payTypes.Count;
            int padding = ((width - leftMargin * 2) - (int)(payTypeCount * (width * 0.1))) / (payTypeCount + 1); // 每个LOGO之间间距
            int index = 0;
            foreach (var item in payTypes)
            {
                string payLogoPath = item.ImgUrl;
                Image payLogo = GetImageAsync(payLogoPath).Result;

                int payLogoLeft = (leftMargin + padding) + (payLogoWidth + padding) * index;

                int payLogoTop = bottomRect.Top + (int)((bottomRect.Bottom - bottomRect.Top - payLogoHeight) / 2);

                graphics.DrawImage(payLogo, payLogoLeft, payLogoTop, payLogoWidth, payLogoHeight);

                index++;
            }

            graphics.Dispose();

            // 将图像保存到文件
            //image.Save(outputPath, ImageFormat.Png);
            return image;
        }

        public static string BitmapToBase64Str(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Jpeg);
                byte[] bytes = memoryStream.ToArray();
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        private static void DrawTitleText(this Graphics graphics, string title, Brush brush, int width, int height, int topMargin = 0)
        {
            // 绘制大标题文本
            Font font = new Font("Arial", 48, FontStyle.Bold);
            SizeF titleSize = graphics.MeasureString(title, font);
            PointF textPos = new PointF((width - titleSize.Width) / 2, topMargin + (height - titleSize.Height) / 2);
            graphics.DrawString(title, font, brush, textPos);
        }

        private static void DrawMainLogo(this Graphics graphics, string logoPath, int width, int height, int topMargin = 0)
        {
            // 加载logo图片
            Image logo = GetImageAsync(logoPath).Result;
            // 计算logo的位置和大小
            int logoWidth = logo.Width > (width - (width * 0.1)) ? (int)(width - (width * 0.1)) : logo.Width;
            int logoHeight = logo.Height > height ? height : logo.Height;
            int logoLeft = (width - logoWidth) / 2;
            int logoTop = topMargin + (height - logoHeight) / 2;
            // 在画布上绘制logo
            graphics.DrawImage(logo, logoLeft, logoTop, logoWidth, logoHeight);
        }

        private static async Task<Image> GetImageAsync(string path)
        {
            Uri uriResult;
            bool isUrl = Uri.TryCreate(path, UriKind.Absolute, out uriResult)
                         && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            Image img;

            if (isUrl)
            {
                // logoPath 是一个 URL 地址
                // 可以使用 HttpClient 对象来获取图像流
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(path))
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    img = Image.FromStream(stream);
                }
            }
            else
            {
                // logoPath 不是一个 URL 地址
                // 可以使用 Image.FromFile 方法从磁盘上的文件加载图像
                img = Image.FromFile(path);
            }

            // 在这里使用图像对象 logo
            return img;
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
