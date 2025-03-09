using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Diagnostics;
using Microsoft.Identity.Client;
using Farmer_Project.Models.Login;
using System.Security.Cryptography;
using System.Text;
using 文章寫作平台.Models.Entity;

namespace Farmer_Project.Controllers
{
    //[Route("Home/[Action]")]
    public class LoginController : Controller
    {
        

        // 註冊登入［忘記密碼］
        public IActionResult resetPasswd()
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            TempData.Remove("Message");

            TempData["Login"] = TempData["Login"];  // 確認會員身分後設定回到哪個首頁
            TempData["Account"] = TempData["Account"];
            TempData["Account_Ref"] = TempData["Account_Ref"];

            TempData["isLogin1"] = TempData["isLogin1"];
            TempData["isLogin2"] = TempData["isLogin2"];
            TempData["isLogin3"] = TempData["isLogin3"];

            TempData.Keep();  // 用於讓 TempData 的值保存不刪除

            return View();
        }

        [HttpPost]
        public IActionResult resetPasswd(forgetPasswd userAccount)
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框

            TempData["Login"] = TempData["Login"];  // 確認會員身分後設定回到哪個首頁
            TempData["Account"] = TempData["Account"];
            TempData["Account_Ref"] = TempData["Account_Ref"];

            TempData["isLogin1"] = TempData["isLogin1"];
            TempData["isLogin2"] = TempData["isLogin2"];
            TempData["isLogin3"] = TempData["isLogin3"];

            TempData.Keep();  // 用於讓 TempData 的值保存不刪除

            DBmanager dbmanager = new DBmanager();
            List<forgetPasswd> accounts = dbmanager.checkAccounts(userAccount);   // 此為啟動DBmanager當中的checkAccounts指令
            // 驗證表單是否完整
            if (string.IsNullOrEmpty(userAccount.account))
            {
                TempData["dele"] = true;
                ViewBag.Message = "請填寫所有欄位。";
                return View();
            }
            else if (!userAccount.account.Contains(".com"))
            {
                ViewBag.Msg1 = "帳號輸入錯誤";
                return View();
            }
            else if (accounts.Count == 0)
            {
                ViewBag.Msg1 = "這個帳號不存在";
                return View();
            }
            TempData["Message"] = "帳號確認成功";

            // 以下用於將帳號儲存於Session，以便後面使用
            string account = userAccount.account;
            TempData["ResetPwdUserId"] = account;

            //HttpContext.Session.SetString("ResetPwdUserId", account);  // 此表示如：Session[ResetPwdUserId] = account;
            // 結束
            TempData.Keep();  // 用於讓 TempData 的值保存不刪除
            return RedirectToAction("newPasswd");    // 注意點選的按鈕是否在<form></form>之間
        }


        public IActionResult newPasswd()
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            TempData.Remove("Message");

            TempData["Login"] = TempData["Login"];  // 確認會員身分後設定回到哪個首頁
            TempData["Account"] = TempData["Account"];
            TempData["Account_Ref"] = TempData["Account_Ref"];

            TempData["isLogin1"] = TempData["isLogin1"];
            TempData["isLogin2"] = TempData["isLogin2"];
            TempData["isLogin3"] = TempData["isLogin3"];

            TempData.Keep();  // 用於讓 TempData 的值保存不刪除

            return View();
        }

        [HttpPost]
        public IActionResult newPasswd(forgetPasswd userPasswd)
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            string account = TempData["ResetPwdUserId"].ToString();  // 此用於將儲存的變數 "TempData["ResetPwdUserId"]"代入

            TempData["Login"] = TempData["Login"];  // 確認會員身分後設定回到哪個首頁
            TempData["Account"] = TempData["Account"];
            TempData["Account_Ref"] = TempData["Account_Ref"];

            TempData["isLogin1"] = TempData["isLogin1"];
            TempData["isLogin2"] = TempData["isLogin2"];
            TempData["isLogin3"] = TempData["isLogin3"];

            TempData.Keep();  // 用於讓 TempData 的值保存不刪除

            DBmanager dbmanager = new DBmanager();
            try
            {
                dbmanager.updatePasswd(userPasswd, account);   // 此為啟動DBmanager當中的registerAccount指令，並且把變數 "user" 接收的資料加入進去
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            };
            // 驗證表單是否完整
            if (string.IsNullOrEmpty(userPasswd.passwd) || string.IsNullOrEmpty(userPasswd.passwd2))
            {
                TempData["dele"] = true;
                ViewBag.Message = "請填寫所有欄位。";
                return View();
            }
            else if (userPasswd.passwd.Length < 8 || userPasswd.passwd.Length > 12 || userPasswd.passwd2.Length < 8 || userPasswd.passwd2.Length > 12 || userPasswd.passwd != userPasswd.passwd2)
            {
                ViewBag.Msg1 = "密碼輸入錯誤";
                return View();
            }
            TempData["Message"] = "密碼更改完成";
            return RedirectToAction("login","Home");  // 注意點選的按鈕是否在<form></form>之間
        }
        // 結束

        // 註冊登入［註冊會員］
        public IActionResult register()
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            TempData.Remove("Message");

            TempData["Login"] = TempData["Login"];  // 確認會員身分後設定回到哪個首頁
            TempData["Account"] = TempData["Account"];
            TempData["Account_Ref"] = TempData["Account_Ref"];

            TempData["isLogin1"] = TempData["isLogin1"];
            TempData["isLogin2"] = TempData["isLogin2"];
            TempData["isLogin3"] = TempData["isLogin3"];

            TempData.Keep();  // 用於讓 TempData 的值保存不刪除

            return View();
        }

        // 下方的register(register user)，表示將前面的register()回傳的內容，由register.cs的變數"user"接收，然後再做其中的細分
        // 注意："register.cshtml" 的輸入格name 與 "register.cs" 的 public變數，兩者對應的名稱要一致(不分大小寫)
        [HttpPost]  // 以下的部分要上報到其他控制器，必須加上這個，否則會掛掉
        public IActionResult register(register user)
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框

            TempData["Login"] = TempData["Login"];  // 確認會員身分後設定回到哪個首頁
            TempData["Account"] = TempData["Account"];
            TempData["Account_Ref"] = TempData["Account_Ref"];

            TempData["isLogin1"] = TempData["isLogin1"];
            TempData["isLogin2"] = TempData["isLogin2"];
            TempData["isLogin3"] = TempData["isLogin3"];

            TempData.Keep();  // 用於讓 TempData 的值保存不刪除

            DBmanager dbmanager = new DBmanager();
            List<register> accounts = dbmanager.NoSameAccounts(user);   // 此為啟動DBmanager當中的newAccount指令
            int Count = accounts.Count;
            try
            {
                dbmanager.registerAccount(user, Count);   // 此為啟動DBmanager當中的registerAccount指令，並且把變數 "user" 接收的資料加入進去
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            // 驗證表單是否完整
            if (string.IsNullOrEmpty(user.NickName) || string.IsNullOrEmpty(user.Account) || string.IsNullOrEmpty(user.Passwd))
            {
                TempData["dele"] = true;
                ViewBag.Message = "請填寫所有欄位。";
                return View();
            }
            else if (user.NickName.Length < 2 || user.NickName.Length > 15)
            {
                ViewBag.Msg1 = "使用者名稱不要少於2個字元，也不超過15個字元";
                return View();
            }
            else if (!user.Account.Contains(".com"))
            {
                ViewBag.Msg2 = "帳號請輸入完整的電子信箱帳號";
                return View();
            }
            else if (user.Passwd.Length < 8 || user.Passwd.Length > 12)
            {
                ViewBag.Msg3 = "密碼需要8~12位數的符號";
                return View();
            }
            else if (accounts.Count != 0)
            {
                ViewBag.Msg2 = "帳號已經存在";
                return View();
            }
            else
            {
                TempData["Message"] = "註冊成功";
            }
            return RedirectToAction("login", "Home");  // 此設定為導回 "index" 的網頁
        }
        // 結束

        
        
        
    }
}