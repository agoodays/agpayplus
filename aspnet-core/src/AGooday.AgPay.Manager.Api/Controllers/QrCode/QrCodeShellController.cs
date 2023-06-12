using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.Permissions;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Common.Models;
using AGooday.AgPay.Common.Utils;
using AGooday.AgPay.Manager.Api.Attributes;
using AGooday.AgPay.Manager.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace AGooday.AgPay.Manager.Api.Controllers.QrCode
{
    [ApiController, AllowAnonymous]
    [Route("api/qrc/shell")]
    public class QrCodeShellController : ControllerBase
    {
        private readonly IWebHostEnvironment _env; 
        private readonly ILogger<QrCodeController> _logger;
        private readonly IQrCodeShellService _qrCodeShellService;

        public QrCodeShellController(IWebHostEnvironment env, ILogger<QrCodeController> logger, 
            IQrCodeShellService qrCodeShellService)
        {
            _env = env;
            _logger = logger;
            _qrCodeShellService = qrCodeShellService;
        }

        /// <summary>
        /// 码牌模板
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpGet, Route(""), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_SHELL_LIST)]
        public ApiRes List([FromQuery] QrCodeShellQueryDto dto)
        {
            var data = _qrCodeShellService.GetPaginatedData<QrCodeShellDto>(dto);
            return ApiRes.Ok(new { Records = data.ToList(), Total = data.TotalCount, Current = data.PageIndex, HasNext = data.HasNext });
        }

        /// <summary>
        /// 新增码牌模板
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="BizException"></exception>
        [HttpPost, Route(""), MethodLog("新增码牌模板")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_SHELL_ADD)]
        public ApiRes Add(QrCodeShellDto dto)
        {
            bool result = _qrCodeShellService.Add(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_CREATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 删除码牌模板
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpDelete, Route("{recordId}"), MethodLog("删除码牌模板")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_SHELL_DEL)]
        public ApiRes Delete(int recordId)
        {
            bool result = _qrCodeShellService.Remove(recordId);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_DELETE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 更新码牌模板
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut, Route("{recordId}"), MethodLog("更新码牌模板")]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_SHELL_EDIT)]
        public ApiRes Update(QrCodeShellDto dto)
        {
            bool result = _qrCodeShellService.Update(dto);
            if (!result)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_UPDATE);
            }
            return ApiRes.Ok();
        }

        /// <summary>
        /// 查看码牌模板
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        [HttpGet, Route("{recordId}"), NoLog]
        [PermissionAuth(PermCode.MGR.ENT_DEVICE_QRC_SHELL_VIEW, PermCode.MGR.ENT_DEVICE_QRC_SHELL_EDIT)]
        public ApiRes Detail(int recordId)
        {
            var qrCodeShell = _qrCodeShellService.GetById(recordId);
            if (qrCodeShell == null)
            {
                return ApiRes.Fail(ApiCode.SYS_OPERATION_FAIL_SELETE);
            }
            return ApiRes.Ok(qrCodeShell);
        }

        [HttpPost, AllowAnonymous, Route("view")]
        public ApiRes View(QrCodeShellDto dto)
        {
            var configInfo = JsonConvert.DeserializeObject<QrCodeConfigInfo>(dto.ConfigInfo.ToString());
            var logoPath = Path.Combine(_env.WebRootPath, "images", "jeepay.png");

            var backgroundColor = configInfo.BgColor == "custom" ? configInfo.CustomBgColor : configInfo.BgColor;
            foreach (var item in configInfo.PayTypeList)
            {
                item.ImgUrl = string.IsNullOrWhiteSpace(item.ImgUrl) ? Path.Combine(_env.WebRootPath, "images", $"{item.Name}.png") : item.ImgUrl;
            }
            string text = configInfo.ShowIdFlag ? "No.220101000001" : string.Empty;
            var icon = new Bitmap(Path.Combine(_env.WebRootPath, "images", "avatar.png"));
            Bitmap bitmap = null;
            switch (dto.StyleCode)
            {
                case CS.STYLE_CODE.A:
                    bitmap = DrawQrCode.GenerateStyleAImage(backgroundColor: backgroundColor, title: "", logoPath: logoPath, icon: icon, text: text, payTypes: configInfo.PayTypeList);
                    break;
                case CS.STYLE_CODE.B:
                    logoPath = Path.Combine(_env.WebRootPath, "images", "jeepay_blue.png");
                    bitmap = DrawQrCode.GenerateStyleBImage(backgroundColor: backgroundColor, title: "", logoPath: logoPath, icon: icon, text: text, payTypes: configInfo.PayTypeList);
                    break;
                default:
                    break;
            }
            var imageBase64Data = bitmap == null ? "" : $"data:image/png;base64,{DrawQrCode.BitmapToBase64Str(bitmap)}";
            return ApiRes.Ok(imageBase64Data);
        }

        [HttpGet, AllowAnonymous, Route("nostyle.png")]
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

        [HttpGet, AllowAnonymous, Route("stylea.png")]
        public IActionResult StyleA()
        {
            var logoPath = Path.Combine(_env.WebRootPath, "images", "jeepay.png");

            var payTypes = new List<QrCodePayType>() {
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "unionpay.png"), Alias = "银联", Name="unionpay"  },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "ysfpay.png"), Alias = "云闪付", Name="ysfpay" },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "wxpay.png"), Alias = "微信", Name="wxpay"  },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "alipay.png"), Alias = "支付宝", Name="alipay" },
            };

            var bitmap = DrawQrCode.GenerateStyleAImage(title: "", logoPath: logoPath, icon: new Bitmap(Path.Combine(_env.WebRootPath, "images", "avatar.png")), payTypes: payTypes);
            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            bitmap.Dispose();
            return File(ms.GetBuffer(), "image/png");
        }

        [HttpGet, AllowAnonymous, Route("styleb.png")]
        public IActionResult StyleB()
        {
            var logoPath = Path.Combine(_env.WebRootPath, "images", "jeepay_blue.png");

            var payTypes = new List<QrCodePayType>() {
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "unionpay.png"), Alias = "银联", Name="unionpay"  },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "ysfpay.png"), Alias = "云闪付", Name="ysfpay" },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "wxpay.png"), Alias = "微信", Name="wxpay"  },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "alipay.png"), Alias = "支付宝", Name="alipay" },
            };

            var bitmap = DrawQrCode.GenerateStyleBImage(title: "", logoPath: logoPath, icon: new Bitmap(Path.Combine(_env.WebRootPath, "images", "avatar.png")), payTypes: payTypes);
            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            bitmap.Dispose();
            return File(ms.GetBuffer(), "image/png");
        }

        [HttpGet, AllowAnonymous, Route("stylec.png")]
        public IActionResult StyleC()
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

            var payTypes = new List<QrCodePayType>() {
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "unionpay.png"), Alias = "银联", Name="unionpay"  },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "ysfpay.png"), Alias = "云闪付", Name="ysfpay" },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "wxpay.png"), Alias = "微信", Name="wxpay"  },
                new QrCodePayType (){ ImgUrl = Path.Combine(_env.WebRootPath, "images", "alipay.png"), Alias = "支付宝", Name="alipay" },
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

        [HttpGet, AllowAnonymous, Route("styled.png")]
        public IActionResult StyleD()
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
    }
}
