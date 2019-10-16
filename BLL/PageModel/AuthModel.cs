using Base;
using System.ComponentModel.DataAnnotations;

namespace BLL.PageModel
{
    public class AuthModel : Result
    {
        public int AuthenticId { get; set; }

        [Required(ErrorMessage = "帳號是必填欄位")]
        public string Account { get; set; }

        [Required(ErrorMessage = "密碼是必填欄位")]
        public string Password { get; set; }
    }
}
