using System.Collections.Generic;

namespace BankDbHelper
{
    public class ParamSql
    {
        public string Sql { get; set; }

        public List<SqlHelperParameter> Params { get; set; }

        public ParamSql()
        {
            this.Sql = string.Empty;
            this.Params = new List<SqlHelperParameter>();
        }

        public ParamSql(string sql, List<SqlHelperParameter> lst)
        {
            this.Sql = sql;
            this.Params = lst;
        }
    }
}