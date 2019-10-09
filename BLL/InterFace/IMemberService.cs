using Base;
using BLL.PageModel;
using DAL.DBModel;
using System.Collections.Generic;

namespace BLL.InterFace
{
    public interface IMemberService
    {
        /// <summary>
        /// 依商品項目 計算金額
        /// </summary>
        /// <param name="products">編號</param>
        decimal CalculateAmounts(IEnumerable<Products> products);
    }
}
