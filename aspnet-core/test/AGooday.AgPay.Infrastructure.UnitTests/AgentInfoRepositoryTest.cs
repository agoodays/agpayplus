using AGooday.AgPay.Domain.Models;
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
        public void GetParentAgentsTest()
        {
            var agentInfos = _repository.GetParentAgents("A1702728742");
            Assert.IsNotNull(agentInfos);
        }

        [TestMethod]
        public void GetAllSubAgentsTest()
        {
            var agentInfos = _repository.GetAllSubAgents("A1702728742");
            Assert.IsNotNull(agentInfos);
        }

        [TestMethod]
        public void GetAgentsTest()
        {
            var agentInfos = _repository.GetAll()
                .Include(a => a.ParentAgent)
                .Include(a => a.SubAgents)
                .ToList();
            Assert.IsNotNull(agentInfos);
        }

        [TestMethod]
        public void GetAgentTest()
        {
            var agentInfo = _repository.GetAll()
            .Include(a => a.ParentAgent)
            .ThenInclude(a => a.ParentAgent) // 加载上级的上级代理商
            .Include(a => a.SubAgents)
            .FirstOrDefault(d => d.AgentNo == "A1702728742");
            Assert.IsNotNull(agentInfo);
        }

        [TestMethod]
        public void GetAgentDetailsTest()
        {
            // 查询所有代理商并加载直接上级代理商和子代理商
            var allAgentInfos = _repository.GetAll()
                .Include(a => a.ParentAgent)
                .Include(a => a.SubAgents)
                .ToList();

            // 找到指定的代理商
            var agentInfo = allAgentInfos.FirstOrDefault(d => d.AgentNo == "A1702728742");

            if (agentInfo == null)
            {
                Assert.Fail();
            }

            // 递归获取所有祖先代理商
            var allAncestors = GetAncestors(agentInfo, allAgentInfos);

            // 递归获取所有子孙代理商
            var allDescendants = GetDescendants(agentInfo, allAgentInfos);

            // 将结果传递
            var result = new
            {
                AgentInfo = agentInfo,
                Ancestors = allAncestors,
                Descendants = allDescendants
            };
        }

        private List<AgentInfo> GetAncestors(AgentInfo agentInfo, List<AgentInfo> allAgentInfos)
        {
            var ancestors = new List<AgentInfo>();

            while (agentInfo.ParentAgent != null)
            {
                agentInfo = allAgentInfos.FirstOrDefault(d => d.AgentNo == agentInfo.Pid);
                if (agentInfo != null)
                {
                    ancestors.Add(agentInfo);
                }
                else
                {
                    break;
                }
            }

            // 返回祖先代理商列表，从最近的上级到最远的上级
            ancestors.Reverse();
            return ancestors;
        }

        private List<AgentInfo> GetDescendants(AgentInfo agentInfo, List<AgentInfo> allAgentInfos)
        {
            var descendants = new List<AgentInfo>();

            if (agentInfo.SubAgents != null && agentInfo.SubAgents.Any())
            {
                foreach (var subAgentInfo in agentInfo.SubAgents)
                {
                    descendants.Add(subAgentInfo);
                    descendants.AddRange(GetDescendants(subAgentInfo, allAgentInfos));
                }
            }

            return descendants;
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