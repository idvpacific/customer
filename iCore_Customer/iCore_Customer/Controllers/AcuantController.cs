using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using iCore_Customer.Modules;
using iCore_Customer.Modules.SecurityAuthentication;

namespace iCore_Customer.Areas.ManagementPortal.Controllers
{
    public class AcuantController : Controller
    {
        //====================================================================================================================
        iCore_Administrator.Modules.AuthenticationTester AAuth = new iCore_Administrator.Modules.AuthenticationTester();
        iCore_Administrator.Modules.SQL_Tranceiver Sq = new iCore_Administrator.Modules.SQL_Tranceiver();
        iCore_Administrator.Modules.PublicFunctions Pb = new iCore_Administrator.Modules.PublicFunctions();
        //====================================================================================================================
        public ActionResult Index() { return RedirectToAction("Index", "Dashboard", new { id = "", area = "ManagementPortal" }); }
        //====================================================================================================================
        [HttpGet]
        public ActionResult GIC()
        {
            try
            {
                DataTable DT = new DataTable();
                DT = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select Top(1) * From Setting_Basic_01_Acuant");
                ViewBag.Configuration = DT.Rows[0];
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "", area = "ManagementPortal" });
            }
        }
        //====================================================================================================================
        [HttpGet]
        public ActionResult LNT()
        {
            try
            {
                DataTable DT = new DataTable();
                DT = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select Top(1) * From Setting_Basic_01_Acuant");
                ViewBag.Configuration = DT.Rows[0];
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Dashboard", new { id = "", area = "ManagementPortal" });
            }
        }
        //====================================================================================================================
        [HttpPost]
        public void GICLOG(string A1, string A2, string A3,string A4,string A5, HttpPostedFileBase A6)
        {
            try
            {
                A1 = A1.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                A2 = A2.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                A3 = A3.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                A4 = A4.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                A5 = A5.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                string InsDate = Sq.Sql_Date();
                string InsTime = Sq.Sql_Time();
                Stream fs = A6.InputStream;
                BinaryReader br = new BinaryReader(fs);
                byte[] PIMG = br.ReadBytes((Int32)fs.Length);
                SqlConnection CN = new SqlConnection();
                CN = Sq.Get_Sql_Connection(iCore_Administrator.Modules.DataBase_Selector.Administrator);
                string qry = "Insert Into Log_01_Guidedimagecapture values (@User_ID,@Form_ID,@Relation_ID,@Customer_IP,@BrowserName,@InsDate,@InsTime,@Result_IMG)";
                SqlCommand SqlCom = new SqlCommand(qry, CN);
                SqlCom.Parameters.Add(new SqlParameter("@User_ID", A1));
                SqlCom.Parameters.Add(new SqlParameter("@Form_ID", A2));
                SqlCom.Parameters.Add(new SqlParameter("@Relation_ID", A3));
                SqlCom.Parameters.Add(new SqlParameter("@Customer_IP", A4));
                SqlCom.Parameters.Add(new SqlParameter("@BrowserName", A5));
                SqlCom.Parameters.Add(new SqlParameter("@InsDate", InsDate));
                SqlCom.Parameters.Add(new SqlParameter("@InsTime", InsTime));
                SqlCom.Parameters.Add(new SqlParameter("@Result_IMG", (object)PIMG));
                CN.Open();
                try { SqlCom.ExecuteNonQuery(); } catch (Exception) { }
                CN.Close();
            }
            catch (Exception) { }
        }
        //====================================================================================================================
        [HttpPost]
        public void LNTLOG(string A1, string A2, string A3, string A4, string A5, string A6, string A7, string A8, string A9, string A10, string A11)
        {
            try
            {
                A1 = A1.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                A2 = A2.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                A3 = A3.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                A4 = A4.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                A5 = A5.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                A6 = A6.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                A7 = A7.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                A8 = A8.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                A9 = A9.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                A10 = A10.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                A11 = A11.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                string InsDate = Sq.Sql_Date();
                string InsTime = Sq.Sql_Time();
                Sq.Execute_TSql(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Insert Into Log_02_Livenesstest Values ('" +A1  + "','" + A2 + "','" +A3  + "','" +A4 + "','" +A5 +"','" + InsDate + "','" + InsTime + "','" +A6  + "','" +A7  + "','" +A8  + "','" +A9  + "','" +A10  + "','" +A11  +"')");
            }
            catch (Exception) { }
        }
        //====================================================================================================================
    }
}