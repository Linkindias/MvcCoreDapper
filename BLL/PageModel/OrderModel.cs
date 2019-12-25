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
        public int TotalCount { get; set; } //總筆數
        public int CurrentPage { get; set; } //目前頁數
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
