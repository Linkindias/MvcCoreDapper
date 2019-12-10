using Base;
using DAL.DBModel;
using DAL.DTOModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.PageModel
{
    public class OrderModel : Result
    {
        public List<OrderDTO> orders { get; set; }
    }
}
