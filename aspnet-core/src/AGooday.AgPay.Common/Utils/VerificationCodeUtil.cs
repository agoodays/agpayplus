using System.Drawing;
using System.Drawing.Imaging;

namespace AGooday.AgPay.Common.Utils
{
    public class VerificationCodeUtil
    {
        public static string RandomVerificationCode(int lengths)
        {
            string[] chars = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "P", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            string code = "";
            Random random = new Random();
            for (int i = 0; i < lengths; i++)
            {
                code += chars[random.Next(chars.Length)];
            }
            return code;
        }
        public static Bitmap DrawImage(string code, int? imageWidth = null, int imageHeight = 32, int fontSize = 15)
        {
            imageWidth ??= code.Length * 18;
            Color[] colors = {
                Color.LightSalmon, Color.Aqua, Color.LightSkyBlue, Color.Aquamarine,
                Color.Lime, Color.MediumOrchid, Color.Chartreuse, Color.Chocolate,
                Color.MediumPurple, Color.MediumSeaGreen, Color.CornflowerBlue, Color.MediumTurquoise,
                Color.MediumSpringGreen, Color.OrangeRed, Color.DarkOrange, Color.Orchid,
                Color.DarkOrchid, Color.DarkViolet, Color.DeepPink, Color.DeepSkyBlue,
                Color.DodgerBlue, Color.ForestGreen, Color.Red, Color.Green,
                Color.GreenYellow, Color.HotPink, Color.SpringGreen, Color.LawnGreen,
                Color.Tomato, Color.Yellow, Color.YellowGreen, Color.Gold
            };
            string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体" };
            Random random = new Random();
            // 创建一个 Bitmap 图片类型对象
            Bitmap bitmap = new Bitmap(imageWidth.Value, imageHeight);
            // 创建一个图形画笔
            Graphics graphics = Graphics.FromImage(bitmap);
            // 将图片背景填充成白色
            graphics.Clear(Color.White);
            // 绘制验证码噪点
            for (int i = 0; i < random.Next(60, 80); i++)
            {
                int pointX = random.Next(bitmap.Width);
                int pointY = random.Next(bitmap.Height);
                graphics.DrawLine(new Pen(Color.LightGray, 1), pointX, pointY, pointX + 1, pointY);
            }
            // 绘制随机线条 1 条
            graphics.DrawLine(
                    new Pen(colors[random.Next(colors.Length)], random.Next(3)),
                    new Point(
                        random.Next(bitmap.Width),
                        random.Next(bitmap.Height)),
                    new Point(random.Next(bitmap.Width),
                    random.Next(bitmap.Height)));
            // 绘制验证码
            for (int i = 0; i < code.Length; i++)
            {
                graphics.DrawString(
                    code.Substring(i, 1),
                    new Font(fonts[random.Next(fonts.Length)], fontSize, FontStyle.Bold),
                    new SolidBrush(colors[random.Next(colors.Length)]),
                    16 * i + 1,
                    random.Next(0, 5)
                    );
            }
            // 绘制验证码噪点
            for (int i = 0; i < random.Next(30, 50); i++)
            {
                int pointX = random.Next(bitmap.Width);
                int pointY = random.Next(bitmap.Height);
                graphics.DrawLine(new Pen(colors[random.Next(colors.Length)], 1), pointX, pointY, pointX, pointY + 1);
            }
            // 绘制随机线条 1 条
            graphics.DrawLine(
                    new Pen(colors[random.Next(colors.Length)], random.Next(3)),
                    new Point(
                        random.Next(bitmap.Width),
                        random.Next(bitmap.Height)),
                    new Point(random.Next(bitmap.Width),
                    random.Next(bitmap.Height)));
            return bitmap;
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
    }
}
