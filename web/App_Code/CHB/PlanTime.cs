using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using SmartFramework4v2.Web.WebExecutor;
using SmartFramework4v2.Data.SqlServer;
using System.Data.SqlClient;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Net;
using System.Text;
using System.Diagnostics;
using SmartFramework4v2.Data;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using System.Net.Sockets;
using System.Web.Script.Serialization;

/// <summary>
/// PlanTime 的摘要说明
/// </summary>
public class PlanTime : Registry
{
    public PlanTime()
    {
        // Schedule an IJob to run at an interval
        // 立即执行每两秒一次的计划任务。（指定一个时间间隔运行，根据自己需求，可以是秒、分、时、天、月、年等。）
        Schedule<MyJob>().ToRunNow().AndEvery(1).Minutes();

        // Schedule an IJob to run once, delayed by a specific time interval
        // 延迟一个指定时间间隔执行一次计划任务。（当然，这个间隔依然可以是秒、分、时、天、月、年等。）
        //Schedule<MyJob>().ToRunOnceIn(5).Seconds();

        // Schedule a simple job to run at a specific time
        // 在一个指定时间执行计划任务（最常用。这里是在每天的下午 1:10 分执行）
        //Schedule(() => Trace.WriteLine("It‘s 1:10 PM now.")).ToRunEvery(1).Days().At(13, 10);

        //Schedule(() =>
        //{
        //    // 做你想做的事儿。
        //    Trace.WriteLine("It‘s 1:10 PM now.");

        //}).ToRunEvery(1).Days().At(13, 10);

        // Schedule a more complex action to run immediately and on an monthly interval
        // 立即执行一个在每月的星期一 3:00 的计划任务（可以看出来这个一个比较复杂点的时间，它意思是它也能做到！）
        //Schedule<MyComplexJob>().ToRunNow().AndEvery(1).Months().OnTheFirst(DayOfWeek.Monday).At(3, 0);

        // Schedule multiple jobs to be run in a single schedule
        // 在同一个计划中执行两个（多个）任务
        //Schedule<MyJob>().AndThen<MyOtherJob>().ToRunNow().AndEvery(5).Minutes();

    }
}

public class MyJob : IJob
{

    void IJob.Execute()
    {
        using (var db = new DBConnection())
        {
            try
            {
                db.BeginTransaction();

                string sql = "select OrderID,PlatToSaleId,ZhiFuZT,AddTime,Points from tb_b_order where status = 0 and ZhiFuZT = 0 and SXZT = 0";
                DataTable dt_order = db.ExecuteDataTable(sql);

                //sql = "select PlatToSaleId,points from tb_b_plattosale where PlatToSaleId in (select PlatToSaleId from tb_b_order where status = 0 and ZhiFuZT = 0)";
                //DataTable dt_plat = db.ExecuteDataTable(sql);

                //DataTable dt_update = db.GetEmptyDataTable("tb_b_plattosale");
                //DataTableTracker dtt_update = new DataTableTracker(dt_update);

                DataTable dt_order_update = db.GetEmptyDataTable("tb_b_order");
                DataTableTracker dtt_order_update = new DataTableTracker(dt_order_update);

                Hashtable ht = new Hashtable();

                for (var i = 0; i < dt_order.Rows.Count; i++)
                {
                    if ((DateTime.Now - Convert.ToDateTime(dt_order.Rows[i]["AddTime"].ToString())).TotalSeconds > 180)
                    {
                        DataRow dr_order = dt_order_update.NewRow();
                        dr_order["OrderID"] = dt_order.Rows[i]["OrderID"];
                        dr_order["Status"] = 1;
                        dt_order_update.Rows.Add(dr_order);

                        sql = "select PlatToSaleId,points from tb_b_plattosale where PlatToSaleId = '" + dt_order.Rows[i]["PlatToSaleId"].ToString() + "'";
                        DataTable dt_plat = db.ExecuteDataTable(sql);

                        if (dt_plat.Rows.Count > 0)
                        {
                            sql = "update tb_b_plattosale set points = '" + Convert.ToInt32(Convert.ToInt32(dt_order.Rows[i]["Points"]) + Convert.ToInt32(dt_plat.Rows[0]["points"])) + "' where PlatToSaleId = '" + dt_order.Rows[i]["PlatToSaleId"].ToString() + "'";
                            db.ExecuteNonQuery(sql);
                        }
                        //DataRow[] drs = dt_plat.Select("PlatToSaleId = '" + dt_order.Rows[i]["PlatToSaleId"].ToString() + "'");
                        //if (drs.Length > 0)
                        //{

                        //    DataRow dr = dt_update.NewRow();
                        //    dr["PlatToSaleId"] = dt_order.Rows[i]["PlatToSaleId"];
                        //    dr["points"] = Convert.ToInt32(dt_order.Rows[i]["Points"]) + Convert.ToInt32(drs[0]["points"]);
                        //    dt_update.Rows.Add(dr);

                        //    drs[0]["points"] = Convert.ToInt32(Convert.ToInt32(dt_order.Rows[i]["Points"]) + Convert.ToInt32(drs[0]["points"]));
                        //}
                    }
                }
                //db.UpdateTable(dt_update, dtt_update);
                sql = "select OrderID,PlatToSaleId,ZhiFuZT,AddTime,Points from tb_b_order where status = 0 and ZhiFuZT = 0 and SXZT = 1";
                DataTable dt_order_sx = db.ExecuteDataTable(sql);
                for (var i = 0; i < dt_order_sx.Rows.Count; i++)
                {
                    if ((DateTime.Now - Convert.ToDateTime(dt_order_sx.Rows[i]["AddTime"].ToString())).TotalSeconds > 240)
                    {
                        DataRow dr_order = dt_order_update.NewRow();
                        dr_order["OrderID"] = dt_order_sx.Rows[i]["OrderID"];
                        dr_order["Status"] = 1;
                        dt_order_update.Rows.Add(dr_order);

                        sql = "select PlatToSaleId,points from tb_b_plattosale where PlatToSaleId = '" + dt_order_sx.Rows[i]["PlatToSaleId"].ToString() + "'";
                        DataTable dt_plat = db.ExecuteDataTable(sql);

                        if (dt_plat.Rows.Count > 0)
                        {
                            sql = "update tb_b_plattosale set points = '" + Convert.ToInt32(Convert.ToInt32(dt_order_sx.Rows[i]["Points"]) + Convert.ToInt32(dt_plat.Rows[0]["points"])) + "' where PlatToSaleId = '" + dt_order_sx.Rows[i]["PlatToSaleId"].ToString() + "'";
                            db.ExecuteNonQuery(sql);
                        }
                    }
                }

                db.UpdateTable(dt_order_update, dtt_order_update);
                db.CommitTransaction();
            }
            catch (Exception ex)
            {
                db.RoolbackTransaction();
                throw ex;
            }
        }

    }
}

public class MyOtherJob : IJob
{

    void IJob.Execute()
    {
        Trace.WriteLine("这是另一个 Job ，现在时间是：" + DateTime.Now);
    }
}

public class MyComplexJob : IJob
{

    void IJob.Execute()
    {
        Trace.WriteLine("这是比较复杂的 Job ，现在时间是：" + DateTime.Now);
    }
}