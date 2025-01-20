using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayInterfaceDefineRepository : AgPayRepository<PayInterfaceDefine>, IPayInterfaceDefineRepository
    {
        public PayInterfaceDefineRepository(AgPayDbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// 根据支付方式查询可用的支付接口列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="wayCode"></param>
        /// <param name="appId"></param>
        /// <param name="infoType"></param>
        /// <param name="mchType"></param>
        /// <returns></returns>
        public IEnumerable<T> SelectAvailablePayInterfaceList<T>(string wayCode, string appId, string infoType, byte mchType)
        {
            var sql = $"""
                select pid.if_code IfCode, pid.if_name IfName, pid.config_page_type ConfigPageType, pid.bg_color BgColor, pid.icon Icon, pic.if_params IfParams, pic.if_rate IfRate from t_pay_interface_define pid
                inner join t_pay_interface_config pic on pid.if_code = pic.if_code
                where JSON_CONTAINS(pid.way_codes, JSON_OBJECT('wayCode', @WayCode))
                and pid.state = 1
                and pic.state = 1
                and pic.info_type = @InfoType
                and pic.info_id = @AppId
                and (pic.if_params is not null and trim(pic.if_params) != '')
                """;
            switch (mchType)
            {
                case 1:
                    sql += "\nand pid.is_mch_mode = 1";
                    break;
                case 2:
                    sql += "\nand pid.is_isv_mode = 1";
                    break;
            }
            return FromSql<T>(sql, new
            {
                WayCode = wayCode,
                InfoType = infoType,
                AppId = appId
            });
        }
    }
}
