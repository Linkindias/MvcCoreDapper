﻿using Base;
using DAL.DBModel;
using DAL.DTOModel;
using DAL.PageModel;
using System;
using System.Collections.Generic;

namespace BLL.InterFace
{
    public interface IMemberService
    {
        /// <summary>
        /// 依會員編號取得會員資訊
        /// </summary>
        /// <param name="Id">編號</param>
        MemberModel GetMember(string Id);
    }
}
