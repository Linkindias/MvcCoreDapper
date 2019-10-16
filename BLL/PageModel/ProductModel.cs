using Base;
using DAL.DBModel;
using DAL.DTOModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.PageModel
{
    public class ProductModel : Result
    {
        public IEnumerable<SelectListItem> Categories { get; set; }

        public List<ProductCountDTO> Products { get; set; }

        public string ProductName { get; set; }
    }
}
