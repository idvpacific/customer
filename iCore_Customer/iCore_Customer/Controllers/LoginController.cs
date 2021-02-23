using iCore_Customer.Modules;
using iCore_Customer.Modules.SecurityAuthentication;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Services.Description;
namespace iCore_Customer.Controllers
{
    public class LoginController : Controller
    {
        //====================================================================================================================
        iCore_Administrator.Modules.AuthenticationTester AAuth = new iCore_Administrator.Modules.AuthenticationTester();
        iCore_Administrator.Modules.SQL_Tranceiver Sq = new iCore_Administrator.Modules.SQL_Tranceiver();
        iCore_Administrator.Modules.PublicFunctions Pb = new iCore_Administrator.Modules.PublicFunctions();
        iCore_Administrator.Modules.EmailSender Email = new iCore_Administrator.Modules.EmailSender();
        //====================================================================================================================
        public ActionResult Index()
        {
            Session["User_UID"] = "0";
            Session["User_UNM"] = "";
            Session["User_Type"] = "0";
            Session["User_TypeText"] = "";
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "UC", new { id = "", area = "" });
        }

        [HttpPost]
        public JsonResult User_Request(string Username, string Password)
        {
            try
            {
                Session["User_UID"] = "0";
                Session["User_UNM"] = "";
                Session["User_Type"] = "0";
                Session["User_TypeText"] = "";
                FormsAuthentication.SignOut();
                string ResVal = "0"; string ResSTR = "";
                Username = Username.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                Password = Password.Replace(",", " ").Replace("#", "").Replace("  ", " ").Trim();
                bool UserDetected = false;
                DataTable DT = new DataTable();
                DT = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select ID,Name,LName,Type_Code,Type_Text From Users_02_SingleUser Where ((Email = '" + Username + "') Or (Account_Login_Username = '" + Username + "')) And (Account_Login_Password = '" + Password + "') And (Status_Code = '1') And (Removed = '0')");
                if (DT.Rows.Count == 1) { UserDetected = true; }
                if (UserDetected == true)
                {
                    Session["User_UID"] = DT.Rows[0][0].ToString().Trim();
                    Session["User_UNM"] = (DT.Rows[0][1].ToString().Trim() + " " + DT.Rows[0][2].ToString().Trim()).Trim();
                    Session["User_Type"] = DT.Rows[0][3].ToString().Trim();
                    Session["User_TypeText"] = DT.Rows[0][4].ToString().Trim();
                    FormsAuthentication.SetAuthCookie("ICULFP", false);
                }
                else
                {
                    ResVal = "1"; ResSTR = "Your username or password is incorrect, please try again later after check";
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
        public ActionResult AccessDenied()
        {
            return View();
        }
        //====================================================================================================================
        public ActionResult HSURF()
        {
            try
            {
                string Form_UnicID = Url.RequestContext.RouteData.Values["id"].ToString().Trim();
                DataTable DT_Form = new DataTable();
                DT_Form = Sq.Get_DTable_TSQL(iCore_Administrator.Modules.DataBase_Selector.Administrator, "Select Customer_Side_URL,Customer_Side_iFrame From Users_04_Hospitality_SingleUser_RegisterForms Where ((Customer_Side_URL = '" + Form_UnicID + "') Or (Customer_Side_iFrame = '" + Form_UnicID + "')) And (Status_Code = '1') And (Removed = '0')");
                if (DT_Form.Rows != null)
                {
                    if (DT_Form.Rows.Count == 1)
                    {
                        if (DT_Form.Rows[0][0].ToString().Trim().ToUpper() == Form_UnicID.Trim().ToUpper())
                        {
                            return RedirectToAction("RGF", "H", new { id = Form_UnicID, area = "SPF" });
                        }
                        else
                        {
                            if (DT_Form.Rows[0][1].ToString().Trim().ToUpper() == Form_UnicID.Trim().ToUpper())
                            {
                                return RedirectToAction("RGFIF", "H", new { id = Form_UnicID, area = "SPF" });
                            }
                            else
                            {
                                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
                            }
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
    }
}