using System.Runtime.Caching;

namespace Impinj_xPortal
{
    public class Configuration
    {
        public string ConnectionString { get; set; }
        public string TableName { get; set; }
        public string Address { get; set; }
        public string AntennaName { get; set; }
        public CacheItemPolicy CachePolicy { get; set; }
    }
}
