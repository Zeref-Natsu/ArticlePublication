using Farmer_Project.Models.Entity;
using Microsoft.Data.SqlClient;
using System.Security.Principal;

namespace 文章寫作平台.Models.Entity
{
    public class DBmanager
    {
        private readonly string connStr = "Data Source=(localdb)\\MSSQLLocalDB;Database=Article;User ID=LAPTOP-G5P63730\\KEN;Trusted_Connection=True";
        int ArticlesCount;

        public int getArticles()
        {
            ArticlesCount = 0;
            
            SqlConnection sqlConnection = new SqlConnection(connStr);
            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM WebArticle");
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

        public void newArticles(Articles author, bool isPublish, int ArticlesCount)
        {
            SqlConnection sqlconnection = new SqlConnection(connStr);
            SqlCommand sqlcommand = new SqlCommand($@"INSERT INTO WebArticle(Number,Author,Article,ArticleType,ArticleImagePath,ArticleSummary,IsPublished,CreatedDate) VALUES(@Number,@Author,@Article,N'{author.ArticleType}',@ArticleImagePath,N'{author.ArticleSummary}',@IsPublished,@CreatedDate)");
            sqlcommand.Connection = sqlconnection;

            sqlcommand.Parameters.Add(new SqlParameter("@Number", ArticlesCount + 1));
            sqlcommand.Parameters.Add(new SqlParameter("@Author", "KEN"));
            sqlcommand.Parameters.Add(new SqlParameter("@Article", "test"));
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
    }
}
