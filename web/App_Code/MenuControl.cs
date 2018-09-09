using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using SmartFramework4v2.Data.SqlServer;
using SmartFramework4v2.Web.WebExecutor;

/// <summary>
///MenuControl 的摘要说明
/// </summary>
public class MenuControl
{
//    public static String xmlMenu = @"
//        <MainMenu>
//            <Menu Name='系统维护中心'>
//                <Item Name='角色管理'>
//                    <Tab p='系统管理.角色管理' Name='角色管理'>approot/r/page/UserMag/RoleManage.html</Tab>
//                </Item>
//                <Item Name='人员管理'>
//                    <Tab p='系统管理.人员管理' Name='人员管理'>approot/r/page/UserMag/UserManage.html</Tab>
//                </Item>        
//                <Item Name='权限管理'>
//                    <Tab p='系统管理.权限管理' Name='权限管理'>approot/r/page/UserMag/PrivilegeManage.html</Tab>
//                </Item>
//            </Menu>
//        </MainMenu>
//    ";

    public static string loadXml()
    {
        using (var db = new DBConnection())
        {
            string roleId = SystemUser.CurrentUser.RoleID;
            string sql_menu = "select b.* from tb_b_menu_role a left join tb_b_menu b on a.menuId = b.menuId where a.roleId = @roleId";
            SqlCommand cmd = db.CreateCommand(sql_menu);
            cmd.Parameters.Add("@roleId", roleId);
            DataTable dt_menu = db.ExecuteDataTable(cmd);

            string sql_module = "select * from tb_b_module where moduleId in (select b.moduleId from tb_b_menu_role a left join tb_b_menu b on a.menuId = b.menuId where a.roleId = @roleId) order by modulePx";
            cmd = db.CreateCommand(sql_module);
            cmd.Parameters.Add("@roleId", roleId);
            DataTable dt_module = db.ExecuteDataTable(cmd);

            string xmlMenu = @"
                    <MainMenu>";

            for (int i = 0; i < dt_module.Rows.Count; i++)
            {
                DataRow[] drs = dt_menu.Select("moduleId = '" + dt_module.Rows[i]["moduleId"].ToString() + "'", "menuPx asc");
                if (drs.Length > 0)
                {
                    xmlMenu += "<Menu Name='" + dt_module.Rows[i]["moduleName"].ToString() + "'>";
                    for (var j = 0; j < drs.Length; j++)
                    {
                        xmlMenu += @"
                                <Item Name='"+drs[j]["menuName"].ToString()+@"'>
                                    <Tab p='" + dt_module.Rows[i]["moduleName"].ToString() + @"." + drs[j]["menuName"].ToString() + @"' Name='" + drs[j]["menuName"].ToString() + @"'>" + drs[j]["menuurl"].ToString() + @"</Tab>
                                </Item>";
                    }
                    xmlMenu += "</Menu>";
                }
            }
            xmlMenu += @"</MainMenu>";

            return xmlMenu;
        }
    }


    public static string GenerateMenuByPrivilege()
    {
        System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        doc.LoadXml(loadXml());
        StringBuilder sb = new StringBuilder();
        int num = 0;

        var cu = SystemUser.CurrentUser;

        sb.Append("[");
        foreach (System.Xml.XmlElement MenuEL in doc.SelectNodes("/MainMenu/Menu"))
        {
            if (num > 0)
            {
                sb.Append(",");
            }
            num++;

            string title = MenuEL.GetAttribute("Name").ToString().Trim();

            string lis = "";
            foreach (System.Xml.XmlElement ItemEl in MenuEL.SelectNodes("Item"))
            {
                string secname = ItemEl.GetAttribute("Name");
                string msg = "";
                foreach (XmlElement TabEl in ItemEl.SelectNodes("Tab"))
                {
                    string p = TabEl.GetAttribute("p").ToString().Trim();
                    string pantitle = TabEl.GetAttribute("Name").ToString().Trim();
                    string src = TabEl.InnerText;
                    if (msg == "")
                    {
                        msg += pantitle + "," + src;
                    }
                    else
                    {
                        msg += "|" + pantitle + "," + src;
                    }
                }
                if (msg != "")
                {
                    lis += "+ '<li class=\"fore\"><a class=\"MenuItem\" href=\"page/TabMenu.html?msg=" + msg + "\" target=\"mainframe\"><img height=16 width=16 align=\"absmiddle\" style=\"border:0\" src=\"../CSS/images/application.png\" />　" + secname + "</a></li>'";

                }
            }

            if (lis != "")
            {
                sb.Append("{");
                sb.Append("xtype: 'panel',");
                sb.Append("collapsed: false,");
                sb.Append("iconCls: 'cf',");
                sb.Append("title: '" + title + "',");
                sb.Append("html: '<ul class=\"MenuHolder\">'");
                sb.Append(lis);
                sb.Append("+ '</ul>'");
                sb.Append("}");
            }
        }
        sb.Append("]");
        return sb.ToString();
    }
}
