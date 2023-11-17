using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using AGooday.AgPay.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.UnitTests
{
    [TestClass]
    public class RepositoryTest
    {
        private DbContextOptions<AgPayDbContext> _options;
        private AgPayDbContext _dbContext;
        private SysUserRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            var connectionString = "server=localhost;port=3306;uid=root;pwd=mysql*;database=agpayplusdb_unit_test";

            _options = new DbContextOptionsBuilder<AgPayDbContext>()
                .UseMySql(connectionString, MySqlServerVersion.LatestSupportedServerVersion)
                .Options;

            _dbContext = new AgPayDbContext(_options);
            _repository = new SysUserRepository(_dbContext);
        }

        [TestMethod]
        public void GetAllTest()
        {
            var sysUsers = _repository.GetAll();
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
            _repository.UpdateProperty(e => e.SysUserId == 801, e => new { SafeWord = "test", State = (byte)1 });
            var reault = _repository.SaveChanges(out int count);
            Assert.IsTrue(reault);
        }

        [TestCleanup]
        public void Cleanup()
        {
            //_dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}