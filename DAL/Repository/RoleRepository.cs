using Base;
using DAL.DTOModel;
using Dapper;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace DAL.Repository
{
    public class RoleRepository : ConnectionBase
    {
        public RoleRepository(string con, int timeout) : base(con, timeout)
        {
        }

        /// <summary>
        /// 依帳號取得角色資訊
        /// </summary>
        /// <param name="EmployeeId">帳號</param>
        public (Result rtn, List<RoleOfMenuDTO> roleDto) GetRoleInfoById(int Id)
        {
            string sqlCmd = @"
select RoleId,RoleName from Role
where RoleId in (
    select distinct RoleId from RoleEmployee
    where EmployeeId = @EmployeeId or CustomerId = @Id and Status = 10
	)
and Status = 10";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", Id);

            var result = this.GetList<RoleOfMenuDTO>(sqlCmd, parameters);

            return (result.rtn, result.result.ToList());
        }
    }
}
