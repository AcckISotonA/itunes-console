using System;
using System.Collections.Generic;

using itunes_console.models;
using itunes_console.services;


namespace itunes_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Исполнитель:");
            string artist = Console.ReadLine();

            Search search = new Search();

            List<SearchResultInfo> results = search.searchByArtist(artist);
            bool loadFromCache = results.Count == 0;
            if (loadFromCache)
                results = search.loadFromCache(artist);

            showResults(results, loadFromCache);
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
