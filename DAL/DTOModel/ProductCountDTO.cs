using DAL.DBModel;
using DAL.DTOModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOModel
{
    public class ProductCountDTO : Products
    {
        public int Inventory { get; set; } //庫存

        public int Sales { get; set; } //銷售

        public int Quantity { get; set; } //數量
    }
}
