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

/// <summary>
///JsMag 的摘要说明
/// </summary>
[CSClass("JsGlClass")]
public class JsMag
{
    public JsMag()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }

    [CSMethod("SaveRole")]
    public object SaveJs(JSReader jsr)
    {
        var user = SystemUser.CurrentUser;
        string companyId = user.CompanyID;
        using (DBConnection dbc = new DBConnection())
        {
            string lbmc = jsr["roleName"];
            if (lbmc == "")
            {
                throw new Exception("角色名称不能为空！");
            }

            string jsxh = jsr["rolePx"];
            if (jsxh == "")
            {
                throw new Exception("角色序号不能为空！");
            }

            if (jsr["roleId"].ToString() == "")
            {
                //新增
                string jsid = Guid.NewGuid().ToString();

                var dt = dbc.GetEmptyDataTable("tb_b_roledb");
                var sr = dt.NewRow();
                sr["roleId"] = new Guid(jsid);
                sr["roleName"] = lbmc;
                sr["rolePx"] = jsr["rolePx"].ToInteger();
                sr["companyId"] = companyId;
                dt.Rows.Add(sr);
                dbc.InsertTable(dt);
            }
            else
            {
                //修改
                string jsid = jsr["roleId"].ToString();
                var dt = dbc.GetEmptyDataTable("tb_b_roledb");
                var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                var sr = dt.NewRow();
                sr["roleId"] = new Guid(jsid);
                sr["roleName"] = lbmc;
                sr["rolePx"] = jsr["rolePx"].ToInteger();
                sr["companyId"] = companyId;
                dt.Rows.Add(sr);
                dbc.UpdateTable(dt, dtt);
            }

            return GetRole();
        }
    }

    [CSMethod("GetRole")]
    public object GetRole()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                var companyId = SystemUser.CurrentUser.CompanyID;
                string sql = "select * from tb_b_roledb where companyId = @companyId order by rolePx";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.Add("@companyId", companyId);
                DataTable dt = dbc.ExecuteDataTable(cmd);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("DeleteRole")]
    public object DeleteJs(JSReader jsr)
    {
        var user = SystemUser.CurrentUser;
        string userid = user.UserID;
        string companyId = user.CompanyID;

        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    //判断角色是否被用户关联就不能删除
                    string str = "select count(*) from tb_b_user_role where roleId=@roleId and companyId=@companyId";
                    SqlCommand ocmd = new SqlCommand(str);
                    ocmd.Parameters.AddWithValue("@roleId", jsr.ToArray()[i].ToString());
                    ocmd.Parameters.AddWithValue("@companyId", companyId);
                    int num = Convert.ToInt32(dbc.ExecuteScalar(ocmd));

                    if (num > 0)
                    {
                        throw new Exception("你所要删除的角色已经被用户关联，请先删除用户再进行此操作！");
                    }
                    else
                    {

                        string delstr = "delete from tb_b_roledb where roleId=@roleId and companyId=@companyId";
                        ocmd = new SqlCommand(delstr);
                        ocmd.Parameters.AddWithValue("roleId", jsr.ToArray()[i].ToString());
                        ocmd.Parameters.AddWithValue("companyId", companyId);
                        dbc.ExecuteNonQuery(ocmd);
                    }
                }

                dbc.CommitTransaction();

                return GetRole();
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }
}
