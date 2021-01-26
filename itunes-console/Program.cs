using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using itunes_console.models;
using itunes_console.repository;

namespace itunes_console
{
    class Program
    {
        static void Main(string[] args)
        {
            SearchResultCache repository = new SearchResultCache();
            Console.WriteLine("Исполнитель:");
            string artist = Console.ReadLine();

            string url = "https://itunes.apple.com/search?entity=album&limit=6&term=" + artist;
            using (var webClient = new WebClient())
            {
                try
                {
                    var responce = webClient.DownloadString(url).Replace("\n", "").Replace(" ", "");
                    SearchResult searchResult = JsonSerializer.Deserialize<SearchResult>(responce);
                    List<SearchResultInfo> searchResultInfos = searchResult.results.Where(x => x.collectionType == "Album").ToList();
                    repository.save(artist, searchResultInfos);
                    showResults(searchResultInfos);
                }
                catch (WebException)
                {
                    showResults(repository.load(artist), true);
                }
            }
        }

        static void showResults(List<SearchResultInfo> searchResults, bool cached = false)
        {
            if (searchResults.Count > 0)
            {
                Console.WriteLine("\nАльбомы:");
                foreach (var searchResultInfo in searchResults)
                    Console.WriteLine(searchResultInfo.collectionName);
            }
            else
                Console.WriteLine("\nНичего не найдено!");

            if (cached)
                Console.WriteLine("\n\nНет связи сервисом! Показаны результаты сохраненные в кэше.");
        }
    }
}
