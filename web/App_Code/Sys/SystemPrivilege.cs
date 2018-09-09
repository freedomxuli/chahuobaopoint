using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///SystemPrivilege 的摘要说明
/// </summary>
namespace Smart.SystemPrivilege
{
   
    public class 系统管理_角色管理
    {
        public static PrivilegeDescription 角色管理 = new PrivilegeDescription("系统管理_角色管理-角色管理", "查看", 1);
    }
    public class 系统管理_人员管理
    {
        public static PrivilegeDescription 人员管理 = new PrivilegeDescription("系统管理_人员管理-人员管理", "查看", 1);
    }

}

