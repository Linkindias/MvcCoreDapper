using Base;
using BLL.PageModel;
using DAL.Repository;
using static Base.Enums;

namespace BLL.Model
{
    public class MemberService
    {
        EmployeeRepository EmployeeRep;
        CustomerRepository CustomerRep;
        MemberModel member;

        public MemberService(EmployeeRepository employeeRepository, CustomerRepository customerRepository, MemberModel memberModel)
        {
            this.EmployeeRep = employeeRepository;
            this.CustomerRep = customerRepository;
            this.member = memberModel;
        }

        /// <summary>
        /// 依會員編號取得會員資訊
        /// </summary>
        /// <param name="Id">編號</param>
        public MemberModel GetMember(string Id)
        {
            int EmployeeId = 0;
            int.TryParse(Id, out EmployeeId);

            var empResult = EmployeeRep.GetEmployeeById(EmployeeId, (int)DataStatus.Enable);
            var cusResult = CustomerRep.GetCustomerById(Id, (int)DataStatus.Enable);

            //當有員工且無會員，則回傳員工資訊
            if (empResult.employee != null && cusResult.customer == null)
            {
                this.member.membertype = MemberStatus.Employee;
                this.member.employee = empResult.employee;
                this.member.IsSuccess = true;
                return this.member;
            }

            //當有會員且無員工，則回傳會員資訊
            if (cusResult.customer != null && empResult.employee == null)
            {
                this.member.membertype = MemberStatus.Customer;
                this.member.customer = cusResult.customer;
                this.member.IsSuccess = true;
                return this.member;
            }

            //當有取得員工有錯誤，則回傳取得員工錯誤訊息
            if (!empResult.rtn.IsSuccess)
            {
                this.member.membertype = MemberStatus.Employee;
                this.member.IsSuccess = empResult.rtn.IsSuccess;
                this.member.ErrorMsg = empResult.rtn.ErrorMsg;
                return this.member;
            }

            //當有取得會員有錯誤，則回傳取得會員錯誤訊息
            if (!cusResult.rtn.IsSuccess)
            {
                this.member.membertype = MemberStatus.Customer;
                this.member.IsSuccess = cusResult.rtn.IsSuccess;
                this.member.ErrorMsg = cusResult.rtn.ErrorMsg;
                return this.member;
            }
            return this.member;
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="member">會員</param>
        public Result UpdateMember(MemberModel member)
        {
            (Result rtn, int exeRows) updateMember = (new Result(), 0);

            if (member.membertype == MemberStatus.Customer)
                updateMember = CustomerRep.UpdateCustomer(member.customer); 
            else
                updateMember = EmployeeRep.UpdateEmployee(member.employee);

            if (updateMember.rtn.IsSuccess) updateMember.rtn.SuccessMsg = "會員更新成功";

            return updateMember.rtn;
        }
    }
}
