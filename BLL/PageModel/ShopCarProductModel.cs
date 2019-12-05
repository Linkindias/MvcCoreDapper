using DAL.DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.PageModel
{
    public class ShopCarProductModel: Products
    {
        public int Id { get; set; }

        public int Count { get; set; }

        public int Amount { get; set; }

        public string CategoryName { get; set; }
        public string CategoryDescription { get; set; }
        public string SupplierCompanyName { get; set; }
        public string SupplierContactName { get; set; }
        public string SupplierContactTitle { get; set; }
        public string SupplierPhone { get; set; }
        public string SupplierAddress { get; set; }
    }
}
