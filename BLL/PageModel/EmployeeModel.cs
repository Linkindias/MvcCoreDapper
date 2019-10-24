using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using DAL.DBModel;

namespace BLL.PageModel
{
    public class EmployeeModel : MemberModel
    {
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "名是必填欄位")]
        [StringLength(20)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "姓是必填欄位")]
        [StringLength(10)]
        public string FirstName { get; set; }

        [StringLength(30)]
        public string Title { get; set; }

        [StringLength(25)]
        public string TitleOfCourtesy { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime? HireDate { get; set; }

        [StringLength(24)]
        public string HomePhone { get; set; }

        [StringLength(4)]
        public string Extension { get; set; }

        public byte[] Photo { get; set; }

        public string Notes { get; set; }

        public int? ReportsTo { get; set; }

        [StringLength(255)]
        public string PhotoPath { get; set; }

        public override (int totalAmount, double discount) CalculateAmounts(int TotalAmount)
        {
            double discount = 0.95;
            return ((int)Math.Round(Convert.ToDouble(TotalAmount) * discount), discount);
        }
    }
}
