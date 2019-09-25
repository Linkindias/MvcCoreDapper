﻿using BLL.InterFace;
using DAL.DTOModel;
using DAL.PageModel;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Text;
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

            MemberModel memberModel = new MemberModel();
            var empResult = EmployeeRep.GetEmployeeById(EmployeeId, (int)DataStatus.Enable);
            var cusResult = CustomerRep.GetCustomerById(Id, (int)DataStatus.Enable);

            if (empResult.rtn.IsSuccess && empResult.employee != null) memberModel.employee = empResult.employee;

            if (cusResult.rtn.IsSuccess && cusResult.customer != null) memberModel.customer = cusResult.customer;

            return memberModel;
        }
    }
}
