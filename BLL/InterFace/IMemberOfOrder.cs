using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.InterFace
{
    public interface IMemberOfOrder
    {
        /// <summary>
        /// 依會員編號取得會員資訊
        /// </summary>
        /// <param name="Id">編號</param>
        dynamic GetMember(string Id);
    }
}
