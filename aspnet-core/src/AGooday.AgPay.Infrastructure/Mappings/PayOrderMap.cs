using AGooday.AgPay.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Infrastructure.Mappings
{
    public class PayOrderMap : IEntityTypeConfiguration<PayOrder>
    {
        public void Configure(EntityTypeBuilder<PayOrder> builder)
        {
            //实体名称Map
            builder.ToTable("t_pay_order");
            #region 实体属性Map
            //实体属性Map
            builder.Property(c => c.Id)
                .HasColumnName("id")
                ;

            builder.Property(c => c.PayOrderId)
                .HasColumnType("varchar(30)")
                .HasColumnName("pay_order_id")
                .HasMaxLength(30)
                .IsRequired()//是否必须
                ;

            builder.Property(c => c.MchNo)
                .HasColumnType("varchar(64)")
                .HasColumnName("mch_no")
                .HasMaxLength(64)
                .IsRequired()
                ;
            builder.Property(c => c.State)
                .HasDefaultValue(0)//默认值
                ;

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("getdate()")//默认值
                ;

            builder.Property(c => c.UpdatedAt)
                .HasDefaultValueSql("getdate()")//默认值
                ;
            #endregion
        }
    }
}
