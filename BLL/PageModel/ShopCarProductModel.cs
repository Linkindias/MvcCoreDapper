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
    }
}
