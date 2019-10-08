using Base;
using DAL.DBModel;
using static Base.Enums;

namespace BLL.PageModel
{
    public class MemberModel: Result
    {
        public MemberStatus membertype { get; set; }
        public Employees employee { get; set; }
        public Customers customer { get; set; }
    }
}
