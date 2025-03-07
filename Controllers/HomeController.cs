using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using 文章寫作平台.Models;

namespace 文章寫作平台.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string connStr = "Data Source=(localdb)\\MSSQLLocalDB;Database=account;User ID=LAPTOP-G5P63730\\KEN;Trusted_Connection=True";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            TempData["Account"] = "未登入";  // 顯示是否有登入，有的話顯示登入的帳號
            TempData["Account_Ref"] = "/Home/login";   // 設定 "登入" 連結的路徑

            //TempData["isLogin1"] = "生活";
            //TempData["isLogin2"] = "娛樂";
            //TempData["isLogin3"] = "企業";
            TempData.Keep();  // 用於讓 TempData 的值保存不刪除
            return View();
        }

        public IActionResult MyArticle()
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            TempData["Account"] = TempData["Account"];  // 顯示是否有登入，有的話顯示登入的帳號
            TempData["Account_Ref"] = TempData["Account_Ref"];

            //TempData["isLogin1"] = "生活";
            //TempData["isLogin2"] = "娛樂";
            //TempData["isLogin3"] = "企業";
            TempData.Keep();  // 用於讓 TempData 的值保存不刪除
            return View();
        }

        public IActionResult Login()
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框

            TempData["Login"] = "/Home/index";  // 確認會員身分後設定回到哪個首頁
            TempData["Account"] = "未登入";  // 顯示是否有登入，有的話顯示登入的帳號
            TempData["Account_Ref"] = "/Home/login";  // 判斷是否為登入狀態後，設定 "登入" 的路徑

            TempData["isLogin1"] = "";  // 確定是否為登入狀態，設定登入狀態的下拉選單[修改會員資料]
            TempData["isLogin2"] = "登入";  // 確定是否為登入狀態，不是就設定登入狀態的下拉選單[登入]
            TempData["isLogin3"] = "";  // 確定是否為登入狀態，是舊設定登入狀態的下拉選單[登出]

            TempData.Keep("Login");  // 用於讓 TempData 的值保存不刪除
            TempData.Keep("Account");  // 用於讓 TempData 的值保存不刪除
            TempData.Keep("Account_Ref");  // 用於讓 TempData 的值保存不刪除
            TempData.Keep("isLogin1");  // 用於讓 TempData 的值保存不刪除
            TempData.Keep("isLogin2");  // 用於讓 TempData 的值保存不刪除
            TempData.Keep("isLogin3");  // 用於讓 TempData 的值保存不刪除
            return View();
        }






        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
