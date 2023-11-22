using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using AGooday.AgPay.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

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

        [TestInitialize]
        public void Setup()
        {
            var connectionString = "server=localhost;port=3306;uid=root;pwd=mysql*;database=agpayplusdb_unit_test";
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
            _repository = new SysUserRepository(_dbContext);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var sysUsers = _repository.GetAll();
            _logger.LogInformation($"输出Sql：{sysUsers.ToQueryString()}");
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
            _repository.Update(e => e.SysUserId == 801, e => new { SafeWord = "test", State = (byte)1 });
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


    public class TestLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentQueue<string> _logs;

        public TestLoggerProvider()
        {
            _logs = new ConcurrentQueue<string>();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new TestLogger(_logs);
        }

        public void Dispose()
        {
            // 可选的清理操作
        }

        public string[] GetLogs()
        {
            return _logs.ToArray();
        }
    }

    public class TestLogger : ILogger
    {
        private readonly ConcurrentQueue<string> _logs;

        public TestLogger(ConcurrentQueue<string> logs)
        {
            _logs = logs;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logs.Enqueue(formatter(state, exception));
        }
    }

    internal class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();

        public void Dispose()
        {
            // 不执行任何操作
        }
    }
}