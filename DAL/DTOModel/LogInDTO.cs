﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.DTOModel
{
    public class LogInDTO
    {
        [Required(ErrorMessage = "帳號是必填欄位")]
        public string account { get; set; }
        [Required(ErrorMessage = "密碼是必填欄位")]
        public string password { get; set; }
    }
}