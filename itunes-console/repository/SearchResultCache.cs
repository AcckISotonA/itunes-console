using itunes_console.models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace itunes_console.repository
{
    class SearchResultCache
    {
        private const string connectionSting = "Data Source=cache.db";

        // Инициализация SQLite БД
        protected virtual void Initialize()
        {
            using (var connection = new SqliteConnection(connectionSting))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    List<string> commandTexts = new List<string>()
                    {
                        @"CREATE TABLE IF NOT EXISTS Artists (ArtistId INTEGER PRIMARY KEY AUTOINCREMENT, Artist STRING NOT NULL);",
                        @"CREATE TABLE IF NOT EXISTS SearchResultCache (SearchResultCacheId INTEGER PRIMARY KEY AUTOINCREMENT, ArtistId INTEGER NOT NULL, Album STRING NOT NULL, FOREIGN KEY(ArtistId) REFERENCES Artists(ArtistId));"
                    };
                    foreach (string commandText in commandTexts)
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.Transaction = transaction;
                            cmd.CommandText = commandText;
                            cmd.ExecuteNonQuery();
                        }

                    transaction.Commit();
                }
            }
        }

        // Получение идентификатора исполнителя по его имени
        protected Int64 artistId(string artist, SqliteTransaction transaction)
        {
            // Запрашиваем из SQLite БД идентификатор исполнителя по его имени
            using (var cmdArtistId = transaction.Connection.CreateCommand())
            {
                cmdArtistId.Transaction = transaction;
                cmdArtistId.CommandText = @"SELECT ArtistId FROM Artists WHERE Artist=$artist";
                cmdArtistId.Parameters.AddWithValue("$artist", artist);
                var r = cmdArtistId.ExecuteScalar();
                if (r != null)
                    return (Int64)r;
            }

            // Если запись была только что вставлена в SQLite БД, то запрашиваем идентификатор вызвав команду last_insert_rowid
            using (var cmdInsertedArtistId = transaction.Connection.CreateCommand())
            {
                cmdInsertedArtistId.Transaction = transaction;
                cmdInsertedArtistId.CommandText = @"SELECT last_insert_rowid()";
                var r = cmdInsertedArtistId.ExecuteScalar();
                if (r != null)
                    return (Int64)r;
            }

            return 0;
        }


        public SearchResultCache()
        {
            Initialize();
        }

        // Кэширование информации о результатах поиска по артисту
        public void save(string artist, List<SearchResultInfo> searchResults)
        {
            using (var connection = new SqliteConnection(connectionSting))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var artistId = this.artistId(artist, transaction);
                    if (artistId == 0)
                    {
                        // Сохраняем в SQLite БД имя артиста
                        using (var cmdArtistInsert = connection.CreateCommand())
                        {
                            cmdArtistInsert.Transaction = transaction;
                            cmdArtistInsert.CommandText = @"INSERT INTO Artists (Artist) VALUES($artist)";
                            cmdArtistInsert.Parameters.AddWithValue("$artist", artist);
                            cmdArtistInsert.ExecuteNonQuery();
                        }
                        artistId = this.artistId(artist, transaction);
                    }

                    // Удаляем из SQLite БД ранее сохраненную информацию о результатах поиска по артисту
                    using (var cmdSearchResultCacheClear = connection.CreateCommand())
                    {
                        cmdSearchResultCacheClear.Transaction = transaction;
                        cmdSearchResultCacheClear.CommandText = @"DELETE FROM SearchResultCache WHERE ArtistId=$artistId";
                        cmdSearchResultCacheClear.Parameters.AddWithValue("$artistId", artistId);
                        cmdSearchResultCacheClear.ExecuteNonQuery();
                    }

                    // Сохраняем в SQLite БД новые результаты поиска по артисту
                    foreach (var searchResult in searchResults)
                        using (var cmdSearchResultCacheInsert = connection.CreateCommand())
                        {
                            cmdSearchResultCacheInsert.Transaction = transaction;
                            cmdSearchResultCacheInsert.CommandText = @"INSERT INTO SearchResultCache (ArtistId, Album) VALUES($artistId, $album)";
                            cmdSearchResultCacheInsert.Parameters.AddWithValue("$artistId", artistId);
                            cmdSearchResultCacheInsert.Parameters.AddWithValue("$album", searchResult.collectionName);
                            cmdSearchResultCacheInsert.ExecuteNonQuery();
                        }

                    transaction.Commit();
                }
            }
        }

        // Загрузка из кэша информации о результатах поиска по артисту
        public List<SearchResultInfo> load(string artist)
        {
            List<SearchResultInfo> results = new List<SearchResultInfo>();
            using (var connection = new SqliteConnection(connectionSting))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var artistId = this.artistId(artist, transaction);

                    // Загрузка из SQLite БД информации о результатах поиска по артисту
                    using (var cmdSearchResultCacheLoad = connection.CreateCommand())
                    {
                        cmdSearchResultCacheLoad.Transaction = transaction;
                        cmdSearchResultCacheLoad.CommandText = @"SELECT Album FROM SearchResultCache WHERE ArtistId=$artistId";
                        cmdSearchResultCacheLoad.Parameters.AddWithValue("$artistId", artistId);

                        using (var reader = cmdSearchResultCacheLoad.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add( new SearchResultInfo() {
                                    collectionName = reader.GetString(0)
                                });
                            }
                        }
                    }

                    transaction.Commit();
                }
            }

            return results;
        }
    }
}
