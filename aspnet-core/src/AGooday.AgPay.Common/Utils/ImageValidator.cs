namespace AGooday.AgPay.Common.Utils
{
    public class ImageValidator
    {
        private static readonly Dictionary<string, string> ImageExtensions =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".jpe", "image/jpeg" },
                { ".jfif", "image/jpeg" },
                { ".png", "image/png" },
                { ".gif", "image/gif" },
                { ".bmp", "image/bmp" },
                { ".tiff", "image/tiff" },
                { ".tif", "image/tiff" },
                { ".ico", "image/x-icon" },
                { ".wbmp", "image/vnd.wap.wbmp" },
                { ".fax", "image/fax" },
                { ".net", "image/pnetvue" }
            };

        private static readonly Dictionary<ReadOnlyMemory<byte>, string> ImageSignatures =
            new Dictionary<ReadOnlyMemory<byte>, string>(new ByteArrayEqualityComparer())
            {
                { new byte[] { 0xFF, 0xD8, 0xFF }, "image/jpeg" }, // JPEG/JFIF/JPE
                { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "image/png" }, // PNG
                { new byte[] { 0x47, 0x49, 0x46, 0x38 }, "image/gif" }, // GIF (GIF87a or GIF89a)
                { new byte[] { 0x42, 0x4D }, "image/bmp" }, // BMP
                { new byte[] { 0x49, 0x49, 0x2A, 0x00 }, "image/tiff" }, // TIFF (little endian)
                { new byte[] { 0x4D, 0x4D, 0x00, 0x2A }, "image/tiff" }, // TIFF (big endian)
                { new byte[] { 0x00, 0x00, 0x01, 0x00 }, "image/x-icon" }, // ICO
                { new byte[] { 0x00, 0x00, 0x00, 0x0C, 0x66, 0x74, 0x79, 0x70, 0x77, 0x62, 0x6D, 0x70 }, "image/vnd.wap.wbmp" }, // WBMP
            };

        /// <summary>
        /// 验证文件是否为图片，并返回 MIME 类型
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="mimeType">输出 MIME 类型</param>
        /// <returns>是否为图片</returns>
        public static bool IsImage(string filePath, out string mimeType)
        {
            mimeType = null;

            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return false; // 文件路径无效或文件不存在
            }

            string extension = Path.GetExtension(filePath);
            if (string.IsNullOrEmpty(extension) || !ImageExtensions.TryGetValue(extension, out mimeType))
            {
                return false; // 扩展名无效
            }

            try
            {
                // 读取文件头
                byte[] fileHeader = new byte[12]; // 读取前12个字节以覆盖所有已知的图片文件签名
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    int bytesRead = fs.Read(fileHeader, 0, fileHeader.Length);
                    if (bytesRead == 0)
                    {
                        return false; // 文件为空
                    }

                    // 检查文件签名
                    foreach (var signature in ImageSignatures)
                    {
                        if (fileHeader.AsSpan(0, signature.Key.Length).SequenceEqual(signature.Key.Span))
                        {
                            mimeType = signature.Value;
                            return true;
                        }
                    }
                }

                // 如果文件签名不匹配，但扩展名有效，仍然认为它是图片
                return true;
            }
            catch (Exception)
            {
                return false; // 文件读取失败
            }
        }

        /// <summary>
        /// 自定义比较器用于比较 byte 数组
        /// </summary>
        private class ByteArrayEqualityComparer : IEqualityComparer<ReadOnlyMemory<byte>>
        {
            public bool Equals(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y)
            {
                return x.Span.SequenceEqual(y.Span);
            }

            public int GetHashCode(ReadOnlyMemory<byte> obj)
            {
                HashCode hashCode = new HashCode();
                foreach (byte b in obj.Span)
                {
                    hashCode.Add(b);
                }
                return hashCode.ToHashCode();
            }
        }
    }
}