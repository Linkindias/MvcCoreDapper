using Base;
using DAL.DBModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.PageModel
{
    public abstract class MemberModel : Result
    {
        [StringLength(60)]
        public string Address { get; set; }

        [StringLength(15)]
        public string City { get; set; }

        [StringLength(15)]
        public string Region { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        [StringLength(15)]
        public string Country { get; set; }

        [StringLength(20)]
        public string Account { get; set; }

        public int Status { get; set; }

        public abstract (int totalAmount, double discount) CalculateAmounts(int TotalAmount);
    }
}
