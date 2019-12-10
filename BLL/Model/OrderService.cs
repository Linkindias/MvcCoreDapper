using Base;
using BLL.InterFace;
using BLL.PageModel;
using DAL.DBModel;
using DAL.DTOModel;
using DAL.Repository;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace BLL.Model
{
    public class OrderService
    {
        IMemberOfOrder memberService;
        OrderRepository OrderRep;
        OrderDetailRepository OrderDetailRep;
        OrderModel Order;

        public OrderService(IMemberOfOrder memberOfOrder,
            OrderRepository orderRepository, OrderDetailRepository orderDetailRepository,
            OrderModel orderModel)
        {
            this.OrderRep = orderRepository;
            this.OrderDetailRep = orderDetailRepository;
            this.memberService = memberOfOrder;
            this.Order = orderModel;
        }

        /// <summary>
        /// 依帳號取得訂單資訊
        /// </summary>
        /// <param name="Id">會員編號</param>
        /// <param name="員工、會員">會員類別</param>
        /// <param name="Start">起始日期</param>
        /// <param name="End">終止日期</param>
        /// <returns></returns>
        public OrderModel GetOrderById(string Id, dynamic Member, DateTime Start, DateTime End)
        {
            Result rtn = new Result();
            (Result rtn, List<OrderDTO> orderDto) result = (new Result(), new List<OrderDTO>());

            if (Member is CustomerModel)
                result = OrderRep.GetOrderById(Member.CustomerID , 0, Start, End);
            else
                result = OrderRep.GetOrderById(string.Empty , Member.EmployeeID, Start, End);

            Order.orders = result.orderDto;
            Order.IsSuccess = result.rtn.IsSuccess;
            Order.SuccessMsg = result.rtn.SuccessMsg;
            Order.ErrorMsg = result.rtn.ErrorMsg;
            Order.ErrorCode = result.rtn.ErrorCode;
            return Order;
        }

        /// <summary>
        /// 建立訂單
        /// </summary>
        /// <param name="Id">會員編號</param>
        /// <param name="shopCar">購物車資訊</param>
        /// <returns></returns>
        public Result CreateOrder(string Id, ShopCarModel shopCar)
        {
            var Member = memberService.GetMember(Id);

            using (var scope = new TransactionScope())
            {
                DateTime OrderDate = DateTime.Now;

                var orderResult = OrderRep.CreateOrder(new DAL.DBModel.Orders()
                {
                    CustomerID = Member.CustomerID,
                    OrderDate = OrderDate,
                    ShipName = Member.CompanyName,
                    ShipAddress = Member.Address,
                    ShipCity = Member.City,
                    ShipRegion = Member.Region,
                    ShipPostalCode = Member.PostalCode,
                    ShipCountry = Member.Country
                });

                if (!orderResult.rtn.IsSuccess) return orderResult.rtn;

                var IdResult = OrderRep.GetOrderByParams(Member.CustomerID, OrderDate);

                if (!IdResult.Item1.IsSuccess) return IdResult.Item1;

                List<Order_Details> details = new List<Order_Details>();
                foreach (ShopCarProductModel product in shopCar.products)
                {
                    details.Add(new Order_Details()
                    {
                        OrderID = IdResult.Item2,
                        Discount = Convert.ToSingle(shopCar.discount),
                        ProductID = product.ProductID,
                        UnitPrice = product.UnitPrice.Value,
                        Quantity = Convert.ToInt16(product.Count)
                    });
                }

                var detialResult = OrderDetailRep.CreateOrderDetail(details.ToArray());

                if (detialResult.rtn.IsSuccess) scope.Complete();

                return detialResult.Item1;
            }
        }
    }
}
