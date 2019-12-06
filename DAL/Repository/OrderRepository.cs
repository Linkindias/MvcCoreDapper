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
        public (Result rtn, OrderDTO orderDto) GetOrderById(int OrderId)
        {
            Result result = new Result();
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

                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.ErrorMsg = ex.Message;
                }
            }

            return (result, myOrderDTO);
        }

        /// <summary>
        /// 依客戶編號、訂單日期，取得訂單
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public (Result rtn, int orderId) GetOrderByParams(string CustomerId, DateTime OrdeDate)
        {
            string sqlCmd = @"
select * from orders
where CustomerID = @Id and OrderDate = @Date";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", CustomerId);
            parameters.Add("@Date", OrdeDate);

            var result = this.GetSingleDefault<Orders>(sqlCmd, parameters);

            return (result.rtn, result.result.OrderID);
        }

        /// <summary>
        /// 建立訂單
        /// </summary>
        public virtual (Result rtn, int exeRows) CreateOrder(Orders order)
        {
            string sqlCmd = @"Insert Orders Values(@CustomerId, @EmployeeId, @OrderDate, @RequiredDate, @ShippedDate, @ShipVia, 
                    @Freight, @ShipName, @ShipAddress, @ShipCity, @ShipRegion, @ShipPostalCode, @ShipCountry)";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@CustomerId", order.CustomerID);
            parameters.Add("@EmployeeId", order.EmployeeID);
            parameters.Add("@OrderDate", order.OrderDate);
            parameters.Add("@RequiredDate", order.RequiredDate);
            parameters.Add("@ShippedDate", order.ShippedDate);
            parameters.Add("@ShipVia", order.ShipVia);
            parameters.Add("@Freight", order.Freight);
            parameters.Add("@ShipName", order.ShipName);
            parameters.Add("@ShipAddress", order.ShipAddress);
            parameters.Add("@ShipCity", order.ShipCity);
            parameters.Add("@ShipRegion", order.ShipRegion);
            parameters.Add("@ShipPostalCode", order.ShipPostalCode);
            parameters.Add("@ShipCountry", order.ShipCountry);

            var result = this.GetCUDOfRow(sqlCmd, parameters);

            return (result.rtn, result.Rows);
        }
    }
}
