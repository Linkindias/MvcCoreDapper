using Base;
using DAL.DBModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.PageModel
{
    public class MemberModel: Result
    {
        public Employees employee { get; set; }
        public Customers customer { get; set; }
    }
}
