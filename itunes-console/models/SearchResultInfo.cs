using System;
using System.Collections.Generic;
using System.Text;

namespace itunes_console.models
{
    class SearchResultInfo
    {
        public string wrapperType { get; set; }
        public string collectionType { get; set; }
        public long collectionId { get; set; }
        public long amgArtistId { get; set; }
        public string artistName { get; set; }
        public string collectionName { get; set; }
        public string collectionCensoredName { get; set; }
        public string artistViewUrl { get; set; }
        public string collectionViewUrl { get; set; }
        public string artworkUrl60 { get; set; }
        public string artworkUrl100 { get; set; }
        public double collectionPrice { get; set; }
        public string collectionExplicitness { get; set; }
        public long trackCount { get; set; }
        public string copyright { get; set; }
        public string country { get; set; }
        public string currency { get; set; }
        public DateTime releaseDate { get; set; }
        public string primaryGenreName { get; set; }
    }
}
