using SkiaSharp;
using System.Runtime.InteropServices;

namespace AGooday.AgPay.Common.Utils
{
    public class VerificationCodeUtil
    {
        /// <summary>
        ///  随机生成验证码
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string GenerateValidateCode(int len)
        {
            // 可选字符集  
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            // 创建一个新的随机数生成器  
            Random random = new();

            // 生成验证码  
            string code = new string(Enumerable.Repeat(chars, len)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return code;
        }

        public static string RandomVerificationCode(int lengths)
        {
            string[] chars = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "P", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"];
            string code = "";
            Random random = new Random();
            for (int i = 0; i < lengths; i++)
            {
                code += chars[random.Next(chars.Length)];
            }
            return code;
        }

        public static SKBitmap DrawImage(string code, int? imageWidth = null, int imageHeight = 32, int fontSize = 15)
        {
            var width = imageWidth ?? code.Length * 18;
            var colors = new SKColor[] {
                SKColors.LightSalmon, SKColors.Aqua, SKColors.LightSkyBlue, SKColors.Aquamarine,
                SKColors.Lime, SKColors.MediumOrchid, SKColors.Chartreuse, SKColors.Chocolate,
                SKColors.MediumPurple, SKColors.MediumSeaGreen, SKColors.CornflowerBlue, SKColors.MediumTurquoise,
                SKColors.MediumSpringGreen, SKColors.OrangeRed, SKColors.DarkOrange, SKColors.Orchid,
                SKColors.DarkOrchid, SKColors.DarkViolet, SKColors.DeepPink, SKColors.DeepSkyBlue,
                SKColors.DodgerBlue, SKColors.ForestGreen, SKColors.Red, SKColors.Green,
                SKColors.GreenYellow, SKColors.HotPink, SKColors.SpringGreen, SKColors.LawnGreen,
                SKColors.Tomato, SKColors.Yellow, SKColors.YellowGreen, SKColors.Gold
            };

            var random = new Random();
            var fonts = new[] { "verdana.ttf", "micross.ttf", "comic.ttf", "arial.ttf", "simsun.ttc" };
            // 获取字体文件的完整路径
            string fontFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", fonts[random.Next(fonts.Length)]);

            // 加载字体文件
            SKTypeface typeface = SKTypeface.FromFile(fontFilePath);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fonts = new[] { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };
                typeface = SKTypeface.FromFamilyName(fonts[random.Next(fonts.Length)], SKFontStyle.Bold);
            }

            // 创建一个 SKBitmap 对象
            var bitmap = new SKBitmap(width, imageHeight);
            using (var canvas = new SKCanvas(bitmap))
            {
                // 将图片背景填充成白色
                canvas.Clear(SKColors.White);

                // 绘制验证码噪点
                for (int i = 0; i < random.Next(60, 80); i++)
                {
                    int pointX = random.Next(width);
                    int pointY = random.Next(imageHeight);
                    canvas.DrawLine(pointX, pointY, pointX + 1, pointY, new SKPaint() { Color = SKColors.LightGray });
                }

                // 绘制随机线条 1 条
                canvas.DrawLine(
                    new SKPoint(random.Next(width), random.Next(imageHeight)),
                    new SKPoint(random.Next(width), random.Next(imageHeight)),
                    new SKPaint() { Color = colors[random.Next(colors.Length)], StrokeWidth = random.Next(3) });

                // 绘制验证码
                for (int i = 0; i < code.Length; i++)
                {
                    float x = (width - fontSize * code.Length) / 2 + fontSize * i + random.Next(fontSize / 2);
                    float y = (imageHeight + fontSize) / 2 + random.Next(-fontSize / 4, fontSize / 4);
                    canvas.DrawText(code.Substring(i, 1),
                        x,
                        y,
                        new SKPaint() { Color = colors[random.Next(colors.Length)], TextSize = fontSize, Typeface = typeface });
                }

                // 绘制验证码噪点
                for (int i = 0; i < random.Next(30, 50); i++)
                {
                    int pointX = random.Next(width);
                    int pointY = random.Next(imageHeight);
                    canvas.DrawLine(pointX, pointY, pointX, pointY + 1, new SKPaint() { Color = colors[random.Next(colors.Length)], StrokeWidth = 1 });
                }

                // 绘制随机线条 1 条
                canvas.DrawLine(
                    new SKPoint(random.Next(width), random.Next(imageHeight)),
                    new SKPoint(random.Next(width), random.Next(imageHeight)),
                    new SKPaint() { Color = colors[random.Next(colors.Length)], StrokeWidth = random.Next(3) });
            }
            return bitmap;
        }

        public static string BitmapToBase64String(SKBitmap bitmap)
        {
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            return Convert.ToBase64String(data.ToArray());
        }

        public static string BitmapToImageBase64String(SKBitmap bitmap)
        {
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            return $"data:image/jpeg;base64,{Convert.ToBase64String(data.ToArray())}";
        }
    }
}
