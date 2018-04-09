using Impinj.OctaneSdk;
using System;
using System.Runtime.Caching;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Configuration;


namespace Impinj_xPortal
{
    class Program
    {
        static ImpinjReader Reader { get; set; }
        static Configuration Config { get; set; }

        static void Main(string[] args)
        {
            ReadConfiguration();
            DeviceInitialization();
            EventSubscription();

            ConnectReader();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            Reader.Stop();
            Reader.Disconnect();
        }

        private static void ReadConfiguration()
        {
            Config = new ConfigurationReader().Read();
            if (Config == null)
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private static void ConnectReader()
        {
            try
            {
                Reader.ConnectAsync();
            }
            catch (OctaneSdkException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        private static void DeviceInitialization()
        {
            Reader = new ImpinjReader(Config.Address, Config.AntennaName);
        }

        private static void EventSubscription()
        {
            Reader.TagsReported += Reader_TagsReported;
            Reader.ConnectAsyncComplete += Reader_ConnectAsyncComplete;
            Reader.ReaderStarted += Reader_ReaderStarted;
        }

        private static void Reader_ConnectAsyncComplete(ImpinjReader reader, ConnectAsyncResult result, string errorMessage)
        {
            if (result == ConnectAsyncResult.Success)
            {
                reader.ApplyDefaultSettings();
                reader.Start();
                Console.WriteLine("Started...");
            }
            else
            {
                Console.WriteLine(errorMessage);
            }
        }

        private static void Reader_ReaderStarted(ImpinjReader reader, ReaderStartedEvent e)
        {
            reader.QueryTags();
        }

        private static void Reader_TagsReported(ImpinjReader reader, TagReport report)
        {
            foreach (var tag in report.Tags)
            {
                var tagReport = new TagReportData(tag, reader.Name);

                var existInCache = MemoryCache.Default.AddOrGetExisting(tagReport.GetTagId(), tagReport.EventDateTimeUtc, Config.CachePolicy);

                if (existInCache == null)
                {
                    Task.Run(() => SaveToDatabase(tagReport));
                    Console.WriteLine($"{tagReport.EventDateTimeUtc.ToLocalTime().ToLongTimeString()} Tag reported: {tagReport.GetTagId()}");
                }
            }
        }

        private static async Task SaveToDatabase(TagReportData tagReport)
        {
            using (IDbConnection db = new SqlConnection(Config.ConnectionString))
            {
                var sql = tagReport.GetSqlInsertStatement(Config);
                await db.ExecuteAsync(sql);
            }
        }


    }
}
