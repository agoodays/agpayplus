using AGooday.AgPay.Domain.Commands.MchInfos;
using AGooday.AgPay.Domain.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Domain.Validations.MchInfos
{
    public abstract class MchInfoValidation<T> : AbstractValidator<T> where T : MchInfoCommand
    {
        /// <summary>
        /// 验证商户名称
        /// </summary>
        protected void ValidateMchName()
        {
            //定义规则，c 就是当前 SysUserValidation 类
            RuleFor(c => c.MchName)
                .NotEmpty().WithMessage("商户名称不能为空！")//判断不能为空，如果为空则显示Message
                ;
        }
        /// <summary>
        /// 验证商户简称
        /// </summary>
        protected void ValidateMchShortName()
        {
            //定义规则，c 就是当前 SysUserValidation 类
            RuleFor(c => c.MchShortName)
                .NotEmpty().WithMessage("商户名称不能为空！")//判断不能为空，如果为空则显示Message
                ;
        }

        /// <summary>
        /// 验证登录用户名
        /// </summary>
        protected void ValidateLoginUsername()
        {
            //定义规则，c 就是当前 SysUserValidation 类
            RuleFor(c => c.LoginUsername)
                .NotEmpty().WithMessage("登录用户名不能为空！")//判断不能为空，如果为空则显示Message
                .Length(2, 10).WithMessage("登录用户名在6~32个字符之间")//定义字段的长度
                ;
        }

        /// <summary>
        /// 验证联系人姓名
        /// </summary>
        protected void ValidateContactName()
        {
            RuleFor(c => c.ContactName)
                .NotEmpty().WithMessage("联系人姓名不能为空！")
                ;
        }

        /// <summary>
        /// 验证联系人手机号
        /// </summary>
        protected void ValidateContactTel()
        {
            RuleFor(c => c.ContactTel)
                .NotEmpty()
                .Must(HavePhone)
                .WithMessage("联系人手机号应该为11位！")
                ;
        }

        // 表达式
        private static bool HavePhone(string phone)
        {
            return phone?.Length == 11;
        }
    }
}
