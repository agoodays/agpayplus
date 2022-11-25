using AGooday.AgPay.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AGooday.AgPay.Infrastructure.Mappings
{
    public class PayOrderMap : IEntityTypeConfiguration<PayOrder>
    {
        public void Configure(EntityTypeBuilder<PayOrder> builder)
        {
            //实体名称Map
            builder.ToTable("t_pay_order");
            #region 实体属性Map
            builder.Property(c => c.PayOrderId)
                .HasColumnType("varchar(30)")
                .HasColumnName("pay_order_id")
                .HasComment("支付订单号")
                .HasMaxLength(30)
                .IsRequired()//是否必须
                ;

            builder.Property(c => c.State)
                .HasDefaultValue(0)//默认值
                ;

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")//默认值
                ;

            builder.Property(c => c.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)")//默认值
                ;
            #endregion
        }
    }
}
