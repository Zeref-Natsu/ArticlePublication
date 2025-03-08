using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farmer_Project.Models.Entity
{
    public class Articles
    {
        public int Number { set; get; }
        public string Author { set; get; }
        public string ArticleType { set; get; }
        public string Article { get; set; }
        public string ArticleImagePath { set; get; }
        public string ArticleSummary { set; get; }
        public bool IsPublished { set; get; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        //導覽屬性(一對"多")
        //public virtual FarmersInfo FarmersInfo { get; set; }

        //導覽屬性("一"對多)
        //public virtual List<FarmersArticlesDetails> FarmersArticlesDetails { get; set; }
    }
}
