using Farmer_Project.Models.Entity;
using Farmer_Project.Models.Login;
using Microsoft.Data.SqlClient;
using System.Security.Principal;
using System.Text;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Net.Mime.MediaTypeNames;

namespace 文章寫作平台.Models.Entity
{
    public class DBmanager
    {
        private readonly string connStr = "Data Source=(localdb)\\MSSQLLocalDB;Database=Article;User ID=LAPTOP-G5P63730\\KEN;Trusted_Connection=True";
        int ArticlesCount;

        // 抓取所有已有且公開的文章評論資訊
        public List<Articles> getArticles()
        {
            List<Articles> Articles = new List<Articles>();
            SqlConnection sqlConnection = new SqlConnection(connStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM WebArticle where IsPublished='Public' order by Number asc");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Articles Article = new Articles
                    {
                        Number = reader.GetInt32(reader.GetOrdinal("Number")),   // 這個欄位的屬性要設定為"int"
                        Author = reader.GetString(reader.GetOrdinal("Author")),
                        Article = reader.GetString(reader.GetOrdinal("Article")),
                        ArticleType = reader.GetString(reader.GetOrdinal("ArticleType")),
                        ArticleImagePath = reader.GetString(reader.GetOrdinal("ArticleImagePath")),
                        ArticleSummary = reader.GetString(reader.GetOrdinal("ArticleSummary")),
                    };
                    Articles.Add(Article);
                }

            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            sqlConnection.Close();
            return Articles;
        }

        // 用於計算現有的資料數量
        public int getArticlesCount()
        {
            ArticlesCount = 0;
            
            SqlConnection sqlConnection = new SqlConnection(connStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM WebArticle order by Number asc");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    ArticlesCount += 1;
                }
            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            sqlConnection.Close();
            return ArticlesCount;
        }

        // 抓取自己的文章
        public List<Articles> getMyArticles()
        {
            List<Articles> Articles = new List<Articles>();
            SqlConnection sqlConnection = new SqlConnection(connStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM WebArticle where Author='KEN' order by Number asc");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Articles Article = new Articles
                    {
                        Number = reader.GetInt32(reader.GetOrdinal("Number")),   // 這個欄位的屬性要設定為"int"
                        Author = reader.GetString(reader.GetOrdinal("Author")),
                        Article = reader.GetString(reader.GetOrdinal("Article")),
                        ArticleType = reader.GetString(reader.GetOrdinal("ArticleType")),
                        ArticleImagePath = reader.GetString(reader.GetOrdinal("ArticleImagePath")),
                        ArticleSummary = reader.GetString(reader.GetOrdinal("ArticleSummary")),
                        IsPublished = reader.GetString(reader.GetOrdinal("IsPublished")),
                    };
                    Articles.Add(Article);
                }

            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            sqlConnection.Close();
            return Articles;
        }

        // 抓取自己的文章
        public List<Articles> ArticleContent(int Number)
        {
            List<Articles> Articles = new List<Articles>();
            SqlConnection sqlConnection = new SqlConnection(connStr);
            SqlCommand sqlCommand = new SqlCommand($@"SELECT * FROM WebArticle where Number={Number}order by Number asc");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Articles Article = new Articles
                    {
                        Number = reader.GetInt32(reader.GetOrdinal("Number")),   // 這個欄位的屬性要設定為"int"
                        Author = reader.GetString(reader.GetOrdinal("Author")),
                        Article = reader.GetString(reader.GetOrdinal("Article")),
                        ArticleType = reader.GetString(reader.GetOrdinal("ArticleType")),
                        ArticleImagePath = reader.GetString(reader.GetOrdinal("ArticleImagePath")),
                        ArticleSummary = reader.GetString(reader.GetOrdinal("ArticleSummary")),
                        IsPublished = reader.GetString(reader.GetOrdinal("IsPublished")),
                    };
                    Articles.Add(Article);
                }
            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            sqlConnection.Close();
            return Articles;
        }


        // 抓取索引的文章
        public List<Articles> SearchArticles(string author, string keyword)
        {
            List<Articles> SearchArticles = new List<Articles>();
            SqlConnection sqlConnection = new SqlConnection(connStr);

            SqlCommand sqlCommand = new SqlCommand();

            // 確認是否為全部或我的文章
            if (author == "")
            {
                sqlCommand = new SqlCommand($@"SELECT * FROM WebArticle where Article=N'{keyword}' or ArticleType=N'{keyword}' or ArticleSummary=N'{keyword}'order by Number asc");
            }
            else
            {
                sqlCommand = new SqlCommand($@"SELECT * FROM WebArticle where author=N'{author}' and (Article=N'{keyword}' or ArticleType=N'{keyword}' or ArticleSummary=N'{keyword}') order by Number asc");
            }

            
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Articles SearchArticle = new Articles
                    {
                        Number = reader.GetInt32(reader.GetOrdinal("Number")),   // 這個欄位的屬性要設定為"int"
                        Author = reader.GetString(reader.GetOrdinal("Author")),
                        Article = reader.GetString(reader.GetOrdinal("Article")),
                        ArticleType = reader.GetString(reader.GetOrdinal("ArticleType")),
                        ArticleImagePath = reader.GetString(reader.GetOrdinal("ArticleImagePath")),
                        ArticleSummary = reader.GetString(reader.GetOrdinal("ArticleSummary")),
                        IsPublished = reader.GetString(reader.GetOrdinal("IsPublished")),
                    };
                    SearchArticles.Add(SearchArticle);
                }

            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            sqlConnection.Close();

            return SearchArticles;
        }

        // 新增自己的文章評論
        public void AddMyArticles(Articles author, string ArticleimagePath, bool isPublish, int ArticlesCount)
        {
            SqlConnection sqlconnection = new SqlConnection(connStr);
            SqlCommand sqlcommand = new SqlCommand($@"INSERT INTO WebArticle(Number,Author,Article,ArticleType,ArticleImagePath,ArticleSummary,IsPublished,CreatedDate) VALUES(@Number,@Author,N'{author.Article}',N'{author.ArticleType}',@ArticleImagePath,N'{author.ArticleSummary}',@IsPublished,@CreatedDate)");
            sqlcommand.Connection = sqlconnection;

            sqlcommand.Parameters.Add(new SqlParameter("@Number", ArticlesCount + 1));
            sqlcommand.Parameters.Add(new SqlParameter("@Author", "KEN"));
            //sqlcommand.Parameters.Add(new SqlParameter("@Article", "test"));
            //sqlcommand.Parameters.Add(new SqlParameter("@ArticleType", author.ArticleType));
            sqlcommand.Parameters.Add(new SqlParameter("@ArticleImagePath", $"{ArticleimagePath}"));
            //sqlcommand.Parameters.Add(new SqlParameter("@ArticleSummary", author.ArticleSummary));
            if (isPublish)
            {
                sqlcommand.Parameters.Add(new SqlParameter("@IsPublished", "Public"));
            }
            else
            {
                sqlcommand.Parameters.Add(new SqlParameter("@IsPublished", "Private"));
            }
            
            sqlcommand.Parameters.Add(new SqlParameter("@CreatedDate", ""));

            sqlconnection.Open();
            sqlcommand.ExecuteNonQuery();
            sqlconnection.Close();
        }

        // 編輯修改自己的評論
        // 1. 首先將文章的資料送到編輯畫面
        public List<Articles>EditMyArticles(int Number)
        {
            List<Articles> Articles = new List<Articles>();
            SqlConnection sqlConnection = new SqlConnection(connStr);
            SqlCommand sqlCommand = new SqlCommand($@"SELECT * FROM WebArticle where Author='KEN' and Number={Number}order by Number asc");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Articles Article = new Articles
                    {
                        Number = reader.GetInt32(reader.GetOrdinal("Number")),   // 這個欄位的屬性要設定為"int"
                        Author = reader.GetString(reader.GetOrdinal("Author")),
                        Article = reader.GetString(reader.GetOrdinal("Article")),
                        ArticleType = reader.GetString(reader.GetOrdinal("ArticleType")),
                        ArticleImagePath = reader.GetString(reader.GetOrdinal("ArticleImagePath")),
                        ArticleSummary = reader.GetString(reader.GetOrdinal("ArticleSummary")),
                        IsPublished = reader.GetString(reader.GetOrdinal("IsPublished")),
                    };
                    Articles.Add(Article);
                }

            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            sqlConnection.Close();
            return Articles;
        }
        // 2. 最後將現在的資料更新至資料表
        public void UpdateMyArticles(int Number, string Article, string ArticleType, string ArticleImagePath, string ArticleSummary, string IsPublished, IFormFile?  image)
        {
            SqlConnection sqlconnection = new SqlConnection(connStr);
            SqlCommand sqlcommand = new SqlCommand($@"Update WebArticle SET Article=N'{Article}', ArticleType=N'{ArticleType}', ArticleImagePath=N'{ArticleImagePath}', ArticleSummary=N'{ArticleSummary}', IsPublished=N'{IsPublished}' where Author='KEN' and Number={Number}");
            sqlcommand.Connection = sqlconnection;
            sqlconnection.Open();

            sqlcommand.ExecuteNonQuery();
            sqlconnection.Close();
        }


        // 刪除自己的評論
        public void DeleteMyArticles(int Number)
        {
            SqlConnection sqlconnection = new SqlConnection(connStr);
            SqlCommand sqlcommand = new SqlCommand($@"DELETE FROM WebArticle WHERE Number={Number}");
            sqlcommand.Connection = sqlconnection;
            sqlconnection.Open();

            sqlcommand.ExecuteNonQuery();
            sqlconnection.Close();
        }

        //------------以下為使用者登入的部分--------------

        // 搜尋帳號的資料
        public List<login> searchAccounts(login member)
        {
            List<login> accounts = new List<login>();
            SqlConnection sqlConnection = new SqlConnection(connStr);
            SqlCommand sqlCommand = new SqlCommand($@"SELECT * FROM memberInformation where account='{member.account}'");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    login account = new login
                    {
                        sort = reader.GetString(reader.GetOrdinal("sort")),
                        nickName = reader.GetString(reader.GetOrdinal("nickName")),
                        account = reader.GetString(reader.GetOrdinal("account")),
                        passwd = reader.GetString(reader.GetOrdinal("passwd"))
                    };
                    accounts.Add(account);
                }
            }
            sqlConnection.Close();
            return accounts;

        }

        // 確認登入的帳號密碼資料
        public List<string> loginAccounts(login member)
        {
            // 將新密碼使用 SHA256 雜湊運算(不可逆)
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(member.passwd); //將密碼鹽及新密碼組合
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }

            string NewPwd = result.ToString(); // 雜湊運算後密碼
            // 結束

            List<string> accounts = new List<string>();   // 用於辨別是否有帳號、以及為一般會員或管理者

            // 查詢資料庫指定內容［設定 "一般會員" 與 "管理者" 兩個部分］
            // 第一步驟：確認輸入的帳號是否存在
            SqlConnection sqlConnection = new SqlConnection(connStr);
            SqlCommand sqlCommand = new SqlCommand($@"SELECT * FROM memberInformation where account='{member.account}' and passwd='{NewPwd}'");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    accounts.Add("0");
                }
            }
            sqlConnection.Close();

            // 第二步驟：確認
            // ［一般會員］
            SqlConnection sqlConnection2 = new SqlConnection(connStr);
            SqlCommand sqlCommand2 = new SqlCommand($@"SELECT * FROM memberInformation where account='{member.account}' and passwd='{NewPwd}' and sort='0'");
            sqlCommand2.Connection = sqlConnection2;
            sqlConnection2.Open();

            SqlDataReader reader2 = sqlCommand2.ExecuteReader();
            if (reader2.HasRows)
            {
                while (reader2.Read())
                {
                    accounts.Add("1");
                }
            }
            sqlConnection2.Close();
            // ［管理者］
            SqlConnection sqlConnection3 = new SqlConnection(connStr);
            SqlCommand sqlCommand3 = new SqlCommand($@"SELECT * FROM memberInformation where account='{member.account}' and passwd='{NewPwd}' and sort='1'");
            sqlCommand3.Connection = sqlConnection3;
            sqlConnection3.Open();

            SqlDataReader reader3 = sqlCommand3.ExecuteReader();
            if (reader3.HasRows)
            {
                while (reader3.Read())
                {
                    accounts.Add("2");
                }
            }
            sqlConnection3.Close();
            // 結束

            return accounts;
        }
        // 結束


        // ［忘記密碼］:確認帳號是否存在的部分
        public List<forgetPasswd> checkAccounts(forgetPasswd userAccount)
        {
            List<forgetPasswd> accounts = new List<forgetPasswd>();
            SqlConnection sqlConnection = new SqlConnection(connStr);
            SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM memberInformation where account ='{userAccount.account}'");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    forgetPasswd account = new forgetPasswd
                    {
                        account = reader.GetString(reader.GetOrdinal("account"))
                    };
                    accounts.Add(account);
                }
            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            sqlConnection.Close();
            return accounts;
        }

        // 用於延續上一個，將帳號對應的密碼更改
        public void updatePasswd(forgetPasswd userPasswd, string account)
        {
            SqlConnection sqlConnection = new SqlConnection(connStr);
            SqlCommand sqlCommand = new SqlCommand($@"UPDATE memberInformation SET passwd=@newPasswd WHERE account='{account}'");
            sqlCommand.Connection = sqlConnection;

            // 將新密碼使用 SHA256 雜湊運算(不可逆)
            //string salt = userPasswd.passwd.ToString().Substring(0, 1).ToLower(); //使用帳號前一碼當作密碼鹽
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(userPasswd.passwd); //將密碼鹽及新密碼組合
            byte[] hash = sha256.ComputeHash(bytes);
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            string NewPwd = result.ToString(); // 雜湊運算後密碼

            sqlCommand.Parameters.Add(new SqlParameter("@newPasswd", NewPwd));
            // 結束

            sqlConnection.Open();
            sqlCommand.ExecuteNonQuery();
            sqlConnection.Close();
        }


        // ［註冊新會員］
        public void registerAccount(register user, int count)
        {
            if (user.NickName != null && user.Account != null && user.Passwd != null && user.Account.Contains(".com") && user.Passwd.Length >= 8 && user.Passwd.Length <= 12 && count == 0)
            {
                SqlConnection sqlconnection = new SqlConnection(connStr);
                SqlCommand sqlcommand = new SqlCommand($@"INSERT INTO memberInformation(sort,nickName,account,passwd) VALUES(@sort,N'{user.NickName}',@account,@passwd)");
                sqlcommand.Connection = sqlconnection;

                // 將新密碼使用 SHA256 雜湊運算(不可逆)
                //string salt = userPasswd.passwd.ToString().Substring(0, 1).ToLower(); //使用帳號前一碼當作密碼鹽
                SHA256 sha256 = SHA256.Create();
                byte[] bytes = Encoding.UTF8.GetBytes(user.Passwd); //將密碼鹽及新密碼組合
                byte[] hash = sha256.ComputeHash(bytes);
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    result.Append(hash[i].ToString("X2"));
                }
                string NewPwd = result.ToString(); // 雜湊運算後密碼

                sqlcommand.Parameters.Add(new SqlParameter("@Passwd", NewPwd));
                // 結束

                sqlcommand.Parameters.Add(new SqlParameter("@sort", "0"));
                sqlcommand.Parameters.Add(new SqlParameter("@account", user.Account));
                //sqlcommand.Parameters.Add(new SqlParameter("@passwd", user.passwd));

                sqlconnection.Open();
                sqlcommand.ExecuteNonQuery();
                sqlconnection.Close();
            }
        }
        // 呈上，確認帳號是否有重複的部分
        public List<register> NoSameAccounts(register user)
        {
            List<register> accounts = new List<register>();
            SqlConnection sqlConnection = new SqlConnection(connStr);
            SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM memberInformation where account ='{user.Account}'");
            sqlCommand.Connection = sqlConnection;
            sqlConnection.Open();

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    register account = new register
                    {
                        Account = reader.GetString(reader.GetOrdinal("account"))
                    };
                    accounts.Add(account);
                }
            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            sqlConnection.Close();
            return accounts;
        }

    }
}
