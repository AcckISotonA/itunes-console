using itunes_console.models;
using itunes_console.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

namespace itunes_console.services
{
    class Search
    {
        private SearchResultCache repository = new SearchResultCache();

        public List<SearchResultInfo> searchByArtist(string artist)
        {
            List<SearchResultInfo> result = new List<SearchResultInfo>();

            string url = "https://itunes.apple.com/search?entity=album&limit=6&term=" + artist;
            using (var webClient = new WebClient())
            {
                try
                {
                    var responce = webClient.DownloadString(url).Replace("\n", "").Replace(" ", "");
                    SearchResult searchResult = JsonSerializer.Deserialize<SearchResult>(responce);
                    result = searchResult.results.Where(x => x.collectionType == "Album").ToList();
                    repository.save(artist, result);
                }
                catch (WebException) {}
            }

            return result;
        }

        public List<SearchResultInfo> loadFromCache(string artist)
        {
            return repository.load(artist);
        }
    }
}
