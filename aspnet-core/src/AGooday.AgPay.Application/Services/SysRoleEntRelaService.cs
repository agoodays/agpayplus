using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Common.Constants;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Services
{
    /// <summary>
    /// 系统角色权限关联表 服务实现类
    /// </summary>
    public class SysRoleEntRelaService : ISysRoleEntRelaService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly ISysRoleEntRelaRepository _sysRoleEntRelaRepository;
        private readonly ISysEntitlementRepository _sysEntitlementRepository;
        private readonly ISysUserRoleRelaRepository _sysUserRoleRelaRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public SysRoleEntRelaService(IMapper mapper, IMediatorHandler bus,
            ISysRoleEntRelaRepository sysRoleEntRelaRepository,
            ISysEntitlementRepository sysEntitlementRepository,
            ISysUserRoleRelaRepository sysUserRoleRelaRepository)
        {
            _mapper = mapper;
            Bus = bus;
            _sysRoleEntRelaRepository = sysRoleEntRelaRepository;
            _sysEntitlementRepository = sysEntitlementRepository;
            _sysUserRoleRelaRepository = sysUserRoleRelaRepository;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(SysRoleEntRelaDto dto)
        {
            var m = _mapper.Map<SysRoleEntRela>(dto);
            _sysRoleEntRelaRepository.Add(m);
            _sysRoleEntRelaRepository.SaveChanges();
        }

        public void Remove(string recordId)
        {
            _sysRoleEntRelaRepository.Remove(recordId);
            _sysRoleEntRelaRepository.SaveChanges();
        }

        public void Update(SysRoleEntRelaDto dto)
        {
            var m = _mapper.Map<SysRoleEntRela>(dto);
            _sysRoleEntRelaRepository.Update(m);
            _sysRoleEntRelaRepository.SaveChanges();
        }

        public SysRoleEntRelaDto GetById(string recordId)
        {
            var entity = _sysRoleEntRelaRepository.GetById(recordId);
            var dto = _mapper.Map<SysRoleEntRelaDto>(entity);
            return dto;
        }

        public IEnumerable<SysRoleEntRelaDto> GetAll()
        {
            var sysRoleEntRelas = _sysRoleEntRelaRepository.GetAll();
            return _mapper.Map<IEnumerable<SysRoleEntRelaDto>>(sysRoleEntRelas);
        }

        /// <summary>
        /// 根据人查询出所有权限ID集合
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isAdmin"></param>
        /// <param name="sysType"></param>
        /// <returns></returns>
        public IEnumerable<string> SelectEntIdsByUserId(long userId, byte isAdmin, string sysType)
        {
            if (isAdmin == CS.YES)
            {
                var result = _sysEntitlementRepository.GetAll()
                    .Where(w => w.SysType == sysType && w.State == CS.PUB_USABLE)
                    .Select(s => s.EntId);
                return result;
            }
            else
            {
                var result = _sysUserRoleRelaRepository.GetAll()
                    .Join(_sysRoleEntRelaRepository.GetAll(),
                    ur => ur.RoleId, re => re.RoleId,
                    (ur, re) => new { ur.UserId, re.EntId })
                    .Join(_sysEntitlementRepository.GetAll(),
                        ue => ue.EntId, ent => ent.EntId,
                        (ue, ent) => new { ue.UserId, ent.EntId, ent.SysType, ent.State })
                    .Where(w => w.UserId == userId && w.SysType == sysType && w.State == CS.PUB_USABLE)
                    .Select(s => s.EntId);
                return result;
            }
        }

        /// <summary>
        /// 重置 角色 - 权限 关联关系
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="entIdList"></param>
        public void ResetRela(string roleId, List<string> entIdList)
        {
            //1. 删除
            _sysRoleEntRelaRepository.RemoveByRoleId(roleId);

            //2. 插入
            entIdList.ForEach((entId) =>
            {
                var m = new SysRoleEntRela()
                {
                    RoleId = roleId,
                    EntId = entId,
                };
                _sysRoleEntRelaRepository.Add(m);
            });

            _sysRoleEntRelaRepository.SaveChanges();
        }
    }
}
