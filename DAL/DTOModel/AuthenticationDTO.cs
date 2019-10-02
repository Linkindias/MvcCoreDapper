using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOModel
{
    public class AuthenticationDTO
    {
        public int AuthenticId { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
    }
}
