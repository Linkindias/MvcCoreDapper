using Base;
using BLL.InterFace;
using BLL.PageModel;
using DAL.Repository;
using static Base.Enums;

namespace BLL.Model
{
    public class MemberService : IMemberService
    {
        EmployeeRepository EmployeeRep;
        CustomerRepository CustomerRep;

        public MemberService(EmployeeRepository employeeRepository, CustomerRepository customerRepository)
        {
            this.EmployeeRep = employeeRepository;
            this.CustomerRep = customerRepository;
        }

        /// <summary>
        /// 依會員編號取得會員資訊
        /// </summary>
        /// <param name="Id">編號</param>
        public MemberModel GetMember(string Id)
        {
            int EmployeeId = 0;
            int.TryParse(Id, out EmployeeId);

            MemberModel member = new MemberModel();
            var empResult = EmployeeRep.GetEmployeeById(EmployeeId, (int)DataStatus.Enable);
            var cusResult = CustomerRep.GetCustomerById(Id, (int)DataStatus.Enable);

            //當有員工且無會員，則回傳員工資訊
            if (empResult.employee != null && cusResult.customer == null)
            {
                member.employee = empResult.employee;
                member.IsSuccess = true;
                return member;
            }

            //當有會員且無員工，則回傳會員資訊
            if (cusResult.customer != null && empResult.employee == null)
            {
                member.customer = cusResult.customer;
                member.IsSuccess = true;
                return member;
            }

            //當有取得員工有錯誤，則回傳取得員工錯誤訊息
            if (!empResult.rtn.IsSuccess)
            {
                member.IsSuccess = empResult.rtn.IsSuccess;
                member.ErrorMsg = empResult.rtn.ErrorMsg;
                return member;
            }

            //當有取得會員有錯誤，則回傳取得會員錯誤訊息
            if (!cusResult.rtn.IsSuccess)
            {
                member.IsSuccess = cusResult.rtn.IsSuccess;
                member.ErrorMsg = cusResult.rtn.ErrorMsg;
                return member;
            }
            return member;
        }

        /// <summary>
        /// 更新會員
        /// </summary>
        /// <param name="member">會員</param>
        public Result UpdateMember(MemberModel member)
        {
            (Result rtn, int exeRows) updateMember = (new Result(), 0);

            if (member.customer.CustomerID != null)
                updateMember = CustomerRep.UpdateCustomer(member.customer); 
            else
                updateMember = EmployeeRep.UpdateEmployee(member.employee);

            if (updateMember.rtn.IsSuccess) updateMember.rtn.SuccessMsg = "會員更新成功";

            return updateMember.rtn;
        }
    }
}
