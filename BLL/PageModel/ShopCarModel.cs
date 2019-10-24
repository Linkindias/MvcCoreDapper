﻿using DAL.DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.PageModel
{
    public class ShopCarModel
    {
        public List<ShopCarProductModel> shopcarProducts { get; set; }

        public int TotalAmount { get; set; }

        public double Discount { get; set; }
    }
}