using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using 文章寫作平台.Models;

namespace 文章寫作平台.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            TempData["Account"] = "未登入";  // 顯示是否有登入，有的話顯示登入的帳號
            
            TempData.Keep();  // 用於讓 TempData 的值保存不刪除
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
