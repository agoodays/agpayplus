using AGooday.AgPay.Common.DB;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Infrastructure.Context
{
    public class AgPayDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public AgPayDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region DbSets
        public DbSet<IsvInfo> IsvInfo { get; set; }
        public DbSet<MchApp> MchApp { get; set; }
        public DbSet<MchDivisionReceiver> MchDivisionReceiver { get; set; }
        public DbSet<MchDivisionReceiverGroup> MchDivisionReceiverGroup { get; set; }
        public DbSet<MchInfo> MchInfo { get; set; }
        public DbSet<MchNotifyRecord> MchNotifyRecord { get; set; }
        public DbSet<MchPayPassage> MchPayPassage { get; set; }
        public DbSet<OrderSnapshot> OrderSnapshot { get; set; }
        public DbSet<PayInterfaceConfig> PayInterfaceConfig { get; set; }
        public DbSet<PayInterfaceDefine> PayInterfaceDefine { get; set; }
        public DbSet<PayOrder> PayOrder { get; set; }
        public DbSet<PayOrderDivisionRecord> PayOrderDivisionRecord { get; set; }
        public DbSet<PayWay> PayWay { get; set; }
        public DbSet<RefundOrder> RefundOrder { get; set; }
        public DbSet<SysConfig> SysConfig { get; set; }
        public DbSet<SysEntitlement> SysEntitlement { get; set; }
        public DbSet<SysLog> SysLog { get; set; }
        public DbSet<SysRole> SysRole { get; set; }
        public DbSet<SysRoleEntRela> SysRoleEntRela { get; set; }
        public DbSet<SysUser> SysUser { get; set; }
        public DbSet<SysUserAuth> SysUserAuth { get; set; }
        public DbSet<SysUserRoleRela> SysUserRoleRela { get; set; }
        public DbSet<TransferOrder> TransferOrder { get; set; }
        #endregion

        //public AgPayDbContext(DbContextOptions<AgPayDbContext> options)
        //    : base(options)
        //{
        //}

        /// <summary>
        /// 重写连接数据库
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var conn = "server=localhost;port=3306;uid=root;pwd=mysql*;database=agpaydb_dev";//Configuration.GetConnectionString("Default")
                optionsBuilder.UseMySql(conn, MySqlServerVersion.LatestSupportedServerVersion);
            }
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
            //switch (BaseDBConfig.DbType)
            //{
            //    case DataBaseType.MySql:
            //        optionsBuilder.
            //            // 安装NuGet包 Pomelo.EntityFrameworkCore.MySql
            //            UseMySql(BaseDBConfig.ConnectionString, MySqlServerVersion.LatestSupportedServerVersion);
            //        break;
            //        //case DataBaseType.SqlServer:
            //        //    optionsBuilder
            //        //        // 安装NuGet包 Microsoft.EntityFrameworkCore.SqlServer
            //        //        .UseSqlServer(BaseDBConfig.ConnectionString);
            //        //    break;
            //        //case DataBaseType.Sqlite:
            //        //    optionsBuilder.
            //        //        // 安装NuGet包 Microsoft.EntityFrameworkCore.Sqlite
            //        //        UseSqlite(BaseDBConfig.ConnectionString);
            //        //    break;
            //        //case DataBaseType.Oracle:
            //        //    break;
            //        //case DataBaseType.PostgreSQL:
            //        //    break;
            //        //default:
            //        //    optionsBuilder.UseSqlServer(BaseDBConfig.ConnectionString);
            //        //break;
            //}
            #endregion

            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// 重写自定义Map配置
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MchNotifyRecord>().HasIndex(c => new { c.OrderId, c.OrderType }, "Uni_OrderId_Type").IsUnique(true);
            modelBuilder.Entity<MchPayPassage>().HasIndex(c => new { c.AppId, c.IfCode, c.WayCode }, "Uni_AppId_IfCode_WayCode").IsUnique(true);
            modelBuilder.Entity<PayInterfaceConfig>().HasIndex(c => new { c.InfoType, c.InfoId, c.IfCode }, "Uni_InfoType_InfoId_IfCode").IsUnique(true);
            modelBuilder.Entity<PayOrder>().HasIndex(c => new { c.MchNo, c.MchOrderNo }, "Uni_MchNo_MchOrderNo").IsUnique(true);
            modelBuilder.Entity<PayOrder>().HasIndex(c => new { c.CreatedAt }, "Idx_CreatedAt");
            modelBuilder.Entity<RefundOrder>().HasIndex(c => new { c.MchNo, c.MchRefundNo }, "Uni_MchNo_MchRefundNo").IsUnique(true);
            modelBuilder.Entity<RefundOrder>().HasIndex(c => new { c.CreatedAt }, "Idx_CreatedAt");
            modelBuilder.Entity<SysUser>().HasIndex(c => new { c.SysType, c.LoginUsername }, "Uni_SysType_LoginUsername").IsUnique(true);
            modelBuilder.Entity<SysUser>().HasIndex(c => new { c.SysType, c.Telphone }, "Uni_SysType_Telphone").IsUnique(true);
            modelBuilder.Entity<SysUser>().HasIndex(c => new { c.SysType, c.UserNo }, "Uni_SysType_UserNo").IsUnique(true);
            modelBuilder.Entity<TransferOrder>().HasIndex(c => new { c.MchNo, c.MchOrderNo }, "Uni_MchNo_MchOrderNo").IsUnique(true);
            modelBuilder.Entity<TransferOrder>().HasIndex(c => new { c.CreatedAt }, "Idx_CreatedAt");

            modelBuilder.Entity<OrderSnapshot>().HasKey(c => new { c.OrderId, c.OrderType });
            modelBuilder.Entity<OrderSnapshot>().HasKey(c => new { c.OrderId, c.OrderType });
            modelBuilder.Entity<SysRoleEntRela>().HasKey(c => new { c.RoleId, c.EntId });
            modelBuilder.Entity<SysUserRoleRela>().HasKey(c => new { c.UserId, c.RoleId });

            //对 PayOrderMap 进行配置
            //modelBuilder.ApplyConfiguration(new PayOrderMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
