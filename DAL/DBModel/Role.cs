using System;

namespace DAL.DBModel
{
    public partial class Role
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public DateTime UpdateDate { get; set; }

        public int UpdateOperator { get; set; }

        public int Status { get; set; }
    }
}
