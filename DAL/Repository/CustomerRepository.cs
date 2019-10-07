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
        public virtual (Result rtn, Customers custom) GetCustomerByAccount(string Account, int Status)
        {
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
        public virtual (Result rtn, Customers customer) GetCustomerById(string CustomerID, int Status)
        {
            string sqlCmd = "SELECT * FROM Customers Where CustomerID = @Id And Status = @Status ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", CustomerID);
            parameters.Add("@Status", Status);

            var result = this.GetSingleDefault<Customers>(sqlCmd, parameters);

            return (result.rtn, result.result);
        }

        /// <summary>
        /// 更新客戶
        /// </summary>
        /// <param name="customer">客戶</param>
        public (Result rtn, int exeRows) UpdateCustomer(Customers customer)
        {
            string sqlCmd = @"Update Customers Set CompanyName = @CompanyName, ContactName = @ContactName, ContactTitle = @ContactTitle, Phone = @Phone, Address = @Address
                                Where CustomerId = @CusId";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@CompanyName", customer.CompanyName);
            parameters.Add("@ContactName", customer.ContactName);
            parameters.Add("@ContactTitle", customer.ContactTitle);
            parameters.Add("@Phone", customer.Phone);
            parameters.Add("@Fax", customer.Fax);
            parameters.Add("@Address", customer.Address);
            parameters.Add("@CusId", customer.CustomerID);

            var result = this.GetCUDOfRow(sqlCmd, parameters);

            return (result.rtn, result.Rows);
        }
    }
}
