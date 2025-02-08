using System.Collections.Concurrent;

namespace AGooday.AgPay.Common.Utils
{
    public class MimeTypeResolver
    {
        private static readonly ConcurrentDictionary<string, string> MimeTypes =
            new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                [".html"] = "text/html",
                [".htm"] = "text/html",
                [".txt"] = "text/plain",
                [".css"] = "text/css",
                [".js"] = "application/javascript",
                [".json"] = "application/json",
                [".xml"] = "application/xml",
                [".jpg"] = "image/jpeg",
                [".jpe"] = "image/jpeg",
                [".jpeg"] = "image/jpeg",
                [".jfif"] = "image/jpeg",
                [".png"] = "image/png",
                [".gif"] = "image/gif",
                [".bmp"] = "image/bmp",
                [".wbmp"] = "image/vnd.wap.wbmp",
                [".fax"] = "image/fax",
                [".net"] = "image/pnetvue",
                [".tiff"] = "image/tiff",
                [".ico"] = "image/x-icon",
                [".svg"] = "image/svg+xml",
                [".pdf"] = "application/pdf",
                [".zip"] = "application/zip",
                [".doc"] = "application/msword",
                [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                [".xls"] = "application/vnd.ms-excel",
                [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                [".ppt"] = "application/vnd.ms-powerpoint",
                [".pptx"] = "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                // 可以继续添加更多类型...
            };

        private const string DefaultMimeType = "application/octet-stream";

        /// <summary>
        /// 获取文件的 MIME 类型
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>MIME 类型</returns>
        public static string GetMimeType(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("文件路径不能为空", nameof(filePath));
            }

            string extension = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(extension))
            {
                return DefaultMimeType;
            }

            return MimeTypes.TryGetValue(extension, out string mimeType)
                ? mimeType
                : DefaultMimeType;
        }

        /// <summary>
        /// 添加或更新 MIME 类型
        /// </summary>
        /// <param name="extension">文件扩展名（例如：".mp4"）</param>
        /// <param name="mimeType">MIME 类型（例如："video/mp4"）</param>
        public static void AddOrUpdateMimeType(string extension, string mimeType)
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException("文件扩展名不能为空", nameof(extension));
            }

            if (string.IsNullOrEmpty(mimeType))
            {
                throw new ArgumentException("MIME 类型不能为空", nameof(mimeType));
            }

            MimeTypes.AddOrUpdate(extension, mimeType, (_, _) => mimeType);
        }

        /// <summary>
        /// 移除 MIME 类型
        /// </summary>
        /// <param name="extension">文件扩展名</param>
        public static bool RemoveMimeType(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException("文件扩展名不能为空", nameof(extension));
            }

            return MimeTypes.TryRemove(extension, out _);
        }
    }
}