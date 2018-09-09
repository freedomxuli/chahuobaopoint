using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartFramework4v2.Web.WebExecutor;
using System.Data;
using SmartFramework4v2.Data.SqlServer;
using System.Text;
using System.Data.SqlClient;
using SmartFramework4v2.Web.Common.JSON;
using System.Collections;

/// <summary>
/// Module 的摘要说明
/// </summary>
[CSClass("Module")]
public class Module
{
	public Module()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    [CSMethod("SaveModule")]
    public object SaveModule(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            string mkmc = jsr["moduleName"];
            if (mkmc == "")
            {
                throw new Exception("模块名称不能为空！");
            }

            string mkpx = jsr["modulePx"];
            if (mkpx == "")
            {
                throw new Exception("模块序号不能为空！");
            }

            if (string.IsNullOrEmpty(jsr["moduleId"].ToString()))
            {
                //新增
                string moduleId = Guid.NewGuid().ToString();

                var dt = dbc.GetEmptyDataTable("tb_b_module");
                var sr = dt.NewRow();
                sr["moduleId"] = new Guid(moduleId);
                sr["moduleName"] = mkmc;
                sr["modulePx"] = mkpx;
                dt.Rows.Add(sr);
                dbc.InsertTable(dt);
            }
            else
            {
                //修改
                string moduleId = jsr["moduleId"].ToString();
                var dt = dbc.GetEmptyDataTable("tb_b_module");
                var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                var sr = dt.NewRow();
                sr["moduleId"] = new Guid(moduleId);
                sr["moduleName"] = mkmc;
                sr["modulePx"] = mkpx;
                dt.Rows.Add(sr);
                dbc.UpdateTable(dt, dtt);
            }

            return true;
        }
    }

    [CSMethod("SaveMenu")]
    public object SaveMenu(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            string menuName = jsr["menuName"];
            if (menuName == "")
            {
                throw new Exception("菜单名称不能为空！");
            }

            string menuurl = jsr["menuurl"];
            if (menuurl == "")
            {
                throw new Exception("菜单链接不能为空！");
            }

            string menuPx = jsr["menuPx"];
            if (menuPx == "")
            {
                throw new Exception("菜单序号不能为空！");
            }

            if (string.IsNullOrEmpty(jsr["menuId"].ToString()))
            {
                //新增
                string menuId = Guid.NewGuid().ToString();

                var dt = dbc.GetEmptyDataTable("tb_b_menu");
                var sr = dt.NewRow();
                sr["menuId"] = new Guid(menuId);
                sr["moduleId"] = jsr["moduleId"].ToString();
                sr["menuName"] = menuName;
                sr["menuPx"] = menuPx;
                sr["menuurl"] = menuurl;
                dt.Rows.Add(sr);
                dbc.InsertTable(dt);
            }
            else
            {
                //修改
                string menuId = jsr["menuId"].ToString();
                var dt = dbc.GetEmptyDataTable("tb_b_menu");
                var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                var sr = dt.NewRow();
                sr["menuId"] = new Guid(menuId);
                sr["moduleId"] = jsr["moduleId"].ToString();
                sr["menuName"] = menuName;
                sr["menuPx"] = menuPx;
                sr["menuurl"] = menuurl;
                dt.Rows.Add(sr);
                dbc.UpdateTable(dt, dtt);
            }

            return true;
        }
    }

    [CSMethod("SavePrivilege")]
    public object SavePrivilege(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            string privilegeName = jsr["privilegeName"];
            if (privilegeName == "")
            {
                throw new Exception("权限名称不能为空！");
            }

            string privilegePx = jsr["privilegePx"];
            if (privilegePx == "")
            {
                throw new Exception("权限序号不能为空！");
            }

            if (string.IsNullOrEmpty(jsr["privilegeId"].ToString()))
            {
                //新增
                string privilegeId = Guid.NewGuid().ToString();

                var dt = dbc.GetEmptyDataTable("tb_b_privilege");
                var sr = dt.NewRow();
                sr["privilegeId"] = new Guid(privilegeId);
                sr["menuId"] = jsr["menuId"].ToString();
                sr["moduleId"] = jsr["moduleId"].ToString();
                sr["privilegeName"] = privilegeName;
                sr["privilegePx"] = privilegePx;
                dt.Rows.Add(sr);
                dbc.InsertTable(dt);
            }
            else
            {
                //修改
                string privilegeId = jsr["privilegeId"].ToString();
                var dt = dbc.GetEmptyDataTable("tb_b_privilege");
                var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                var sr = dt.NewRow();
                sr["privilegeId"] = new Guid(privilegeId);
                sr["menuId"] = jsr["menuId"].ToString();
                sr["moduleId"] = jsr["moduleId"].ToString();
                sr["privilegeName"] = privilegeName;
                sr["privilegePx"] = privilegePx;
                dt.Rows.Add(sr);
                dbc.UpdateTable(dt, dtt);
            }

            return true;
        }
    }

    [CSMethod("DeleteModule")]
    public bool DeleteModule(string moduleId)
    {
        using (var db = new DBConnection())
        {
            string sql = "select count(*) num from tb_b_menu where moduleId = @moduleId";
            SqlCommand cmd = db.CreateCommand(sql);
            cmd.Parameters.Add("@moduleId", moduleId);
            string num = db.ExecuteScalar(cmd).ToString();
            if (Convert.ToInt32(num) == 0)
            {
                sql = "delete from tb_b_module where moduleId = @moduleId";
                cmd = db.CreateCommand(sql);
                cmd.Parameters.Add("@moduleId", moduleId);
                db.ExecuteNonQuery(cmd);
            }
            else
            {
                throw new Exception("该模块下有菜单，无法删除！");
            }
            return true;
        }
    }

    [CSMethod("DeletePrivilege")]
    public bool DeletePrivilege(string privilegeId)
    {
        using (var db = new DBConnection())
        {
            string sql = "delete from tb_b_privilege where privilegeId = @privilegeId";
            SqlCommand cmd = db.CreateCommand(sql);
            cmd.Parameters.Add("@privilegeId", privilegeId);
            db.ExecuteNonQuery(cmd);
            return true;
        }
    }

    [CSMethod("GetModuleTree")]
    public object GetModuleTree()
    {
        using (var db = new DBConnection())
        {
            try
            {
                string sql = "select * from tb_b_module order by modulePx";
                SqlCommand cmd = db.CreateCommand(sql);
                DataTable dt_module = db.ExecuteDataTable(cmd);

                sql = "select * from tb_b_menu order by menuPx";
                cmd = db.CreateCommand(sql);
                DataTable dt_menu = db.ExecuteDataTable(cmd);

                sql = "select * from tb_b_privilege order by privilegePx";
                cmd = db.CreateCommand(sql);
                DataTable dt_privilege = db.ExecuteDataTable(cmd);

                List<Hashtable> list = new List<Hashtable>();

                for (int i = 0; i < dt_module.Rows.Count; i++)
                {
                    Hashtable has = new Hashtable();
                    has["ML_ID"] = dt_module.Rows[i]["moduleId"];
                    has["ML_MC"] = dt_module.Rows[i]["moduleName"];
                    has["ML_LB"] = 0;
                    has["ML_URL"] = "";
                    has["MODULE_ID"] = dt_module.Rows[i]["moduleId"];
                    has["MENU_ID"] = "";
                    has["ML_PX"] = dt_module.Rows[i]["modulePx"];
                    has["expanded"] = true;

                    DataRow[] drs_menu = dt_menu.Select("moduleId = '" + dt_module.Rows[i]["moduleId"] + "'");
                    List<Hashtable> list1 = new List<Hashtable>();
                    for (var j = 0; j < drs_menu.Length; j++)
                    {
                        Hashtable has1 = new Hashtable();
                        has1["ML_ID"] = drs_menu[j]["menuId"];
                        has1["ML_MC"] = drs_menu[j]["menuName"];
                        has1["ML_LB"] = 1;
                        has1["ML_URL"] = drs_menu[j]["menuurl"];
                        has1["MODULE_ID"] = drs_menu[j]["moduleId"];
                        has1["MENU_ID"] = drs_menu[j]["menuId"];
                        has1["ML_PX"] = drs_menu[j]["menuPx"];
                        has1["expanded"] = true;
                        DataRow[] drs_privilege = dt_privilege.Select("moduleId = '" + dt_module.Rows[i]["moduleId"] + "' and menuId = '" + drs_menu[j]["menuId"] + "'");
                        List<Hashtable> list2 = new List<Hashtable>();
                        for (var k = 0; k < drs_privilege.Length; k++)
                        {
                            Hashtable has2 = new Hashtable();
                            has2["ML_ID"] = drs_privilege[k]["privilegeId"];
                            has2["ML_MC"] = drs_privilege[k]["privilegeName"];
                            has2["ML_LB"] = 2;
                            has2["ML_URL"] = "";
                            has2["MODULE_ID"] = drs_privilege[k]["moduleId"];
                            has2["MENU_ID"] = drs_privilege[k]["menuId"];
                            has2["ML_PX"] = drs_privilege[k]["privilegePx"];
                            has2["leaf"] = true;
                            list2.Add(has2);
                        }
                        has1["children"] = list2;
                        list1.Add(has1);
                    }
                    has["children"] = list1;
                    list.Add(has);
                }
                return new { ML_ID = "", ML_MC = "菜单目录", expanded = true, children = list };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("DeleteMenu")]
    public bool DeleteMenu(string menuId)
    {
        using (var db = new DBConnection())
        {
            string sql = "select count(*) num from tb_b_privilege where menuId = @menuId";
            SqlCommand cmd = db.CreateCommand(sql);
            cmd.Parameters.Add("@menuId", menuId);
            string num = db.ExecuteScalar(cmd).ToString();
            if (Convert.ToInt32(num) == 0)
            {
                sql = "delete from tb_b_menu where menuId = @menuId";
                cmd = db.CreateCommand(sql);
                cmd.Parameters.Add("@menuId", menuId);
                db.ExecuteNonQuery(cmd);
            }
            else
            {
                throw new Exception("该模块下有权限，无法删除！");
            }
            return true;
        }
    }

    [CSMethod("GetPrivilegeByRole")]
    public object GetPrivilegeByRole(string roleId)
    {
        using (var db = new DBConnection())
        {
            try
            {
                string sql = "select * from tb_b_module order by modulePx";
                SqlCommand cmd = db.CreateCommand(sql);
                DataTable dt_module = db.ExecuteDataTable(cmd);

                sql = "select * from tb_b_menu order by menuPx";
                cmd = db.CreateCommand(sql);
                DataTable dt_menu = db.ExecuteDataTable(cmd);

                sql = "select * from tb_b_privilege order by privilegePx";
                cmd = db.CreateCommand(sql);
                DataTable dt_privilege = db.ExecuteDataTable(cmd);

                sql = "select * from tb_b_menu_role where roleId = @roleId";
                cmd = db.CreateCommand(sql);
                cmd.Parameters.Add("@roleId", roleId);
                DataTable dt_menu_role = db.ExecuteDataTable(cmd);

                sql = "select * from tb_b_privilege_role where roleId = @roleId";
                cmd = db.CreateCommand(sql);
                cmd.Parameters.Add("@roleId", roleId);
                DataTable dt_privilege_role = db.ExecuteDataTable(cmd);

                List<Hashtable> list = new List<Hashtable>();
                for (int i = 0; i < dt_module.Rows.Count; i++)
                {
                    bool ischeck = true;
                    Hashtable has = new Hashtable();
                    has["ML_ID"] = dt_module.Rows[i]["moduleId"];
                    has["ML_MC"] = dt_module.Rows[i]["moduleName"];
                    has["ML_LB"] = 0;
                    has["ML_URL"] = "";
                    has["MODULE_ID"] = dt_module.Rows[i]["moduleId"];
                    has["MENU_ID"] = "";
                    has["ML_PX"] = dt_module.Rows[i]["modulePx"];
                    has["expanded"] = true;

                    DataRow[] drs_menu = dt_menu.Select("moduleId = '" + dt_module.Rows[i]["moduleId"] + "'");
                    List<Hashtable> list1 = new List<Hashtable>();
                    for (var j = 0; j < drs_menu.Length; j++)
                    {
                        bool ischeck1 = true;
                        Hashtable has1 = new Hashtable();
                        has1["ML_ID"] = drs_menu[j]["menuId"];
                        has1["ML_MC"] = drs_menu[j]["menuName"];
                        has1["ML_LB"] = 1;
                        has1["ML_URL"] = drs_menu[j]["menuurl"];
                        has1["MODULE_ID"] = drs_menu[j]["moduleId"];
                        has1["MENU_ID"] = drs_menu[j]["menuId"];
                        has1["ML_PX"] = drs_menu[j]["menuPx"];
                        has1["expanded"] = true;
                        DataRow[] drs_privilege = dt_privilege.Select("moduleId = '" + dt_module.Rows[i]["moduleId"] + "' and menuId = '" + drs_menu[j]["menuId"] + "'");
                        List<Hashtable> list2 = new List<Hashtable>();
                        for (var k = 0; k < drs_privilege.Length; k++)
                        {
                            bool ischeck2 = true;
                            DataRow[] drs_privilege_role = dt_privilege_role.Select("privilegeId = '" + drs_privilege[k]["privilegeId"].ToString() + "'");
                            if (drs_privilege_role.Length == 0)
                            {
                                ischeck1 = false;
                                ischeck2 = false;
                                ischeck = false;
                            }
                            Hashtable has2 = new Hashtable();
                            has2["ML_ID"] = drs_privilege[k]["privilegeId"];
                            has2["ML_MC"] = drs_privilege[k]["privilegeName"];
                            has2["ML_LB"] = 2;
                            has2["ML_URL"] = "";
                            has2["MODULE_ID"] = drs_privilege[k]["moduleId"];
                            has2["MENU_ID"] = drs_privilege[k]["menuId"];
                            has2["ML_PX"] = drs_privilege[k]["privilegePx"];
                            has2["leaf"] = true;
                            has2["checked"] = ischeck2;
                            list2.Add(has2);
                        }
                        has1["children"] = list2;
                        has1["checked"] = ischeck1;
                        list1.Add(has1);
                    }
                    has["children"] = list1;
                    has["checked"] = ischeck;
                    list.Add(has);
                }
                return new { ML_ID = "", ML_MC = "菜单目录", expanded = true, children = list };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("SavePrivilegeByRole")]
    public bool SavePrivilegeByRole(JSReader[] privilegeId,string roleId)
    {
        using (var db = new DBConnection())
        {
            string companyId = SystemUser.CurrentUser.CompanyID;
            string sql = "delete from tb_b_privilege_role where roleId = @roleId and companyId = @companyId";
            SqlCommand cmd = db.CreateCommand(sql);
            cmd.Parameters.Add("@roleId", roleId);
            cmd.Parameters.Add("@companyId", companyId);
            db.ExecuteNonQuery(cmd);

            sql = "delete from tb_b_menu_role where roleId = @roleId and companyId = @companyId";
            cmd = db.CreateCommand(sql);
            cmd.Parameters.Add("@roleId", roleId);
            cmd.Parameters.Add("@companyId", companyId);
            db.ExecuteNonQuery(cmd);

            sql = "select * from tb_b_privilege where " + db.C_In("privilegeId", privilegeId);
            cmd = db.CreateCommand(sql);
            DataTable dt_privilege = db.ExecuteDataTable(cmd);

            sql = "select * from tb_b_menu where menuId in (select menuId from tb_b_privilege where " + db.C_In("privilegeId", privilegeId) + ")";
            cmd = db.CreateCommand(sql);
            DataTable dt_menu = db.ExecuteDataTable(cmd);

            DataTable dt_menu_new = db.GetEmptyDataTable("tb_b_menu_role");
            DataTable dt_privilege_new = db.GetEmptyDataTable("tb_b_privilege_role");

            for (var i = 0; i < dt_menu.Rows.Count; i++)
            {
                DataRow dr = dt_menu_new.NewRow();
                dr["id"] = Guid.NewGuid();
                dr["menuId"] = dt_menu.Rows[i]["menuId"];
                dr["roleId"] = roleId;
                dr["companyId"] = companyId;
                dt_menu_new.Rows.Add(dr);
            }

            for (var i = 0; i < dt_privilege.Rows.Count; i++)
            {
                DataRow dr = dt_privilege_new.NewRow();
                dr["id"] = Guid.NewGuid();
                dr["privilegeId"] = dt_privilege.Rows[i]["privilegeId"];
                dr["roleId"] = roleId;
                dr["companyId"] = companyId;
                dt_privilege_new.Rows.Add(dr);
            }

            db.InsertTable(dt_menu_new);
            db.InsertTable(dt_privilege_new);
            
            return true;
        }
    }
}