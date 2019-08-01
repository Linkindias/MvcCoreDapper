using System;

namespace DAL.DBModel
{
    public partial class Shippers
    {
        public Shippers()
        {
        }

        public int ShipperID { get; set; }

        public string CompanyName { get; set; }

        public string Phone { get; set; }
    }
}
