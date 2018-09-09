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
/// LPMag 的摘要说明
/// </summary>
[CSClass("LPMag")]
public class LPMag
{
	public LPMag()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    [CSMethod("GetLPByUserid")]
    public object GetLPByUserid(string userid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select UL_ID,a.lp_id,lp_name,lp_path,UL_PX from tb_b_User_LP_GL a left join tb_b_LeaderPage b on a.lp_id=b.lp_id where a.delflag=0 and b.delflag=0 and a.User_ID='" + userid + "' order by UL_PX";
                DataTable dt = dbc.ExecuteDataTable(cmd);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetLP")]
    public object GetLP()
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select * from tb_b_LeaderPage where delflag=0";
                DataTable dt = dbc.ExecuteDataTable(cmd);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("SaveLP")]
    public object SaveLP(string userid, JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {                
                JSReader[] js = jsr.ToArray();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"delete from tb_b_User_LP_GL where User_ID='"+userid+"'";
                dbc.ExecuteNonQuery(cmd);
                for (int i = 0; i < js.Length; i++)
                {
                    cmd = new SqlCommand();
                    cmd.CommandText = @"insert into tb_b_User_LP_GL select newid(),@userid,@lp_id,null,0,getdate(),getdate(),'9b628843-8ff6-4100-a7f9-73b4043871ad'";
                    cmd.Parameters.AddWithValue("@userid",userid);
                    cmd.Parameters.AddWithValue("@lp_id",js[i].ToString());
                    dbc.ExecuteNonQuery(cmd);
                }
                dbc.CommitTransaction();
                cmd = new SqlCommand();
                cmd.CommandText = @"select UL_ID,a.lp_id,lp_name,lp_path,UL_PX from tb_b_User_LP_GL a left join tb_b_LeaderPage b on a.lp_id=b.lp_id where a.delflag=0 and a.User_ID='" + userid + "' order by UL_PX";
                DataTable dt = dbc.ExecuteDataTable(cmd);
                return dt;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

 

    [CSMethod("Delete")]
    public object Delete(string userid,JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                JSReader[] js = jsr.ToArray();
                SqlCommand cmd = new SqlCommand();
                for (int i = 0; i < js.Length; i++)
                {
                    cmd = new SqlCommand();
                    cmd.CommandText = @"delete from tb_b_User_LP_GL where UL_ID=@UL_ID";
                    cmd.Parameters.AddWithValue("@UL_ID", js[i].ToString());
                    dbc.ExecuteNonQuery(cmd);
                }
                dbc.CommitTransaction();
                cmd = new SqlCommand();
                cmd.CommandText = @"select UL_ID,a.lp_id,lp_name,lp_path,UL_PX from tb_b_User_LP_GL a left join tb_b_LeaderPage b on a.lp_id=b.lp_id where a.delflag=0 and a.User_ID='" + userid + "' order by UL_PX";
                DataTable dt = dbc.ExecuteDataTable(cmd);
                return dt;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }

    [CSMethod("Update")]
    public object Update(string ul_id,string ul_px,string userid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"update tb_b_User_LP_GL set UL_PX=" + ul_px+" where UL_ID='"+ul_id+"'";
                dbc.ExecuteNonQuery(cmd);
                dbc.CommitTransaction();

                cmd = new SqlCommand();
                cmd.CommandText = @"select UL_ID,a.lp_id,lp_name,lp_path,UL_PX from tb_b_User_LP_GL a left join tb_b_LeaderPage b on a.lp_id=b.lp_id where a.delflag=0 and a.User_ID='" + userid + "' order by UL_PX";
                DataTable dt = dbc.ExecuteDataTable(cmd);
                return dt;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }
}