using DAL.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.DTOModel
{
    public class OrderDTO : Orders
    {
        public List<Order_Details> Details { get; set; }
    }
}
