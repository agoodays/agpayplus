using AGooday.AgPay.Infrastructure.Context;
using AGooday.AgPay.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AGooday.AgPay.Infrastructure.UnitTests
{
    [TestClass]
    public class AgentInfoRepositoryTest
    {
        private DbContextOptions<AgPayDbContext> _options;
        private AgPayDbContext _dbContext;
        private AgentInfoRepository _repository;
        private TestLoggerProvider _loggerProvider;
        private ILogger _logger;
        private IConfiguration _configuration;

        [TestInitialize]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // 配置文件的基路径
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // 配置文件的路径
                .Build();

            var connectionString = _configuration.GetConnectionString("Default");
            _loggerProvider = new TestLoggerProvider();

            //// 创建一个 LoggerFactory 实例并添加 ConsoleLoggerProvider
            //var loggerFactory = LoggerFactory.Create(builder =>
            //{
            //    builder.AddConsole() // 需要引用 Microsoft.Extensions.Logging.Console 包
            //    .SetMinimumLevel(LogLevel.Information);  // 设置最小日志级别为 Information
            //});

            var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(_loggerProvider)
                //.SetMinimumLevel(LogLevel.Trace)
            );

            _logger = loggerFactory.CreateLogger<RepositoryTest>();

            _options = new DbContextOptionsBuilder<AgPayDbContext>()
                .UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion)
                .EnableSensitiveDataLogging()
                .UseLoggerFactory(loggerFactory)
                .Options;

            _dbContext = new AgPayDbContext(_options);
            _repository = new AgentInfoRepository(_dbContext);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var agentInfos = _repository.GetParentAgents("A1702728742");
            _logger.LogInformation($"输出Sql：{agentInfos.ToQueryString()}");
            Assert.IsNotNull(agentInfos);
        }

        [TestCleanup]
        public void Cleanup()
        {
            var logs = _loggerProvider.GetLogs();
            foreach (var log in logs)
            {
                Console.WriteLine(log);
            }

            //_dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}