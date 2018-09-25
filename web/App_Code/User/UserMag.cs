﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SmartFramework4v2.Web.WebExecutor;
using System.Data;
using SmartFramework4v2.Data.SqlServer;
using System.Text;
using System.Data.SqlClient;
using SmartFramework4v2.Web.Common.JSON;
using SmartFramework4v2.Data;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
/// <summary>
///UserMag 的摘要说明
/// </summary>

[CSClass("YHGLClass")]
public class UserMag
{
    [CSMethod("GetUserListTotal")]
    public object GetUserListTotal(string jsid, string xm)
    {
        if (!string.IsNullOrEmpty(jsid))
        {
            try
            {
                Guid guid = new Guid(jsid);
            }
            catch
            {
                throw new Exception("角色ID出错！");
            }
        }

        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                string where = "";
                if (!string.IsNullOrEmpty(jsid))
                {
                    where += " and User_ID in (SELECT User_ID FROM tb_b_User_JS_Gl where JS_ID='" + jsid + "' and delflag=0 )";
                }

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("User_XM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = "select * from tb_b_Users where User_DelFlag=0  ";
                str += where;
                str += " order by LoginName,User_XM";

                DataTable dt = dbc.ExecuteDataTable(str);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    [CSMethod("UploadPicForProduct", 1)]
    public object UploadPicForProduct(FileData[] fds, string UserID)
    {
        var sqlStr = "insert into tb_b_FJ (fj_id,fj_mc,fj_pid,fj_nr,addtime,updatetime,status,xgyh_id)"
                    + "values (@fj_id,@fj_mc,@fj_pid,@fj_nr,getdate(),getdate(),0,@xgyh_id)";
        using (DBConnection dbc = new DBConnection())
        {
            string FJID = Guid.NewGuid().ToString();
            SqlCommand cmd = new SqlCommand(sqlStr);
            cmd.Parameters.AddWithValue("@fj_id", FJID);
            cmd.Parameters.AddWithValue("@fj_mc", fds[0].FileName);
            cmd.Parameters.AddWithValue("@fj_pid", UserID);
            cmd.Parameters.AddWithValue("@fj_nr", KiResizeImage(fds[0].FileBytes));
            cmd.Parameters.AddWithValue("@xgyh_id", DBNull.Value);
            int retInt = dbc.ExecuteNonQuery(cmd);
            if (retInt > 0)
                return new { fileurl = "files/" + FJID + "/" + fds[0].FileName, isdefault = 0, fileid = FJID };
            return null;
        }
    }

    [CSMethod("DelProductImageByPicID")]
    public bool DelProductImageByPicID(string fj_id)
    {
        string sqlStr = "update tb_b_FJ set STATUS = 1,UPDATETIME = getdate(),XGYH_ID = @XGYH_ID where fj_id = @fj_id ";
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                dbc.BeginTransaction();
                SqlCommand cmd = new SqlCommand(sqlStr);
                cmd.Parameters.AddWithValue("@XGYH_ID", DBNull.Value);
                cmd.Parameters.AddWithValue("@fj_id", fj_id);
                dbc.ExecuteNonQuery(cmd);
                dbc.CommitTransaction();
                return true;
            }
            catch
            {
                dbc.RoolbackTransaction();
                return false;
            }
        }
    }

    [CSMethod("GetProductImages")]
    public DataTable GetProductImages(string pid)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(" select 'files/'+cast(t.fj_id as nvarchar(50))+'/'+t.fj_mc as FILEURL,");
        sb.Append(" case");
        sb.Append(" when exists");
        sb.Append(" (select * from tb_b_User where tb_b_User.UserID =@FJ_PID) then");
        sb.Append(" 1");
        sb.Append("  else");
        sb.Append(" 0");
        sb.Append("  end as isdefault,t.fj_id");
        sb.Append(" from tb_b_FJ t");
        sb.Append(" where  t.FJ_PID = @FJ_PID and t.STATUS = 0");
        sb.Append(" order by t.ADDTIME desc");
        using (DBConnection dbc = new DBConnection())
        {
            SqlCommand cmd = new SqlCommand(sb.ToString());
            cmd.Parameters.AddWithValue("@FJ_PID", pid);
            return dbc.ExecuteDataTable(cmd);
        }
    }

    /// <summary>
    /// Resize图片
    /// </summary>
    /// <param name="bmp">原始Bitmap</param>
    /// <param name="newW">新的宽度</param>
    /// <param name="newH">新的高度</param>
    /// <param name="Mode">保留着，暂时未用</param>
    /// <returns>处理以后的图片</returns>
    public static byte[] KiResizeImage(byte[] stream)
    {
        try
        {

            Stream bmp = new MemoryStream(stream);

            Bitmap bitMap = new Bitmap(bmp);//创建bitmap
            bmp.Dispose();

            int x = 0;
            int y = 0;
            int width = 500;//缩放后的图的高，1280缩小成这个特定的高度
            int height = 375;//缩放后的图的宽，768缩小成这个特定的宽度
            Bitmap bitMap2 = new Bitmap(width, height, PixelFormat.Format32bppArgb);//缩放后的新图

            Graphics gImg = Graphics.FromImage(bitMap2);
            gImg.CompositingQuality = CompositingQuality.HighQuality;
            gImg.InterpolationMode = InterpolationMode.High;
            // 指定高质量插值法。 指定这个算法缩放图片后，得到的这张特定图片是黑图，而指定高质量的双三次插值法InterpolationMode.HighQualityBicubic也会生成黑图，而指定其他算法就没有问题。另外，缩放成其他大小也没有问题，比如width=547, height=328。
            gImg.SmoothingMode = SmoothingMode.HighQuality;
            gImg.Clear(Color.Transparent);
            gImg.DrawImage(bitMap, x, y, width, height);

            //Bitmap 转化为 Byte[]       
            byte[] bReturn = null;
            MemoryStream ms = new MemoryStream();
            bitMap2.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            bReturn = ms.GetBuffer();

            bitMap.Dispose();
            gImg.Dispose();
            bitMap2.Dispose();
            return bReturn;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static bool getJs_cj()
    {
        var user = SystemUser.CurrentUser;
        string userid = user.UserID;

        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                string str = "SELECT * FROM tb_b_User_JS_Gl where delflag=0 AND JS_ID='F6613AFB-06E2-454A-881F-8C51483976F3' and USER_ID='" + userid + "'";
                DataTable dt = dbc.ExecuteDataTable(str);
                if (dt.Rows.Count == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }

    [CSMethod("GetUserList")]
    public object GetUserList(int pagnum, int pagesize, string roleId, string yhm,string xm)
    {
        if (!string.IsNullOrEmpty(roleId))
        {
            try
            {
                Guid guid = new Guid(roleId);
            }
            catch
            {
                throw new Exception("角色ID出错！");
            }
        }

        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                int cp = pagnum;
                int ac = 0;

                string where = "";
                if (!string.IsNullOrEmpty(roleId))
                {
                    where += " and a.UserID in (SELECT userId FROM tb_b_user_role where roleId='" + roleId + "')";
                }

                if (!string.IsNullOrEmpty(yhm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserName", yhm.Trim(), LikeStyle.LeftAndRightLike);
                }

                if (!string.IsNullOrEmpty(xm.Trim()))
                {
                    where += " and " + dbc.C_Like("a.UserXM", xm.Trim(), LikeStyle.LeftAndRightLike);
                }

                string str = @"select a.*,c.roleName,b.roleId from tb_b_user a left join tb_b_user_role b on a.UserID=b.UserID
                                left join tb_b_roledb c on b.roleId=c.roleId where 1=1  ";
                str += where;

                //开始取分页数据
                System.Data.DataTable dtPage = new System.Data.DataTable();
                dtPage = dbc.GetPagedDataTable(str + " order by a.UserName,a.UserXM", pagesize, ref cp, out ac);

                return new { dt = dtPage, cp = cp, ac = ac };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

    //[CSMethod("GetUserList")]
    //public object GetUserList(int cp, string userName, string deptType, string dept)
    //{
    //    string condition = "";
    //    if (deptType.Trim() != "")
    //    {
    //        condition += " and t2.dw_lx = '" + deptType.Replace("'", "''") + "'";
    //    }
    //    if (userName.Trim() != "")
    //    {
    //        condition += " and t1.yh_dlm like '%" + userName.Replace("'", "''") + "%'";
    //    }
    //    if (dept.Trim() != "")
    //    {
    //        condition += " and t1.dw_id = '" + dept.Replace("'", "''") + "'";
    //    }
    //    StringBuilder sb = new StringBuilder();
    //    sb.Append("select t1.yh_id, ");
    //    sb.Append("t1.yh_dlm, ");
    //    sb.Append("t1.addtime, ");
    //    sb.Append("t2.dw_mc, ");
    //    sb.Append("case t2.dw_lx ");
    //    sb.Append("when 0 then ");
    //    sb.Append("'物价局' ");
    //    sb.Append("when 1 then ");
    //    sb.Append("'配供中心' ");
    //    sb.Append("when 2 then ");
    //    sb.Append("'平价商店' ");
    //    sb.Append("when 3 then ");
    //    sb.Append("'蔬菜基地' ");
    //    sb.Append("end as dw_lx ,case t1.yh_enable when 0 then '启用' else '禁用' end as YH_ENABLE ");
    //    sb.Append("from TZCLZ_T_YH t1, TZCLZ_T_DW t2 ");
    //    sb.AppendFormat("where t1.dw_id = t2.dw_id and t1.status = 0 {0} order by t1.addtime", condition);
    //    int allCount = 0;
    //    int currentPage = cp;
    //    currentPage = cp;
    //    using (DBConnection dbc = new DBConnection())
    //    {
    //        DataTable dt = dbc.ExecuteDataTable(sb.ToString());
    //        return new { dtUser = dt, currentPage = cp, allCount = allCount };
    //    }
    //}

    [CSMethod("GetUserAndJs")]
    public object GetUserAndJs(string UserId)
    {
        
        using (DBConnection dbc = new DBConnection())
        {
            string sqlStrUser = "select * from tb_b_Users where User_ID='" + UserId + "'";
            DataTable dtuser=dbc.ExecuteDataTable(sqlStrUser);
            string sqlStrJs = "select distinct JS_ID from tb_b_User_JS_Gl where delflag=0 and User_ID='" + UserId + "'";
            DataTable dtjs=dbc.ExecuteDataTable(sqlStrJs);

            return new { dtuser = dtuser, dtjs = dtjs};
        }
    }


    [CSMethod("GetUser")]
    public DataTable GetUser(string UserId)
    {
        string sqlStr = "select * from tb_b_Users where User_ID='" + UserId + "'";
        using (DBConnection dbc = new DBConnection())
        {
            return dbc.ExecuteDataTable(sqlStr);
        }
    }

    [CSMethod("GetUserJs")]
    public DataTable GetUserJs(string UserId)
    {
        string sqlStr = "select distinct JS_ID from tb_b_User_JS_Gl where delflag=0 and User_ID='" + UserId + "'";
        using (DBConnection dbc = new DBConnection())
        {
            return dbc.ExecuteDataTable(sqlStr);
        }
    }

    [CSMethod("GetRole")]
    public DataTable GetRole()
    {
        string sqlStr = "select roleId,roleName from tb_b_roledb order by rolePx";
        using (DBConnection dbc = new DBConnection())
        {
            return dbc.ExecuteDataTable(sqlStr);
        }
    }

    [CSMethod("GetDWByJsid")]
    public DataTable GetDWByJsid(string UserId,string jsid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            DataTable dt = new DataTable();
            string sqlStr = "select * from tb_b_roledb where status=0 and roleId='" + jsid + "'";
            DataTable dtjs = dbc.ExecuteDataTable(sqlStr);
            if (dtjs != null && dtjs.Rows.Count > 0)
            {
                int jstype = Convert.ToInt32(dtjs.Rows[0]["JS_Type"]);
                if (jstype == 0)
                {
                    string str = "select distinct DW_ID from tb_b_User_Dw_Gl where delflag=0 and DW_ID in(SELECT DW_ID  FROM tb_b_Department where STATUS=0) and User_ID='" + UserId + "'";
                    dt = dbc.ExecuteDataTable(str);
                }
                else
                {
                    switch (jsid.ToUpper())
                    {
                        case "F6613AFB-06E2-454A-881F-8C51483976F3":
                            dt = dbc.ExecuteDataTable("select distinct DW_ID from tb_b_User_Dw_Gl where delflag=0 and DW_ID in( select DW_ID from tb_b_DW where DW_LX=4 and STATUS=0) and User_ID='" + UserId + "'");
                            break;
                        case "7E53492E-CF66-411F-83C4-7923467F59B4":
                            dt = dbc.ExecuteDataTable("select distinct DW_ID from tb_b_User_Dw_Gl where delflag=0 and DW_ID in( select PJSD_ID from tb_b_PJSD where delflag=0) and User_ID='" + UserId + "'");
                            break;
                    }
                }
            }
            return dt;
        }
    }


    [CSMethod("GetDW")]
    public DataTable GetDW(string jsid)
    {
        using (DBConnection dbc = new DBConnection())
        {
            DataTable dt = new DataTable();
            string sqlStr = "select * from tb_b_JS where status=0 and JS_ID='" + jsid + "'";
            DataTable dtjs = dbc.ExecuteDataTable(sqlStr);
            if (dtjs != null && dtjs.Rows.Count > 0)
            {
                int jstype = Convert.ToInt32(dtjs.Rows[0]["JS_Type"]);
                if (jstype == 0)
                {
                    string str1 = "SELECT DW_ID ID,DW_MC MC FROM tb_b_Department where STATUS=0";
                    dt = dbc.ExecuteDataTable(str1);
                }
                else
                {
                    switch (jsid.ToUpper())
                    {
                        case "F6613AFB-06E2-454A-881F-8C51483976F3":
                            dt = dbc.ExecuteDataTable("select DW_ID ID,DW_MC MC from tb_b_DW where DW_LX=4 and STATUS=0");
                            break;
                        case "7E53492E-CF66-411F-83C4-7923467F59B4":
                            dt = dbc.ExecuteDataTable("select PJSD_ID ID,PJSD_MC MC from dbo.tb_b_PJSD where delflag=0");
                            break;
                    }
                }
            }
            return dt;
        }
    }

    [CSMethod("GetDWAndGl")]
    public object GetDWAndGl( string jsid,string UserId)
    {
        using (DBConnection dbc = new DBConnection())
        {
            try
            {
                //单位
                DataTable dt = new DataTable();
                //权限
                DataTable dtqx = new DataTable();
                //权限关联
                DataTable dtqxgl = new DataTable();
                //用户关联
                DataTable dtusergl = new DataTable();

                string sqlStr = "select * from tb_b_JS where status=0 and JS_ID='" + jsid + "'";
                DataTable dtjs = dbc.ExecuteDataTable(sqlStr);

                if (dtjs != null && dtjs.Rows.Count > 0)
                {
                    int jstype = Convert.ToInt32(dtjs.Rows[0]["JS_Type"]);
                    if (jstype == 0)
                    {
                        string str1 = "SELECT DW_ID ID,DW_MC MC FROM tb_b_Department where STATUS=0";
                        dt = dbc.ExecuteDataTable(str1);
                        string str2 = "SELECT PRIVILEGECODE ID,MODULENAME MC FROM tb_b_YH_QX where MODULENAME not like '平价商店权限_%' order by substring(MODULENAME,1,charindex('-',MODULENAME)),ORDERNO";
                        dtqx = dbc.ExecuteDataTable(str2);
                        if (UserId != null)
                        {
                            string str3 = "select a.PRIVILEGECODE,b.MODULENAME from tb_b_YH_YHQX a left join tb_b_YH_QX b on a.PRIVILEGECODE=b.PRIVILEGECODE where USERID='" + UserId + "'";
                            dtqxgl = dbc.ExecuteDataTable(str3);
                        }
                    }
                    else
                    {
                        switch (jsid.ToUpper())
                        {
                            case "F6613AFB-06E2-454A-881F-8C51483976F3":
                                dt = dbc.ExecuteDataTable("select DW_ID ID,DW_MC MC from tb_b_DW where DW_LX=4 and STATUS=0");
                                break;
                            case "7E53492E-CF66-411F-83C4-7923467F59B4":
                                dt = dbc.ExecuteDataTable("select PJSD_ID ID,PJSD_MC MC,QY_NAME from tb_b_PJSD a left join tb_b_Eare b on a.qy_id=b.qy_id where delflag=0 order by QY_PX,PJSD_NO");
                                string str2 = "SELECT PRIVILEGECODE ID,MODULENAME MC FROM tb_b_YH_QX where MODULENAME like '平价商店权限_%' order by substring(MODULENAME,1,charindex('-',MODULENAME)),ORDERNO";
                                dtqx = dbc.ExecuteDataTable(str2);
                                if (UserId != null)
                                {
                                    string str3 = "select a.PRIVILEGECODE,b.MODULENAME from tb_b_YH_YHQX a left join tb_b_YH_QX b on a.PRIVILEGECODE=b.PRIVILEGECODE where USERID='" + UserId + "'";
                                    dtqxgl = dbc.ExecuteDataTable(str3); 
                                }
                                break;
                        }
                    }
                }


                if (UserId != null)
                {
                    dtusergl = GetDWByJsid(UserId, jsid);
                }

                return new { dtdw = dt, usergl = dtusergl, dtqx = dtqx, dtqxgl = dtqxgl };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    [CSMethod("GetDeptsByType")]
    public DataTable GetDeptsByType(string DeptType)
    {
        string sqlStr = "select * from tb_b_Department where DW_LX = @DW_LX and STATUS = 0";
        using (DBConnection dbc = new DBConnection())
        {
            SqlCommand cmd = new SqlCommand(sqlStr);
            cmd.Parameters.AddWithValue("@DW_LX", DeptType);
            return dbc.ExecuteDataTable(cmd);
        }
    }
    [CSMethod("GetDepts")]
    public DataTable GetDepts()
    {
        string sqlStr = "select * from tb_b_Department where  STATUS = 0";
        using (DBConnection dbc = new DBConnection())
        {
            SqlCommand cmd = new SqlCommand(sqlStr);
            return dbc.ExecuteDataTable(cmd);
        }
    }

    [CSMethod("SaveUser")]
    public bool SaveUser(JSReader jsr)
    {
        if (jsr["UserName"].IsNull || jsr["UserName"].IsEmpty)
        {
            throw new Exception("用户名不能为空");
        }
        if (jsr["Password"].IsNull || jsr["Password"].IsEmpty)
        {
            throw new Exception("密码不能为空");
        }

        if (jsr["roleId"].IsNull || jsr["roleId"].IsEmpty)
        {
            throw new Exception("没有用户角色！");
        }

        var companyId = SystemUser.CurrentUser.CompanyID;
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                if (jsr["UserID"].ToString() == "")
                {
                    DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName='" + jsr["UserName"].ToString() + "'");
                    if (dt_user.Rows.Count > 0)
                    {
                        throw new Exception("该用户名已存在！");
                    }
                    //用户表
                    var YHID = Guid.NewGuid().ToString();
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dr = dt.NewRow();
                    dr["UserID"] = new Guid(YHID);
                    dr["UserName"] = jsr["UserName"].ToString();
                    dr["Password"] = jsr["Password"].ToString();
                    dr["AddTime"] = DateTime.Now;
                    dr["IsSHPass"] = 1;
                    dr["Points"] = 0;
                    dr["ClientKind"] = 0;
                    //dr["Discount"] = ;
                    dr["UserXM"] = jsr["UserXM"].ToString();
                    dr["UserTel"] = jsr["UserTel"].ToString();
                    //dr["FromRoute"] = ;
                    //dr["ToRoute"] = ;
                    dr["companyId"] = companyId;
                    //dr["PayPassword"] = ;
                    dt.Rows.Add(dr);
                    dbc.InsertTable(dt);

                    //角色用户关联表
                    var rdt = dbc.GetEmptyDataTable("tb_b_user_role");
                    var rdr = rdt.NewRow();
                    rdr["userroleId"] = Guid.NewGuid().ToString();
                    rdr["userId"] = new Guid(YHID);
                    rdr["roleId"] = jsr["roleId"].ToString();
                    rdr["companyId"] = companyId;
                    rdt.Rows.Add(rdr);
                    dbc.InsertTable(rdt);

                }
                else
                {
                    var YHID = jsr["UserID"].ToString();
                    var oldname = dbc.ExecuteScalar("select UserName from tb_b_user where UserID='" + YHID + "'");
                    if (!jsr["UserName"].ToString().Equals(oldname.ToString()))
                    {
                        DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_user where UserName='" + jsr["UserName"].ToString() + "'");
                        if (dt_user.Rows.Count > 0)
                        {
                            throw new Exception("该用户名已存在！");
                        }
                    }
                    var dt = dbc.GetEmptyDataTable("tb_b_user");
                    var dtt = new SmartFramework4v2.Data.DataTableTracker(dt);
                    var dr = dt.NewRow();
                    dr["UserID"] = new Guid(YHID);
                    dr["UserName"] = jsr["UserName"].ToString();
                    dr["Password"] = jsr["Password"].ToString();
                    dr["UserXM"] = jsr["UserXM"].ToString();
                    dr["UserTel"] = jsr["UserTel"].ToString();
                    dt.Rows.Add(dr);
                    dbc.UpdateTable(dt, dtt);

                    //删除用户的角色关联
                    string del_js = "delete from tb_b_user_role where userId=@userId";
                    SqlCommand cmd = new SqlCommand(del_js);
                    cmd.Parameters.AddWithValue("@userId", YHID);
                    dbc.ExecuteNonQuery(cmd);

                    //建立用户角色关联
                    var rdt = dbc.GetEmptyDataTable("tb_b_user_role");
                    var rdr = rdt.NewRow();
                    rdr["userroleId"] = Guid.NewGuid().ToString();
                    rdr["userId"] = new Guid(YHID);
                    rdr["roleId"] = jsr["roleId"].ToString();
                    rdr["companyId"] = companyId;
                    rdt.Rows.Add(rdr);
                    dbc.InsertTable(rdt);
                }
                dbc.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }

    }
    //[CSMethod("SaveUser")]
    //public bool SaveUser(JSReader jsr, JSReader yhjs,JSReader yhjsdw,JSReader qxids)
    //{
    //    if (jsr["LoginName"].IsNull || jsr["LoginName"].IsEmpty)
    //    {
    //        throw new Exception("用户名不能为空");
    //    }
    //    if (jsr["Password"].IsNull || jsr["Password"].IsEmpty)
    //    {
    //        throw new Exception("密码不能为空");
    //    }

    //    if (yhjs.ToArray().Length == 0)
    //    {
    //        throw new Exception("没有用户角色！");
    //    }

    //    var EditUser = SystemUser.CurrentUser;

    //    using (DBConnection dbc = new DBConnection())
    //    {
    //        dbc.BeginTransaction();
    //        try
    //        {
    //            if (jsr["User_ID"].ToString() == "")
    //            {
    //                DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_Users where LoginName='" + jsr["LoginName"].ToString() + "' and User_DelFlag=0");
    //                if (dt_user.Rows.Count > 0)
    //                {
    //                    throw new Exception("该用户名已存在！");
    //                }

    //                var YHID = Guid.NewGuid().ToString();
    //                //建立用户
    //                string sqlStr = "";
    //                if (jsr["QY_ID"].ToString() != "")
    //                {
    //                    sqlStr = "insert into tb_b_Users (User_ID,LoginName,Password,User_DM,User_XM,User_ZW,User_DH,User_SJ,User_Email,User_DZ,User_Enable,User_DelFlag,addtime,updatetime,updateuser,QY_ID) " +
    //                        "values (@User_ID,@LoginName,@Password,@User_DM,@User_XM,@User_ZW,@User_DH,@User_SJ,@User_Email,@User_DZ,@User_Enable,@User_DelFlag,@addtime,@updatetime,@updateuser,@qyid)";
    //                }
    //                else
    //                {
    //                    sqlStr = "insert into tb_b_Users (User_ID,LoginName,Password,User_DM,User_XM,User_ZW,User_DH,User_SJ,User_Email,User_DZ,User_Enable,User_DelFlag,addtime,updatetime,updateuser) " +
    //                        "values (@User_ID,@LoginName,@Password,@User_DM,@User_XM,@User_ZW,@User_DH,@User_SJ,@User_Email,@User_DZ,@User_Enable,@User_DelFlag,@addtime,@updatetime,@updateuser)";
    //                }
    //                SqlCommand cmd = new SqlCommand(sqlStr);
    //                cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                cmd.Parameters.AddWithValue("@LoginName", jsr["LoginName"].ToString());
    //                cmd.Parameters.AddWithValue("@Password", jsr["Password"].ToString());
    //                cmd.Parameters.AddWithValue("@User_DM", jsr["User_DM"].ToString());
    //                cmd.Parameters.AddWithValue("@User_XM", jsr["User_XM"].ToString());
    //                cmd.Parameters.AddWithValue("@User_ZW", jsr["User_ZW"].ToString());
    //                cmd.Parameters.AddWithValue("@User_DH", jsr["User_DH"].ToString());
    //                cmd.Parameters.AddWithValue("@User_SJ", jsr["User_SJ"].ToString());
    //                cmd.Parameters.AddWithValue("@User_Email", jsr["User_Email"].ToString());
    //                cmd.Parameters.AddWithValue("@User_DZ", jsr["User_DZ"].ToString());
    //                if (jsr["QY_ID"].ToString() != "")
    //                {
    //                    cmd.Parameters.AddWithValue("@qyid", jsr["QY_ID"].ToString());
    //                }
    //                cmd.Parameters.AddWithValue("@User_Enable", Convert.ToInt32(jsr["User_Enable"].ToString()));
    //                cmd.Parameters.AddWithValue("@User_DelFlag", 0);
    //                cmd.Parameters.AddWithValue("@addtime", DateTime.Now);
    //                cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);

    //                dbc.ExecuteNonQuery(cmd);

    //                //建立用户角色关联
    //                for (int i = 0; i < yhjs.ToArray().Length; i++)
    //                {
    //                    string sqlstr_js = "insert into tb_b_User_JS_Gl (UserGl_id,User_ID,JS_ID,delflag,addtime,updatetime,updateuser) values(@UserGl_id,@User_ID,@JS_ID,@delflag,@addtime,@updatetime,@updateuser)";
    //                    cmd = new SqlCommand(sqlstr_js);
    //                    cmd.Parameters.AddWithValue("@UserGl_id", Guid.NewGuid());
    //                    cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                    cmd.Parameters.AddWithValue("@JS_ID", yhjs.ToArray()[i].ToString());
    //                    cmd.Parameters.AddWithValue("@delflag", 0);
    //                    cmd.Parameters.AddWithValue("@addtime", DateTime.Now);
    //                    cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                    cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);
    //                    dbc.ExecuteNonQuery(cmd);
    //                }


    //                //建立用户单位关联
    //                for (int i = 0; i < yhjsdw.ToArray().Length; i++)
    //                {
    //                    JSReader[] arr_dw = yhjsdw.ToArray()[i].ToArray();
    //                    for (int k = 0; k < arr_dw.Length; k++)
    //                    {
    //                        string sqlstr_dw = "insert into tb_b_User_Dw_Gl(UserDwGL_id,User_ID,DW_ID,delflag,addtime,updatetime,updateuser) values(@UserDwGL_id,@User_ID,@DW_ID,@delflag,@addtime,@updatetime,@updateuser)";
    //                        cmd = new SqlCommand(sqlstr_dw);
    //                        cmd.Parameters.AddWithValue("@UserDwGL_id", Guid.NewGuid());
    //                        cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                        cmd.Parameters.AddWithValue("@DW_ID", arr_dw[k].ToString());
    //                        cmd.Parameters.AddWithValue("@delflag", 0);
    //                        cmd.Parameters.AddWithValue("@addtime", DateTime.Now);
    //                        cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                        cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);
    //                        dbc.ExecuteNonQuery(cmd);
    //                    }
    //                }

    //                //建立用户权限关联
    //                for (int i = 0; i < qxids.ToArray().Length; i++)
    //                {
    //                    string sqlstr_qx = "insert into tb_b_YH_YHQX (PRIVILEGECODE,USERID) values(@PRIVILEGECODE,@USERID)";
    //                    cmd = new SqlCommand(sqlstr_qx);
    //                    cmd.Parameters.AddWithValue("@PRIVILEGECODE", new Guid(qxids.ToArray()[i]));
    //                    cmd.Parameters.AddWithValue("@USERID", YHID);
    //                    dbc.ExecuteNonQuery(cmd);
    //                }

    //            }
    //            else
    //            {
    //                var YHID = jsr["User_ID"].ToString();
    //                var oldname = dbc.ExecuteScalar("select LoginName from tb_b_Users where User_ID='" + YHID + "'");
    //                if (!jsr["LoginName"].ToString().Equals(oldname.ToString()))
    //                {
    //                    DataTable dt_user = dbc.ExecuteDataTable("select * from tb_b_Users where LoginName='" + jsr["LoginName"].ToString() + "' and User_DelFlag=0");
    //                    if (dt_user.Rows.Count > 0)
    //                    {
    //                        throw new Exception("该用户名已存在！");
    //                    }
    //                }
    //                var con = "";
    //                if (jsr["QY_ID"].ToString() != "")
    //                {
    //                    con = ",QY_ID='" + jsr["QY_ID"].ToString() + "'";
    //                }
    //                else
    //                {
    //                    con = ",QY_ID=null";
                        
    //                }
    //                string sqlstr = "update tb_b_Users set LoginName=@LoginName,Password=@Password,User_DM=@User_DM,User_XM=@User_XM,User_ZW=@User_ZW,User_DH=@User_DH,User_SJ=@User_SJ,User_Email=@User_Email,User_DZ=@User_DZ,User_Enable=@User_Enable,updatetime=@updatetime,updateuser=@updateuser " + con + " where User_ID=@User_ID";
    //                SqlCommand cmd = new SqlCommand(sqlstr);
    //                cmd.Parameters.AddWithValue("@LoginName", jsr["LoginName"].ToString());
    //                cmd.Parameters.AddWithValue("@Password", jsr["Password"].ToString());
    //                cmd.Parameters.AddWithValue("@User_DM", jsr["User_DM"].ToString());
    //                cmd.Parameters.AddWithValue("@User_XM", jsr["User_XM"].ToString());
    //                cmd.Parameters.AddWithValue("@User_ZW", jsr["User_ZW"].ToString());
    //                cmd.Parameters.AddWithValue("@User_DH", jsr["User_DH"].ToString());
    //                cmd.Parameters.AddWithValue("@User_SJ", jsr["User_SJ"].ToString());
    //                cmd.Parameters.AddWithValue("@User_Email", jsr["User_Email"].ToString());
    //                cmd.Parameters.AddWithValue("@User_DZ", jsr["User_DZ"].ToString());
    //                cmd.Parameters.AddWithValue("@User_Enable", Convert.ToInt32(jsr["User_Enable"].ToString()));
    //                cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);
    //                cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                dbc.ExecuteNonQuery(cmd);

    //                //删除用户的角色关联
    //                string del_js = "update tb_b_User_JS_Gl set delflag=1,updatetime=@updatetime,updateuser=@updateuser where User_ID=@User_ID";
    //                cmd = new SqlCommand(del_js);
    //                cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);
    //                cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                dbc.ExecuteNonQuery(cmd);
    //                //删除用户的单位关联
    //                string del_dw = "update tb_b_User_Dw_Gl set delflag=1,updatetime=@updatetime,updateuser=@updateuser where User_ID=@User_ID";
    //                cmd = new SqlCommand(del_dw);
    //                cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);
    //                cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                dbc.ExecuteNonQuery(cmd);
    //                //删除用户的权限关联
    //                string del_qx = "delete from tb_b_YH_YHQX where USERID=@USERID";
    //                cmd = new SqlCommand(del_qx);
    //                cmd.Parameters.AddWithValue("@USERID", YHID);
    //                dbc.ExecuteNonQuery(cmd);

    //                //建立用户角色关联
    //                for (int i = 0; i < yhjs.ToArray().Length; i++)
    //                {
    //                    string sqlstr_js = "insert into tb_b_User_JS_Gl (UserGl_id,User_ID,JS_ID,delflag,addtime,updatetime,updateuser) values(@UserGl_id,@User_ID,@JS_ID,@delflag,@addtime,@updatetime,@updateuser)";
    //                    cmd = new SqlCommand(sqlstr_js);
    //                    cmd.Parameters.AddWithValue("@UserGl_id", Guid.NewGuid());
    //                    cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                    cmd.Parameters.AddWithValue("@JS_ID", yhjs.ToArray()[i].ToString());
    //                    cmd.Parameters.AddWithValue("@delflag", 0);
    //                    cmd.Parameters.AddWithValue("@addtime", DateTime.Now);
    //                    cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                    cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);
    //                    dbc.ExecuteNonQuery(cmd);
    //                }


    //                //建立用户单位关联
    //                for (int i = 0; i < yhjsdw.ToArray().Length; i++)
    //                {
    //                    JSReader[] arr_dw = yhjsdw.ToArray()[i].ToArray();
    //                    for (int k = 0; k < arr_dw.Length; k++)
    //                    {
    //                        string sqlstr_dw = "insert into tb_b_User_Dw_Gl(UserDwGL_id,User_ID,DW_ID,delflag,addtime,updatetime,updateuser) values(@UserDwGL_id,@User_ID,@DW_ID,@delflag,@addtime,@updatetime,@updateuser)";
    //                        cmd = new SqlCommand(sqlstr_dw);
    //                        cmd.Parameters.AddWithValue("@UserDwGL_id", Guid.NewGuid());
    //                        cmd.Parameters.AddWithValue("@User_ID", YHID);
    //                        cmd.Parameters.AddWithValue("@DW_ID", arr_dw[k].ToString());
    //                        cmd.Parameters.AddWithValue("@delflag", 0);
    //                        cmd.Parameters.AddWithValue("@addtime", DateTime.Now);
    //                        cmd.Parameters.AddWithValue("@updatetime", DateTime.Now);
    //                        cmd.Parameters.AddWithValue("@updateuser", EditUser.UserID);
    //                        dbc.ExecuteNonQuery(cmd);
    //                    }
    //                }

    //                //建立用户权限关联
    //                for (int i = 0; i < qxids.ToArray().Length; i++)
    //                {
    //                    string sqlstr_qx = "insert into tb_b_YH_YHQX (PRIVILEGECODE,USERID) values(@PRIVILEGECODE,@USERID)";
    //                    cmd = new SqlCommand(sqlstr_qx);
    //                    cmd.Parameters.AddWithValue("@PRIVILEGECODE", new Guid(qxids.ToArray()[i]));
    //                    cmd.Parameters.AddWithValue("@USERID", YHID);
    //                    dbc.ExecuteNonQuery(cmd);
    //                }
    //            }

    //            dbc.CommitTransaction();
    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            dbc.RoolbackTransaction();
    //            throw ex;
    //        }
    //    }
    //}

    //[CSMethod("SaveUser")]
    //public bool SaveUser(JSReader jsr, string[] Privileges)
    //{
    //    if (jsr["YH_DLM"].IsNull || jsr["YH_DLM"].IsEmpty)
    //    {
    //        throw new Exception("登陆名不能为空");
    //    }
    //    if (jsr["YH_MIMA"].IsNull || jsr["YH_MIMA"].IsEmpty)
    //    {
    //        throw new Exception("密码不能为空");
    //    }
    //    if (jsr["DW_LX"].IsNull || jsr["DW_LX"].IsEmpty)
    //    {
    //        throw new Exception("单位类型不能为空");
    //    }
    //    if (jsr["DW_ID"].IsNull || jsr["DW_ID"].IsEmpty)
    //    {
    //        throw new Exception("密码不能为空");
    //    }
    //    bool UserEditSuccess = false;
    //    if (jsr["YH_ID"].ToString() == "")
    //    {
    //        UserEditSuccess = SystemUser.CreateUser(jsr["YH_DLM"].ToString(),"", jsr["YH_MIMA"].ToString(),"","","","","","");
    //    }
    //    else
    //    {
    //        UserEditSuccess = EditUser(jsr["YH_DLM"].ToString(), jsr["YH_MIMA"].ToString(), jsr["DW_ID"].ToString(), jsr["YH_ID"].ToString());
    //    }
    //    if (UserEditSuccess)
    //    {

    //        if (jsr["DW_LX"].ToString() == "0" && Privileges.Length > 0)
    //        {
    //            try
    //            {
    //                SystemUser.GetUserByUserName(jsr["YH_DLM"].ToString()).RemoveAllPriviledge();
    //                foreach (string Privilege in Privileges)
    //                {
    //                    SystemUser.GetUserByUserName(jsr["YH_DLM"].ToString()).AddPriviledge(new Guid(Privilege));
    //                }
    //                UserEditSuccess = true;
    //            }
    //            catch
    //            {
    //                UserEditSuccess = false;
    //            }
    //        }
    //    }
    //    return UserEditSuccess;
    //}

    //[CSMethod("DelUser")]
    //public bool DelUser(string userid)
    //{
    //    if (userid.Trim() == "")
    //        return false;
    //    using (DBConnection dbc = new DBConnection())
    //    {
    //        int retInt = dbc.ExecuteNonQuery("update tb_b_Users set User_DelFlag = 1 where user_id = '" + userid + "'");
    //        if (retInt > 0)
    //            return true;
    //        return false;
    //    }
    //}

    [CSMethod("DelUser")]
    public bool DelUser(JSReader jsr)
    {
        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    string delstr = "delete from tb_b_user_role where userId=@userId";
                    SqlCommand cmd = new SqlCommand(delstr);
                    cmd.Parameters.AddWithValue("@userId", jsr.ToArray()[i].ToString());
                    dbc.ExecuteNonQuery(cmd);

                    string str = "delete from tb_b_user where UserID=@UserID";
                    SqlCommand ucmd = new SqlCommand(str);
                    ucmd.Parameters.AddWithValue("@UserID", jsr.ToArray()[i].ToString());
                    dbc.ExecuteNonQuery(ucmd);
                }

                dbc.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
        }
    }


    [CSMethod("DelUserByids")]
    public bool DelUserByids(JSReader jsr)
    {
        var user = SystemUser.CurrentUser;
        string userid = user.UserID;

        using (DBConnection dbc = new DBConnection())
        {
            dbc.BeginTransaction();
            try
            {
                for (int i = 0; i < jsr.ToArray().Length; i++)
                {
                    string str = "update tb_b_Users set User_DelFlag = 1,Updatetime=@Updatetime,Updateuser=@Updateuser where user_id =@user_id";
                    SqlCommand cmd = new SqlCommand(str);
                    cmd.Parameters.AddWithValue("@Updatetime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Updateuser", userid);
                    cmd.Parameters.AddWithValue("@user_id", jsr.ToArray()[i].ToString());
                    dbc.ExecuteNonQuery(cmd);
                    
                }

                dbc.CommitTransaction();
                return true;
            }
            catch (Exception ex)
            {
                dbc.RoolbackTransaction();
                throw ex;
            }
           
        }
    }

    [CSMethod("EnableUser")]
    public bool EnableUser(string[] userIds, bool enable)
    {
        if (userIds.Length > 0)
        {
            string userIdStr = "'" + string.Join(",", userIds).Replace(",", "','") + "'";
            var sqlStr = string.Format("update tb_b_Users set User_Enable = @YH_ENABLE where User_ID IN({0})", userIdStr);
            SqlCommand cmd = new SqlCommand(sqlStr);
            cmd.Parameters.AddWithValue("@YH_ENABLE", enable ? 0 : 1);
            using (DBConnection dbc = new DBConnection())
            {
                var retInt = dbc.ExecuteNonQuery(cmd);
                if (retInt > 0)
                    return true;
                return false;
            }
        }
        return false;
    }
    private bool EditUser(string dlm, string mima, string dwid, string userid)
    {
        string sqlStr = "update tzclz_t_yh set yh_dlm=:yh_dlm,yh_mima = :yh_mima,updatetime = sysdate,dw_id = :dw_id,XGYH_ID = :XGYH_ID where YH_ID = :YH_ID and status = 0";
        SqlCommand cmd = new SqlCommand(sqlStr);
        cmd.Parameters.AddWithValue("yh_dlm", dlm);
        cmd.Parameters.AddWithValue("yh_mima", mima);
        cmd.Parameters.AddWithValue("dw_id", dwid);
        cmd.Parameters.AddWithValue("YH_ID", userid);
        cmd.Parameters.AddWithValue("XGYH_ID", SystemUser.CurrentUser.UserID);
        using (DBConnection dbc = new DBConnection())
        {
            int retInt = dbc.ExecuteNonQuery(cmd);
            if (retInt > 0)
                return true;
            return false;
        }
    }
    [CSMethod("GetAllPrivilege")]
    public object GetAllPrivilege()
    {
        var Mod = PrivilegeDescription.PrivilegeType();

        using (DBConnection dbc = new DBConnection())
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" select t.orderno,");
            sb.Append(" substr(ModuleName, 0, instr(ModuleName, '-') -1) as Mod,");
            sb.Append(" substr(ModuleName,");
            sb.Append(" instr(ModuleName, '-') + 1,");
            sb.Append(" (length(ModuleName) - instr(ModuleName, '-'))) as Item");
            sb.Append(" ,t.privilegecode");
            sb.Append(" from TZCLZ_T_YH_QX t");
            var Items = dbc.ExecuteDataTable(sb.ToString());
            return new { Mod = Mod, Items = Items };
        }
    }
    [CSMethod("GetUserPrivileges")]
    public string[] GetAllPrivilege(string userid)
    {
        List<string> Privileges = new List<string>();

        var dtPrivilege = SystemUser.GetUserByID(userid).GetUserPriviledgeInfo();
        foreach (DataRow drPrivilege in dtPrivilege.Rows)
        {
            Privileges.Add(drPrivilege["PRIVILEGECODE"].ToString());
        }
        return Privileges.ToArray();
    }

}
