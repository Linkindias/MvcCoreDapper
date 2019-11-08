using System.Collections.Generic;
using Base;
using BLL.InterFace;
using BLL.PageModel;
using DAL.DBModel;
using DAL.DTOModel;
using DAL.Repository;
using Omu.ValueInjecter;
using static Base.Enums;

namespace BLL.Model
{
    public class MemberService : IMemberOfAuth, IMemberOfMenu, IMemberOfProduct
    {
        EmployeeRepository EmployeeRep;
        CustomerRepository CustomerRep;
        RoleRepository RoleRep;
        EmployeeModel Employee;
        CustomerModel Customer;

        public MemberService(EmployeeRepository employeeRepository, CustomerRepository customerRepository, RoleRepository roleRepository,
            EmployeeModel employee, CustomerModel customer)
        {
            this.EmployeeRep = employeeRepository;
            this.CustomerRep = customerRepository;
            this.RoleRep = roleRepository;
            this.Employee = employee;
            this.Customer = customer;
        }

        /// <summary>
        /// 依會員編號取得會員資訊
        /// </summary>
        /// <param name="Id">編號</param>
        public dynamic GetMember(string Id)
        {
            int EmployeeId = 0;
            int.TryParse(Id, out EmployeeId);

            var empResult = EmployeeRep.GetEmployeeById(EmployeeId, (int)DataStatus.Enable);
            var cusResult = CustomerRep.GetCustomerById(Id, (int)DataStatus.Enable);

            //當有取得員工有錯誤，則回傳取得員工錯誤訊息
            if (!empResult.rtn.IsSuccess)
            {
                this.Employee.IsSuccess = empResult.rtn.IsSuccess;
                this.Employee.ErrorMsg = empResult.rtn.ErrorMsg;
                return this.Employee;
            }

            //當有取得會員有錯誤，則回傳取得會員錯誤訊息
            if (!cusResult.rtn.IsSuccess)
            {
                this.Customer.IsSuccess = cusResult.rtn.IsSuccess;
                this.Customer.ErrorMsg = cusResult.rtn.ErrorMsg;
                return this.Customer;
            }

            //當有會員且無員工，則回傳會員資訊
            if (cusResult.customer != null && empResult.employee == null)
            {
                this.Customer.InjectFrom(cusResult.customer);
                this.Customer.IsSuccess = true;
                return this.Customer;
            }

            //當有員工且無會員，則回傳員工資訊
            if (empResult.employee != null && cusResult.customer == null)
            {
                this.Employee.InjectFrom(empResult.employee);
                this.Employee.IsSuccess = true;
                return this.Employee;
            }

            return null;
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="member">會員</param>
        public Result UpdateMember(CustomerModel customer, EmployeeModel employee)
        {
            (Result rtn, int exeRows) updateMember = (new Result(), 0);

            if (customer != null)
            {
                Customers dbCustomer = (Customers)new Customers().InjectFrom(customer);
                updateMember = CustomerRep.UpdateCustomer(dbCustomer);
            }
            else
            {
                Employees dbEmployee = (Employees)new Employees().InjectFrom(employee);
                updateMember = EmployeeRep.UpdateEmployee(dbEmployee);
            }

            if (updateMember.rtn.IsSuccess) updateMember.rtn.SuccessMsg = "會員更新成功";

            return updateMember.rtn;
        }

        /// <summary>
        /// 取得是否有會員
        /// </summary>
        /// <param name="account">帳號</param>
        public (Result rtn, int EmpId, string CusId, string Id, string Name) IsExistMemberByAccount(string account)
        {
            Result rtn = new Result();
            var myEmployee = EmployeeRep.GetEmployeeByAccount(account, (int)DataStatus.Enable); //員工
            var myCustomer = CustomerRep.GetCustomerByAccount(account, (int)DataStatus.Enable); //客戶

            //當人員不正確，則顯示訊息
            if (myEmployee.employee == null && myCustomer.custom == null)
            {
                rtn.IsSuccess = false;
                rtn.ErrorMsg = $"查無此帳號 {account}，請確認";
            }

            else if (!myEmployee.rtn.IsSuccess) //當員工錯誤，則回傳錯誤訊息
                rtn = myEmployee.rtn;

            else if (!myCustomer.rtn.IsSuccess) //當會員錯誤，則回傳錯誤訊息
                rtn = myCustomer.rtn;
            else
            {
                rtn.IsSuccess = true;
                int EmployeeID = -1;
                string CustomerID = string.Empty, Name = string.Empty, Id = string.Empty;

                if (myEmployee.employee != null)
                {
                    EmployeeID = myEmployee.employee.EmployeeID;
                    Name = myEmployee.employee.FirstName + myEmployee.employee.LastName;
                    Id = EmployeeID.ToString();
                }
                else
                {
                    CustomerID = Id = myCustomer.custom.CustomerID;
                    Name = myCustomer.custom.CompanyName;
                }
                return (rtn, EmployeeID, CustomerID, Id, Name);
            }
            return (rtn, 0, string.Empty, string.Empty, string.Empty);
        }

        /// <summary>
        /// 取得是否有會員
        /// </summary>
        /// <param name="Id">會員編號 </param>
        public (Result rtn, int EmpId, string CusId) IsExistMemberById(string Id)
        {
            Result rtn = new Result();
            int EmployeeId = 0;
            int.TryParse(Id, out EmployeeId);
            var myEmployee = EmployeeRep.GetEmployeeById(EmployeeId, (int)DataStatus.Enable); //員工
            var myCustomer = CustomerRep.GetCustomerById(Id, (int)DataStatus.Enable); //客戶

            //當人員不正確，則顯示訊息，否則檢查權限
            if (myEmployee.employee == null && myCustomer.customer == null)
            {
                rtn.IsSuccess = false;
                rtn.ErrorMsg = $"查無此帳號 {Id}，請確認";
            }
            else if (!myEmployee.rtn.IsSuccess) //當員工錯誤，則回傳錯誤訊息
                rtn = myEmployee.rtn;

            else if (!myCustomer.rtn.IsSuccess) //當會員錯誤，則回傳錯誤訊息
                rtn = myCustomer.rtn;
            else
                rtn.IsSuccess = true;

            int EmployeeID = myEmployee.employee != null ? myEmployee.employee.EmployeeID : -1; //員工
            string CustomerID = myCustomer.customer != null ? myCustomer.customer.CustomerID : string.Empty; //客戶

            return (rtn, EmployeeID, CustomerID);
        }

        /// <summary>
        /// 依帳號取得角色資訊
        /// </summary>
        /// <param name="EmployeeId">帳號</param>
        public (Result rtn, List<RoleOfMenuDTO> roles) GetRolesByAccount(string Id)
        {
            return RoleRep.GetRolesByAccount(Id);
        }

        /// <summary>
        /// 依帳號計算金額及折扣
        /// </summary>
        /// <param name="Id">帳號</param>
        /// <param name="TotalAmount">總金額</param>
        public (int TotalAmount, double Discount) GetCalculateAmounts(string Id, int TotalAmount)
        {
            int EmployeeId = 0;
            int.TryParse(Id, out EmployeeId);
            if (EmployeeId == 0)
            {
                var resultCus = Customer.CalculateAmounts(TotalAmount);
                return (resultCus.totalAmount, resultCus.discount);
            }
            else
            {
                var resultEmp = Employee.CalculateAmounts(TotalAmount);
                return (resultEmp.totalAmount,resultEmp.discount);
            }
        }
    }
}
