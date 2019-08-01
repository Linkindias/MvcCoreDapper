using System;

namespace DAL.DBModel
{
    public partial class Region
    {
        public Region()
        {
        }

        public int RegionID { get; set; }

        public string RegionDescription { get; set; }
    }
}
