using AGooday.AgPay.Common.DB;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Infrastructure.Context
{
    public class AgPayDbContext : DbContext
    {
        #region DbSets
        /// <summary>
        /// 系统用户信息
        /// </summary>
        public DbSet<SysUser> SysUser { get; set; }

        /// <summary>
        /// 支付订单信息
        /// </summary>
        public DbSet<PayOrder> PayOrder { get; set; }
        #endregion
        ////执行
        //public AgPayDbContext(DbContextOptions<AgPayDbContext> options)
        //    : base(options)
        //{
        //}
        /// <summary>
        /// 重写自定义Map配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //对 PayOrderMap 进行配置
            modelBuilder.ApplyConfiguration(new PayOrderMap());

            base.OnModelCreating(modelBuilder);
        }
        /// <summary>
        /// 重写连接数据库
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #region
            //// 从 appsetting.json 中获取配置信息
            //var config = new ConfigurationBuilder()
            //    // 安装NuGet包 Microsoft.Extensions.Configuration
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    // 安装NuGet包 Microsoft.Extensions.Configuration.Json（支持JSON配置文件）
            //    .AddJsonFile("appsettings.json")
            //    .Build();

            //// 定义要使用的数据库
            //// 我是读取的文件内容，为了数据安全
            //optionsBuilder
            //    // 安装NuGet包 Microsoft.EntityFrameworkCore.SqlServer
            //    .UseSqlServer(BaseDBConfig.GetConnectionString(config.GetConnectionString("DefaultConnectionFile"), config.GetConnectionString("DefaultConnection")));
            #endregion

            #region
            switch (BaseDBConfig.DbType)
            {
                case DataBaseType.MySql:
                    var serverVersion = MySqlServerVersion.LatestSupportedServerVersion;
                    serverVersion = new MySqlServerVersion(new Version(8, 0, 29));
                    optionsBuilder.
                        // 安装NuGet包 Pomelo.EntityFrameworkCore.MySql
                        UseMySql(BaseDBConfig.ConnectionString, serverVersion);
                    break;
                    //case DataBaseType.SqlServer:
                    //    optionsBuilder
                    //        // 安装NuGet包 Microsoft.EntityFrameworkCore.SqlServer
                    //        .UseSqlServer(BaseDBConfig.ConnectionString);
                    //    break;
                    //case DataBaseType.Sqlite:
                    //    optionsBuilder.
                    //        // 安装NuGet包 Microsoft.EntityFrameworkCore.Sqlite
                    //        UseSqlite(BaseDBConfig.ConnectionString);
                    //    break;
                    //case DataBaseType.Oracle:
                    //    break;
                    //case DataBaseType.PostgreSQL:
                    //    break;
                    //default:
                    //    optionsBuilder.UseSqlServer(BaseDBConfig.ConnectionString);
                    //break;
            }
            #endregion

            //base.OnConfiguring(optionsBuilder);
        }
    }
}
