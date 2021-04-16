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
    public class HSUController : Controller
    {
        //====================================================================================================================
        iCore_Administrator.Modules.AuthenticationTester AAuth = new iCore_Administrator.Modules.AuthenticationTester();
        iCore_Administrator.Modules.SQL_Tranceiver Sq = new iCore_Administrator.Modules.SQL_Tranceiver();
        iCore_Administrator.Modules.PublicFunctions Pb = new iCore_Administrator.Modules.PublicFunctions();
        iCore_Administrator.Modules.HSU_Application.Application_MasterFunction AppFn = new iCore_Administrator.Modules.HSU_Application.Application_MasterFunction();
        iCore_Administrator.Modules.HSU_Application.HSU_Callback HSUCB = new iCore_Administrator.Modules.HSU_Application.HSU_Callback();
        //====================================================================================================================
        public ActionResult Index() { return new HttpNotFoundResult(); }
        //====================================================================================================================
        public ActionResult RCF()
        {
            try
            {
                string ReqData = Url.RequestContext.RouteData.Values["id"].ToString().Trim();
                string[] SepData = ReqData.Split('$');
                string Req_Form_ID = SepData[0].Trim();
                string Req_SecrekKey = SepData[1].Trim();
                DataTable DT_Form = new DataTable();
                DT_Form = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select ID,User_ID,UnicID From Users_04_Hospitality_SingleUser_RegisterForms Where (Customer_Side_iFrame = '" + Req_Form_ID + "') And (Status_Code = '1') And (Removed = '0')");
                if (DT_Form.Rows != null)
                {
                    if (DT_Form.Rows.Count == 1)
                    {
                        DataTable DT_User = new DataTable();
                        DT_User = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select ID From Users_02_SingleUser Where (ID = '" + DT_Form.Rows[0][1].ToString().Trim() + "') And (Status_Code = '1') And (Removed = '0')");
                        if (DT_User.Rows != null)
                        {
                            if (DT_User.Rows.Count == 1)
                            {
                                var DomainURL = $"{Request.Url.GetLeftPart(UriPartial.Authority)}{Url.Content("~/")}";
                                if (DomainURL.Substring(DomainURL.Length - 1, 1) != "/") { DomainURL = DomainURL + "/"; }
                                ViewBag.Req_URL = DomainURL;
                                ViewBag.Form_ID = DT_Form.Rows[0][0].ToString().Trim();
                                ViewBag.User_ID = DT_Form.Rows[0][1].ToString().Trim();
                                ViewBag.User_URL = Req_Form_ID;
                                ViewBag.DT_Group = null;
                                ViewBag.DT_Sections = null;
                                ViewBag.Google_API_Place = "";
                                try
                                {
                                    DataTable DT_Google = new DataTable();
                                    DT_Google = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select Google_Place_Key From Setting_Basic_02_Google");
                                    ViewBag.Google_API_Place = DT_Google.Rows[0][0].ToString().Trim();
                                }
                                catch (Exception) { }
                                DataTable DT_Sections = new DataTable();
                                DT_Sections = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select Section_ID,Width,Icon,Name From Users_05_Hospitality_SingleUser_RegisterForms_Section Where (Group_ID = '" + DT_Form.Rows[0][0].ToString().Trim() + "') And (Status_Code = '1') And (Removed = '0') Order By Row_Index,Name");
                                ViewBag.DT_Group = DT_Form.Rows;
                                ViewBag.DT_Sections = DT_Sections.Rows;
                                string iCoreUserURL = System.Configuration.ConfigurationManager.AppSettings["iCore_User_URL"];
                                ViewBag.iCoreUserURL = iCoreUserURL;
                                ViewBag.SectionAsPage = "0";
                                try
                                {
                                    DataTable DT_Form_Config = new DataTable();
                                    DT_Form_Config = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select ShowAsPage From Users_07_Hospitality_SingleUser_RegisterForms_Configuration Where (User_ID = '" + DT_Form.Rows[0][1].ToString().Trim() + "') And (Form_ID = '" + DT_Form.Rows[0][0].ToString().Trim() + "')");
                                    ViewBag.SectionAsPage = DT_Form_Config.Rows[0][0].ToString().Trim();
                                }
                                catch (Exception) { }
                                return View();
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
        [HttpPost]
        public JsonResult RCF_GI(string EID, string Cnt)
        {
            try
            {
                string ResVal = "0";
                string ResSTR = "";
                EID = EID.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                Cnt = Cnt.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                DataTable DT = new DataTable();
                DT = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select * From Users_06_Hospitality_SingleUser_RegisterForms_Elements Where (Element_ID = '" + EID + "') And (Status_Code = '1') And (Removed = '0')");
                if (DT.Rows != null)
                {
                    if (DT.Rows.Count == 1)
                    {
                        ResSTR = "";
                        string MasterInput = "ELMNTGI";
                        ResSTR += "<div class=\"form-inline ELMT\" style=\"width:100%;margin-top:20px\" id=\"EGI_" + EID + "_" + Cnt + "\">";
                        ResSTR += "<div class=\"col-lg-12\" style=\"text-align:right\">";
                        ResSTR += "<i class=\"fa fa-trash text-danger\" style=\"cursor:pointer;font-size:16px\" onclick=\"RemoveGroupinput('EGI_" + EID + "_" + Cnt + "')\"></i>";
                        ResSTR += "</div>";
                        if (DT.Rows[0][37].ToString().Trim() != "")
                        {
                            ResSTR += "<div class=\"col-lg-" + DT.Rows[0][36].ToString().Trim() + "\">";
                            ResSTR += "<div class=\"text-bold-300 font-medium-1\" style=\"margin-top:10px;margin-left:5px;width:100%\">";
                            ResSTR += DT.Rows[0][37].ToString().Trim();
                            ResSTR += "</div>";
                            ResSTR += "<fieldset class=\"form-group position-relative\" style=\"width:100%\">";
                            ResSTR += "<input type=\"text\" class=\"form-control " + MasterInput + "\" id=\"GroupInput_" + DT.Rows[0][0].ToString().Trim() + "_" + Cnt + "_1\" placeholder=\"" + DT.Rows[0][38].ToString().Trim() + "\" style=\"width:100%\">";
                            ResSTR += "</fieldset>";
                            ResSTR += "</div>";
                            MasterInput = "";
                        }
                        if (DT.Rows[0][41].ToString().Trim() != "")
                        {
                            ResSTR += "<div class=\"col-lg-" + DT.Rows[0][40].ToString().Trim() + "\">";
                            ResSTR += "<div class=\"text-bold-300 font-medium-1\" style=\"margin-top:10px;margin-left:5px;width:100%\">";
                            ResSTR += DT.Rows[0][41].ToString().Trim();
                            ResSTR += "</div>";
                            ResSTR += "<fieldset class=\"form-group position-relative\" style=\"width:100%\">";
                            ResSTR += "<input type=\"text\" class=\"form-control " + MasterInput + "\" id=\"GroupInput_" + DT.Rows[0][0].ToString().Trim() + "_" + Cnt + "_2\" placeholder=\"" + DT.Rows[0][42].ToString().Trim() + "\" style=\"width:100%\">";
                            ResSTR += "</fieldset>";
                            ResSTR += "</div>";
                            MasterInput = "";
                        }
                        if (DT.Rows[0][45].ToString().Trim() != "")
                        {
                            ResSTR += "<div class=\"col-lg-" + DT.Rows[0][44].ToString().Trim() + "\">";
                            ResSTR += "<div class=\"text-bold-300 font-medium-1\" style=\"margin-top:10px;margin-left:5px;width:100%\">";
                            ResSTR += DT.Rows[0][45].ToString().Trim();
                            ResSTR += "</div>";
                            ResSTR += "<fieldset class=\"form-group position-relative\" style=\"width:100%\">";
                            ResSTR += "<input type=\"text\" class=\"form-control " + MasterInput + "\" id=\"GroupInput_" + DT.Rows[0][0].ToString().Trim() + "_" + Cnt + "_3\" placeholder=\"" + DT.Rows[0][46].ToString().Trim() + "\" style=\"width:100%\">";
                            ResSTR += "</fieldset>";
                            ResSTR += "</div>";
                            MasterInput = "";
                        }
                        if (DT.Rows[0][49].ToString().Trim() != "")
                        {
                            ResSTR += "<div class=\"col-lg-" + DT.Rows[0][48].ToString().Trim() + "\">";
                            ResSTR += "<div class=\"text-bold-300 font-medium-1\" style=\"margin-top:10px;margin-left:5px;width:100%\">";
                            ResSTR += DT.Rows[0][49].ToString().Trim();
                            ResSTR += "</div>";
                            ResSTR += "<fieldset class=\"form-group position-relative\" style=\"width:100%\">";
                            ResSTR += "<input type=\"text\" class=\"form-control " + MasterInput + "\" id=\"GroupInput_" + DT.Rows[0][0].ToString().Trim() + "_" + Cnt + "_4\" placeholder=\"" + DT.Rows[0][50].ToString().Trim() + "\" style=\"width:100%\">";
                            ResSTR += "</fieldset>";
                            ResSTR += "</div>";
                            MasterInput = "";
                        }
                        if (DT.Rows[0][53].ToString().Trim() != "")
                        {
                            ResSTR += "<div class=\"col-lg-" + DT.Rows[0][52].ToString().Trim() + "\">";
                            ResSTR += "<div class=\"text-bold-300 font-medium-1\" style=\"margin-top:10px;margin-left:5px;width:100%\">";
                            ResSTR += DT.Rows[0][53].ToString().Trim();
                            ResSTR += "</div>";
                            ResSTR += "<fieldset class=\"form-group position-relative\" style=\"width:100%\">";
                            ResSTR += "<input type=\"text\" class=\"form-control " + MasterInput + "\" id=\"GroupInput_" + DT.Rows[0][0].ToString().Trim() + "_" + Cnt + "_5\" placeholder=\"" + DT.Rows[0][54].ToString().Trim() + "\" style=\"width:100%\">";
                            ResSTR += "</fieldset>";
                            ResSTR += "</div>";
                            MasterInput = "";
                        }
                        if (DT.Rows[0][57].ToString().Trim() != "")
                        {
                            ResSTR += "<div class=\"col-lg-" + DT.Rows[0][56].ToString().Trim() + "\">";
                            ResSTR += "<div class=\"text-bold-300 font-medium-1\" style=\"margin-top:10px;margin-left:5px;width:100%\">";
                            ResSTR += DT.Rows[0][57].ToString().Trim();
                            ResSTR += "</div>";
                            ResSTR += "<fieldset class=\"form-group position-relative\" style=\"width:100%\">";
                            ResSTR += "<input type=\"text\" class=\"form-control " + MasterInput + "\" id=\"GroupInput_" + DT.Rows[0][0].ToString().Trim() + "_" + Cnt + "_6\" placeholder=\"" + DT.Rows[0][58].ToString().Trim() + "\" style=\"width:100%\">";
                            ResSTR += "</fieldset>";
                            ResSTR += "</div>";
                            MasterInput = "";
                        }
                        ResSTR += "</div>";
                    }
                    else { ResVal = "1"; }
                }
                else { ResVal = "1"; }
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Value = ResVal, Text = ResSTR.Trim() } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem>
                { new SelectListItem{Text = "The server encountered an error while executing your request" , Value = "1"}};
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //====================================================================================================================
        // Application Ststus Code :
        // 1 : Pending
        // 2 : Confirmed
        // 3 : Rejected
        // 4 : Review
        // 5 : Failed
        //====================================================================================================================
        [HttpPost]
        public JsonResult HSU_Form_Create_Application(string FID, string UID, string UIP, string UBN, string URLP)
        {
            try
            {
                string ResVal = "0"; string ResSTR = "";
                FID = FID.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                UID = UID.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                UIP = UIP.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                UBN = UBN.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                URLP = URLP.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                DataTable DT = new DataTable();
                DT = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select ID From Users_02_SingleUser Where (ID = '" + UID + "') And (Status_Code = '1') And (Removed = '0')");
                if (DT.Rows != null)
                {
                    if (DT.Rows.Count == 1)
                    {
                        string InsDate = Sq.Sql_Date();
                        string InsTime = Sq.Sql_Time();
                        DataTable DTRes = new DataTable();
                        DTRes = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Insert Into Users_08_Hospitality_SingleUser_Application OUTPUT Inserted.ID Values ('','" + FID + "','" + UID + "','" + UIP + "','" + UBN + "','','','','1','Pending','" + InsDate + "','" + InsTime + "','" + InsDate + "','" + InsTime + "','0','0','0','0','0','','','0','','','','0','" + InsDate + "','" + InsTime + "','0','0','" + InsDate + "','" + InsTime + "','')");
                        string TRCD = DTRes.Rows[0][0].ToString().Trim() + Pb.Make_Security_Code(10);
                        Thread.Sleep(10);
                        Sq.Execute_TSql(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Update Users_08_Hospitality_SingleUser_Application Set [App_UnicID] = '" + "HSU-" + DTRes.Rows[0][0].ToString().Trim() + Pb.Make_Security_Code(40) + "',[TrakingCode] = '" + TRCD + "' Where (ID = '" + DTRes.Rows[0][0].ToString().Trim() + "')");
                        string AppIDLST = DTRes.Rows[0][0].ToString().Trim();
                        Sq.Execute_TSql(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Delete From Users_13_Hospitality_SingleUser_Application_DataInfo Where (App_ID = '" + AppIDLST + "')");
                        Sq.Execute_TSql(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Insert Into Users_13_Hospitality_SingleUser_Application_DataInfo Values ('" + AppIDLST + "','','','','','','','','','','','','','','','','','','','','','0','0','','0','','','','','0','0','','0','','','','','0','0','','','','','','','0','0','','','','','','','')");
                        string[,] Param_Define = new string[,] { { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" }, { "", "" } };
                        try
                        {
                            string[] URLPM = URLP.Split('^');
                            for (int i = 0; i < URLPM.Length; i++)
                            {
                                try
                                {
                                    string[] PVal = URLPM[i].Split('=');
                                    Param_Define[i, 0] = PVal[0].Trim();
                                    Param_Define[i, 1] = PVal[1].Trim();
                                }
                                catch (Exception) { }
                            }
                        }
                        catch (Exception) { }
                        try
                        {
                            Sq.Execute_TSql(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Update Users_13_Hospitality_SingleUser_Application_DataInfo Set [Parameter_Key_01] = '" + Param_Define[0, 0] + "',[Parameter_Value_01] = '" + Param_Define[0, 1] + "',[Parameter_Key_02] = '" + Param_Define[1, 0] + "',[Parameter_Value_02] = '" + Param_Define[1, 1] + "',[Parameter_Key_03] = '" + Param_Define[2, 0] + "',[Parameter_Value_03] = '" + Param_Define[2, 1] + "',[Parameter_Key_04] = '" + Param_Define[3, 0] + "',[Parameter_Value_04] = '" + Param_Define[3, 1] + "',[Parameter_Key_05] = '" + Param_Define[4, 0] + "',[Parameter_Value_05] = '" + Param_Define[4, 1] + "',[Parameter_Key_06] = '" + Param_Define[5, 0] + "',[Parameter_Value_06] = '" + Param_Define[5, 1] + "',[Parameter_Key_07] = '" + Param_Define[6, 0] + "',[Parameter_Value_07] = '" + Param_Define[6, 1] + "',[Parameter_Key_08] = '" + Param_Define[7, 0] + "',[Parameter_Value_08] = '" + Param_Define[7, 1] + "',[Parameter_Key_09] = '" + Param_Define[8, 0] + "',[Parameter_Value_09] = '" + Param_Define[8, 1] + "',[Parameter_Key_10] = '" + Param_Define[9, 0] + "',[Parameter_Value_10] = '" + Param_Define[9, 1] + "' Where (App_ID = '" + AppIDLST + "')");

                        }
                        catch (Exception) { }
                        ResSTR = AppIDLST + "-" + TRCD;
                    }
                    else
                    {
                        ResVal = "1"; ResSTR = "Your department manager could not be identified";
                    }
                }
                else
                {
                    ResVal = "1"; ResSTR = "Your department manager could not be identified";
                }
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Value = ResVal, Text = ResSTR.Trim() } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem>
                { new SelectListItem{Text = "The server encountered an error while executing your request" , Value = "1"}};
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //====================================================================================================================
        [HttpPost]
        public void HSU_Form_Upload_Value(string AID, string TAG, string EFID, string EID, string VA, string TX, string O1, string O2, string O3, string O4, string O5, string O6, string O7, string ISF, HttpPostedFileBase UF)
        {
            try
            {
                AID = AID.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                TAG = TAG.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                EFID = EFID.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                EID = EID.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                VA = VA.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                TX = TX.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                O1 = O1.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                O2 = O2.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                O3 = O3.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                O4 = O4.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                O5 = O5.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                O6 = O6.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                O7 = O7.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                ISF = ISF.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                string InsDate = Sq.Sql_Date();
                string InsTime = Sq.Sql_Time();
                string FileName = ""; string FileType = "";
                bool FileReady = false;
                if (UF != null) { try { FileName = UF.FileName.Trim(); FileType = Pb.GetFile_Type(FileName); FileReady = true; } catch (Exception) { } }
                Sq.Execute_TSql(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Insert Into Users_09_Hospitality_SingleUser_Application_Elements Values ('" + AID + "','" + TAG + "','" + EFID + "','" + EID + "','" + VA + "','" + TX + "','" + O1 + "','" + O2 + "','" + O3 + "','" + O4 + "','" + O5 + "','" + O6 + "','" + O7 + "','" + InsDate + "','" + InsTime + "','" + InsDate + "','" + InsTime + "','0','0','0','','','" + ISF + "','" + FileName + "','" + FileType + "')");
                if (FileReady == true)
                {
                    AID = AID.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                    EID = EID.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                    string BasFolder = Server.MapPath("~/Drive/Hospitality/CustomerData/" + AID);
                    string BasPath = Server.MapPath("~/Drive/Hospitality/CustomerData/" + AID + "/" + EID + ".IDV");
                    if (System.IO.Directory.Exists(BasFolder) == false) { System.IO.Directory.CreateDirectory(BasFolder); }
                    UF.SaveAs(BasPath);
                    string UURL = System.Configuration.ConfigurationManager.AppSettings["iCore_User_URL"];
                    var client = new RestClient(UURL + "/UIMG/HSU_Form_Upload");
                    client.Timeout = -1;
                    var request = new RestRequest(Method.POST);
                    request.AddParameter("AID", AID);
                    request.AddParameter("EID", EID);
                    request.AddFile("UF", BasPath);
                    IRestResponse response = client.Execute(request);
                }
            }
            catch (Exception) { }
        }
        //====================================================================================================================
        [HttpPost]
        public JsonResult HSU_Form_VFLD(string UI, string FI, string FT, string FV)
        {
            try
            {
                string ResVal = "0"; string ResSTR = "";
                UI = UI.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                FI = FI.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                FT = FT.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                FV = FV.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                switch (FT)
                {
                    case "4": // Username
                        {
                            if (AAuth.Public_Username_IsUnic(FV) == false)
                            {
                                ResVal = "1";
                                ResSTR = "The username you entered is duplicate, please try again after check";
                            }
                            break;
                        }
                    case "6": // Unic ID
                        {
                            if (AAuth.Public_UnicID_IsUnic(FV) == false)
                            {
                                ResVal = "1";
                                ResSTR = "The id you entered is duplicate, please try again after check";
                            }
                            break;
                        }
                    case "8": // Email Address
                        {
                            if (AAuth.Public_Email_IsUnic(FV) == false)
                            {
                                ResVal = "1";
                                ResSTR = "The email you entered is duplicate, please try again after check";
                            }
                            break;
                        }
                }
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Value = ResVal, Text = ResSTR.Trim() } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem>
                { new SelectListItem{Text = "The server encountered an error while executing your request" , Value = "1"}};
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //====================================================================================================================
        [HttpPost]
        public void HSU_Form_Start(string AID, string FID)
        {
            try
            {
                AID = AID.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                FID = FID.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                Task.Run(() => HSUCB.Send_CallBack_Submit(AID, FID));
                Task.Run(() => HSUCB.Send_CallBack_Submit2(AID, FID));
                string UURL = System.Configuration.ConfigurationManager.AppSettings["iCore_User_URL"];
                string SecretKey = System.Configuration.ConfigurationManager.AppSettings["iCore_API_SecretKey"];
                var client = new RestClient(UURL + "/UIMG/HSU_Form_Start");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddParameter("SK", SecretKey);
                request.AddParameter("AID", AID);
                IRestResponse response = client.Execute(request);
            }
            catch (Exception)
            { }
        }
        //====================================================================================================================
        // 0 : No New data
        // 1 : Error
        // 2 : Guided image capture
        // 3 : Liveness test
        [HttpPost]
        public JsonResult HSU_GD(string UID, string URL, string SID, string FID, string EID)
        {
            try
            {
                string ResVal = "0"; string ResSTR = "";
                UID = UID.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                URL = URL.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                SID = SID.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                FID = FID.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                EID = EID.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                DataTable DT = new DataTable();
                DT = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select Element_Type,Feeback_D1,Feeback_D2,Feeback_D3,Feeback_D4,Feeback_D5,Feeback_D6,Feeback_D7,Feeback_D8,Feeback_D9 From Users_13_Hospitality_SingleUser_Application_QR_Links Where (UID = '" + UID + "') And (Request_URL = '" + URL + "') And (User_ID = '" + SID + "') And (Form_ID = '" + FID + "') And (Element_ID = '" + EID + "') And (Data_Change = '1') And (Data_Read = '0')");
                if (DT != null)
                {
                    if (DT.Rows != null)
                    {
                        if (DT.Rows.Count == 1)
                        {
                            ResVal = DT.Rows[0][0].ToString().Trim();
                            if (DT.Rows[0][0].ToString().Trim() == "2")
                            {
                                string BasPath = Server.MapPath("~/Drive/Hospitality/QRUpload/" + UID + ".jpg");
                                if (System.IO.File.Exists(BasPath) == true)
                                {
                                    byte[] ImgBytes = System.IO.File.ReadAllBytes(BasPath);
                                    var base64 = Convert.ToBase64String(ImgBytes);
                                    ResSTR = EID + "!##!" + UID  + "!##!" + String.Format("data:image/jpg;base64,{0}", base64);
                                }
                            }
                            else
                            {
                                ResSTR = EID + "!##!" + UID + "!##!" + DT.Rows[0][1].ToString().Trim() + "!##!" + DT.Rows[0][2].ToString().Trim() + "!##!" + DT.Rows[0][3].ToString().Trim() + "!##!" + DT.Rows[0][4].ToString().Trim() + "!##!" + DT.Rows[0][5].ToString().Trim() + "!##!" + DT.Rows[0][6].ToString().Trim() + "!##!" + DT.Rows[0][7].ToString().Trim() + "!##!" + DT.Rows[0][8].ToString().Trim() + "!##!" + DT.Rows[0][9].ToString().Trim();
                            }
                            Sq.Execute_TSql(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Update Users_13_Hospitality_SingleUser_Application_QR_Links Set [Data_Read] = '1' Where (UID = '" + UID + "') And (Request_URL = '" + URL + "') And (User_ID = '" + SID + "') And (Form_ID = '" + FID + "') And (Element_ID = '" + EID + "')");
                        }
                    }
                }
                IList<SelectListItem> FeedBack = new List<SelectListItem> { new SelectListItem { Value = ResVal, Text = ResSTR.Trim() } };
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                IList<SelectListItem> FeedBack = new List<SelectListItem>
                { new SelectListItem{Text = "" , Value = "1"}};
                return Json(FeedBack, JsonRequestBehavior.AllowGet);
            }
        }
        //====================================================================================================================
    }
}