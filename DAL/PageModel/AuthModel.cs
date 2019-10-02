using Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.PageModel
{
    public class AuthModel : Result
    {
        public int AuthenticId { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
    }
}
