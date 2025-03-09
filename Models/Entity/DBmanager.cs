using Farmer_Project.Models.Entity;
using Microsoft.Data.SqlClient;
using System.Security.Principal;

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
        public void AddMyArticles(Articles author, bool isPublish, int ArticlesCount)
        {
            SqlConnection sqlconnection = new SqlConnection(connStr);
            SqlCommand sqlcommand = new SqlCommand($@"INSERT INTO WebArticle(Number,Author,Article,ArticleType,ArticleImagePath,ArticleSummary,IsPublished,CreatedDate) VALUES(@Number,@Author,N'{author.Article}',N'{author.ArticleType}',@ArticleImagePath,N'{author.ArticleSummary}',@IsPublished,@CreatedDate)");
            sqlcommand.Connection = sqlconnection;

            sqlcommand.Parameters.Add(new SqlParameter("@Number", ArticlesCount + 1));
            sqlcommand.Parameters.Add(new SqlParameter("@Author", "KEN"));
            //sqlcommand.Parameters.Add(new SqlParameter("@Article", "test"));
            //sqlcommand.Parameters.Add(new SqlParameter("@ArticleType", author.ArticleType));
            sqlcommand.Parameters.Add(new SqlParameter("@ArticleImagePath", ""));
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
        public void UpdateMyArticles(int Number, string Article, string ArticleType, string ArticleImagePath, string ArticleSummary, string IsPublished)
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
    }
}
