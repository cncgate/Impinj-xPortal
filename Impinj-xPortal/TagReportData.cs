using System;
using Impinj.OctaneSdk;

namespace Impinj_xPortal
{
    public class TagReportData
    {
        public TagReportData(Tag tag, string readerName)
        {
            Tag = tag;
            ReaderName = readerName;
            EventDateTimeUtc = DateTime.UtcNow;
        }

        public Tag Tag { get; private set; }
        public DateTime EventDateTimeUtc { get; private set; }
        public string ReaderName { get; private set; }

        public string GetTagId()
        {
            return Tag.Epc.ToString();
        }

        public string GetSqlInsertStatement(Configuration config)
        {
            var sql = $"INSERT INTO [dbo].[{config.TableName}] {Constants.SqlInsertHeader}('{ReaderName}', '{EventDateTimeUtc.ToString(Constants.DbDateTimeFormat)}', '{GetTagId()}')";
            return sql;
        }
    }
}
