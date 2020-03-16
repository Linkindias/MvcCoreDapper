using Base;
using DAL.DBModel;
using DAL.DTOModel;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DAL.Repository
{
    public class OrderRepository : ConnectionBase
    {
        public OrderRepository(IConfiguration config, ILogger<ConnectionBase> log) : base(config, log)
        {
        }

        //public OrderRepository(string con, int timeout) : base(con, timeout)
        //{
        //}

        /// <summary>
        /// 依客戶編號取得訂單資訊
        /// </summary>
        /// <param name="Account">帳號</param>
        public (Result rtn, List<OrderDTO> orders) GetOrderById(string CustomerId, int EmployeeId, DateTime Start, DateTime End)
        {
            Result result = new Result();

            DynamicParameters parameters = new DynamicParameters();
            string sqlCmd = @"
select o.*,cus.ContactName as 'CustomerName' ,emp.FirstName  + emp.LastName as 'EmployeeName', od.*, p.*, c.*, s.* from orders as o 
left join Customers as cus on o.CustomerID = cus.CustomerID
left join Employees as emp on o.EmployeeID = emp.EmployeeID
inner join [Order Details] as od on o.OrderID = od.OrderID 
left join Products as p on od.ProductID = p.ProductID
left join Categories as c on p.CategoryID = c.CategoryID
left join Suppliers as s on p.SupplierID = s.SupplierID ";

            if (!string.IsNullOrEmpty(CustomerId))
            {
                sqlCmd += "where o.CustomerID = @Id";
                parameters.Add("@Id", CustomerId);
            }
            else
            {
                sqlCmd += "where o.EmployeeID = @Id or o.EmployeeID is null";
                parameters.Add("@Id", EmployeeId);
            }
            sqlCmd += " and @Start <= OrderDate and OrderDate <= @End order by o.OrderID Desc";
            parameters.Add("@Start", Start);
            parameters.Add("@End", End);

            List<OrderDTO> orders = null;

            using (var connection = new SqlConnection(this.ConnectionString))
            {
                var orderDictionary = new Dictionary<int, OrderDTO>();
                try
                {
                    orders = connection.Query<OrderDTO, OrderDetailDTO, OrderDTO>(
                        sqlCmd,
                        (order, orderDetail) =>
                        {
                            OrderDTO orderEntry;

                            if (!orderDictionary.TryGetValue(order.OrderID, out orderEntry))
                            {
                                orderEntry = order;
                                orderEntry.Details = new List<OrderDetailDTO>();
                                orderDictionary.Add(orderEntry.OrderID, orderEntry);
                            }

                            orderEntry.Details.Add(orderDetail);
                            return orderEntry;
                        },
                        parameters,
                        splitOn: "OrderID").Distinct().ToList();

                    result.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.ErrorMsg = ex.Message;
                }
            }

            return (result, orders);
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
