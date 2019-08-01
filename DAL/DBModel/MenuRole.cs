using System;

namespace DAL.DBModel
{
    public partial class MenuRole
    {
        public int MenuRoleId { get; set; }

        public int MenuId { get; set; }

        public int RoleId { get; set; }

        public DateTime UpdateDate { get; set; }

        public int UpdateOperator { get; set; }

        public int Status { get; set; }
    }
}
