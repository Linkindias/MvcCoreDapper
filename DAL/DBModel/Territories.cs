using System.Collections.Generic;

namespace DAL.DBModel
{
    public partial class Territories
    {
        public Territories()
        {
        }

        public string TerritoryID { get; set; }

        public string TerritoryDescription { get; set; }

        public int RegionID { get; set; }
    }
}
