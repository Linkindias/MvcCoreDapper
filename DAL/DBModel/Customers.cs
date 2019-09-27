using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.DBModel
{
    public partial class Customers
    {
        public Customers()
        {
        }

        public string CustomerID { get; set; }

        [Required(ErrorMessage = "公司名稱是必填欄位")]
        [StringLength(40)]
        public string CompanyName { get; set; }

        [StringLength(30)]
        public string ContactName { get; set; }

        [StringLength(30)]
        public string ContactTitle { get; set; }

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

        [StringLength(24)]
        public string Phone { get; set; }

        [StringLength(24)]
        public string Fax { get; set; }

        [StringLength(20)]
        public string Account { get; set; }

        public int Status { get; set; }
    }
}
