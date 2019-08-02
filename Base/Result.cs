using System;
using System.Collections.Generic;
using System.Text;

namespace Base
{
    public class Result
    {
        public bool IsSuccess { get; set; } //是否成功
        public string SuccessMsg { get; set; } //成功訊息
        public string ErrorMsg { get; set; } //錯誤訊息
        public string ErrorCode { get; set; } //錯誤代碼(給於展示層顯示不同訊息判斷)
    }
}
