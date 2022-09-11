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

        public AgPayDbContext(DbContextOptions<AgPayDbContext> options)
            : base(options)
        {
        }
    }
}
