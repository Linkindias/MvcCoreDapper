using Base;
using BLL.InterFace;
using DAL.DBModel;
using BLL.PageModel;
using DAL.Repository;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using static Base.Enums;

namespace BLL.Model
{
    public class AuthService
    {
        AuthenticationRepository AuthRep;
        EmployeeRepository EmployeeRep;
        CustomerRepository CustomerRep;
        IMemoryCache cache;

        static string keyCode = "abcd1234EFGH0987";
        static string ivCode = "ABCD7890efgh4321";

        public AuthService(AuthenticationRepository authenticationRepository,
                EmployeeRepository employeeRepository, CustomerRepository customerRepository,
                IMemoryCache memoryCache)
        {
            this.AuthRep = authenticationRepository;
            this.EmployeeRep = employeeRepository;
            this.CustomerRep = customerRepository;
            this.cache = memoryCache;
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="Password">密碼</param>
        public (Result rtn, Employees employee, Customers customer, Guid guid) LogIn(string Account, string Password)
        {
            Result rtn = new Result();
            string account = this.DecryptAES(Account);
            var myEmployee = EmployeeRep.GetEmployeeByAccount(account, (int)DataStatus.Enable); //員工
            var myCustomer = CustomerRep.GetCustomerByAccount(account, (int)DataStatus.Enable); //客戶
            Guid guid = Guid.Empty;

            //當人員不正確，則顯示訊息，否則檢查權限
            if (myEmployee.employee == null && myCustomer.custom == null)
            {
                rtn.IsSuccess = false;
                rtn.ErrorMsg = $"查無此帳號 {account}，請確認";
            }
            //當員工錯誤，則回傳錯誤訊息
            else if (!myEmployee.rtn.IsSuccess && myEmployee.employee != null)
                rtn = myEmployee.rtn;
            //當會員錯誤，則回傳錯誤訊息
            else if (!myCustomer.rtn.IsSuccess && myCustomer.custom != null)
                rtn = myCustomer.rtn;
            else
            {
                int EmployeeID = myEmployee.employee != null ? myEmployee.employee.EmployeeID : -1; //員工
                string CustomerID = myCustomer.custom != null ? myCustomer.custom.CustomerID : string.Empty; //客戶
                var myAuth = AuthRep.GetAuthenticationByParams(EmployeeID, CustomerID, Password, (int)DataStatus.Enable);

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
                    else if (myAuth.auth.VerifyCode != null)
                        rtn.ErrorMsg = $"帳號 {account}已登入中，無法登入";

                    else
                    {
                        rtn.IsSuccess = true;
                        guid = Guid.NewGuid();
                        AuthRep.UpdateAuthCode(EmployeeID, CustomerID, guid, (int)DataStatus.Enable);
                    }
                }
            }
            return (rtn, myEmployee.employee, myCustomer.custom, guid);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="Id">帳號</param>
        public Result LogOut(string Id)
        {
            (Result rtn, int exeRows) updateAuthCode = (new Result(), 0);
            int EmployeeId = 0;
            int.TryParse(Id, out EmployeeId);
            var myEmployee = EmployeeRep.GetEmployeeById(EmployeeId, (int)DataStatus.Enable); //員工
            var myCustomer = CustomerRep.GetCustomerById(Id, (int)DataStatus.Enable); //客戶

            //當人員不正確，則顯示訊息，否則檢查權限
            if (myEmployee.employee == null && myCustomer.customer == null)
            {
                updateAuthCode.rtn.IsSuccess = false;
                updateAuthCode.rtn.ErrorMsg = $"查無此帳號 {Id}，請確認";
            }
            else
                updateAuthCode = AuthRep.UpdateAuthCode(EmployeeId, Id, null, (int)DataStatus.Enable);

            if (updateAuthCode.rtn.IsSuccess)
            {
                updateAuthCode.rtn.SuccessMsg = $"{Id} 登出成功";
                cache.Remove($"GetMenus{Id}"); //選單
                cache.Remove($"GetRoles{Id}"); //角色
            }

            return updateAuthCode.rtn;
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

        public byte[] HexStringToByteArray(string strHex)
        {
            byte[] r = new byte[strHex.Length / 2];
            for (int i = 0; i <= strHex.Length - 1; i += 2)
            {
                r[i / 2] = Convert.ToByte(Convert.ToInt32(strHex.Substring(i, 2), 16));
            }
            return r;
        }

        protected virtual byte[] GenerateKey(string strPassword, byte[] salt, int iterations)
        {
            Rfc2898DeriveBytes rfc2898 = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(strPassword), salt, iterations);
            return rfc2898.GetBytes(128 / 8);
        }
    }
}

