using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wikia_Unofficial.Models
{
    public class WikiItemSearchResult
    {
        public WikiSearchResult data { get; set; }
    }
    public class WikiSearchResult
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Headline { get; set; }
        public string Desc { get; set; }
        public string Image { get; set; }
        public Uri Image_Uri { get; set; }
        public double Wam_Score { get; set; }
        public int Id { get; set; }
        public string Hub { get; set; }
        public string Language { get; set; }
        public string Url { get; set; }
        public WikiStats Stats { get; set; }
        public string Prefered_Name { get; set; }
    }
    
    public class ArticleSearchResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string Thumbnail { get; set; }
        public Uri Image_Uri { get; set; }
    }

    public class WikiStats
    {
        public int Articles { get; set; }
        public int Pages { get; set; }
        public int Images { get; set; }
        public int Videos { get; set; }
    }

    public class ArticleInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class PageOverview
    {
        public string WikiUrl { get; set; }
        public int ArticleId { get; set; }

        public PageOverview() { }

        public PageOverview(string Uri, int Id)
        {
            WikiUrl = Uri;
            ArticleId = Id;
        }
    }
}
