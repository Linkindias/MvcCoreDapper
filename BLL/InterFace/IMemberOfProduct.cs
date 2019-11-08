using Base;
using DAL.DTOModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.InterFace
{
    public interface IMemberOfProduct
    {
        /// <summary>
        /// 依帳號計算金額及折扣
        /// </summary>
        /// <param name="Id">帳號</param>
        /// <param name="TotalAmount">總金額</param>
        (int TotalAmount, double Discount) GetCalculateAmounts(string Id, int TotalAmount);
    }
}
