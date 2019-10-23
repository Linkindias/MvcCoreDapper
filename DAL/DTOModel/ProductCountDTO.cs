using DAL.DBModel;
using DAL.DTOModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOModel
{
    public class ProductCountDTO : Products
    {
        public int Sales { get; set; } //銷售

        public int Quantity { get; set; } //數量

        public IEnumerable<SelectListItem> QuantityOptions { get; set; } //數量選項
    }
}
