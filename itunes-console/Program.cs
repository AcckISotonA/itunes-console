using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using itunes_console.models;


namespace itunes_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Исполнитель:");
            string artist = Console.ReadLine();

            string url = "https://itunes.apple.com/search?entity=album&limit=6&term=" + artist;
            using (var webClient = new WebClient())
            {
                var responce = webClient.DownloadString(url).Replace("\n", "").Replace(" ", "");
                SearchResult searchResult = JsonSerializer.Deserialize<SearchResult>(responce);
                List<SearchResultInfo> searchResultInfos = searchResult.results.Where(x => x.collectionType == "Album").ToList();
                if (searchResultInfos.Count > 0)
                {
                    Console.WriteLine("\nАльбомы:");
                    foreach (var searchResultInfo in searchResultInfos)
                        Console.WriteLine(searchResultInfo.collectionName + " (исполнитель: " + searchResultInfo.artistName + ")");
                }
                else
                    Console.WriteLine("\nНичего не найдено!");
            }
        }
    }
}
