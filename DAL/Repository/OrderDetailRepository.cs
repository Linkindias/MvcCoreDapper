using Base;
using DAL.DBModel;
using DAL.DTOModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        public virtual (Result rtn, List<ProductCountDTO> productCounts) GetOrderProductQuantitys(int[] ProductId)
        {
            string sqlCmd = "SELECT ProductId, sum(Quantity) as Sales FROM [dbo].[Order Details] ";
            object parameters = null;

            if (ProductId.Count() > 0) {
                sqlCmd += " Where ProductID in @ID";
                parameters = new { ID = ProductId };
            }
            sqlCmd += " GROUP BY ProductId";

            var result = this.GetList<ProductCountDTO>(sqlCmd, parameters);

            return (result.rtn, result.result.ToList());
        }

        /// <summary>
        /// 建立訂單明細
        /// </summary>
        public virtual (Result rtn, int exeRows) CreateOrderDetail(Order_Details[] orderDetails)
        {
            string sqlCmd = @"Insert [Order Details] Values";
            DynamicParameters parameters = new DynamicParameters();

            for (int i=0; i< orderDetails.Count(); i++)
            {
                sqlCmd += "(";
                foreach (PropertyInfo prop in orderDetails[i].GetType().GetProperties())
                {
                    string Column = $"{prop.Name}{i}";
                    sqlCmd += $"@{Column},";
                    parameters.Add($"@{Column}", prop.GetValue(orderDetails[i]));
                }
                sqlCmd = sqlCmd.Substring(0, sqlCmd.Length - 1); //去除逗號
                sqlCmd += "),";
            }
            sqlCmd = sqlCmd.Substring(0, sqlCmd.Length - 1); //去除逗號
            var result = this.GetCUDOfRow(sqlCmd, parameters);

            return (result.rtn, result.Rows);
        }
    }
}
