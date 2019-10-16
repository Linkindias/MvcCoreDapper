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
        public virtual (Result rtn, IEnumerable<Products> products) GetProductsByDiscontinued(bool IsDiscontinue)
        {
            string sqlCmd = "SELECT * FROM Products Where Discontinued = @Discontinued ";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Discontinued", IsDiscontinue);

            var result = this.GetList<Products>(sqlCmd, parameters);

            return (result.rtn, result.result);
        }
    }
}
