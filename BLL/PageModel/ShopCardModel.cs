using Base;
using DAL.DBModel;
using DAL.DTOModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.PageModel
{
    public class ShopCarModel : Result
    {
        public int ProductId { get; set; }

        public int Count { get; set; }
    }
}
