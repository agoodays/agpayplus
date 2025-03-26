using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using AGooday.AgPay.Infrastructure.Interceptor;
using AGooday.AgPay.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AGooday.AgPay.Infrastructure.UnitTests
{
    [TestClass]
    public class RepositoryTest
    {
        private DbContextOptions<AgPayDbContext> _options;
        private AgPayDbContext _dbContext;
        private SysUserRepository _repository;
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
                .AddInterceptors(new TimestampInterceptor())
                .Options;

            _dbContext = new AgPayDbContext(_options);
            _repository = new SysUserRepository(_dbContext);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var sysUsers = _repository.GetAllAsNoTracking();
            _logger.LogInformation("输出Sql：{QueryString}", sysUsers.ToQueryString());
            Assert.IsNotNull(sysUsers);
        }

        [TestMethod]
        public void UpdateColumnsTest()
        {
            // 更新指定实体的指定列
            //var entityToUpdate = _repository.GetById(801);
            _repository.Update(new SysUser() { SysUserId = 801, SafeWord = "test", State = 1 }, e => new { e.SafeWord, e.State });
            var reault = _repository.SaveChanges(out int count);
            Assert.IsTrue(reault);
        }

        [TestMethod]
        public void UpdatePropertyTest()
        {
            // 更新符合条件的多个实体的指定列
            _repository.Update(e => e.SysUserId == 801, e => new { SafeWord = "test", State = (byte)1, CreatedAt = DateTime.Now });
            var reault = _repository.SaveChanges(out int count);
            Assert.IsTrue(reault);
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