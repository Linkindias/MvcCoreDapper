using Base;
using DAL.DBModel;

namespace BLL.PageModel
{
    public class MemberModel: Result
    {
        public Employees employee { get; set; }
        public Customers customer { get; set; }
    }
}
