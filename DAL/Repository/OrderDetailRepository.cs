using Base;
using DAL.DTOModel;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repository
{
    public class OrderDetailRepository : ConnectionBase
    {
        public OrderDetailRepository(string con, int timeout) : base(con, timeout)
        {
        }

        /// <summary>
        /// 取得訂單產品數量
        /// </summary>
        public virtual (Result rtn, List<ProductCountDTO> productCounts) GetOrderProductQuantitys(int ProductId)
        {
            string sqlCmd = "SELECT ProductId, sum(Quantity) as Sales FROM Order_Details ";
            DynamicParameters parameters = new DynamicParameters();

            if (ProductId > 0) {
                sqlCmd += " Where ProductID == @ProductId";
                parameters.Add("@ProductId", ProductId);
            }
            sqlCmd += " GROUP BY ProductId";

            var result = this.GetList<ProductCountDTO>(sqlCmd, parameters);

            return (result.rtn, result.result.ToList());
        }
    }
}
