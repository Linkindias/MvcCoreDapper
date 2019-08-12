using Base;
using DAL.DBModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using static Base.Enums;

namespace DAL.Repository
{
    public class CustomerRepository : ConnectionBase
    {
        public CustomerRepository(string con, int timeout) : base(con, timeout)
        {
        }

        /// <summary>
        /// 依帳號取得客戶
        /// </summary>
        /// <param name="Account">帳號</param>
        public (Result rtn, Customers custom) GetCustomerByAccount(string Account, int Status)
        {
            Result rtn = new Result();
            string sqlCmd = "SELECT * FROM Customers Where Account = @Account And Status = @Status ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Account", Account);
            parameters.Add("@Status", Status);
            Customers custom = null;

            var result = this.GetSingleDefault<Customers>(sqlCmd, parameters);

            return (result.rtn, result.result);
        }

        /// <summary>
        /// 依會員編號取得人員
        /// </summary>
        /// <param name="CostomerId">會員編號</param>
        public (Result rtn, Customers customer) GetCustomerById(int CustomerID, int Status)
        {
            string sqlCmd = "SELECT * FROM Customers Where CustomerID = @Id And Status = @Status ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", CustomerID);
            parameters.Add("@Status", Status);

            var result = this.GetSingleDefault<Customers>(sqlCmd, parameters);

            return (result.rtn, result.result);
        }
    }
}
