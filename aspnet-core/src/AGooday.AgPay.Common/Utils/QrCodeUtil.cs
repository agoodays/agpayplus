using AGooday.AgPay.Common.Models;
using SkiaSharp;
using SkiaSharp.QrCode;
using SkiaSharp.QrCode.Models;

namespace AGooday.AgPay.Common.Utils
{
    public static class ColorAdjuster
    {
        public static SKColor LightenColor(SKColor color, float hueShift)
        {
            // 将给定颜色转换为 HSL 颜色模型
            ColorToHSL(color, out float hue, out float saturation, out float lightness);

            // 降低色相
            hue -= hueShift;
            if (hue < 0)
            {
                hue += 360;
            }

            // 提高亮度
            lightness = Math.Min(lightness + 0.45f, lightness < 0.9f ? 0.9f : (lightness + (1f - lightness) / 2f));

            // 将 HSL 颜色模型转换回 RGB 颜色模型
            SKColor newColor = HSLToColor(hue, saturation, lightness);

            return newColor;
        }

        public static SKBitmap ChangeColor(string imgPath, string originColor, string targetColor)
        {
            using (var originalBitmap = QrCodeBuilder.GetSKBitmapAsync(imgPath).Result)
            {
                // 创建一个新的位图对象，作为更新后的图像
                SKBitmap updatedBitmap = new SKBitmap(originalBitmap.Width, originalBitmap.Height);

                var oldColor = SKColor.Parse(originColor);
                var newColor = SKColor.Parse(targetColor);
                // 遍历图像的每个像素
                for (int x = 0; x < originalBitmap.Width; x++)
                {
                    for (int y = 0; y < originalBitmap.Height; y++)
                    {
                        // 获取当前像素的颜色
                        SKColor pixelColor = originalBitmap.GetPixel(x, y);

                        // 检查是否与旧颜色匹配
                        if (pixelColor == oldColor)
                        {
                            // 替换为新颜色
                            updatedBitmap.SetPixel(x, y, newColor);
                        }
                        else
                        {
                            // 保留原始颜色
                            updatedBitmap.SetPixel(x, y, pixelColor);
                        }
                    }
                }

                return updatedBitmap;
            }
        }

        // 将 RGB 颜色模型转换为 HSL 颜色模型
        public static void ColorToHSL(SKColor color, out float hue, out float saturation, out float lightness)
        {
            float r = color.Red / 255f;
            float g = color.Green / 255f;
            float b = color.Blue / 255f;

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
        public static SKColor HSLToColor(float hue, float saturation, float lightness)
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

            return new SKColor((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
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
        public static byte[] Generate(string content)
        {
            int width = 1080, height = 1080;
            return GenerateQrCode(content, SKColors.Black, ECCLevel.Q, width, height);
        }

        public static byte[] Generate(string content = "https://www.example.com", string iconPath = null)
        {
            int width = 1080, height = 1080, iconSizePercent = 16;
            int iconWidth = width / 100 * iconSizePercent;
            int iconHeight = height / 100 * iconSizePercent;

            if (string.IsNullOrWhiteSpace(iconPath))
            {
                return GenerateQrCode(content, SKColors.Black, ECCLevel.Q, width, height);
            }
            else
            {
                using (var image = GetSKBitmapAsync(iconPath).Result)
                {
                    using (var bitmap = new SKBitmap(iconWidth, iconHeight))
                    {
                        using (var canvas = new SKCanvas(bitmap))
                        {
                            canvas.Clear(SKColors.Transparent);

                            // 创建圆角路径
                            var path = new SKPath();
                            var rect = new SKRect(0, 0, iconWidth, iconHeight);
                            float cornerRadius = iconWidth * 0.1f;
                            path.AddRoundRect(rect, cornerRadius, cornerRadius);

                            // 在剪切区域内绘制图像
                            canvas.ClipPath(path);
                            canvas.DrawBitmap(image, rect);
                        }

                        var icon = new IconData
                        {
                            Icon = bitmap,
                            IconSizePercent = iconSizePercent,
                        };

                        return GenerateQrCode(content, SKColors.Black, icon, ECCLevel.Q, width, height);
                    }
                }
            }
        }

        public static byte[] GenerateQrCode(string content, SKColor? codeColor, ECCLevel eccLevel = ECCLevel.L, int width = 512, int height = 512, bool useRect = false)
        {
            // Generate QrCode
            using var generator = new QRCodeGenerator();
            var qr = generator.CreateQrCode(content, eccLevel);

            // Render to canvas
            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            if (!codeColor.HasValue)
            {
                canvas.Render(qr, info.Width, info.Height);
            }
            else
            {
                if (useRect)
                {
                    canvas.Render(qr, new SKRect(0, 0, info.Width, info.Height), SKColor.Empty, codeColor.Value);
                }
                else
                {
                    canvas.Render(qr, info.Width, info.Height, SKColor.Empty, codeColor.Value);
                }
            }

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            return data.ToArray();
        }

        public static byte[] GenerateQrCode(string content, SKColor codeColor, SKColor backgroundColor, ECCLevel eccLevel = ECCLevel.L, int width = 512, int height = 512, bool useRect = false)
        {
            // Generate QrCode
            using var generator = new QRCodeGenerator();
            var qr = generator.CreateQrCode(content, eccLevel);

            // Render to canvas
            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            if (useRect)
            {
                canvas.Render(qr, new SKRect(0, 0, info.Width, info.Height), SKColor.Empty, codeColor, backgroundColor);
            }
            else
            {
                canvas.Render(qr, info.Width, info.Height, SKColor.Empty, codeColor, backgroundColor);
            }

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            return data.ToArray();
        }

        public static byte[] GenerateQrCode(string content, SKColor? codeColor, IconData iconData, ECCLevel eccLevel = ECCLevel.L, int width = 512, int height = 512, bool useRect = false)
        {
            // Generate QrCode
            using var generator = new QRCodeGenerator();
            var qr = generator.CreateQrCode(content, eccLevel);

            // Render to canvas
            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            if (useRect)
            {
                canvas.Render(qr, new SKRect(0, 0, info.Width, info.Height), SKColor.Empty, codeColor.Value, iconData);
            }
            else
            {
                canvas.Render(qr, info.Width, info.Height, SKColor.Empty, codeColor.Value, iconData);
            }

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            return data.ToArray();
        }

        public static byte[] GenerateQrCode(string content, SKColor codeColor, SKColor backgroundColor, IconData iconData, ECCLevel eccLevel = ECCLevel.L, int width = 512, int height = 512, bool useRect = false)
        {
            // Generate QrCode
            using var generator = new QRCodeGenerator();
            var qr = generator.CreateQrCode(content, eccLevel);

            // Render to canvas
            var info = new SKImageInfo(width, height);
            using var surface = SKSurface.Create(info);
            var canvas = surface.Canvas;
            if (useRect)
            {
                canvas.Render(qr, new SKRect(0, 0, info.Width, info.Height), SKColor.Empty, codeColor, backgroundColor, iconData);
            }
            else
            {
                canvas.Render(qr, info.Width, info.Height, SKColor.Empty, codeColor, backgroundColor, iconData);
            }

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            return data.ToArray();
        }

        public static async Task<SKBitmap> GetSKBitmapAsync(string path)
        {
            bool isUrl = Uri.TryCreate(path, UriKind.Absolute, out Uri uriResult)
                         && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            SKBitmap bitmap;

            if (isUrl)
            {
                // logoPath 是一个 URL 地址
                // 可以使用 HttpClient 对象来获取图像流
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(path))
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    bitmap = SKBitmap.Decode(stream);
                }
            }
            else
            {
                // logoPath 不是一个 URL 地址
                // 可以使用 Image.FromFile 方法从磁盘上的文件加载图像
                bitmap = SKBitmap.Decode(path);
            }

            // 在这里使用图像对象 logo
            return bitmap;
        }
    }

    public static class DrawQrCode
    {
        public static byte[] GenerateStyleAImage(int width = 1190, int height = 1684, string backgroundColor = "#ff0000", int cornerRadius = 50, string logoPath = null, string title = null, string content = "https://www.example.com", string iconPath = null, string text = "No.220101000001", List<QrCodePayType> payTypes = null)
        {
            using (var bitmap = new SKBitmap(width, height))
            {
                using (var canvas = new SKCanvas(bitmap))
                {
                    var bgColor = SKColor.Parse(backgroundColor);
                    // 清空画布绘制背景色
                    canvas.Clear(bgColor);

                    int leftMargin = (int)(width * 0.1);
                    int topMargin = (int)(width * 0.3);
                    int bottomMargin = (int)(width * 0.1);

                    // 创建中间的白色圆角矩形
                    int middleWidth = width - (leftMargin * 2);
                    int middleHeight = height - topMargin - bottomMargin;
                    int middleLeft = leftMargin;
                    int middleTop = topMargin;
                    int middleRight = middleLeft + middleWidth;
                    int middleBottom = middleTop + middleHeight;

                    using (var paint = new SKPaint { Color = SKColors.White, IsAntialias = true })
                    {
                        var rect = new SKRect(middleLeft, middleTop, middleRight, middleBottom - cornerRadius);
                        canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                    }

                    SKColor color = ColorAdjuster.LightenColor(bgColor, 0);
                    using (var paint = new SKPaint { Color = color, IsAntialias = true })
                    {
                        var rect = new SKRect(middleLeft, middleTop + middleWidth, middleRight, middleBottom);
                        canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                    }

                    using (var paint = new SKPaint { Color = color, IsAntialias = true })
                    {
                        var rect = new SKRect(middleLeft, middleTop + middleWidth, middleRight, middleBottom - cornerRadius);
                        canvas.DrawRoundRect(rect, 0, 0, paint);
                    }

                    if (string.IsNullOrWhiteSpace(logoPath) && !string.IsNullOrWhiteSpace(title))
                    {
                        var fontManager = SKFontManager.Default;
                        var typeface = fontManager.MatchCharacter(null, SKFontStyle.Bold, null, '汉');
                        // 创建画笔对象
                        using (var paint = new SKPaint
                        {
                            TextSize = 48,
                            IsAntialias = true,
                            Color = SKColors.White,
                            TextAlign = SKTextAlign.Center,
                            Typeface = typeface
                        })
                        {
                            // 在画布上绘制文本
                            canvas.DrawText(title, width / 2, topMargin / 2, paint);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(logoPath))
                    {
                        // 在画布上绘制Logo图片
                        using (var logoBitmap = QrCodeBuilder.GetSKBitmapAsync(logoPath).Result)
                        {
                            int logoX = (width - logoBitmap.Width) / 2;
                            int logoY = (topMargin - logoBitmap.Height) / 2;
                            canvas.DrawBitmap(logoBitmap, logoX, logoY);
                        }
                    }

                    // 绘制二维码
                    var qrCodeByte = QrCodeBuilder.Generate(content, iconPath);
                    using (var stream = new MemoryStream(qrCodeByte))
                    using (var qrCodeImage = SKBitmap.Decode(stream))
                    {
                        using (var qrCodeSkBitmap = new SKBitmap(middleWidth, middleWidth))
                        {
                            using (var qrCodeCanvas = new SKCanvas(qrCodeSkBitmap))
                            {
                                qrCodeCanvas.Clear(SKColors.Transparent);

                                // 创建圆角路径
                                var path = new SKPath();
                                var qrCodeRect = new SKRect(0, 0, middleWidth, middleWidth);
                                path.AddRoundRect(qrCodeRect, cornerRadius, cornerRadius);

                                // 在剪切区域内绘制图像
                                qrCodeCanvas.ClipPath(path);
                                qrCodeCanvas.DrawBitmap(qrCodeImage, qrCodeRect);
                            }

                            var qrCodeSkImage = SKImage.FromBitmap(qrCodeSkBitmap);
                            var rect = new SKRect(middleLeft, middleTop, middleRight, middleTop + middleWidth);
                            canvas.DrawImage(qrCodeSkImage, rect);
                        }
                    }

                    // 创建画笔对象
                    using (var paint = new SKPaint
                    {
                        TextSize = 48,
                        IsAntialias = true,
                        Color = SKColors.Black,
                        TextAlign = SKTextAlign.Center,
                        Typeface = SKTypeface.FromFamilyName("Arial")
                    })
                    {
                        // 在画布上绘制文本
                        canvas.DrawText(text, width / 2, topMargin + middleWidth - cornerRadius / 2, paint);
                    }

                    payTypes = payTypes.Where(w => !string.IsNullOrWhiteSpace(w.ImgUrl)).ToList();
                    int payLogoWidth = (int)(width * 0.1);
                    int payTypeCount = payTypes.Count;
                    int payLogoPadding = ((width - leftMargin * 2) - (int)(payTypeCount * (width * 0.1))) / (payTypeCount + 1); // 每个LOGO之间间距
                    bool isExistAlias = payTypes.Any(a => !string.IsNullOrWhiteSpace(a.Alias));
                    int index = 0;
                    foreach (var item in payTypes)
                    {
                        int payLogoLeft = (leftMargin + payLogoPadding) + (payLogoWidth + payLogoPadding) * index;
                        int payLogoTop = (topMargin + middleWidth) + (height - (topMargin + middleWidth + bottomMargin) - payLogoWidth) / 2;

                        int payLogoRight = payLogoLeft + payLogoWidth;

                        int textHeight = 50;

                        int payLogoHeight = payLogoWidth + (isExistAlias ? textHeight : 0);
                        int payLogoBottom = payLogoTop + payLogoHeight;

                        string payLogoPath = item.ImgUrl;
                        using (var payLogoOriginImage = QrCodeBuilder.GetSKBitmapAsync(payLogoPath).Result)
                        {
                            using (var payLogoBitmap = new SKBitmap(payLogoWidth, payLogoHeight))
                            {
                                using (var pagLogoCanvas = new SKCanvas(payLogoBitmap))
                                {
                                    pagLogoCanvas.Clear(SKColors.Transparent);

                                    var pagLogoOriginRect = new SKRect(0, 0, payLogoWidth, payLogoWidth);
                                    pagLogoCanvas.DrawBitmap(payLogoOriginImage, pagLogoOriginRect);

                                    if (!string.IsNullOrWhiteSpace(item.Alias))
                                    {
                                        var fontManager = SKFontManager.Default;
                                        var typeface = fontManager.MatchCharacter('汉');
                                        // 创建画笔对象
                                        using (var paint = new SKPaint
                                        {
                                            TextSize = 25,
                                            IsAntialias = true,
                                            Color = SKColors.White,
                                            TextAlign = SKTextAlign.Center,
                                            Typeface = typeface,
                                        })
                                        {
                                            // 在画布上绘制文本
                                            pagLogoCanvas.DrawText(item.Alias, payLogoWidth / 2, payLogoWidth + (payLogoHeight - payLogoWidth) / 2, paint);
                                        }
                                    }
                                }

                                payLogoTop -= isExistAlias ? textHeight / 2 : 0;
                                payLogoBottom -= isExistAlias ? textHeight / 2 : 0;
                                var payLogoImage = SKImage.FromBitmap(payLogoBitmap);
                                var pagLogoRect = new SKRect(payLogoLeft, payLogoTop, payLogoRight, payLogoBottom);
                                canvas.DrawImage(payLogoImage, pagLogoRect);
                            }

                            //var payLogoImage = SKImage.FromBitmap(payLogoOriginImage);
                            //var pagLogoRect = new SKRect(payLogoLeft, payLogoTop, payLogoRight, payLogoBottom);
                            //canvas.DrawImage(payLogoImage, pagLogoRect);
                        }

                        index++;
                    }
                }

                // 将绘制的图像保存到内存流中
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 100))
                {
                    return data.ToArray();
                }
            }
        }

        public static byte[] GenerateStyleBImage(int width = 1190, int height = 1684, string backgroundColor = "#ff0000", int cornerRadius = 50, string logoPath = null, string title = null, string content = "https://www.example.com", string iconPath = null, string text = "No.220101000001", List<QrCodePayType> payTypes = null)
        {
            using (var bitmap = new SKBitmap(width, height))
            {
                using (var canvas = new SKCanvas(bitmap))
                {
                    var bgColor = SKColor.Parse(backgroundColor);
                    // 清空画布绘制背景色
                    canvas.Clear(bgColor);

                    int leftMargin = (int)(width * 0.1);
                    int topMargin = (int)(width * 0.1);
                    int bottomMargin = (int)(width * 0.1);

                    // 创建中间的白色圆角矩形
                    int middleWidth = width - (leftMargin * 2);
                    int middleHeight = (int)((height - topMargin * 2) * 0.8);
                    int middleLeft = leftMargin;
                    int middleTop = topMargin;
                    int middleRight = middleLeft + middleWidth;
                    int middleBottom = middleTop + middleHeight;

                    using (var paint = new SKPaint { Color = SKColors.White, IsAntialias = true })
                    {
                        var rect = new SKRect(middleLeft, middleTop, middleRight, middleBottom);
                        canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                    }

                    SKColor color = ColorAdjuster.LightenColor(bgColor, 0);
                    var padding = leftMargin / 2;
                    var bottomHeight = height - (middleBottom + padding + bottomMargin);
                    using (var paint = new SKPaint { Color = color, IsAntialias = true })
                    {
                        var rect = new SKRect(middleLeft, middleBottom + padding, middleRight, middleBottom + padding + bottomHeight);
                        canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                    }

                    if (string.IsNullOrWhiteSpace(logoPath) && !string.IsNullOrWhiteSpace(title))
                    {
                        var fontManager = SKFontManager.Default;
                        var typeface = fontManager.MatchCharacter(null, SKFontStyle.Bold, null, '汉');
                        // 创建画笔对象
                        using (var paint = new SKPaint
                        {
                            TextSize = 48,
                            IsAntialias = true,
                            Color = bgColor,
                            TextAlign = SKTextAlign.Center,
                            Typeface = typeface
                        })
                        {
                            // 在画布上绘制文本
                            canvas.DrawText(title, width / 2, middleTop + (middleHeight - middleWidth) / 2 + cornerRadius, paint);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(logoPath))
                    {
                        // 在画布上绘制Logo图片
                        using (var logoBitmap = QrCodeBuilder.GetSKBitmapAsync(logoPath).Result)
                        {
                            int logoX = (width - logoBitmap.Width) / 2;
                            int logoY = middleTop + (middleHeight - middleWidth - logoBitmap.Height) / 2 + cornerRadius;
                            canvas.DrawBitmap(logoBitmap, logoX, logoY);
                        }
                    }

                    // 绘制二维码
                    var qrCodeByte = QrCodeBuilder.Generate(content, iconPath);
                    using (var stream = new MemoryStream(qrCodeByte))
                    using (var qrCodeImage = SKBitmap.Decode(stream))
                    {
                        using (var qrCodeSkBitmap = new SKBitmap(middleWidth, middleWidth))
                        {
                            using (var qrCodeCanvas = new SKCanvas(qrCodeSkBitmap))
                            {
                                qrCodeCanvas.Clear(SKColors.Transparent);

                                // 创建圆角路径
                                var path = new SKPath();
                                var qrCodeRect = new SKRect(0, 0, middleWidth, middleWidth);
                                path.AddRoundRect(qrCodeRect, cornerRadius, cornerRadius);

                                // 在剪切区域内绘制图像
                                qrCodeCanvas.ClipPath(path);
                                qrCodeCanvas.DrawBitmap(qrCodeImage, qrCodeRect);
                            }

                            var qrCodeSkImage = SKImage.FromBitmap(qrCodeSkBitmap);
                            var rect = new SKRect(middleLeft, middleBottom - middleWidth, middleRight, middleBottom);
                            canvas.DrawImage(qrCodeSkImage, rect);
                        }
                    }

                    // 创建画笔对象
                    using (var paint = new SKPaint
                    {
                        TextSize = 48,
                        IsAntialias = true,
                        Color = SKColors.Black,
                        TextAlign = SKTextAlign.Center,
                        Typeface = SKTypeface.FromFamilyName("Arial")
                    })
                    {
                        // 在画布上绘制文本
                        canvas.DrawText(text, width / 2, middleBottom - cornerRadius / 2, paint);
                    }

                    payTypes = payTypes.Where(w => !string.IsNullOrWhiteSpace(w.ImgUrl)).ToList();
                    int payLogoWidth = (int)(width * 0.1);
                    int payTypeCount = payTypes.Count;
                    int payLogoPadding = ((width - leftMargin * 2) - (int)(payTypeCount * (width * 0.1))) / (payTypeCount + 1); // 每个LOGO之间间距
                    bool isExistAlias = payTypes.Any(a => !string.IsNullOrWhiteSpace(a.Alias));
                    int index = 0;
                    foreach (var item in payTypes)
                    {
                        int textHeight = 50;
                        int payLogoHeight = payLogoWidth + (isExistAlias ? textHeight : 0);

                        int payLogoLeft = (leftMargin + payLogoPadding) + (payLogoWidth + payLogoPadding) * index;
                        int payLogoTop = middleBottom + padding + (bottomHeight - payLogoHeight) / 2;

                        int payLogoRight = payLogoLeft + payLogoWidth;

                        int payLogoBottom = payLogoTop + payLogoHeight;

                        string payLogoPath = item.ImgUrl;
                        using (var payLogoOriginImage = QrCodeBuilder.GetSKBitmapAsync(payLogoPath).Result)
                        {
                            using (var payLogoBitmap = new SKBitmap(payLogoWidth, payLogoHeight))
                            {
                                using (var pagLogoCanvas = new SKCanvas(payLogoBitmap))
                                {
                                    pagLogoCanvas.Clear(SKColors.Transparent);

                                    var pagLogoOriginRect = new SKRect(0, 0, payLogoWidth, payLogoWidth);
                                    pagLogoCanvas.DrawBitmap(payLogoOriginImage, pagLogoOriginRect);

                                    if (!string.IsNullOrWhiteSpace(item.Alias))
                                    {
                                        var fontManager = SKFontManager.Default;
                                        var typeface = fontManager.MatchCharacter('汉');
                                        // 创建画笔对象
                                        using (var paint = new SKPaint
                                        {
                                            TextSize = 25,
                                            IsAntialias = true,
                                            Color = SKColors.White,
                                            TextAlign = SKTextAlign.Center,
                                            Typeface = typeface,
                                        })
                                        {
                                            // 在画布上绘制文本
                                            pagLogoCanvas.DrawText(item.Alias, payLogoWidth / 2, payLogoWidth + (payLogoHeight - payLogoWidth) / 2, paint);
                                        }
                                    }
                                }

                                payLogoTop += isExistAlias ? textHeight / 4 : 0;
                                payLogoBottom += isExistAlias ? textHeight / 4 : 0;
                                var payLogoImage = SKImage.FromBitmap(payLogoBitmap);
                                var pagLogoRect = new SKRect(payLogoLeft, payLogoTop, payLogoRight, payLogoBottom);
                                canvas.DrawImage(payLogoImage, pagLogoRect);
                            }

                            //var payLogoImage = SKImage.FromBitmap(payLogoOriginImage);
                            //var pagLogoRect = new SKRect(payLogoLeft, payLogoTop, payLogoRight, payLogoBottom);
                            //canvas.DrawImage(payLogoImage, pagLogoRect);
                        }

                        index++;
                    }
                }

                // 将绘制的图像保存到内存流中
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 100))
                {
                    return data.ToArray();
                }
            }
        }

        public static string BitmapToBase64String(byte[] inArray)
        {
            return Convert.ToBase64String(inArray);
        }

        public static string BitmapToImageBase64String(byte[] inArray)
        {
            return $"data:image/jpeg;base64,{Convert.ToBase64String(inArray)}";
        }
    }
}
