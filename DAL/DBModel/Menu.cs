using System;

namespace DAL.DBModel
{
    public partial class Menu
    {
        public Menu()
        {
        }

        public int MenuId { get; set; }

        public string MenuName { get; set; }

        public string MenuCode { get; set; }

        public int ParentID { get; set; }

        public DateTime UpdateDate { get; set; }

        public int UpdateOperator { get; set; }

        public bool? Add { get; set; }

        public bool? Update { get; set; }

        public bool? Delete { get; set; }

        public bool? Query { get; set; }

        public bool? View { get; set; }

        public int Status { get; set; }

        public int Sort { get; set; }
    }
}
