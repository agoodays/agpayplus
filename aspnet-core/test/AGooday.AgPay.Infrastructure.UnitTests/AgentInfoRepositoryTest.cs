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
                .SetBasePath(Directory.GetCurrentDirectory()) // �����ļ��Ļ�·��
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // �����ļ���·��
                .Build();

            var connectionString = _configuration.GetConnectionString("Default");
            _loggerProvider = new TestLoggerProvider();

            //// ����һ�� LoggerFactory ʵ������� ConsoleLoggerProvider
            //var loggerFactory = LoggerFactory.Create(builder =>
            //{
            //    builder.AddConsole() // ��Ҫ���� Microsoft.Extensions.Logging.Console ��
            //    .SetMinimumLevel(LogLevel.Information);  // ������С��־����Ϊ Information
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
            .ThenInclude(a => a.ParentAgent) // �����ϼ����ϼ�������
            .Include(a => a.SubAgents)
            .FirstOrDefault(d => d.AgentNo == "A1702728742");
            Assert.IsNotNull(agentInfo);
        }

        [TestMethod]
        public void GetAgentDetailsTest()
        {
            // ��ѯ���д����̲�����ֱ���ϼ������̺��Ӵ�����
            var allAgentInfos = _repository.GetAll()
                .Include(a => a.ParentAgent)
                .Include(a => a.SubAgents)
                .ToList();

            // �ҵ�ָ���Ĵ�����
            var agentInfo = allAgentInfos.FirstOrDefault(d => d.AgentNo == "A1702728742");

            if (agentInfo == null)
            {
                Assert.Fail();
            }

            // �ݹ��ȡ�������ȴ�����
            var allAncestors = GetAncestors(agentInfo, allAgentInfos);

            // �ݹ��ȡ�������������
            var allDescendants = GetDescendants(agentInfo, allAgentInfos);

            // ���������
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

            // �������ȴ������б���������ϼ�����Զ���ϼ�
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