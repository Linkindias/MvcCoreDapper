using Base;
using DAL.DBModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repository
{
    public class SupplierRepository : ConnectionBase
    {
        public SupplierRepository(string con, int timeout) : base(con, timeout)
        {
        }

        /// <summary>
        /// 取得供應商
        /// </summary>
        public virtual (Result rtn, IEnumerable<Suppliers> Suppliers) GetSuppliers()
        {
            string sqlCmd = "SELECT * FROM Suppliers";
            DynamicParameters parameters = new DynamicParameters();

            var result = this.GetList<Suppliers>(sqlCmd, parameters);

            return (result.rtn, result.result);
        }
    }
}
