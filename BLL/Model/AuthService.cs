using Base;
using BLL.InterFace;
using BLL.PageModel;
using DAL.DBModel;
using DAL.Repository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Base.Enums;

namespace BLL.Model
{
    public class AuthService
    {
        IConfiguration config;
        IMemberOfAuth memberService;
        IMemoryCache cache;
        AuthenticationRepository AuthRep;

        static string keyCode = "abcd1234EFGH0987";
        static string ivCode = "ABCD7890efgh4321";

        public AuthService(IConfiguration configuration, IMemberOfAuth memberOfAuth, IMemoryCache memoryCache ,
            AuthenticationRepository authenticationRepository)
        {
            this.config = configuration;
            this.memberService = memberOfAuth;
            this.cache = memoryCache;
            this.AuthRep = authenticationRepository;
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="Password">密碼</param>
        public (Result rtn, string Id, string Name,  Guid guid) LogIn(string Account, string Password)
        {
            Result rtn = new Result();
            string account = this.DecryptAES(Account);
            Guid guid = Guid.Empty;

            var result = memberService.IsExistMemberByAccount(account);

            //當取得無會員，則回傳訊息
            if (!result.rtn.IsSuccess)
                rtn = result.rtn;
            else
            {
                var myAuth = AuthRep.GetAuthenticationByParams(result.EmpId, result.CusId, Password, (int)DataStatus.Enable);

                //當權限不正確，則顯示訊息，否則檢查有效期
                if (myAuth.auth == null)
                {
                    rtn.IsSuccess = false;
                    rtn.ErrorMsg = $"帳號 {account}或密碼不正確，請確認";
                }
                else
                {
                    rtn.IsSuccess = false;

                    //當權限鎖定，則無法登入
                    if (myAuth.auth.State == (int)DataStatus.Lock)
                        rtn.ErrorMsg = $"帳號 {account}已鎖定，無法登入，請連絡客服!";

                    //當有效期小於今日，則已離職
                    else if (DateTime.Compare(DateTime.Now, myAuth.auth.EffectiveDate) >= 0)
                        rtn.ErrorMsg = $"帳號 {account}已離職，無法登入";

                    //當已有認證，則已登入中
                    else if (myAuth.auth.VerifyCode != null && myAuth.auth.VerifyCode != Guid.Parse("00000000-0000-0000-0000-000000000000"))
                        rtn.ErrorMsg = $"帳號 {account}已登入中，無法登入";

                    else
                    {
                        rtn.IsSuccess = true;
                        guid = Guid.NewGuid();
                        AuthRep.UpdateAuthCode(result.EmpId, result.CusId, guid, (int)DataStatus.Enable);
                    }
                }
            }
            return (rtn, result.Id, result.Name, guid);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="Id">帳號</param>
        public Result LogOut(string Id)
        {
            Result rtn = new Result();

            var rtnMember = memberService.IsExistMemberById(Id);

            //當取得無會員，則回傳訊息
            if (!rtnMember.rtn.IsSuccess)
                rtn = rtnMember.rtn;
            else
            {
                var rtnAuth = AuthRep.UpdateAuthCode(rtnMember.EmpId, rtnMember.CusId, null, (int)DataStatus.Enable);
                rtn = rtnAuth.rtn;

                if (rtn.IsSuccess)
                {
                    rtn.SuccessMsg = $"{Id} 登出成功";
                    cache.Remove($"GetMenus{Id}"); //選單
                    cache.Remove($"GetRoles{Id}"); //角色
                }
            }

            return rtn;
        }

        /// <summary>
        /// 依帳號取得權限
        /// </summary>
        /// <param name="Id">編號</param>
        public AuthModel GetAuth(string Id)
        {
            AuthModel auth = new AuthModel();

            var result = AuthRep.GetAuthById(Id, (int)DataStatus.Enable);
            if (result.rtn.IsSuccess)
            {
                auth = new AuthModel()
                {
                    AuthenticId = result.auth.AuthenticId,
                    Account = this.Encrypt(result.auth.Account),
                    Password = string.Empty,
                };
                auth.IsSuccess = result.rtn.IsSuccess;
            }
            else
            {
                auth.IsSuccess = result.rtn.IsSuccess;
                auth.ErrorMsg = result.rtn.ErrorMsg;
            }
            return auth;
        }

        /// <summary>
        /// 更新權限
        /// </summary>
        /// <param name="auth">權限</param>
        public Result UpdateAuth(AuthModel auth)
        {
            (Result rtn, int exeRows) updateAuth = (new Result(), 0);
            string Account = this.DecryptAES(auth.Account); //帳號解碼

            updateAuth = AuthRep.UpdateAuth(auth.AuthenticId, Account, auth.Password, (int)DataStatus.Enable);

            if (updateAuth.rtn.IsSuccess) updateAuth.rtn.SuccessMsg = $"{Account} 更新成功";

            return updateAuth.rtn;
        }

        protected virtual string DecryptAES(string Value)
        {
            var keybytes = Encoding.UTF8.GetBytes(keyCode);
            var iv = Encoding.UTF8.GetBytes(ivCode);

            var encrypted = Convert.FromBase64String(Value);
            var decriptedFromJavascript = DecryptFromBytes(encrypted, keybytes, iv);

            return decriptedFromJavascript;
        }

        protected virtual string DecryptFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            string value = null;

            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                using (var msDecrypt = new MemoryStream(cipherText))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    value = srDecrypt.ReadToEnd();
                }
            }
            return value;
        }

        protected virtual string Encrypt(string value)
        {
            byte[] strText = new System.Text.UTF8Encoding().GetBytes(value);
            RijndaelManaged myRijndael = new RijndaelManaged();
            myRijndael.BlockSize = 128;
            myRijndael.KeySize = 128;
            myRijndael.IV = Encoding.UTF8.GetBytes(ivCode);

            myRijndael.Padding = PaddingMode.PKCS7;
            myRijndael.Mode = CipherMode.CBC;
            myRijndael.Key = this.GenerateKey("coremvc", Encoding.UTF8.GetBytes(keyCode), 1000);
            ICryptoTransform transform = myRijndael.CreateEncryptor();
            byte[] cipherText = transform.TransformFinalBlock(strText, 0, strText.Length);
            return Convert.ToBase64String(cipherText);
        }

        protected virtual byte[] GenerateKey(string strPassword, byte[] salt, int iterations)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(strPassword), salt, iterations);
            return rfc2898.GetBytes(128 / 8);
        }

        /// <summary>
        /// 建立Token
        /// </summary>
        /// <param name="Name">姓名</param>
        /// <param name="Id">編號</param>
        /// <param name="guid">驗證碼</param>
        public string CreateToken(string Name, string Id, Guid guid)
        {
            DateTime dtExp = DateTime.Now.AddHours(double.Parse(config["ExpHour"]));

            // JWT RFC claims 列舉
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub,CheckValue(Name)),
                new Claim(JwtRegisteredClaimNames.NameId,CheckValue(Id)),
                new Claim(JwtRegisteredClaimNames.Jti, guid.ToString()),
            };

            var userClaimsIdentity = new ClaimsIdentity(claims);

            // 建立一組對稱式加密的金鑰，主要用於 JWT 簽章之用
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenSec"]));
            // 使用對稱密鑰
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = config["Issuer"],
                Subject = userClaimsIdentity,
                Expires = dtExp,
                SigningCredentials = signingCredentials
            };

            // 產出所需要的 JWT securityToken 物件，並取得序列化後的 Token 結果(字串格式)
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializeToken = tokenHandler.WriteToken(securityToken);

            return serializeToken;
        }

        private string CheckValue(string Source)
        {
            if (Source == null) return string.Empty;

            return Source;
        }
    }
}

