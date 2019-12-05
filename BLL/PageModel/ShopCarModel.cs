using Base;
using DAL.DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.PageModel
{
    public class ShopCarModel : Result
    {
        public int totalAmount { get; set; }

        public double discount { get; set; }

        public int disAmount { get; set; }

        public List<ShopCarProductModel> products { get; set; }
    }
}
