using System;

namespace DAL.DBModel
{
    public partial class RoleEmployee
    {
        public int RoleEmployeeId { get; set; }

        public int EmployeeId { get; set; }

        public int RoleId { get; set; }

        public DateTime UpdateDate { get; set; }

        public int UpdateOperator { get; set; }

        public int Status { get; set; }

        public string CustomerId { get; set; }

        public virtual Employees Employees { get; set; }

        public virtual Role Role { get; set; }
    }
}
