using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wikia_Unofficial.Models
{
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

    public class WikiStats
    {
        public int Articles { get; set; }
        public int Pages { get; set; }
        public int Images { get; set; }
        public int Videos { get; set; }
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

    public class ArticleSection
    {
        public string Title { get; set; }
        public int Level { get; set; }
        public List<SectionContent> Content { get; set; }
        public List<SectionImage> Images { get; set; }
    }

    public class SectionImage
    {
        public string Source { get; set; }
        public string Caption { get; set; }
    }

    public class SectionContent
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public List<ListElement> Elements { get; set; }
    }

    public class ListElement
    {
        public string Text { get; set; }
        public List<ListElement> Elements { get; set; }
    }

    public class ArticleSectionParagraph
    {
        public string Text { get; set; }
        public double Size { get; set; }
    }

    public class ArticlePage
    {
        public ArticleSearchResult Article { get; set; }
        public WikiSearchResult Wiki { get; set; }

        public ArticlePage(ArticleSearchResult article, WikiSearchResult wiki)
        {
            Article = article;
            Wiki = wiki;
        }
    }
}
