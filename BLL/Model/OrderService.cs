using Base;
using BLL.InterFace;
using BLL.PageModel;
using DAL.DBModel;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace BLL.Model
{
    public class OrderService
    {
        OrderRepository OrderRep;
        OrderDetailRepository OrderDetailRep;
        IMemberOfOrder memberService;

        public OrderService(OrderRepository orderRepository, OrderDetailRepository orderDetailRepository,
            IMemberOfOrder memberOfOrder)
        {
            this.OrderRep = orderRepository;
            this.OrderDetailRep = orderDetailRepository;
            this.memberService = memberOfOrder;
        }

        /// <summary>
        /// 建立訂單
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="shopCar"></param>
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
