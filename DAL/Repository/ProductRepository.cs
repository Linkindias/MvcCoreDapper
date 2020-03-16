using Base;
using DAL.DBModel;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repository
{
    public class ProductRepository : ConnectionBase
    {
        public ProductRepository(IConfiguration config, ILogger<ConnectionBase> log) : base(config, log)
        {
        }

        //public ProductRepository(string con, int timeout) : base(con, timeout)
        //{
        //}

        /// <summary>
        /// 取得商品
        /// </summary>
        public virtual (Result rtn, IEnumerable<Products> products) GetProductsByParam(
            int CategoryId, string ProductName, int[] ProductId, bool IsDiscontinue)
        {
            string sqlCmd = "SELECT * FROM Products Where Discontinued = @Discontinued";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Discontinued", IsDiscontinue);

            if (CategoryId > 0) {
                sqlCmd += " and CategoryID = @CategoryID";
                parameters.Add("@CategoryID", CategoryId);
            }

            if (!string.IsNullOrEmpty(ProductName))
            {
                sqlCmd += " and ProductName like @ProductName";
                parameters.Add("@ProductName", $"%{ProductName}%");
            }

            if (ProductId.Length > 0)
            {
                sqlCmd += " and ProductId in @Ids";
                parameters.Add("@Ids", ProductId);
            }

            var result = this.GetList<Products>(sqlCmd, parameters);

            return (result.rtn, result.result);
        }
    }
}
