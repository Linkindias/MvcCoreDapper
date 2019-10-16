using Base;
using DAL.DBModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repository
{
    public class ProductRepository : ConnectionBase
    {
        public ProductRepository(string con, int timeout) : base(con, timeout)
        {
        }

        /// <summary>
        /// 取得商品
        /// </summary>
        public virtual (Result rtn, IEnumerable<Products> products) GetProductsByParam(int CategoryId, string ProductName, bool IsDiscontinue)
        {
            string sqlCmd = "SELECT * FROM Products Where Discontinued = @Discontinued ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Discontinued", IsDiscontinue);

            if (CategoryId > 0) {
                sqlCmd += "and CategoryID = @CategoryID";
                parameters.Add("@CategoryID", CategoryId);
            }

            if (!string.IsNullOrEmpty(ProductName))
            {
                sqlCmd += "and ProductName like @ProductName";
                parameters.Add("@ProductName", $"%{ProductName}%");
            }

            var result = this.GetList<Products>(sqlCmd, parameters);

            return (result.rtn, result.result);
        }
    }
}
