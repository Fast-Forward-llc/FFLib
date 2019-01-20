using FFLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFLib.Data
{

    public interface ISqlTableSource
    {
        string ToString();
    }

    public class SqlTableSource: ISqlTableSource
    {
        string _queryString;

        public SqlTableSource(string query)
        {
            _queryString = query;
        }

        public override string ToString()
        {
            return _queryString;
        }
    }
    ///// <summary>
    ///// Build SQL Query using fluent interface
    ///// </summary>
    //public class SqlSubQuery:ISqlTableSource
    //{
    //    SqlQuery _query;
    //    string _queryString;

    //    public SqlSubQuery (SqlQuery query)
    //    {
    //        _query = query;
    //    }

    //    public override string ToString()
    //    {
    //        if (_query == null) return _queryString;
    //        return _query.ToString();
    //    }
    //}

    public class SqlUnion:ISqlTableSource
    {
        List<SqlQuery> queries = new List<SqlQuery>(6);
        string modifier = "";

        public SqlUnion()
        { }

        public SqlUnion(string modifier)
        {
            this.modifier = modifier;
        }

        public SqlUnion(string modifier, SqlQuery query1, SqlQuery query2)
        {
            this.modifier = modifier;
            this.Add(query1);
            this.Add(query2);
        }

        public SqlUnion Add(SqlQuery query)
        {
            queries.Add(query);
            return this;
        }

        public SqlQuery this[int index]
        {
            get {
                if (queries.Count < index + 1) return null;
                return queries[index];
            }
        }


        public override string ToString()
        {
            return string.Join("\nUnion " + modifier + "\n", queries);
        }
    }
    /// <summary>
    /// Build SQL Query using fluent interface
    /// </summary>
    public class SqlQuery:ISqlTableSource
    {

        public List<string> selectFragments = new List<string>(20);
        public ISqlTableSource fromFragment;
        public string fromSubQueryAlias;
        public List<JoinStmt> joinFragments = new List<JoinStmt>(20);
        public List<string> whereFragments = new List<string>(20);
        public List<string> groupbyFragments = new List<string>(20);
        public List<string> havingFragments = new List<string>(20);
        public List<string> orderbyFragments = new List<string>(20);
        protected string suffixSql = null;
        protected string unionFragment;
        public List<SqlParameter> SqlParams = new List<SqlParameter>(20);

        public class JoinStmt {
            public string prefix = "";
            public string stmt;
        }

        /// <summary>
        /// Add SELECT fields w/ aliases if desired. The final Sql string will join the fields using a comma delimiter
        /// </summary>
        /// <param name="fields">Field name or sub query w/ optional alias</param>
        /// <returns></returns>
        public virtual SqlQuery Select(string fields)
        {
            this.selectFragments.Add(fields);
            return this;
        }

        /// <summary>
        /// Sets the FROM value. If null or whitespace or not specified the macro placeholder #__TableName will be used automaticly
        /// </summary>
        /// <param name="tableName">TableName or Table-Valued Function Name with parameters, excluding FROM keyword</param>
        /// <returns></returns>
        public virtual SqlQuery From(string tableName)
        {
            this.fromFragment = new SqlTableSource(tableName);
            return this;
        }

        /// <summary>
        /// Sets the FROM value. If null the macro placeholder #__TableName will be used automaticly.
        /// </summary>
        /// <param name="subQuery">SqlQuery or other ISqlTableSource</param>
        /// /// <param name="alias">alias for the ISqlTableSource</param>
        /// <returns></returns>
        public virtual SqlQuery From(ISqlTableSource subQuery,string alias)
        {
            this.fromFragment = subQuery;
            this.fromSubQueryAlias = alias;
            return this;
        }

        public virtual ISqlTableSource From()
        {
            return fromFragment;
        }

        /// <summary>
        /// Add a Join Statement to the Query
        /// </summary>
        /// <param name="joinStmt">A Join Statement excluding the 'JOIN' Keyword</param>
        /// <returns></returns>
        public virtual SqlQuery Join(string joinStmt)
        {
            if (string.IsNullOrWhiteSpace(joinStmt)) return this;
            this.joinFragments.Add(new JoinStmt() { stmt = joinStmt });
            return this;
        }

        /// <summary>
        /// Add a Join Statement to the Query with a Joining prefix like 'Left' or 'Left Outer' or 'Right'
        /// </summary>
        /// <param name="prefix">A Join Prefix like 'Left' or 'Left Outer' or 'Right'</param>
        /// <param name="joinStmt">A Join Statement excluding the 'JOIN' Keyword</param>
        /// <returns></returns>
        public virtual SqlQuery Join(string prefix, string joinStmt)
        {
            if (string.IsNullOrWhiteSpace(joinStmt)) return this;
            this.joinFragments.Add(new JoinStmt() { prefix = prefix, stmt = joinStmt });
            return this;

        }

        /// <summary>
        /// Add WHERE criteria. the final Sql string will be composed by appending all critera together seperated by spaces. Include parenthisis and boolean operators as needed in the criteria parameter.
        /// </summary>
        /// <param name="whereFragment"> A WHERE criteria fragment excluding the WHERE keyword</param>
        /// <returns></returns>
        public virtual SqlQuery Where(string whereFragment)
        {
            if (string.IsNullOrWhiteSpace(whereFragment)) return this;
            this.whereFragments.Add(whereFragment);
            return this;
        }

        /// <summary>
        /// Add GROUP BY criteria. 
        /// the final Sql string will be composed by appending all critera together seperated by commas. Include parenthisis and boolean operators as needed in the criteria parameter.
        /// </summary>
        /// <param name="groupbyFragment"></param>
        /// <returns></returns>
        public virtual SqlQuery GroupBy(string groupbyFragment)
        {
            if (string.IsNullOrWhiteSpace(groupbyFragment)) return this;
            this.groupbyFragments.Add(groupbyFragment);
            return this;
        }

        /// <summary>
        /// Add HAVING criteria. 
        /// the final Sql string will be composed by appending all critera together seperated by commas. Include parenthisis and boolean operators as needed in the criteria parameter.
        /// </summary>
        /// <param name="havingFragment"></param>
        /// <returns></returns>
        public virtual SqlQuery Having(string havingFragment)
        {
            if (string.IsNullOrWhiteSpace(havingFragment)) return this;
            this.havingFragments.Add(havingFragment);
            return this;
        }

        /// <summary>
        /// Add ORDER BY criteria. 
        /// the final Sql string will be composed by appending all critera together seperated by commas. Include parenthisis and boolean operators as needed in the criteria parameter.
        /// </summary>
        /// <param name="orderbyFragment"></param>
        /// <returns></returns>
        public virtual SqlQuery OrderBy(string orderbyFragment)
        {
            if (string.IsNullOrWhiteSpace(orderbyFragment)) return this;
            this.orderbyFragments.Add(orderbyFragment);
            return this;
        }

        /// <summary>
        /// Adds a UNION clause to the end of the Sql Statement
        /// </summary>
        /// <param name="unionFragment">A Valid UNION modifier e.g. ALL or null/whitespace if none</param>
        /// <returns></returns>
        public virtual SqlQuery Union(string unionFragment)
        {
            if (string.IsNullOrWhiteSpace(unionFragment)) this.unionFragment = "UNION";
            else this.unionFragment = "UNION "+unionFragment;
            return this;
        }
        /// <summary>
        /// Adds provided sql to the end of the sql statement. Suffix sql should be already be properly escaped. Sql text provided will overwrite any previous set value.
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual SqlQuery SuffixSql(string sql)
        {
            suffixSql = sql;
            return this;
        }

        /// <summary>
        /// Returns the 'FROM' clause of the SQL Query as a string
        /// </summary>
        public string FromClause
        {
            get
            {
                if (fromFragment == null) return "#__TableName";
                if (!(fromFragment is SqlTableSource))
                {
                    if (string.IsNullOrWhiteSpace(fromSubQueryAlias)) return "(" + fromFragment.ToString() + ")";
                    return "(" + fromFragment.ToString() + ") " + fromSubQueryAlias;
                }
                var tablename = fromFragment.ToString();
                if (string.IsNullOrWhiteSpace(tablename)) return "#__TableName";
                return tablename;
            }
        }

        /// <summary>
        /// Returns the 'WHERE' clause of the Sql Query as a string
        /// </summary>
        public string WhereClause { get { return (whereFragments != null && whereFragments.Count > 0 ? "\nWHERE " + string.Join(" ", whereFragments) : ""); } }

        /// <summary>
        /// Returns the 'ORDER BY' clause of the Sql Query as a string
        /// </summary>
        public string OrderByCaluse { get { return (orderbyFragments != null && orderbyFragments.Count > 0 ? "\nORDER BY " + string.Join(",", orderbyFragments) : ""); } }

        /// <summary>
        /// Generates a string from the Join list
        /// </summary>
        /// <param name="joins"></param>
        /// <returns>SQL string of join statements or an empty string</returns>
        protected string JoinsToString(List<JoinStmt> joins)
        {
            if (joins == null || joins.Count == 0) return string.Empty;
            var r = new StringBuilder(2000);
            foreach (var js in joins)
                r.Append("\n" + js.prefix.NotNull() + " JOIN " + js.stmt);
            return r.ToString();
        }

        /// <summary>
        /// Returns the Sql string of the Sql Query
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "SELECT " + (selectFragments != null && selectFragments.Count > 0 ? string.Join(", ", selectFragments) : "*")
            + " \nFROM " + this.FromClause
            + this.JoinsToString(joinFragments)// (joinFragments != null && joinFragments.Count > 0 ? "\nJOIN " + string.Join("\nJOIN ", joinFragments) : "")
            + (WhereClause)
            + (groupbyFragments != null && groupbyFragments.Count > 0 ? "\nGROUP BY " + string.Join(",", groupbyFragments) : "")
            + (havingFragments != null && havingFragments.Count > 0 ? "\nHAVING " + string.Join(",", havingFragments) : "")
            + (OrderByCaluse)
            + (!string.IsNullOrWhiteSpace(suffixSql) ? "\n" + suffixSql : "" )
            + (!string.IsNullOrWhiteSpace(unionFragment) ? "\n" + unionFragment : "");
        }
    }
}
