using Base;
using DAL.DTOModel;
using Dapper;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DAL.Repository
{
    public class RoleRepository : ConnectionBase
    {
        public RoleRepository(IConfiguration config, ILogger<ConnectionBase> log) : base(config, log)
        {
        }

        //public RoleRepository(string con, int timeout) : base(con, timeout)
        //{
        //}

        /// <summary>
        /// 依帳號取得角色資訊
        /// </summary>
        /// <param name="EmployeeId">帳號</param>
        public virtual (Result rtn, List<RoleOfMenuDTO> roles) GetRolesByAccount(string Id)
        {
            int EmployeeId = 0;
            int.TryParse(Id, out EmployeeId);
            //實值型別初始化為0
            if (EmployeeId == 0) EmployeeId = -1;

            string sqlCmd = @"
select RoleId,RoleName from Role
where RoleId in (
    select distinct RoleId from RoleEmployee
    where EmployeeId = @EmployeeId or CustomerId = @Id and Status = 10
	)
and Status = 10";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@EmployeeId", EmployeeId);
            parameters.Add("@Id", Id);

            var result = this.GetList<RoleOfMenuDTO>(sqlCmd, parameters);

            return (result.rtn, result.result.ToList());
        }
    }
}
