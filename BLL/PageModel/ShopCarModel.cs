using DAL.DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.PageModel
{
    public class ShopCarModel
    {
        public int totalAmount { get; set; }

        public double discount { get; set; }

        public List<ShopCarProductModel> products { get; set; }
    }
}
