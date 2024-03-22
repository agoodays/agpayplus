using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
        public DbSet<AccountBill> AccountBill { get; set; }
        public DbSet<AgentInfo> AgentInfo { get; set; }
        public DbSet<IsvInfo> IsvInfo { get; set; }
        public DbSet<PayRateLevelConfig> PayRateLevelConfig { get; set; }
        public DbSet<MchApp> MchApp { get; set; }
        public DbSet<MchDivisionReceiver> MchDivisionReceiver { get; set; }
        public DbSet<MchDivisionReceiverGroup> MchDivisionReceiverGroup { get; set; }
        public DbSet<MchInfo> MchInfo { get; set; }
        public DbSet<MchNotifyRecord> MchNotifyRecord { get; set; }
        public DbSet<MchPayPassage> MchPayPassage { get; set; }
        public DbSet<MchStore> MchStore { get; set; }
        public DbSet<OrderSnapshot> OrderSnapshot { get; set; }
        public DbSet<PayInterfaceConfig> PayInterfaceConfig { get; set; }
        public DbSet<PayInterfaceDefine> PayInterfaceDefine { get; set; }
        public DbSet<PayOrder> PayOrder { get; set; }
        public DbSet<PayOrderDivisionRecord> PayOrderDivisionRecord { get; set; }
        public DbSet<PayOrderProfit> PayOrderProfit { get; set; }
        public DbSet<PayRateConfig> PayRateConfig { get; set; }
        public DbSet<PayWay> PayWay { get; set; }
        public DbSet<QrCode> QrCode { get; set; }
        public DbSet<QrCodeShell> QrCodeShell { get; set; }
        public DbSet<RefundOrder> RefundOrder { get; set; }
        public DbSet<SysArticle> SysArticle { get; set; }
        public DbSet<SysConfig> SysConfig { get; set; }
        public DbSet<SysEntitlement> SysEntitlement { get; set; }
        public DbSet<SysLog> SysLog { get; set; }
        public DbSet<SysRole> SysRole { get; set; }
        public DbSet<SysRoleEntRela> SysRoleEntRela { get; set; }
        public DbSet<SysUser> SysUser { get; set; }
        public DbSet<SysUserTeam> SysUserTeam { get; set; }
        public DbSet<SysUserAuth> SysUserAuth { get; set; }
        public DbSet<SysUserLoginAttempt> SysUserLoginAttempt { get; set; }
        public DbSet<SysUserRoleRela> SysUserRoleRela { get; set; }
        public DbSet<TransferOrder> TransferOrder { get; set; }
        #endregion

        public AgPayDbContext(DbContextOptions<AgPayDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// 重写连接数据库
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.EnableSensitiveDataLogging();
                optionsBuilder.UseMySql(Configuration.GetConnectionString("Default"),
                    MySqlServerVersion.LatestSupportedServerVersion);
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
            modelBuilder.Entity<AccountBill>().Property(c => c.BeforeBalance).HasDefaultValue(0);
            modelBuilder.Entity<AccountBill>().Property(c => c.ChangeAmount).HasDefaultValue(0);
            modelBuilder.Entity<AccountBill>().Property(c => c.AfterBalance).HasDefaultValue(0);
            modelBuilder.Entity<AccountBill>().Property(c => c.BizType).HasDefaultValue(1);
            modelBuilder.Entity<AccountBill>().Property(c => c.AccountType).HasDefaultValue(1);
            modelBuilder.Entity<AccountBill>().Property(c => c.RelaBizOrderType).HasDefaultValue(1);
            modelBuilder.Entity<AccountBill>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<AccountBill>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<AgentInfo>().Property(c => c.AgentType).HasDefaultValue(1);
            modelBuilder.Entity<AgentInfo>().Property(c => c.Level).HasDefaultValue(1);
            modelBuilder.Entity<AgentInfo>().Property(c => c.AddAgentFlag).HasDefaultValue(0);
            modelBuilder.Entity<AgentInfo>().Property(c => c.State).HasDefaultValue(1);
            modelBuilder.Entity<AgentInfo>().Property(c => c.CashoutFeeRuleType).HasDefaultValue(1);
            modelBuilder.Entity<AgentInfo>().Property(c => c.UnAmount).HasDefaultValue(0);
            modelBuilder.Entity<AgentInfo>().Property(c => c.BalanceAmount).HasDefaultValue(0);
            modelBuilder.Entity<AgentInfo>().Property(c => c.AuditProfitAmount).HasDefaultValue(0);
            modelBuilder.Entity<AgentInfo>().Property(c => c.FreezeAmount).HasDefaultValue(0);
            modelBuilder.Entity<AgentInfo>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<AgentInfo>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)"); 
            modelBuilder.Entity<AgentInfo>().HasMany(a => a.SubAgents)
                .WithOne(a => a.ParentAgent).HasForeignKey(a => a.Pid);
            modelBuilder.Entity<IsvInfo>().Property(c => c.State).HasDefaultValue(1);
            modelBuilder.Entity<IsvInfo>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<IsvInfo>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MchApp>().Property(c => c.AppName).HasDefaultValue("");
            modelBuilder.Entity<MchApp>().Property(c => c.State).HasDefaultValue(1);
            modelBuilder.Entity<MchApp>().Property(c => c.DefaultFlag).HasDefaultValue(0);
            modelBuilder.Entity<MchApp>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MchApp>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MchDivisionReceiver>().Property(c => c.AccName).HasDefaultValue("");
            modelBuilder.Entity<MchDivisionReceiver>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MchDivisionReceiver>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MchDivisionReceiverGroup>().Property(c => c.AutoDivisionFlag).HasDefaultValue(0);
            modelBuilder.Entity<MchDivisionReceiverGroup>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MchDivisionReceiverGroup>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MchInfo>().Property(c => c.Type).HasDefaultValue(1);
            modelBuilder.Entity<MchInfo>().Property(c => c.MchLevel).HasDefaultValue("M0");
            modelBuilder.Entity<MchInfo>().Property(c => c.State).HasDefaultValue(1);
            modelBuilder.Entity<MchInfo>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MchInfo>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MchNotifyRecord>().Property(c => c.NotifyCount).HasDefaultValue(0);
            modelBuilder.Entity<MchNotifyRecord>().Property(c => c.NotifyCountLimit).HasDefaultValue(6);
            modelBuilder.Entity<MchNotifyRecord>().Property(c => c.State).HasDefaultValue(1);
            modelBuilder.Entity<MchNotifyRecord>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MchNotifyRecord>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MchPayPassage>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MchPayPassage>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MchStore>().Property(c => c.StoreName).HasDefaultValue("");
            modelBuilder.Entity<MchStore>().Property(c => c.DefaultFlag).HasDefaultValue(0);
            modelBuilder.Entity<MchStore>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<MchStore>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<OrderSnapshot>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<OrderSnapshot>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayInterfaceConfig>().Property(c => c.State).HasDefaultValue(1);
            modelBuilder.Entity<PayInterfaceConfig>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayInterfaceConfig>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayInterfaceDefine>().Property(c => c.IsMchMode).HasDefaultValue(1);
            modelBuilder.Entity<PayInterfaceDefine>().Property(c => c.IsIsvMode).HasDefaultValue(1);
            modelBuilder.Entity<PayInterfaceDefine>().Property(c => c.ConfigPageType).HasDefaultValue(1);
            modelBuilder.Entity<PayInterfaceDefine>().Property(c => c.State).HasDefaultValue(1);
            modelBuilder.Entity<PayInterfaceDefine>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayInterfaceDefine>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayOrder>().Property(c => c.Currency).HasDefaultValue("CNY");
            modelBuilder.Entity<PayOrder>().Property(c => c.State).HasDefaultValue(0);
            modelBuilder.Entity<PayOrder>().Property(c => c.NotifyState).HasDefaultValue(0);
            modelBuilder.Entity<PayOrder>().Property(c => c.RefundState).HasDefaultValue(0);
            modelBuilder.Entity<PayOrder>().Property(c => c.RefundTimes).HasDefaultValue(0);
            modelBuilder.Entity<PayOrder>().Property(c => c.RefundAmount).HasDefaultValue(0);
            //modelBuilder.Entity<PayOrder>().Property(c => c.DivisionMode).HasDefaultValue(0);
            //modelBuilder.Entity<PayOrder>().Property(c => c.DivisionState).HasDefaultValue(0);
            modelBuilder.Entity<PayOrder>().Property(c => c.NotifyUrl).HasDefaultValue("");
            modelBuilder.Entity<PayOrder>().Property(c => c.ReturnUrl).HasDefaultValue("");
            modelBuilder.Entity<PayOrder>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayOrder>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayOrderDivisionRecord>().Property(c => c.AccName).HasDefaultValue("");
            modelBuilder.Entity<PayOrderDivisionRecord>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayOrderDivisionRecord>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayOrderProfit>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayOrderProfit>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayRateConfig>().Property(c => c.State).HasDefaultValue(0);
            modelBuilder.Entity<PayRateConfig>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayRateConfig>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayRateLevelConfig>().Property(c => c.MinAmount).HasDefaultValue(0);
            modelBuilder.Entity<PayRateLevelConfig>().Property(c => c.MaxAmount).HasDefaultValue(0);
            modelBuilder.Entity<PayRateLevelConfig>().Property(c => c.MinFee).HasDefaultValue(0);
            modelBuilder.Entity<PayRateLevelConfig>().Property(c => c.MaxFee).HasDefaultValue(0);
            modelBuilder.Entity<PayRateLevelConfig>().Property(c => c.State).HasDefaultValue(0);
            modelBuilder.Entity<PayRateLevelConfig>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayRateLevelConfig>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayWay>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<PayWay>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<QrCode>().Property(c => c.FixedPayAmount).HasDefaultValue(0);
            modelBuilder.Entity<QrCode>().Property(c => c.State).HasDefaultValue(1);
            modelBuilder.Entity<QrCode>().Property(c => c.BelongInfoId).HasDefaultValue(0);
            modelBuilder.Entity<QrCode>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<QrCode>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<QrCodeShell>().Property(c => c.BelongInfoId).HasDefaultValue(0);
            modelBuilder.Entity<QrCodeShell>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<QrCodeShell>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<RefundOrder>().Property(c => c.Currency).HasDefaultValue("CNY");
            modelBuilder.Entity<RefundOrder>().Property(c => c.State).HasDefaultValue(0);
            modelBuilder.Entity<RefundOrder>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<RefundOrder>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<SysArticle>().Property(c => c.ArticleType).HasDefaultValue(1);
            modelBuilder.Entity<SysArticle>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<SysArticle>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<SysConfig>().Property(c => c.Type).HasDefaultValue("text");
            modelBuilder.Entity<SysConfig>().Property(c => c.SortNum).HasDefaultValue(0);
            modelBuilder.Entity<SysConfig>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<SysEntitlement>().Property(c => c.QuickJump).HasDefaultValue(0);
            modelBuilder.Entity<SysEntitlement>().Property(c => c.State).HasDefaultValue(1);
            modelBuilder.Entity<SysEntitlement>().Property(c => c.EntSort).HasDefaultValue(0);
            modelBuilder.Entity<SysEntitlement>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<SysEntitlement>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<SysLog>().Property(c => c.UserIp).HasDefaultValue("");
            modelBuilder.Entity<SysLog>().Property(c => c.MethodName).HasDefaultValue("");
            modelBuilder.Entity<SysLog>().Property(c => c.MethodRemark).HasDefaultValue("");
            modelBuilder.Entity<SysLog>().Property(c => c.ReqUrl).HasDefaultValue("");
            modelBuilder.Entity<SysLog>().Property(c => c.OptReqParam).HasDefaultValue("");
            modelBuilder.Entity<SysLog>().Property(c => c.OptResInfo).HasDefaultValue("");
            modelBuilder.Entity<SysLog>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<SysRole>().Property(c => c.BelongInfoId).HasDefaultValue(0);
            modelBuilder.Entity<SysRole>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<SysUser>().Property(c => c.Sex).HasDefaultValue(1);
            modelBuilder.Entity<SysUser>().Property(c => c.InitUser).HasDefaultValue(false);
            modelBuilder.Entity<SysUser>().Property(c => c.State).HasDefaultValue(1);
            modelBuilder.Entity<SysUser>().Property(c => c.BelongInfoId).HasDefaultValue(0);
            modelBuilder.Entity<SysUser>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<SysUser>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<SysUserTeam>().Property(c => c.BelongInfoId).HasDefaultValue(0);
            modelBuilder.Entity<SysUserTeam>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<SysUserTeam>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<SysUserAuth>().Property(c => c.IdentityType).HasDefaultValue(0);
            modelBuilder.Entity<SysUserLoginAttempt>().Property(c => c.IdentityType).HasDefaultValue(1);
            modelBuilder.Entity<SysUserLoginAttempt>().Property(c => c.AttemptTime).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<TransferOrder>().Property(c => c.Currency).HasDefaultValue("CNY");
            modelBuilder.Entity<TransferOrder>().Property(c => c.State).HasDefaultValue(0);
            modelBuilder.Entity<TransferOrder>().Property(c => c.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            modelBuilder.Entity<TransferOrder>().Property(c => c.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            modelBuilder.Entity<MchNotifyRecord>().HasIndex(c => new { c.OrderId, c.OrderType }, "Uni_OrderId_Type").IsUnique(true);
            modelBuilder.Entity<MchPayPassage>().HasIndex(c => new { c.AppId, c.IfCode, c.WayCode }, "Uni_AppId_IfCode_WayCode").IsUnique(true);
            modelBuilder.Entity<PayInterfaceConfig>().HasIndex(c => new { c.InfoType, c.InfoId, c.IfCode }, "Uni_InfoType_InfoId_IfCode").IsUnique(true);
            modelBuilder.Entity<PayOrder>().HasIndex(c => new { c.MchNo, c.MchOrderNo }, "Uni_MchNo_MchOrderNo").IsUnique(true);
            modelBuilder.Entity<PayOrder>().HasIndex(c => new { c.CreatedAt }, "Idx_CreatedAt");
            modelBuilder.Entity<PayOrderProfit>().HasIndex(c => new { c.InfoId, c.InfoType, c.PayOrderId }, "Uni_InfoId_InfoType_PayOrderId").IsUnique(true);
            modelBuilder.Entity<PayOrderProfit>().HasIndex(c => new { c.CreatedAt }, "Idx_CreatedAt");
            modelBuilder.Entity<PayRateConfig>().HasIndex(c => new { c.ConfigType, c.InfoType, c.InfoId, c.IfCode, c.WayCode }, "Uni_InfoId_WayCode").IsUnique(true);
            modelBuilder.Entity<RefundOrder>().HasIndex(c => new { c.MchNo, c.MchRefundNo }, "Uni_MchNo_MchRefundNo").IsUnique(true);
            modelBuilder.Entity<RefundOrder>().HasIndex(c => new { c.CreatedAt }, "Idx_CreatedAt");
            modelBuilder.Entity<SysUser>().HasIndex(c => new { c.SysType, c.LoginUsername }, "Uni_SysType_LoginUsername").IsUnique(true);
            modelBuilder.Entity<SysUser>().HasIndex(c => new { c.SysType, c.Telphone }, "Uni_SysType_Telphone").IsUnique(true);
            modelBuilder.Entity<SysUser>().HasIndex(c => new { c.SysType, c.UserNo }, "Uni_SysType_UserNo").IsUnique(true);
            modelBuilder.Entity<SysUser>().HasIndex(c => new { c.InviteCode }, "Uni_InviteCode").IsUnique(true);
            modelBuilder.Entity<TransferOrder>().HasIndex(c => new { c.MchNo, c.MchOrderNo }, "Uni_MchNo_MchOrderNo").IsUnique(true);
            modelBuilder.Entity<TransferOrder>().HasIndex(c => new { c.CreatedAt }, "Idx_CreatedAt");

            modelBuilder.Entity<OrderSnapshot>().HasKey(c => new { c.OrderId, c.OrderType });
            modelBuilder.Entity<OrderSnapshot>().HasKey(c => new { c.OrderId, c.OrderType });
            modelBuilder.Entity<SysConfig>().HasKey(c => new { c.ConfigKey, c.SysType, c.BelongInfoId });
            modelBuilder.Entity<SysEntitlement>().HasKey(c => new { c.EntId, c.SysType });
            modelBuilder.Entity<SysRoleEntRela>().HasKey(c => new { c.RoleId, c.EntId });
            modelBuilder.Entity<SysUserRoleRela>().HasKey(c => new { c.UserId, c.RoleId });

            //对 PayOrderMap 进行配置
            modelBuilder.ApplyConfiguration(new PayOrderMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
