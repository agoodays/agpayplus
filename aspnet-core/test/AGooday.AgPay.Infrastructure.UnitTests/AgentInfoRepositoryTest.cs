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
            var agentInfos = _repository.GetAllAsNoTracking()
                .Include(a => a.ParentAgent)
                .Include(a => a.SubAgents)
                .ToList();
            Assert.IsNotNull(agentInfos);
        }

        [TestMethod]
        public void GetAllParentAgentsTest(string agentNo = "A1702728742")
        {
            var agent = LoadAgentWithParents(agentNo);
            if (agent == null)
            {
                throw new ArgumentException("代理商号不存在");
            }

            var parentAgents = new List<AgentInfo>();
            while (agent.ParentAgent != null)
            {
                parentAgents.Add(agent.ParentAgent);
                agent = agent.ParentAgent;
            }
            Assert.IsNotNull(parentAgents);
        }

        /// <summary>
        /// 递归加载所有上级代理商
        /// </summary>
        /// <param name="agentNo">代理商号</param>
        /// <returns>加载了所有上级代理商的 AgentInfo 对象</returns>
        private AgentInfo LoadAgentWithParents(string agentNo)
        {
            var agent = _repository.GetAllAsNoTracking()
                .Include(a => a.ParentAgent)
                .FirstOrDefault(a => a.AgentNo == agentNo);

            if (agent?.ParentAgent != null)
            {
                LoadAgentWithParentsRecursive(agent.ParentAgent);
            }

            return agent;
        }

        /// <summary>
        /// 递归加载所有上级代理商
        /// </summary>
        /// <param name="agent">当前代理商</param>
        private void LoadAgentWithParentsRecursive(AgentInfo agent)
        {
            if (agent.ParentAgent != null)
            {
                _repository.DbEntry(agent).Reference(a => a.ParentAgent).Load();
                LoadAgentWithParentsRecursive(agent.ParentAgent);
            }
        }

        /// <summary>
        /// 获取所有下级代理商
        /// </summary>
        /// <param name="agentNo">代理商号</param>
        /// <returns>下级代理商列表</returns>
        [TestMethod]
        public void GetAllSubAgentsTest(string agentNo = "A1702728742")
        {
            var agent = LoadAgentWithSubAgents(agentNo);
            if (agent == null)
            {
                throw new ArgumentException("代理商号不存在");
            }

            var subAgents = new List<AgentInfo>();
            GetAllSubAgentsRecursive(agent, subAgents);

            Assert.IsNotNull(subAgents);
        }

        /// <summary>
        /// 递归加载所有下级代理商
        /// </summary>
        /// <param name="agentNo">代理商号</param>
        /// <returns>加载了所有下级代理商的 AgentInfo 对象</returns>
        private AgentInfo LoadAgentWithSubAgents(string agentNo)
        {
            var agent = _repository.GetAllAsNoTracking()
                .Include(a => a.SubAgents)
                .FirstOrDefault(a => a.AgentNo == agentNo);

            if (agent?.SubAgents != null && agent.SubAgents.Any())
            {
                LoadAgentWithSubAgentsRecursive(agent);
            }

            return agent;
        }

        /// <summary>
        /// 递归加载所有下级代理商
        /// </summary>
        /// <param name="agent">当前代理商</param>
        private void LoadAgentWithSubAgentsRecursive(AgentInfo agent)
        {
            if (agent.SubAgents != null && agent.SubAgents.Any())
            {
                foreach (var subAgent in agent.SubAgents)
                {
                    _repository.DbEntry(agent).Collection(a => a.SubAgents).Load();
                    LoadAgentWithSubAgentsRecursive(subAgent);
                }
            }
        }

        private void GetAllSubAgentsRecursive(AgentInfo agent, List<AgentInfo> subAgents)
        {
            if (agent.SubAgents != null && agent.SubAgents.Any())
            {
                foreach (var subAgent in agent.SubAgents)
                {
                    subAgents.Add(subAgent);
                    GetAllSubAgentsRecursive(subAgent, subAgents);
                }
            }
        }


        [TestMethod]
        public void GetAgentTest()
        {
            var agentInfo = _repository.GetAllAsNoTracking()
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
            var allAgentInfos = _repository.GetAllAsNoTracking()
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