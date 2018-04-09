using System;
using System.Configuration;
using System.Runtime.Caching;

namespace Impinj_xPortal
{
    public class ConfigurationReader
    {
        public Configuration Read()
        {
            try
            {
                var config = new Configuration()
                {
                    ConnectionString = ConfigurationManager.ConnectionStrings["dbContext"].ConnectionString,
                    TableName = ConfigurationManager.AppSettings["Database:TableName"],
                    Address = ConfigurationManager.AppSettings["Antenna:IpAddress"],
                    AntennaName = ConfigurationManager.AppSettings["Antenna:Name"]
                };

                int.TryParse(ConfigurationManager.AppSettings["Cache:ExpirationInSeconds"], out int seconds);
                config.CachePolicy = CreateDefaultCacheItemPolicy(seconds);

                return config;
            }
            catch (ConfigurationException ex)
            {
                Console.WriteLine("Error reading configuration file!");
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private static CacheItemPolicy CreateDefaultCacheItemPolicy(int seconds)
        {
            return new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromSeconds(seconds)
            };
        }
    }
}
