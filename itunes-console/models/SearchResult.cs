using System;
using System.Collections.Generic;
using System.Text;

namespace itunes_console.models
{
    class SearchResult
    {
        public int resultCount { get; set; }
        public List<SearchResultInfo> results { get; set; }
    }
}
