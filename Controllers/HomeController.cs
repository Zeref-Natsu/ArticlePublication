using System;
using System.Diagnostics;
using System.Security.Principal;
using Farmer_Project.Models.Entity;
using Microsoft.AspNetCore.Mvc;
using 文章寫作平台.Models;
using 文章寫作平台.Models.Entity;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Farmer_Project.Models.Login;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace 文章寫作平台.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string keyword)
        {
            TempData.Remove("Message");

            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            TempData["Account"] = "未登入";  // 顯示是否有登入，有的話顯示登入的帳號
            TempData["Account_Ref"] = "/Home/login";   // 設定 "登入" 連結的路徑
            TempData.Keep();  // 用於讓 TempData 的值保存不刪除

            // 抓取已儲存於資料庫的文章資料
            DBmanager dbmanager = new DBmanager();

            if (string.IsNullOrEmpty(keyword))    // 若沒有搜尋關鍵字的狀況
            {
                List<Articles> Articles = dbmanager.getArticles();
                ViewBag.Articles = Articles;

                return View(Articles);
            }
            string author = "";
            List<Articles> SearchArticles = dbmanager.SearchArticles(author,keyword);    //抓取特定的資料並存放於此變數
            ViewBag.Articles = SearchArticles;   //顯示資料

            return View(SearchArticles);
        }

        public IActionResult MyArticle(string keyword)
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            TempData["Account"] = TempData["Account"];  // 顯示是否有登入，有的話顯示登入的帳號
            TempData["Account_Ref"] = TempData["Account_Ref"];
            TempData.Keep();  // 用於讓 TempData 的值保存不刪除

            DBmanager dbmanager = new DBmanager();

            if (string.IsNullOrEmpty(keyword))    // 若沒有搜尋關鍵字的狀況
            {
                List<Articles> Articles = dbmanager.getMyArticles();
                ViewBag.Articles = Articles;

                return View(Articles);
            }
            string author = "KEN";
            List<Articles> SearchArticles = dbmanager.SearchArticles(author, keyword);    //抓取特定的資料並存放於此變數
            ViewBag.Articles = SearchArticles;   //用於將抓取的資料顯示於網頁上

            return View(SearchArticles);
        }

        [HttpGet]
        // 顯示文章的內容網頁
        public IActionResult ArticleContent(int Number)    // Number為接收 "MyArticle" 當中的 href="/Home/ArticleContent/@author.Number" 的 @author.Number，名字要一樣
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            TempData["Account"] = TempData["Account"];  // 顯示是否有登入，有的話顯示登入的帳號
            TempData["Account_Ref"] = TempData["Account_Ref"];
            TempData.Keep();  // 用於讓 TempData 的值保存不刪除

            DBmanager dbmanager = new DBmanager();
            List<Articles> Articles = dbmanager.ArticleContent(Number);   // 用於將指定的資料抓出來
            ViewBag.Articles = Articles;   //用於將抓取的資料顯示於網頁上，cshtml要加上 List<Articles> Articles = ViewBag.Articles

            return View();
        }



        // 以下兩個為新增文章評論
        public IActionResult ArticleAdd(int id)
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            TempData["Account"] = TempData["Account"];  // 顯示是否有登入，有的話顯示登入的帳號
            TempData["Account_Ref"] = TempData["Account_Ref"];
            TempData.Keep();  // 用於讓 TempData 的值保存不刪除

            DBmanager dbmanager = new DBmanager();
            List<Articles> Articles = dbmanager.EditMyArticles(id);   // 用於將指定的資料抓出來
            ViewBag.Articles = Articles;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ArticleAdd(Articles author, string action, string ArticleimagePath, IFormFile? image)
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            TempData["Account"] = TempData["Account"];  // 顯示是否有登入，有的話顯示登入的帳號
            TempData["Account_Ref"] = TempData["Account_Ref"];
            TempData.Keep();  // 用於讓 TempData 的值保存不刪除

            //判斷: "true":發佈 / "false":保存
            bool isPublish = action == "true";

            DBmanager dbmanager = new DBmanager();
            int ArticlesCount = dbmanager.getArticlesCount();  // 此用於得到資料數量

            try
            {
                if (image != null)
                {
                    // 判斷圖片格式是否有效
                    if (!IsImageFormat(image))
                    {
                        ModelState.AddModelError("image", "請上傳有效的圖片格式（jpg, jpeg, png, gif）");
                        return View();
                    }

                    // 計算圖片的 MD5 哈希值
                    var md5Hash = ComputeMd5Hash(image);

                    // 儲存圖片並取得圖片路徑
                    string imagePath = await SaveImage(image, md5Hash);

                    // 設定 farmersInfo 的圖片路徑
                    ArticleimagePath = imagePath;
                }
                else
                {
                    // 沒有上傳圖片，使用預設圖片路徑
                    ArticleimagePath = "/images/f-1.jpg";
                }


                ViewBag.Message = "資料儲存成功！";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "儲存資料發生錯誤，請稍後再試！";
                return View();
            }


            dbmanager.AddMyArticles(author, ArticleimagePath, isPublish, ArticlesCount);   // 此為啟動DBmanager當中的newAccount指令
            return RedirectToAction("MyArticle");  // 此設定為導回 "MyArticle" 的網頁
        }

        // 編輯方法
        public IActionResult ArticleEdit(int id)
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框
            TempData["Account"] = TempData["Account"];  // 顯示是否有登入，有的話顯示登入的帳號
            TempData["Account_Ref"] = TempData["Account_Ref"];
            TempData.Keep();  // 用於讓 TempData 的值保存不刪除

            DBmanager dbmanager = new DBmanager();
            List<Articles> Articles = dbmanager.EditMyArticles(id);   // 用於將指定的資料抓出來
            ViewBag.Articles = Articles;

            return View(Articles);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int Number, string Article, string ArticleType, string ArticleImagePath, string ArticleSummary, string IsPublished, IFormFile? image)
        {
            if (IsPublished != "Public" || IsPublished != "Private")
            {
                ViewBag.Msg1 = "必須選擇";
            }

            try
            {
                if (image != null)
                {
                    // 判斷圖片格式是否有效
                    if (!IsImageFormat(image))
                    {
                        ModelState.AddModelError("image", "請上傳有效的圖片格式（jpg, jpeg, png, gif）");
                        return View();
                    }

                    // 計算圖片的 MD5 哈希值
                    var md5Hash = ComputeMd5Hash(image);

                    // 儲存圖片並取得圖片路徑
                    string imagePath = await SaveImage(image, md5Hash);

                    // 設定 farmersInfo 的圖片路徑
                    ArticleImagePath = imagePath;
                }
                else
                {
                    // 沒有上傳圖片，使用預設圖片路徑
                    ArticleImagePath = "";
                }


                ViewBag.Message = "資料儲存成功！";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "儲存資料發生錯誤，請稍後再試！";
                return View();
            }

            DBmanager dbmanager = new DBmanager();
            dbmanager.UpdateMyArticles(Number, Article, ArticleType, ArticleImagePath, ArticleSummary, IsPublished, image);    // 此用於對 DBmanager 的 UpdateMyArticles 下達更新指令

            return RedirectToAction("MyArticle");
        }


        // 刪除方法
        [HttpPost]
        public ActionResult Delete(int id)
        {
            DBmanager dbmanager = new DBmanager();
            dbmanager.DeleteMyArticles(id);  // 此用於對 DBmanager 的 DeleteMyArticles 下達刪除指令

            return RedirectToAction("MyArticle", "Home");
        }

        [HttpPost]
        public IActionResult ReloadData()
        {
            // 重新抓取所有會員資料
            DBmanager dbmanager = new DBmanager();
            List<Articles> Articles = dbmanager.getMyArticles();
            ViewBag.Articles = Articles;

            
            return RedirectToAction("MyArticle", "Home", Articles);
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

        [HttpPost]
        public IActionResult Login(login member)
        {
            ViewData["BodyClass"] = "sub_page";   // 此用於頁面上出現個空白框

            // 驗證表單是否完整
            if (string.IsNullOrEmpty(member.account) || string.IsNullOrEmpty(member.passwd))
            {
                TempData["dele"] = true;
                ViewBag.Message = "請填寫所有欄位。";
                return View();
            }

            DBmanager dbmanager = new DBmanager();
            List<string> accounts = dbmanager.loginAccounts(member);   // 此為啟動DBmanager當中的checkAccounts指令   // 此為啟動DBmanager當中的checkAccounts指令
            if (!member.account.Contains(".com"))
            {
                ViewBag.Msg2 = "帳號格式輸入錯誤";
                return View();
            }
            else if (!accounts.Contains("0"))
            {
                ViewBag.Msg3 = "這個帳號不存在";
                return View();
            }
            else if (accounts.Contains("0") && (!accounts.Contains("1") && !accounts.Contains("2")))
            {
                ViewBag.Msg4 = "密碼輸入錯誤";
                return View();
            }
            else if (accounts.Contains("1") && accounts.Contains("2"))    // 確認帳號是否有同時存在於兩個部分的狀況
            {
                TempData["Message"] = $"登入失敗！\n警告：{member.account}這個帳號同時擁有一般會員和管理者的權限。";
                return RedirectToAction("login");
            }

            //var Account = _DbContext.MemberInformation.FirstOrDefault(M => M.Account == $"{member.account}");
            if (accounts.Contains("1"))
            {
                TempData["Message"] = "登入成功\n您的身分為：一般會員";

                TempData["Account"] = $"{member.account}";  // 設定登入後的帳戶名稱
                TempData["Account_Ref"] = "";  // 設定 Home 首頁連結的路徑

                // 以下用於讓about、crop、testimonial、contact、Blog的頁面，在有無登入帳號的狀態，設定下拉選單的文字功能
                TempData["isLogin1"] = "修改會員資料";  // 確定是否為登入狀態，設定登入狀態的下拉選單[修改會員資料]

                TempData["isLogin2"] = "";  // 確定是否為登入狀態，不是就設定登入狀態的下拉選單[登入]

                TempData["isLogin3"] = "登出";  // 確定是否為登入狀態，是舊設定登入狀態的下拉選單[登出]
                // 結束

                TempData.Keep();  // 用於讓 TempData 的值保存不刪除

                // RedirectToAction("register", "Login")：表示跳轉至 LoginController 當中的 register.cshtml 頁面
                return RedirectToAction("MemberIndex", "Login"); // * 這裡的register之後要修改為其他人製作的cshtml首頁名稱
            }
            TempData["Message"] = "登入成功\n您的身分為：系統管理者";

            TempData["Account"] = $"{member.account}";  // 設定登入後的帳戶名稱
            TempData["Account_Ref"] = "";  // 設定 Home 首頁連結的路徑

            // 以下用於讓about、crop、testimonial、contact、Blog的頁面，在有無登入帳號的狀態，設定下拉選單的文字功能
            TempData["isLogin1"] = "修改會員資料";  // 確定是否為登入狀態，設定登入狀態的下拉選單[修改會員資料]

            TempData["isLogin2"] = "";  // 確定是否為登入狀態，不是就設定登入狀態的下拉選單[登入]

            TempData["isLogin3"] = "登出";  // 確定是否為登入狀態，是舊設定登入狀態的下拉選單[登出]
            // 結束

            TempData.Keep();  // 用於讓 TempData 的值保存不刪除

            return RedirectToAction("Index", "Home"); // * 這裡的resetPasswd之後要修改為其他人製作的cshtml首頁名稱

        }
        // 結束






        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        //------start of 一般方法-------------------------------------------------------------------------------------------------//       

        //判斷圖片格式
        private bool IsImageFormat(IFormFile image)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(image.FileName)?.ToLower();
            return allowedExtensions.Contains(fileExtension);
        }

        //圖片內容MD-5編碼
        private string ComputeMd5Hash(IFormFile image)
        {
            //using區塊結束後會釋放MD5占用的資源
            using (var md5 = MD5.Create())
            {
                using (var stream = image.OpenReadStream())  // 讀取檔案流
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    StringBuilder sb = new StringBuilder();
                    foreach (var b in hashBytes)
                    {
                        sb.Append(b.ToString("x2")); // 轉換為十六進位格式
                    }
                    return sb.ToString();
                }
            }
        }

        //圖片存進wwwroot
        private async Task<string> SaveImage(IFormFile image, string md5Hash)
        {
            //產生一個唯一的圖片檔案名稱，使用 MD5 作為檔名
            string fileExtension = Path.GetExtension(image.FileName); // 獲取圖片副檔名
            string fileName = md5Hash + fileExtension;  // 以 MD5 哈希值作為檔名

            //設定儲存路徑 (Directory.GetCurrentDirectory() -返回當前應用程式的根目錄路徑)
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            // 檢查圖片是否已存在，如果已存在則不儲存並返回現有的檔案名稱
            if (System.IO.File.Exists(filePath))
            {
                return "/images/" + fileName;  // 返回現有圖片的路徑
            }

            // 儲存檔案 
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream); // 複製圖片資料到伺服器
            }
            return "/images/" + fileName; ;
        }

        // 定義中文到英文的映射
        private string TransDictionary(string input)
        {
            // 如果是英文，直接返回
            if (input.All(c => char.IsLetter(c) && char.IsAscii(c)))
            {
                return input;
            }

            var translateDict = new Dictionary<string, string>
            {
                { "咖啡", "Coffee" },
                { "芒果", "Mango" }
            };

            // 如果字典中有對應的中文，則返回對應的英文，否則返回原始的中文
            if (translateDict.TryGetValue(input, out var translated))
            {
                return translated;
            }

            return input;
        }
    }
}
