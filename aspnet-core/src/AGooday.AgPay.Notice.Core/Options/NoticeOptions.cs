using Microsoft.Extensions.DependencyInjection;

namespace AGooday.AgPay.Notice.Core
{
    public class NoticeOptions 
    {

        public const string SectionName = "Notice";

        /// <summary>
        /// 同一消息发送间隔
        /// </summary>
        public int IntervalSeconds { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Caching.Core.CachingOptions"/> class.
        /// </summary>
        public NoticeOptions()
        {
            Extensions = new List<INoticeOptionsExtension>();
        }

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        /// <value>The extensions.</value>
        public IList<INoticeOptionsExtension> Extensions { get; }

        /// <summary>
        /// Registers the extension.
        /// </summary>
        /// <param name="extension">Extension.</param>
        public void RegisterExtension(INoticeOptionsExtension extension)
        {
            Extensions.Add(extension);
        }
    }
}
