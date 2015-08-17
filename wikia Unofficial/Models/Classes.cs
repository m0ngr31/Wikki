using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wikia_Unofficial.Models
{
    public class WikiSearchResult
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public string Hub { get; set; }
        public string Language { get; set; }
        public string Url { get; set; }
    }
}
