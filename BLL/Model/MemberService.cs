using Base;
using BLL.PageModel;
using DAL.DBModel;
using DAL.Repository;
using Omu.ValueInjecter;
using static Base.Enums;

namespace BLL.Model
{
    public class MemberService
    {
        EmployeeRepository EmployeeRep;
        CustomerRepository CustomerRep;
        EmployeeModel Employee;
        CustomerModel Customer;

        public MemberService(EmployeeRepository employeeRepository, CustomerRepository customerRepository, EmployeeModel employee, CustomerModel customer)
        {
            this.EmployeeRep = employeeRepository;
            this.CustomerRep = customerRepository;
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
    }
}
