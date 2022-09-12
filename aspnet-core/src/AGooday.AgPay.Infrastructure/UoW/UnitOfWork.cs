using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Infrastructure.UoW
{
    /// <summary>
    /// 工作单元类
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        //数据库上下文
        private readonly AgPayDbContext _dbcontext;

        //构造函数注入
        public UnitOfWork(AgPayDbContext context)
        {
            _dbcontext = context;
        }

        //上下文提交
        public bool Commit()
        {
            return _dbcontext.SaveChanges() > 0;
        }

        public async Task<bool> CommitAsync()
        {
            return await _dbcontext.SaveChangesAsync() > 0;
        }

        //手动回收
        public void Dispose()
        {
            _dbcontext.Dispose();
        }
    }
}
