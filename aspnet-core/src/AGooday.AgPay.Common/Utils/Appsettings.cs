using Microsoft.Extensions.Configuration;

namespace AGooday.AgPay.Common.Utils
{
    /// <summary>
    /// appsettings.json 操作类（静态配置访问入口）
    /// </summary>
    /// <remarks>
    /// <b>为什么只有一个构造函数：</b>
    /// <para>
    /// 本类原先还有一个 <c>Appsettings(string contentPath)</c> 构造函数，它用
    /// <c>new ConfigurationBuilder().SetBasePath(contentPath).AddJsonFile("appsettings.json")</c>
    /// 自行构建一份**独立的**配置。在引入共享配置（appsettings.Shared.json）之后，这个写法会产生
    /// 两个严重后果：
    /// </para>
    /// <list type="number">
    /// <item>
    /// 读不到共享配置 —— 它只加载 <c>appsettings.json</c>，而 OSS / MQ / Serilog / SysRSA2 / AesKey /
    /// JWT:Secret / ConnectionStrings / AllowedHosts / AppSettings 等公共项都已收敛到
    /// <c>appsettings.Shared.json</c> 与 <c>appsettings.Shared.{Environment}.json</c>，于是这些配置全部丢失。
    /// </item>
    /// <item>
    /// 读不到环境变量 —— 独立构建的 ConfigurationRoot 不包含环境变量提供程序，而 docker-compose 正是通过
    /// 环境变量（<c>ConnectionStrings__Default</c>、<c>JWT__Secret</c>、<c>Redis__Default__Connection</c>、
    /// <c>MQ__RabbitMQ__*</c> 等）注入生产配置的。用它会导致生产**静默连到错误的数据库/中间件**。
    /// </item>
    /// </list>
    /// <para>
    /// 又因为 <see cref="Configuration"/> 是静态属性，一旦用那个构造函数赋值，就会污染整个进程的配置读取
    /// （<see cref="App(string[])"/> 等）。该构造函数已无任何调用方，故直接删除，避免被误用。
    /// </para>
    /// <para>
    /// <b>正确用法</b>：由宿主提供配置，即
    /// <c>services.AddSingleton(new Appsettings(builder.Configuration));</c>。
    /// 宿主的配置源链已包含共享配置、各服务差异配置、环境变量与命令行
    /// （见 <c>AgPayBase.Api/Hosting/AgPayApiHostExtensions.AddSharedJsonConfiguration</c>）。
    /// </para>
    /// </remarks>
    public class Appsettings
    {
        /// <summary>
        /// 宿主配置（含共享配置、环境差异配置、环境变量与命令行）。由构造函数注入。
        /// </summary>
        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// 使用宿主配置初始化。请始终传入宿主的 <c>builder.Configuration</c>。
        /// </summary>
        public Appsettings(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 封装要操作的字符
        /// </summary>
        /// <param name="sections">节点配置</param>
        /// <returns></returns>
        public static string App(params string[] sections)
        {
            try
            {
                if (Configuration != null && sections.Length != 0)
                {
                    return Configuration[string.Join(":", sections)];
                }
            }
            catch (Exception) { }

            return "";
        }

        /// <summary>
        /// 递归获取配置信息数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sections"></param>
        /// <returns></returns>
        public static List<T> App<T>(params string[] sections)
        {
            List<T> list = new List<T>();
            if (Configuration == null)
            {
                return list;
            }
            // 引用 Microsoft.Extensions.Configuration.Binder 包
            Configuration.Bind(string.Join(":", sections), list);
            return list;
        }

        /// <summary>
        /// 根据路径  configuration["App:Name"];
        /// </summary>
        /// <param name="sectionsPath"></param>
        /// <returns></returns>
        public static string GetValue(string sectionsPath)
        {
            try
            {
                if (Configuration == null)
                {
                    return "";
                }
                return Configuration[sectionsPath];
            }
            catch (Exception)
            {
            }

            return "";
        }
    }
}
