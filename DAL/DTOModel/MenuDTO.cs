using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.DTOModel
{
    public class MenuDTO
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; }

        public string MenuCode { get; set; }

        public int ParentID { get; set; }

        public bool? Add { get; set; }

        public bool? Update { get; set; }

        public bool? Delete { get; set; }

        public bool? Query { get; set; }

        public bool? View { get; set; }

        public List<MenuDTO> SubMenus { get; set; }
    }
}
