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

        protected readonly IConfiguration Configuration;

        public AgPayDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 重写连接数据库
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //如果为实体类单独建了类库，需要在此处配置数据库连接。
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(Configuration.GetConnectionString("Default"),
                    MySqlServerVersion.LatestSupportedServerVersion);
            }
        }
    }
}
