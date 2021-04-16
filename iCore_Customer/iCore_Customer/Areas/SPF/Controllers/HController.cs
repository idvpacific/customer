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
        [Route("SPF_SPF")]
        public ActionResult RGF()
        {
            try
            {
                ViewBag.SectionAsPage = "0";
                string Form_UnicID = Url.RequestContext.RouteData.Values["id"].ToString().Trim();
                string Parameters = "";
                try { Parameters = Url.RequestContext.RouteData.Values["param"].ToString().Trim(); }
                catch (Exception) { }
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
                                            string[,] Param_Define = new string[,] { { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" } };
                                            DataTable DT_Parameter = new DataTable();
                                            DT_Parameter = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select URL_01_Status,URL_01_Mandatory,URL_01_Key,URL_01_Value,URL_02_Status,URL_02_Mandatory,URL_02_Key,URL_02_Value,URL_03_Status,URL_03_Mandatory,URL_03_Key,URL_03_Value,URL_04_Status,URL_04_Mandatory,URL_04_Key,URL_04_Value,URL_05_Status,URL_05_Mandatory,URL_05_Key,URL_05_Value,URL_06_Status,URL_06_Mandatory,URL_06_Key,URL_06_Value,URL_07_Status,URL_07_Mandatory,URL_07_Key,URL_07_Value,URL_08_Status,URL_08_Mandatory,URL_08_Key,URL_08_Value,URL_09_Status,URL_09_Mandatory,URL_09_Key,URL_09_Value,URL_10_Status,URL_10_Mandatory,URL_10_Key,URL_10_Value,URL_Parameter_Error_URL,ShowAsPage From Users_07_Hospitality_SingleUser_RegisterForms_Configuration Where (Form_ID = '" + DT_Form.Rows[0][0].ToString().Trim() + "') And (User_ID = '" + DT_Form.Rows[0][1].ToString().Trim() + "')");
                                            if (DT_Parameter.Rows.Count == 1)
                                            {
                                                string[] Param_Define_Status = new string[] { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };
                                                string[] Param_Define_Mandatory = new string[] { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" };
                                                string[] Param_Define_Key = new string[] { "", "", "", "", "", "", "", "", "", "" };
                                                string[] Param_Define_PreValue = new string[] { "", "", "", "", "", "", "", "", "", "" };
                                                int Param_Counter = 0;
                                                for (int i = 0; i < 40; i = i + 4)
                                                {
                                                    Param_Define_Status[Param_Counter] = DT_Parameter.Rows[0][i].ToString().Trim();
                                                    Param_Define_Mandatory[Param_Counter] = DT_Parameter.Rows[0][i + 1].ToString().Trim();
                                                    Param_Define_Key[Param_Counter] = DT_Parameter.Rows[0][i + 2].ToString().Trim();
                                                    Param_Define_PreValue[Param_Counter] = DT_Parameter.Rows[0][i + 3].ToString().Trim();
                                                    Param_Counter++;
                                                }
                                                Param_Counter = 0;
                                                string[] Parameter_Seprate = Parameters.Split('^');
                                                for (int i = 0; i < 10; i++)
                                                {
                                                    if (Param_Define_Status[i] == "1")
                                                    {
                                                        if (Param_Define_Key[i].Trim() != "")
                                                        {
                                                            Param_Define[i, 0] = Param_Define_Key[i].Trim();
                                                            Param_Define[i, 1] = "";
                                                            try
                                                            {
                                                                for (int j = 0; j < Parameter_Seprate.Length; j++)
                                                                {
                                                                    string[] Parameter_Seprate_Sec = Parameter_Seprate[j].Split('=');
                                                                    if (Parameter_Seprate_Sec[0].Trim().ToUpper() == Param_Define[i, 0].Trim().ToUpper())
                                                                    {
                                                                        Param_Define[i, 1] = Parameter_Seprate_Sec[1].Trim();
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                            catch (Exception) { }
                                                            if (Param_Define_Mandatory[i] == "1")
                                                            {
                                                                if (Param_Define[i, 1].Trim() == "") { return new HttpStatusCodeResult(HttpStatusCode.NotFound); }
                                                            }
                                                            else
                                                            {
                                                                if (Param_Define[i, 1].Trim() == "") { Param_Define[i, 1] = Param_Define_PreValue[i].Trim(); }
                                                            }
                                                        }
                                                    }
                                                }
                                                ViewBag.SectionAsPage = DT_Parameter.Rows[0][41].ToString().Trim();
                                            }
                                            string ParameterValue = "";
                                            for (int i = 0; i < 10; i++)
                                            {
                                                if (Param_Define[i, 0].Trim() != "")
                                                {
                                                    ParameterValue += Param_Define[i, 0].Trim() + "=" + Param_Define[i, 1].Trim() + "^";
                                                }
                                            }
                                            ParameterValue = ParameterValue.Trim();
                                            if (ParameterValue.Length > 0)
                                            {
                                                ParameterValue = ParameterValue.Replace("^^", "^");
                                                ParameterValue = ParameterValue.Replace("^^", "^");
                                                ParameterValue = ParameterValue.Trim();
                                                if (ParameterValue.Length > 0)
                                                {
                                                    if (ParameterValue != "^")
                                                    {
                                                        if (ParameterValue.Substring(ParameterValue.Length - 1, 1) == "^")
                                                        {
                                                            ParameterValue = ParameterValue.Substring(0, ParameterValue.Length - 1);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        ParameterValue = "";
                                                    }
                                                }
                                            }
                                            ParameterValue = ParameterValue.Trim();
                                            ViewBag.ParameterValue = ParameterValue;
                                            // Redirect :
                                            string Redirect_AfterSubmit = "0";
                                            string Redirect_Auto = "0";
                                            string Redirect_BtnCaption = "";
                                            string Redirect_URL = "";
                                            try
                                            {
                                                DataTable DT_Redirect = new DataTable();
                                                DT_Redirect = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select Redirect_Submit,Redirect_Auto,Redirect_Caption,Redirect_URL From Users_07_Hospitality_SingleUser_RegisterForms_Configuration Where (Form_ID = '" + DT_Form.Rows[0][0].ToString().Trim() + "') And (User_ID = '" + DT_Form.Rows[0][1].ToString().Trim() + "')");
                                                if (DT_Redirect.Rows.Count == 1)
                                                {
                                                    Redirect_AfterSubmit = DT_Redirect.Rows[0][0].ToString().Trim();
                                                    Redirect_Auto = DT_Redirect.Rows[0][1].ToString().Trim();
                                                    Redirect_BtnCaption = DT_Redirect.Rows[0][2].ToString().Trim();
                                                    Redirect_URL = DT_Redirect.Rows[0][3].ToString().Trim();
                                                    if (Redirect_URL.Trim() != "")
                                                    {
                                                        for (int i = 0; i < 10; i++)
                                                        {
                                                            if (Param_Define[i, 0].Trim() != "")
                                                            {
                                                                Redirect_URL = Redirect_URL.Replace("{" + Param_Define[i, 0].Trim() + "}", Param_Define[i, 1].Trim());
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Redirect_AfterSubmit = "0";
                                                        Redirect_Auto = "0";
                                                    }
                                                }
                                            }
                                            catch (Exception)
                                            { }
                                            ViewBag.Redirect_AfterSubmit = Redirect_AfterSubmit;
                                            ViewBag.Redirect_Auto = Redirect_Auto;
                                            ViewBag.Redirect_BtnCaption = Redirect_BtnCaption;
                                            ViewBag.Redirect_URL = Redirect_URL;
                                            ViewBag.Header1 = DT_User_Config.Rows[0][0].ToString().Trim();
                                            ViewBag.Header2 = DT_User_Config.Rows[0][1].ToString().Trim();
                                            ViewBag.Footer1 = DT_User_Config.Rows[0][2].ToString().Trim();
                                            ViewBag.Footer2 = DT_User_Config.Rows[0][3].ToString().Trim();
                                            ViewBag.TemplateCode = DT_Form.Rows[0][4].ToString().Trim();
                                            ViewBag.FormName_Title = DT_Form.Rows[0][3].ToString().Trim();
                                            if (DT_User_Config.Rows[0][5].ToString().Trim() != "") { ViewBag.MasterUserWebsiteUrl = DT_User_Config.Rows[0][5].ToString().Trim(); } else { ViewBag.MasterUserWebsiteUrl = "#"; }
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
                                            ViewBag.iFrame_Src = Url.Action("RCF", "HSU", new { id = DT_Form.Rows[0][7].ToString().Trim() + "$" + SK, area = "" });
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