using Base;
using DAL.DBModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Repository
{
    public class CategorieRepository : ConnectionBase
    {
        public CategorieRepository(string con, int timeout) : base(con, timeout)
        {
        }

        /// <summary>
        /// 取得商品種類
        /// </summary>
        public virtual (Result rtn, IEnumerable<Categories> Categorys) GetCategorys()
        {
            string sqlCmd = "SELECT * FROM Categories";
            DynamicParameters parameters = new DynamicParameters();
            
            var result = this.GetList<Categories>(sqlCmd, parameters);

            return (result.rtn, result.result);
        }

    }
}
