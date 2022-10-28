using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using AGooday.AgPay.Infrastructure.Extensions.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class PayInterfaceDefineRepository : Repository<PayInterfaceDefine>, IPayInterfaceDefineRepository
    {
        public PayInterfaceDefineRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public IEnumerable<T> SelectAvailablePayInterfaceList<T>(string wayCode, string appId, byte infoType, byte mchType)
        {
            string sql = $@"select pid.if_code IfCode, pid.if_name IfName, pid.config_page_type ConfigPageType, pid.bg_color BgColor, pid.icon Icon, pic.if_params IfParams, pic.if_rate IfRate from t_pay_interface_define pid
                            inner join t_pay_interface_config pic on pid.if_code = pic.if_code
                            where JSON_CONTAINS(pid.way_codes, JSON_OBJECT('wayCode', @wayCode))
                            and pid.state = 1
                            and pic.state = 1
                            and pic.info_type = @infoType
                            and pic.info_id = @appId
                            and (pic.if_params is not null and trim(pic.if_params) != '')";
            switch (mchType)
            {
                case 1:
                    sql += "\nand pid.is_mch_mode = 1";
                    break;
                case 2:
                    sql += "\nand pid.is_isv_mode = 1";
                    break;
            }
            return Db.Database.FromSql<T>(sql, new {
                WayCode= wayCode,
                InfoType = infoType,
                AppId= appId
            });
        }
    }
}
