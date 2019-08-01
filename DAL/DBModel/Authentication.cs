using System;

namespace DAL.DBModel
{
    public partial class Authentication
    {
        public int AuthenticId { get; set; }

        public string Password { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int State { get; set; }

        public DateTime UpdateDate { get; set; }

        public int UpdateOperator { get; set; }

        public int EmployeesId { get; set; }

        public virtual Employees Employees { get; set; }

        public Guid? VerifyCode { get; set; }

        public int VerifyCount { get; set; }

        public string CustomersId { get; set; }

    }
}
