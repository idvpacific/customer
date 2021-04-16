using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using iCore_Customer.Modules;
using iCore_Customer.Modules.SecurityAuthentication;
using RestSharp;

namespace iCore_Customer.Controllers
{
    public class MobileController : Controller
    {
        //====================================================================================================================
        iCore_Administrator.Modules.SQL_Tranceiver Sq = new iCore_Administrator.Modules.SQL_Tranceiver();
        iCore_Administrator.Modules.PublicFunctions Pb = new iCore_Administrator.Modules.PublicFunctions();
        //====================================================================================================================
        //====================================================================================================================
        [HttpGet]
        public ActionResult Index() { return new HttpStatusCodeResult(HttpStatusCode.NotFound); }
        //====================================================================================================================
        [HttpGet]
        public ActionResult Capturegic()
        {
            try
            {
                if (Pb.isMobileBrowser() == false) { return new HttpStatusCodeResult(HttpStatusCode.NotFound); }
                string Form_UID = Url.RequestContext.RouteData.Values["id"].ToString().Trim();
                DataTable DT_QRLink = new DataTable();
                var DomainURL = $"{Request.Url.GetLeftPart(UriPartial.Authority)}{Url.Content("~/")}";
                if (DomainURL.Substring(DomainURL.Length - 1, 1) != "/") { DomainURL = DomainURL + "/"; }
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-AU");
                DateTime DTNow = DateTime.Now;
                DateTime DTSB = DTNow.AddMinutes(-30);
                string DTSBLST = DTSB.ToString("HH:mm:ss");
                DT_QRLink = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select LinkID From Users_13_Hospitality_SingleUser_Application_QR_Links Where (UID = '" + Form_UID + "') And (Request_URL = '" + DomainURL + "') And (ActiveLink_Date = '" + Sq.Sql_Date() + "') And (ActiveLink_Time >= '" + DTSBLST + "')");
                if (DT_QRLink != null)
                {
                    if (DT_QRLink.Rows != null)
                    {
                        if (DT_QRLink.Rows.Count == 1)
                        {
                            DataTable DT = new DataTable();
                            DT = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select Top(1) * From Setting_Basic_01_Acuant");
                            ViewBag.Configuration = DT.Rows[0];
                            ViewBag.UID = Form_UID;
                            ViewBag.DomainURL = DomainURL;
                            return View();
                        }
                        else
                        {
                            return RedirectToAction("QR_Invalid", "ErrorPage", new { id = "" });
                        }
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
        }
        //====================================================================================================================
        [HttpGet]
        public ActionResult Capturelnt()
        {
            try
            {
                if (Pb.isMobileBrowser() == false) { return new HttpStatusCodeResult(HttpStatusCode.NotFound); }
                string Form_UID = Url.RequestContext.RouteData.Values["id"].ToString().Trim();
                DataTable DT_QRLink = new DataTable();
                var DomainURL = $"{Request.Url.GetLeftPart(UriPartial.Authority)}{Url.Content("~/")}";
                if (DomainURL.Substring(DomainURL.Length - 1, 1) != "/") { DomainURL = DomainURL + "/"; }
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-AU");
                DateTime DTNow = DateTime.Now;
                DateTime DTSB = DTNow.AddMinutes(-30);
                string DTSBLST = DTSB.ToString("HH:mm:ss");
                DT_QRLink = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select * From Users_13_Hospitality_SingleUser_Application_QR_Links Where (UID = '" + Form_UID + "') And (Request_URL = '" + DomainURL + "') And (ActiveLink_Date = '" + Sq.Sql_Date() + "') And (ActiveLink_Time >= '" + DTSBLST + "')");
                if (DT_QRLink != null)
                {
                    if (DT_QRLink.Rows != null)
                    {
                        if (DT_QRLink.Rows.Count == 1)
                        {
                            DataTable DT = new DataTable();
                            DT = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select Top(1) * From Setting_Basic_01_Acuant");
                            ViewBag.Configuration = DT.Rows[0];
                            ViewBag.UID = Form_UID;
                            ViewBag.DomainURL = DomainURL;
                            return View();
                        }
                        else
                        {
                            return RedirectToAction("QR_Invalid", "ErrorPage", new { id = "" });
                        }
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                    }
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
        }
        //====================================================================================================================
        [HttpPost]
        public void HSU_Form_Upload_Value(string UID, string URL, string UIP, string UBN, string O1, string O2, string O3, string O4, string O5, string O6, string O7, HttpPostedFileBase UF)
        {
            try
            {
                O1 = O1.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                O2 = O2.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                O3 = O3.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                O4 = O4.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                O5 = O5.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                O6 = O6.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                O7 = O7.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                URL = URL.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                UIP = UIP.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                UBN = UBN.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                bool FileReady = false;
                try { if (UF != null) { FileReady = true; } } catch (Exception) { }
                if (FileReady == true)
                {
                    string BasPath = Server.MapPath("~/Drive/Hospitality/QRUpload/" + UID + ".jpg");
                    UF.SaveAs(BasPath);
                }
                Sq.Execute_TSql(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Update Users_13_Hospitality_SingleUser_Application_QR_Links Set [Customer_IP] = '" + UIP + "',[Customer_Browser] = '" + UBN + "',[Data_Change] = '1',[Data_Read] = '0',[Feeback_D1] = '" + O1 + "',[Feeback_D2] = '" + O2 + "',[Feeback_D3] = '" + O3 + "',[Feeback_D4] = '" + O4 + "',[Feeback_D5] = '" + O5 + "',[Feeback_D6] = '" + O6 + "',[Feeback_D7] = '" + O7 + "' Where (UID = '" + UID + "') And (Request_URL = '" + URL + "')");
            }
            catch (Exception) { }
        }
        //====================================================================================================================
    }
}