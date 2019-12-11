using DAL.DBModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.DTOModel
{
    public class OrderDTO : Orders
    {
        public string CustomerName { get; set; }

        public string EmployeeName { get; set; }

        public List<OrderDetailDTO> Details { get; set; }
    }
}
