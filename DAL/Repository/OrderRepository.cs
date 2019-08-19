using Base;
using DAL.DBModel;
using DAL.DTOModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DAL.Repository
{
    public class OrderRepository : ConnectionBase
    {
        public OrderRepository(string con, int timeout) : base(con, timeout)
        {
        }

        /// <summary>
        /// 依訂單編號取得訂單資訊
        /// </summary>
        /// <param name="Account">帳號</param>
        public (OrderDTO orderDto, Orders order) GetOrderById(int OrderId)
        {
            string sqlCmd = @"
select o.*,od.* from orders as o 
inner join[Order Details] as od on o.OrderID = od.OrderID
where o.OrderID = @Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", OrderId);
            OrderDTO myOrderDTO = null;

            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var orderDictionary = new Dictionary<int, OrderDTO>();
                try
                {
                    myOrderDTO = connection.Query<OrderDTO, Order_Details, OrderDTO>(
                        sqlCmd,
                        (order, orderDetail) =>
                        {
                            OrderDTO orderEntry;

                            if (!orderDictionary.TryGetValue(order.OrderID, out orderEntry))
                            {
                                orderEntry = order;
                                orderEntry.Details = new List<Order_Details>();
                                orderDictionary.Add(orderEntry.OrderID, orderEntry);
                            }

                            orderEntry.Details.Add(orderDetail);
                            return orderEntry;
                        },
                        parameters,
                        splitOn: "OrderID").Distinct().FirstOrDefault();
                }
                catch (Exception ex)
                {
                }
            }

            return (myOrderDTO, null);
        }
    }
}
