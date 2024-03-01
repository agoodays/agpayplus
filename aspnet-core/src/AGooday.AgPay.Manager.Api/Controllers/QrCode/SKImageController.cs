using AGooday.AgPay.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using SkiaSharp.QrCode;
using SkiaSharp.QrCode.Models;
using System.Drawing;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AGooday.AgPay.Manager.Api.Controllers.QrCode
{
    [Route("api/skqrc")]
    [ApiController, AllowAnonymous]
    public class SKImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public SKImageController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("VerificationCode")]
        public IActionResult VerificationCode()
        {
            var code = VerificationCodeUtil.RandomVerificationCode(6);
            var bitmap = VerificationCodeUtil.DrawImage(code, 137, 40, 20);
            //var imageBase64Data = VerificationCodeUtil.BitmapToBase64Str(bitmap);
            // 将绘制的图像保存到内存流中
            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = new MemoryStream())
            {
                data.SaveTo(stream);

                // 返回生成的图像
                return File(stream.ToArray(), "image/png");
            }
        }

        [HttpGet("GenerateImage")]
        public IActionResult GenerateImage()
        {
            int width = 1190;
            int height = 1684;
            int paddingTop = 30;
            int paddingBottom = 30;
            int paddingMiddle = 20;
            int cornerRadius = 20;

            using (var bitmap = new SKBitmap(width, height))
            {
                using (var canvas = new SKCanvas(bitmap))
                {
                    // 绘制背景色
                    canvas.Clear(SKColors.White);

                    // 计算上中下三个区域的高度
                    int topHeight = (height - paddingTop - paddingMiddle - paddingBottom) / 3;
                    int middleHeight = topHeight + paddingMiddle;
                    int bottomHeight = height - topHeight - middleHeight;

                    // 绘制上部分（红色）
                    using (var paint = new SKPaint { Color = SKColors.Red, IsAntialias = true })
                    {
                        var rect = new SKRect(paddingTop, paddingTop, width - paddingTop, topHeight);
                        canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                    }

                    // 绘制中间部分（蓝色）
                    using (var paint = new SKPaint { Color = SKColors.Blue, IsAntialias = true })
                    {
                        var rect = new SKRect(paddingTop, topHeight + paddingTop, width - paddingTop, topHeight + paddingTop + middleHeight);
                        canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                    }

                    // 绘制下部分（绿色）
                    using (var paint = new SKPaint { Color = SKColors.Green, IsAntialias = true })
                    {
                        var rect = new SKRect(paddingTop, topHeight + middleHeight + paddingTop + paddingMiddle, width - paddingTop, height - paddingBottom);
                        canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                    }
                }

                // 将绘制的图像保存到内存流中
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = new MemoryStream())
                {
                    data.SaveTo(stream);

                    // 返回生成的图像
                    return File(stream.ToArray(), "image/png");
                }
            }
        }

        [HttpGet("GenerateImage2")]
        public IActionResult GenerateImage2()
        {
            int width = 1190;
            int height = 1684;
            int paddingTop = 30;
            int paddingBottom = 30;
            int paddingMiddle = 20;
            int cornerRadius = 20;

            using (var bitmap = new SKBitmap(width, height))
            {
                using (var canvas = new SKCanvas(bitmap))
                {
                    // 绘制背景色
                    canvas.Clear(SKColors.White);

                    // 计算上中下三个区域的高度
                    int topHeight = (int)(height * 0.2);
                    int middleHeight = (int)(height * 0.5);
                    int bottomHeight = (int)(height * 0.3);

                    // 计算上中下三个区域的起始Y坐标
                    int topY = (height - paddingTop - paddingBottom - topHeight - middleHeight - bottomHeight - paddingMiddle) / 2 + paddingTop;
                    int middleY = topY + topHeight + paddingMiddle;
                    int bottomY = middleY + middleHeight + paddingMiddle;

                    // 绘制上部分（红色）
                    using (var paint = new SKPaint { Color = SKColors.Red, IsAntialias = true })
                    {
                        var rect = new SKRect(paddingTop, topY, width - paddingTop, topY + topHeight);
                        canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                    }

                    // 绘制中间部分（蓝色）
                    using (var paint = new SKPaint { Color = SKColors.Blue, IsAntialias = true })
                    {
                        var rect = new SKRect(paddingTop, middleY, width - paddingTop, middleY + middleHeight);
                        canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                    }

                    // 绘制下部分（绿色）
                    using (var paint = new SKPaint { Color = SKColors.Green, IsAntialias = true })
                    {
                        var rect = new SKRect(paddingTop, bottomY, width - paddingTop, bottomY + bottomHeight);
                        canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                    }
                }

                // 将绘制的图像保存到内存流中
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = new MemoryStream())
                {
                    data.SaveTo(stream);

                    // 返回生成的图像
                    return File(stream.ToArray(), "image/png");
                }
            }
        }

        [HttpGet("GenerateImage3")]
        public IActionResult GenerateImage3()
        {
            int width = 1190;
            int height = 1684;
            int paddingTop = 30;
            int paddingBottom = 30;
            int paddingMiddle = 20;
            int cornerRadius = 20;

            // 加载 logo 图像
            using (var logoStream = System.IO.File.OpenRead(Path.Combine(_env.WebRootPath, "images", "jeepay_blue.png")))
            {
                using (var logoBitmap = SKBitmap.Decode(logoStream))
                {
                    using (var bitmap = new SKBitmap(width, height))
                    {
                        using (var canvas = new SKCanvas(bitmap))
                        {
                            // 绘制背景色
                            canvas.Clear(SKColors.White);

                            // 计算上中下三个区域的高度
                            int topHeight = (int)(height * 0.2);
                            int middleHeight = (int)(height * 0.5);
                            int bottomHeight = (int)(height * 0.3);

                            // 计算上中下三个区域的起始Y坐标
                            int topY = (height - paddingTop - paddingBottom - topHeight - middleHeight - bottomHeight - paddingMiddle) / 2 + paddingTop;
                            int middleY = topY + topHeight + paddingMiddle;
                            int bottomY = middleY + middleHeight + paddingMiddle;

                            // 绘制上部分（红色）
                            using (var paint = new SKPaint { Color = SKColors.Red, IsAntialias = true })
                            {
                                var rect = new SKRect(paddingTop, topY, width - paddingTop, topY + topHeight);
                                canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                            }

                            // 绘制中间部分（蓝色）
                            using (var paint = new SKPaint { Color = SKColors.Blue, IsAntialias = true })
                            {
                                var rect = new SKRect(paddingTop, middleY, width - paddingTop, middleY + middleHeight);
                                canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                            }

                            // 绘制下部分（绿色）
                            using (var paint = new SKPaint { Color = SKColors.Green, IsAntialias = true })
                            {
                                var rect = new SKRect(paddingTop, bottomY, width - paddingTop, bottomY + bottomHeight);
                                canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                            }

                            // 计算 logo 的位置和大小（假设位于上部分的中央）
                            int logoWidth = (int)(width * 0.5);
                            int logoHeight = (int)(topHeight * 0.8);
                            int logoX = (width - logoWidth) / 2;
                            int logoY = topY + (topHeight - logoHeight) / 2;

                            // 绘制 logo
                            using (var logoScaledBitmap = logoBitmap.Resize(new SKImageInfo(logoWidth, logoHeight), SKFilterQuality.High))
                            {
                                canvas.DrawBitmap(logoScaledBitmap, logoX, logoY);
                            }

                            // 计算二维码的位置和大小（假设位于中间部分的中央）
                            int qrCodeSize = (int)(middleHeight * 0.8);
                            int qrCodeX = (width - qrCodeSize) / 2;
                            int qrCodeY = middleY + (middleHeight - qrCodeSize) / 2;

                            // 绘制二维码（假设使用 SkiaSharp 提供的方法生成二维码）
                            using (var qrCodeBitmap = GenerateQRCode(qrCodeSize))
                            {
                                canvas.DrawBitmap(qrCodeBitmap, qrCodeX, qrCodeY);
                            }

                            // 计算微信、支付宝、云闪付 logo 的大小和间距
                            int logoSize = (int)(bottomHeight * 0.6);
                            int logoSpacing = (width - logoSize * 3) / 4;

                            // 计算微信 logo 的位置
                            int wechatLogoX = logoSpacing;
                            int wechatLogoY = bottomY + (bottomHeight - logoSize) / 2;

                            // 绘制微信 logo（假设使用 SkiaSharp 提供的方法生成微信 logo）
                            using (var wechatLogoBitmap = GenerateWechatLogo(logoSize))
                            {
                                canvas.DrawBitmap(wechatLogoBitmap, wechatLogoX, wechatLogoY);
                            }

                            // 计算支付宝 logo 的位置
                            int alipayLogoX = wechatLogoX + logoSize + logoSpacing;
                            int alipayLogoY = bottomY + (bottomHeight - logoSize) / 2;

                            // 绘制支付宝 logo（假设使用 SkiaSharp 提供的方法生成支付宝 logo）
                            using (var alipayLogoBitmap = GenerateAlipayLogo(logoSize))
                            {
                                canvas.DrawBitmap(alipayLogoBitmap, alipayLogoX, alipayLogoY);
                            }

                            // 计算云闪付 logo 的位置
                            int unionpayLogoX = alipayLogoX + logoSize + logoSpacing;
                            int unionpayLogoY = bottomY + (bottomHeight - logoSize) / 2;

                            // 绘制云闪付 logo（假设使用 SkiaSharp 提供的方法生成云闪付 logo）
                            using (var unionpayLogoBitmap = GenerateUnionpayLogo(logoSize))
                            {
                                canvas.DrawBitmap(unionpayLogoBitmap, unionpayLogoX, unionpayLogoY);
                            }
                        }

                        // 将绘制的图像保存到内存流中
                        using (var image = SKImage.FromBitmap(bitmap))
                        using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                        using (var stream = new MemoryStream())
                        {
                            data.SaveTo(stream);

                            // 返回生成的图像
                            return File(stream.ToArray(), "image/png");
                        }
                    }
                }
            }
        }

        [HttpGet("GenerateImage4")]
        public IActionResult GenerateImage4()
        {
            int imageWidth = 1190;
            int imageHeight = 1684;
            int padding = 120;
            int spacing = 20;

            using (var bitmap = new SKBitmap(imageWidth, imageHeight))
            {
                using (var canvas = new SKCanvas(bitmap))
                {
                    // 背景色
                    SKRect backgroundRect = new SKRect(0, 0, imageWidth, imageHeight);
                    DrawRectangle(canvas, backgroundRect, SKColors.Yellow, true, false);

                    // 上部分
                    float upperHeight = imageHeight * 0.2f;
                    SKRect upperRect = new SKRect(padding, padding, imageWidth - padding, padding + upperHeight);
                    DrawRectangle(canvas, upperRect, SKColors.Red, true, true);

                    // 中间部分
                    float middleHeight = imageHeight * 0.5f;
                    SKRect middleRect = new SKRect(padding, upperRect.Bottom + spacing, imageWidth - padding, upperRect.Bottom + spacing + middleHeight);
                    DrawRectangle(canvas, middleRect, SKColors.Blue, false, true);

                    // 下部分
                    float lowerHeight = imageHeight * 0.3f;
                    SKRect lowerRect = new SKRect(padding, middleRect.Bottom + spacing, imageWidth - padding, middleRect.Bottom + spacing + lowerHeight);
                    DrawRectangle(canvas, lowerRect, SKColors.Green, true, true);

                    // 在上部分中绘制品牌logo
                    SKRect logoRect = GetCenteredRect(upperRect, 0.8f);
                    DrawLogo(canvas, Path.Combine(_env.WebRootPath, "images", "jeepay_blue.png"), logoRect);

                    // 在下部分绘制多个logo
                    var logoPaths = new[] {
                        Path.Combine(_env.WebRootPath, "images", "wxpay.png"),
                        Path.Combine(_env.WebRootPath, "images", "alipay.png"),
                        Path.Combine(_env.WebRootPath, "images", "ysfpay.png"),
                        Path.Combine(_env.WebRootPath, "images", "unionpay.png")
                    };

                    float logoSize = (lowerRect.Height - spacing * 2) / logoPaths.Length;
                    float totalLogoWidth = logoSize * logoPaths.Length + spacing * 2;
                    float startX = (imageWidth - totalLogoWidth) / 2;
                    float logoY = lowerRect.Top + spacing;

                    foreach (var logoPath in logoPaths)
                    {
                        DrawLogo(canvas, logoPath, new SKRect(startX, logoY, startX + logoSize, logoY + logoSize));
                        startX += logoSize + spacing;
                    }

                    var qrCodeContent = "Your QR Code content";
                    var qrCodeColor = SKColors.Black;
                    var qrCodeIconData = new IconData
                    {
                        Icon = SKBitmap.Decode(Path.Combine(_env.WebRootPath, "images", "wxpay.png")),
                        IconSizePercent = 10,
                    };
                    var qrCodeImage = GenerateQrCode2(qrCodeContent, qrCodeColor, qrCodeIconData);
                    var qrCodeSkImage = SKImage.FromBitmap(qrCodeImage);
                    float childWidth = middleRect.Width * 0.97f;
                    float childHeight = middleRect.Height * 0.97f;
                    float childX = middleRect.Left + (middleRect.Width - childWidth) / 2;
                    float childY = middleRect.Top + (middleRect.Height - childHeight) / 2;
                    var qrCodeRect = new SKRect(childX, childY, childX + childWidth, childY + childHeight);
                    canvas.DrawImage(qrCodeSkImage, qrCodeRect);

                    // 保存图像文件
                    using (var image = SKImage.FromBitmap(bitmap))
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var stream = new MemoryStream())
                    {
                        data.SaveTo(stream);

                        // 返回生成的图像
                        return File(stream.ToArray(), "image/png");
                    }
                }
            }
        }

        private void DrawRectangle(SKCanvas canvas, SKRect rect, SKColor color, bool isTopLeftRightAngle, bool isRounded)
        {
            using (var paint = new SKPaint())
            {
                paint.Color = color;
                paint.IsAntialias = true;
                paint.Style = SKPaintStyle.Fill;

                if (isRounded)
                {
                    float cornerRadius = 42;
                    canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, paint);
                }
                else if (isTopLeftRightAngle)
                {
                    canvas.DrawRect(rect, paint);
                }
                else
                {
                    canvas.DrawRoundRect(rect, 0, 0, paint);
                }
            }
        }

        private void DrawLogo(SKCanvas canvas, string logoPath, SKRect rect)
        {
            using (var paint = new SKPaint())
            {
                using (var logoBitmap = SKBitmap.Decode(logoPath))
                {
                    canvas.DrawBitmap(logoBitmap, rect);
                }
            }
        }

        private SKRect GetCenteredRect(SKRect parentRect, float childRectRatio)
        {
            float childWidth = parentRect.Width * childRectRatio;
            float childHeight = parentRect.Height * childRectRatio;
            float childX = parentRect.Left + (parentRect.Width - childWidth) / 2;
            float childY = parentRect.Top + (parentRect.Height - childHeight) / 2;

            return new SKRect(childX, childY, childX + childWidth, childY + childHeight);
        }

        // 生成二维码的示例方法
        private SKBitmap GenerateQRCode(int size)
        {
            // 在此处实现生成二维码的逻辑
            // 返回 SKBitmap 对象表示的二维码图像
            return SKBitmap.Decode(Path.Combine(_env.WebRootPath, "images", "wxpay.png"));
        }

        // 生成微信 logo 的示例方法
        private SKBitmap GenerateWechatLogo(int size)
        {
            // 在此处实现生成微信 logo 的逻辑
            // 返回 SKBitmap 对象表示的微信 logo 图像
            return SKBitmap.Decode(Path.Combine(_env.WebRootPath, "images", "wxpay.png"));
        }

        // 生成支付宝 logo 的示例方法
        private SKBitmap GenerateAlipayLogo(int size)
        {
            // 在此处实现生成支付宝 logo 的逻辑
            // 返回 SKBitmap 对象表示的支付宝 logo 图像
            return SKBitmap.Decode(Path.Combine(_env.WebRootPath, "images", "alipay.png"));
        }

        // 生成云闪付 logo 的示例方法
        private SKBitmap GenerateUnionpayLogo(int size)
        {
            // 在此处实现生成云闪付 logo 的逻辑
            // 返回 SKBitmap 对象表示的云闪付 logo 图像,
            return SKBitmap.Decode(Path.Combine(_env.WebRootPath, "images", "unionpay.png"));
        }

        // 生成支付宝 logo 的示例方法
        private SKBitmap GenerateYsfpayLogo(int size)
        {
            // 在此处实现生成支付宝 logo 的逻辑
            // 返回 SKBitmap 对象表示的支付宝 logo 图像
            return SKBitmap.Decode(Path.Combine(_env.WebRootPath, "images", "ysfpay.png"));
        }


        [HttpGet("GenerateImage5")]
        public IActionResult GenerateImage5()
        {
            // 设置图形尺寸和边距
            int width = 1190;
            int height = 1684;
            int margin = 60;
            int spacing = 20;
            int topMargin = 30;
            int bottomMargin = 30;

            // 创建位图对象
            using (var bitmap = new SKBitmap(width, height))
            {
                // 创建绘图上下文
                using (var canvas = new SKCanvas(bitmap))
                {
                    // 设置背景颜色
                    canvas.Clear(SKColors.White);

                    // 计算上中下三个部分的高度
                    int topHeight = (int)((height - topMargin - bottomMargin) * 0.2);
                    int middleHeight = (int)((height - topMargin - bottomMargin) * 0.5);
                    int bottomHeight = (int)((height - topMargin - bottomMargin) * 0.3);

                    // 计算中间部分的宽度
                    int middleWidth = width - 2 * margin;

                    // 创建矩形路径和画笔
                    var rect = new SKRect(margin, topMargin, width - margin, topMargin + topHeight);
                    var paint = new SKPaint();
                    paint.IsAntialias = true;

                    // 绘制上部分红色矩形（直角）
                    paint.Color = SKColors.Red;
                    canvas.DrawRect(rect, paint);

                    // 绘制中部分蓝色矩形（圆角）
                    rect = new SKRect(margin, topMargin + topHeight + spacing, width - margin, topMargin + topHeight + spacing + middleHeight);
                    paint.Color = SKColors.Blue;
                    canvas.DrawRoundRect(rect, 20, 20, paint);

                    // 绘制下部分绿色矩形（圆角）
                    rect = new SKRect(margin, topMargin + topHeight + spacing + middleHeight + spacing, width - margin, height - bottomMargin);
                    paint.Color = SKColors.Green;
                    canvas.DrawRoundRect(rect, 20, 20, paint);

                    // 居中绘制品牌logo
                    var brandLogoPath = Path.Combine(_env.WebRootPath, "images", "jeepay_blue.png"); // 品牌logo的路径
                    var brandLogoImage = SKImage.FromEncodedData(brandLogoPath);
                    var brandLogoRect = new SKRect(
                        (width - brandLogoImage.Width) / 2,
                        (topMargin - brandLogoImage.Height) / 2,
                        (width + brandLogoImage.Width) / 2,
                        (topMargin + brandLogoImage.Height) / 2
                    );
                    canvas.DrawImage(brandLogoImage, brandLogoRect);

                    // 水平居中绘制多个logo
                    var logoPaths = new[] {
                        Path.Combine(_env.WebRootPath, "images", "wxpay.png"),
                        Path.Combine(_env.WebRootPath, "images", "alipay.png"),
                        Path.Combine(_env.WebRootPath, "images", "ysfpay.png"),
                        Path.Combine(_env.WebRootPath, "images", "unionpay.png")
                    }; // 多个logo的路径
                    var totalLogoWidth = logoPaths.Length * brandLogoImage.Width + (logoPaths.Length - 1) * spacing;
                    var startX = (middleWidth - totalLogoWidth) / 2 + margin;
                    var startY = topMargin + topHeight + spacing + (middleHeight - brandLogoImage.Height) / 2;
                    foreach (var logoPath in logoPaths)
                    {
                        var logoImage = SKImage.FromEncodedData(logoPath);
                        var logoRect = new SKRect(startX, startY, startX + logoImage.Width, startY + logoImage.Height);
                        canvas.DrawImage(logoImage, logoRect);
                        startX += logoImage.Width + spacing;
                    }

                    // 在中间部分绘制二维码
                    var qrCodeContent = "Your QR Code content";
                    var qrCodeColor = SKColors.Black;
                    var qrCodeIconData = new IconData
                    {
                        Icon = SKBitmap.Decode(Path.Combine(_env.WebRootPath, "images", "wxpay.png")),
                        IconSizePercent = 10,
                    };
                    var qrCodeImage = GenerateQrCode2(qrCodeContent, qrCodeColor, qrCodeIconData);
                    var qrCodeSkImage = SKImage.FromBitmap(qrCodeImage);
                    var qrCodeRect = new SKRect(
                        (width - qrCodeSkImage.Width) / 2,
                        topMargin + topHeight + spacing + (middleHeight - qrCodeSkImage.Height) / 2,
                        (width + qrCodeSkImage.Width) / 2,
                        topMargin + topHeight + spacing + (middleHeight + qrCodeSkImage.Height / 2));
                    canvas.DrawImage(qrCodeSkImage, qrCodeRect);
                }

                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = new MemoryStream())
                {
                    data.SaveTo(stream);

                    // 返回生成的图像
                    return File(stream.ToArray(), "image/png");
                }
            }
        }

        [HttpGet, AllowAnonymous, Route("qrcode.png")]
        public IActionResult GetQRCode()
        {
            // 添加二维码，假设要生成的二维码内容为qrCodeContent
            var qrCodeContent = "https://www.example.com";
            var qrCodeSize = 512; // 二维码尺寸

            var qrinfo = new SKImageInfo(qrCodeSize, qrCodeSize);
            using (var generator = new QRCodeGenerator())
            using (var qrCodeData = generator.CreateQrCode(qrCodeContent, ECCLevel.H))
            using (var qrsurface = SKSurface.Create(qrinfo))
            {
                var qrcanvas = qrsurface.Canvas;
                qrcanvas.Render(qrCodeData, qrinfo.Width, qrinfo.Height);

                using (var image = qrsurface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = new MemoryStream())
                {
                    data.SaveTo(stream);

                    // 返回生成的码牌图片
                    return File(stream.ToArray(), "image/png");
                }
            }
        }

        [HttpGet, AllowAnonymous, Route("qrcode2.png")]
        public IActionResult GetQRCode2()
        {
            var icon = new IconData
            {
                Icon = SKBitmap.Decode(Path.Combine(_env.WebRootPath, "images", "wxpay.png")),
                IconSizePercent = 10,
            };
            var actual = GenerateQrCode("https://www.example.com", SKColor.Parse("000000"), icon);
            using (var stream = new MemoryStream(actual))
            {
                // 返回生成的码牌图片
                return File(stream.ToArray(), "image/png");
            }
        }

        private byte[] GenerateQrCode(string content, SKColor? codeColor, IconData iconData, bool useRect = false)
        {
            // Generate QrCode
            using var generator = new QRCodeGenerator();
            var qr = generator.CreateQrCode(content, ECCLevel.L);

            // Render to canvas
            var info = new SKImageInfo(512, 512);
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

        private SKBitmap GenerateQrCode2(string content, SKColor? codeColor, IconData iconData, bool useRect = false)
        {
            // Generate QrCode
            using var generator = new QRCodeGenerator();
            var qr = generator.CreateQrCode(content, ECCLevel.L);

            // Render to canvas
            var info = new SKImageInfo(512, 512);
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

            using (var image = surface.Snapshot())
            {
                return SKBitmap.FromImage(image);
            }
        }

        private byte[] GenerateQrCode(string content, SKColor codeColor, SKColor backgroundColor, IconData iconData, bool useRect = false)
        {
            // Generate QrCode
            using var generator = new QRCodeGenerator();
            var qr = generator.CreateQrCode(content, ECCLevel.L);

            // Render to canvas
            var info = new SKImageInfo(512, 512);
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

        [HttpGet, AllowAnonymous, Route("styleb.png")]
        public IActionResult GetQRCodeB()
        {
            // 创建码牌画布
            using (var surface = SKSurface.Create(new SKImageInfo(1190, 1684)))
            {
                var canvas = surface.Canvas;

                // 清空画布
                canvas.Clear(SKColors.White);

                var logoPath = Path.Combine(_env.WebRootPath, "images", "jeepay_blue.png");

                // 添加品牌logo，假设品牌logo文件为brandLogo.png
                using (var brandLogo = SKBitmap.Decode(logoPath))
                {
                    var brandLogoWidth = brandLogo.Width; // 品牌logo宽度
                    var brandLogoHeight = brandLogo.Height; // 品牌logo高度
                    var brandLogoX = (canvas.LocalClipBounds.Width - brandLogoWidth) / 2; // 水平居中
                    var brandLogoY = 50; // 在上方添加品牌logo

                    canvas.DrawBitmap(brandLogo, new SKRect(brandLogoX, brandLogoY, brandLogoX + brandLogoWidth, brandLogoY + brandLogoHeight));
                }

                // 添加二维码，假设要生成的二维码内容为qrCodeContent
                var qrCodeContent = "https://www.example.com";
                var qrCodeSize = 512; // 二维码尺寸
                var qrCodeX = (canvas.LocalClipBounds.Width - qrCodeSize) / 2; // 水平居中
                var qrCodeY = 500; // 二维码在品牌logo下方

                var qrinfo = new SKImageInfo(qrCodeSize, qrCodeSize);
                using (var generator = new QRCodeGenerator())
                using (var qrCodeData = generator.CreateQrCode(qrCodeContent, ECCLevel.H))
                using (var qrsurface = SKSurface.Create(qrinfo))
                {
                    var qrcanvas = qrsurface.Canvas;
                    qrcanvas.Clear(SKColors.Red);
                    qrcanvas.Render(qrCodeData, qrinfo.Width, qrinfo.Height);
                    using var image = qrsurface.Snapshot();
                    canvas.DrawImage(image, new SKRect(qrCodeX, qrCodeY, qrCodeX + qrCodeSize, qrCodeY + qrCodeSize));
                }

                #region 添加支付方式logo
                var paymentLogoWidth = 100; // 支付方式logo宽度
                var paymentLogoHeight = 100; // 支付方式logo高度
                var paymentLogoMargin = 20; // 两个支付方式logo间的间距
                var paymentLogoY = qrCodeY + qrCodeSize + paymentLogoMargin; // 在二维码下方

                // 假设有支付宝、微信和云闪付三种支付方式logo
                var paymentLogos = new List<SKBitmap>
                {
                    SKBitmap.Decode(Path.Combine(_env.WebRootPath, "images", "alipay.png")),
                    SKBitmap.Decode(Path.Combine(_env.WebRootPath, "images", "wxpay.png")),
                    SKBitmap.Decode(Path.Combine(_env.WebRootPath, "images", "ysfpay.png")),
                    SKBitmap.Decode(Path.Combine(_env.WebRootPath, "images", "unionpay.png"))
                };

                var totalPaymentLogosWidth = paymentLogos.Count * paymentLogoWidth + (paymentLogos.Count - 1) * paymentLogoMargin;
                var startX = (canvas.LocalClipBounds.Width - totalPaymentLogosWidth) / 2; // 水平居中

                foreach (var paymentLogoBitmap in paymentLogos)
                {
                    canvas.DrawBitmap(paymentLogoBitmap, new SKRect(startX, paymentLogoY, startX + paymentLogoWidth, paymentLogoY + paymentLogoHeight));
                    startX += paymentLogoWidth + paymentLogoMargin;
                }
                #endregion

                // 将码牌图片保存到内存流中
                using (var image = surface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = new MemoryStream())
                {
                    data.SaveTo(stream);

                    // 返回生成的码牌图片
                    return File(stream.ToArray(), "image/png");
                }
            }
        }
    }
}