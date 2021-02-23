using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using iCore_Customer.Modules;
using iCore_Customer.Modules.SecurityAuthentication;
using RestSharp;

namespace iCore_Customer.Areas.SPF.Controllers
{
    public class HController : Controller
    {
        //====================================================================================================================
        iCore_Administrator.Modules.AuthenticationTester AAuth = new iCore_Administrator.Modules.AuthenticationTester();
        iCore_Administrator.Modules.SQL_Tranceiver Sq = new iCore_Administrator.Modules.SQL_Tranceiver();
        iCore_Administrator.Modules.PublicFunctions Pb = new iCore_Administrator.Modules.PublicFunctions();
        //====================================================================================================================
        public ActionResult Index() { return Redirect(Url.Content("~/")); }
        //====================================================================================================================
        public ActionResult RGF()
        {
            try
            {
                string Form_UnicID = Url.RequestContext.RouteData.Values["id"].ToString().Trim();
                DataTable DT_Form = new DataTable();
                DT_Form = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select ID,User_ID,UnicID,Name,Template_Code,Template_Name,Customer_Side_URL,Customer_Side_iFrame From Users_04_Hospitality_SingleUser_RegisterForms Where (Customer_Side_URL = '" + Form_UnicID + "') And (Status_Code = '1') And (Removed = '0')");
                if (DT_Form.Rows != null)
                {
                    if (DT_Form.Rows.Count == 1)
                    {
                        var DomainURL = $"{Request.Url.GetLeftPart(UriPartial.Authority)}{Url.Content("~/")}";
                        DataTable DT_User_Config = new DataTable();
                        DT_User_Config = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select Header1,Header2,Footer1,Footer2,Domain_Address,Domain_Name From Users_03_Hospitality_SingleUser_BasicSetting Where (User_ID = '" + DT_Form.Rows[0][1].ToString().Trim() + "')");
                        if (DT_User_Config.Rows != null)
                        {
                            if (DT_User_Config.Rows.Count == 1)
                            {
                                DataTable DT_User = new DataTable();
                                DT_User = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select ID From Users_02_SingleUser Where (ID = '" + DT_Form.Rows[0][1].ToString().Trim() + "') And (Status_Code = '1') And (Removed = '0')");
                                if (DT_User.Rows != null)
                                {
                                    if (DT_User.Rows.Count == 1)
                                    {
                                        DomainURL = DomainURL.ToLower().Trim();
                                        string CallURL = DT_User_Config.Rows[0][4].ToString().Trim().ToLower();
                                        if (DomainURL.Substring(DomainURL.Length - 1, 1) != "/") { DomainURL = DomainURL + "/"; }
                                        if (CallURL.Substring(CallURL.Length - 1, 1) != "/") { CallURL = CallURL + "/"; }
                                        if (DomainURL == CallURL)
                                        {
                                            ViewBag.Header1 = DT_User_Config.Rows[0][0].ToString().Trim();
                                            ViewBag.Header2 = DT_User_Config.Rows[0][1].ToString().Trim();
                                            ViewBag.Footer1 = DT_User_Config.Rows[0][2].ToString().Trim();
                                            ViewBag.Footer2 = DT_User_Config.Rows[0][3].ToString().Trim();
                                            ViewBag.TemplateCode = DT_Form.Rows[0][4].ToString().Trim();
                                            ViewBag.FormName_Title = DT_Form.Rows[0][3].ToString().Trim();
                                            if(DT_User_Config.Rows[0][5].ToString().Trim()!="") { ViewBag.MasterUserWebsiteUrl = DT_User_Config.Rows[0][5].ToString().Trim(); } else { ViewBag.MasterUserWebsiteUrl = "#"; }
                                            string API_SK = System.Configuration.ConfigurationManager.AppSettings["iCore_API_SecretKey"];
                                            string iCore_User_URL = System.Configuration.ConfigurationManager.AppSettings["iCore_User_URL"];
                                            var client = new RestClient(iCore_User_URL + "/UIMG/ULF/" + DT_User.Rows[0][0].ToString().Trim() + "$" + API_SK);
                                            client.Timeout = -1;
                                            var request = new RestRequest(Method.GET);
                                            byte[] response = client.DownloadData(request);
                                            var base64 = Convert.ToBase64String(response);
                                            string IMGSrc = String.Format("data:image/png;base64,{0}", base64);
                                            ViewBag.LogoData = IMGSrc;
                                            ViewBag.iFrame_Src = "";
                                            string SK = System.Configuration.ConfigurationManager.AppSettings["iCore_API_SecretKey"];
                                            ViewBag.iFrame_Src = Url.Action("RCF", "HSU", new { id= DT_Form.Rows[0][7].ToString().Trim() + "$" + SK, area="" });
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
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                }
                return View();
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
        }
        //====================================================================================================================
        public ActionResult RGFIF()
        {
            try
            {
                string Form_UnicID = Url.RequestContext.RouteData.Values["id"].ToString().Trim();
                DataTable DT_Form = new DataTable();
                DT_Form = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select ID,User_ID,UnicID,Name,Template_Code,Template_Name,Customer_Side_URL,Customer_Side_iFrame From Users_04_Hospitality_SingleUser_RegisterForms Where (Customer_Side_iFrame = '" + Form_UnicID + "') And (Status_Code = '1') And (Removed = '0')");
                if (DT_Form.Rows != null)
                {
                    if (DT_Form.Rows.Count == 1)
                    {
                        var DomainURL = $"{Request.Url.GetLeftPart(UriPartial.Authority)}{Url.Content("~/")}";
                        DataTable DT_User_Config = new DataTable();
                        DT_User_Config = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select Header1,Header2,Footer1,Footer2,Domain_Address,Domain_Name From Users_03_Hospitality_SingleUser_BasicSetting Where (User_ID = '" + DT_Form.Rows[0][1].ToString().Trim() + "')");
                        if (DT_User_Config.Rows != null)
                        {
                            if (DT_User_Config.Rows.Count == 1)
                            {
                                DataTable DT_User = new DataTable();
                                DT_User = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select ID From Users_02_SingleUser Where (ID = '" + DT_Form.Rows[0][1].ToString().Trim() + "') And (Status_Code = '1') And (Removed = '0')");
                                if (DT_User.Rows != null)
                                {
                                    if (DT_User.Rows.Count == 1)
                                    {
                                        DomainURL = DomainURL.ToLower().Trim();
                                        string CallURL = DT_User_Config.Rows[0][4].ToString().Trim().ToLower();
                                        if (DomainURL.Substring(DomainURL.Length - 1, 1) != "/") { DomainURL = DomainURL + "/"; }
                                        if (CallURL.Substring(CallURL.Length - 1, 1) != "/") { CallURL = CallURL + "/"; }
                                        if (DomainURL == CallURL)
                                        {
                                            string SK = System.Configuration.ConfigurationManager.AppSettings["iCore_API_SecretKey"];
                                            return RedirectToAction("RCF", "HSU", new { id = DT_Form.Rows[0][7].ToString().Trim() + "$" + SK, area = "" });
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
    }
}