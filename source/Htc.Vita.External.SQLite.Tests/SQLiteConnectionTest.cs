using System;
using System.IO;
using Htc.Vita.Core.Log;
using Xunit;

namespace Htc.Vita.External.SQLite.Tests
{
    public static class SQLiteConnectionTest
    {
        [Fact]
        public static void Default_0_Properties()
        {
            var defaultFlags = SQLiteConnection.DefaultFlags;
            Logger.GetInstance(typeof(SQLiteConnectionTest)).Info($"SQLiteConnection.DefaultFlags: {defaultFlags}");

            var defineConstants = SQLiteConnection.DefineConstants;
            Assert.False(string.IsNullOrWhiteSpace(defineConstants));
            Logger.GetInstance(typeof(SQLiteConnectionTest)).Info($"SQLiteConnection.DefineConstants: {defineConstants}");

            var interopCompileOptions = SQLiteConnection.InteropCompileOptions;
            Assert.False(string.IsNullOrWhiteSpace(interopCompileOptions));
            Logger.GetInstance(typeof(SQLiteConnectionTest)).Info($"SQLiteConnection.InteropCompileOptions: {interopCompileOptions}");

            var interopSourceId = SQLiteConnection.InteropSourceId;
            Assert.False(string.IsNullOrWhiteSpace(interopSourceId));
            Logger.GetInstance(typeof(SQLiteConnectionTest)).Info($"SQLiteConnection.InteropSourceId: {interopSourceId}");

            var interopVersion = SQLiteConnection.InteropVersion;
            Assert.False(string.IsNullOrWhiteSpace(interopVersion));
            Logger.GetInstance(typeof(SQLiteConnectionTest)).Info($"SQLiteConnection.InteropVersion: {interopVersion}");

            var providerSourceId = SQLiteConnection.ProviderSourceId;
            Assert.False(string.IsNullOrWhiteSpace(providerSourceId));
            Logger.GetInstance(typeof(SQLiteConnectionTest)).Info($"SQLiteConnection.ProviderSourceId: {providerSourceId}");

            var providerVersion = SQLiteConnection.ProviderVersion;
            Assert.False(string.IsNullOrWhiteSpace(providerVersion));
            Logger.GetInstance(typeof(SQLiteConnectionTest)).Info($"SQLiteConnection.ProviderVersion: {providerVersion}");

            var sqLiteCompileOptions = SQLiteConnection.SQLiteCompileOptions;
            Assert.False(string.IsNullOrWhiteSpace(sqLiteCompileOptions));
            Logger.GetInstance(typeof(SQLiteConnectionTest)).Info($"SQLiteConnection.SQLiteCompileOptions: {sqLiteCompileOptions}");

            var sqLiteSourceId = SQLiteConnection.SQLiteSourceId;
            Assert.False(string.IsNullOrWhiteSpace(sqLiteSourceId));
            Logger.GetInstance(typeof(SQLiteConnectionTest)).Info($"SQLiteConnection.SQLiteSourceId: {sqLiteSourceId}");

            var sqLiteVersion = SQLiteConnection.SQLiteVersion;
            Assert.False(string.IsNullOrWhiteSpace(sqLiteVersion));
            Logger.GetInstance(typeof(SQLiteConnectionTest)).Info($"SQLiteConnection.SQLiteVersion: {sqLiteVersion}");
        }

        [Fact]
        public static void Default_1_CreateFile()
        {
            var dbName = $"test-{Core.Util.Convert.ToTimestampInMilli(DateTime.UtcNow)}.db";
            var dbFile = new FileInfo(dbName);
            Assert.False(dbFile.Exists);
            using (var connection = new SQLiteConnection($"Data Source={dbName};Version=3;"))
            {
                connection.Open();
                connection.Close();
            }
            dbFile = new FileInfo(dbName);
            Assert.True(dbFile.Exists);
        }

        [Fact]
        public static void Default_2_CreateTable()
        {
            var dbName = $"test-{Core.Util.Convert.ToTimestampInMilli(DateTime.UtcNow)}.db";
            var dbFile = new FileInfo(dbName);
            Assert.False(dbFile.Exists);
            using (var connection = new SQLiteConnection($"Data Source={dbName};Version=3;"))
            {
                connection.Open();
                const string sql = "CREATE TABLE highscores (name VARCHAR(20), score INT)";
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    var result = command.ExecuteNonQuery();
                    Assert.Equal(0, result);
                }
                connection.Close();
            }
            dbFile = new FileInfo(dbName);
            Assert.True(dbFile.Exists);
            Assert.True(dbFile.Length > 0);
        }
    }
}
