using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDB;
using ToDB.Model;

namespace ToDB.SqlClient
{
    public class TSqlSerializer : ISqlSerializer
    {
        static readonly TSqlSerializerOptions DEFAULT_OPTIONS = new TSqlSerializerOptions { RequireWhereOnUpdateAndDelete = true };
        TSqlSerializerOptions _options;
        public TSqlSerializer() : this(DEFAULT_OPTIONS)
        {

        }
        public TSqlSerializer(TSqlSerializerOptions options)
        {
            _options = options;
        }

        public string ToSql(Command command)
        {
            StringBuilder sql = new StringBuilder();
            ToSql(command, sql);
            return sql.ToString().TrimEnd();
        }

        void ToSql(Command command, StringBuilder sql)
        {
            BuidDelete(command, sql);
            BuildInsert(command, sql);
            BuildValues(command, sql);
            BuildUpdate(command, sql);
            BuildSets(command, sql);
            BuildSelect(command.SelectItems, sql);
            BuildFrom(command, sql); 
            BuildJoins(command.Joins, sql);
            BuildWhere(command, sql);
            BuildUnions(command, sql);
            BuildGroupBys(command, sql);
            BuildHavings(command, sql);
            BuildOrderBy(command, sql);

            if (_options.RequireWhereOnUpdateAndDelete)
            {
                if (!command.WhereClause.Items.Any() && (command.DeleteClause != null || command.UpdateClause != null))
                {
                    throw new ToDBException("Update or Delete with no conditions. Set RequireWhereOnUpdateAndDelete to false in the options if this was intentional.");
                }
            }
        }

        void BuidDelete(Command command, StringBuilder sql)
        {
            if (command.DeleteClause != null)
            {
                sql.Append("delete " + command.DeleteClause.Item + " ");
            }
        }

        private void BuildSets(Command command, StringBuilder sql)
        {
            if (command.SetItems != null && command.SetItems.Any())
            {
                sql.Append("set " + string.Join(",", command.SetItems.Select(x => x.Left + "=" + x.Right)) + " ");                
            }
        }

        private void BuildUpdate(Command command, StringBuilder sql)
        {
            if (command.UpdateClause != null)
                sql.Append("update " + command.UpdateClause.Item + " ");
        }

        void BuildInsert(Command command, StringBuilder sql)
        {
            if (command.InsertClause != null)
            {
                sql.Append("insert " + command.InsertClause.Table.Item + " ");
                if (command.InsertClause.Columns.Any())
                {
                    sql.Append("(" + string.Join(",", command.InsertClause.Columns) + ") ");
                }
            }
        }

        void BuildValues(Command command, StringBuilder sql)
        {
            if (command.ValuesClause != null && command.ValuesClause.Any())
            {
                sql.Append("values ");
                bool isFirst = true;
                foreach (var item in command.ValuesClause)
                {
                    if (!isFirst)
                        sql.Append(",");
                    sql.Append("(" + string.Join(",", item.Parameters) + ")");
                    isFirst = false;
                }
                sql.Append(" ");
            }
        }

        void BuildGroupBys(Command command, StringBuilder sql)
        {
            if (command.GroupBys != null && command.GroupBys.Any())
            {
                sql.Append("group by " + string.Join(",", command.GroupBys.Select(x => x.Column)) + " ");
            }
        }

        void BuildHavings(Command command, StringBuilder sql)
        {
            if (command.HavingClause.Items.Any())
            {
                sql.Append("having ");
                sql.Append(RenderConditions(command.HavingClause));
            }
        }

        void BuildUnions(Command command, StringBuilder sql)
        {
            foreach (Union union in command.Unions)
            {
                string op = "";
                switch (union.TypeOfUnion)
                {
                    case Union.UnionType.Union:
                        op = "union";
                        break;
                    case Union.UnionType.UnionAll:
                        op = "union all";
                        break;
                    case Union.UnionType.Intersect:
                        op = "intersect";
                        break;
                    case Union.UnionType.Except:
                        op = "except";
                        break;
                    default:
                        throw new NotImplementedException();
                }
                sql.Append(op + " " + ToSql(union.Query));
            }
        }

        void BuildFrom(Command command, StringBuilder sql)
        {
            if (command.FromClause != null)
                sql.Append("from " + command.FromClause.Item + " ");
        }

        void BuildWhere(Command command, StringBuilder sql)
        {
            if (command.WhereClause.Items.Any())
            {
                sql.Append("where ");
                sql.Append(RenderConditions(command.WhereClause));
            }
        }

        void BuildOrderBy(Command command, StringBuilder sql)
        {
            if (command.OrderBys.Any())
            {
                sql.Append("order by " + string.Join(",", command.OrderBys.Select(x => x.Column + " " + (x.Desc ? "desc" : "asc"))) + " ");
            }
        }

        string RenderSubQuery(Command command, string alias)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("(");
            ToSql(command, sql);
            sql.Append(")");
            if (!string.IsNullOrWhiteSpace(alias))
                sql.Append(" " + alias);
            return sql.ToString();
        }

        void BuildSelect(List<object> selectItems, StringBuilder sql)
        {
            if (selectItems.Any())
            {
                sql.Append("select " + string.Join(",", selectItems.Select(x =>
                    {
                        if (x is SelectItem)
                            return ((SelectItem)x).Item;
                        else if (x is SelectSubQuery)
                            return RenderSubQuery(((SelectSubQuery)x).SubQuery, ((SelectSubQuery)x).Alias);
                        throw new NotImplementedException();
                    })
                ) + " ");
            }
        }

        void BuildJoins(List<Join> joins, StringBuilder sql)
        {
            foreach (Join join in joins)
            {
                string joinOperator = "";
                switch (join.TypeOfJoin)
                {
                    case Join.JoinType.Inner:
                        joinOperator = "inner join";
                        break;
                    case Join.JoinType.Left:
                        joinOperator = "left outer join";
                        break;
                    case Join.JoinType.Right:
                        joinOperator = "right outer join";
                        break;
                    case Join.JoinType.Full:
                        joinOperator = "full join";
                        break;
                    default:
                        throw new NotImplementedException();
                }
                sql.Append(joinOperator + " " + join.Table.Item + " on " + RenderConditions(join.On));
            }
        }

        string RenderConditions(ConditionBuilder where)
        {
            StringBuilder sql = new StringBuilder();
            bool isFirst = true;
            foreach (var item in where.Items)
            {
                if (item is Conjunction && !isFirst)
                {
                    var conjunction = (Conjunction)item;
                    if (conjunction.TypeOfConjunction == Conjunction.ConjuntionType.Or)
                        sql.Append("or ");
                    else if (conjunction.TypeOfConjunction == Conjunction.ConjuntionType.And)
                        sql.Append("and ");
                }
                else if (item is ConditionBuilder)
                {
                    sql.Append("(");
                    sql.Append(RenderConditions((ConditionBuilder)item));
                    sql.Append(") ");
                }
                else if (item is UninaryComparison)
                {
                    UninaryComparison comparison = (UninaryComparison)item;
                    sql.Append(comparison.Column + " ");
                    switch (comparison.TypeOfComparion)
                    {
                        case UninaryComparison.UninaryComparisonType.IsNull:
                            sql.Append("is null ");
                            break;
                        case UninaryComparison.UninaryComparisonType.IsNotNull:
                            sql.Append("is not null ");
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else if (item is BinaryComparison)
                {
                    BinaryComparison comparison = (BinaryComparison)item;
                    string op = "";
                    switch (comparison.TypeOfComparison)
                    {
                        case BinaryComparison.BinaryComparisonType.Equal:
                            op = "=";
                            break;
                        case BinaryComparison.BinaryComparisonType.NotEqual:
                            op = "<>";
                            break;
                        case BinaryComparison.BinaryComparisonType.Like:
                            op = " like ";
                            break;
                        case BinaryComparison.BinaryComparisonType.LessThan:
                            op = "<";
                            break;
                        case BinaryComparison.BinaryComparisonType.LessThanOrEqual:
                            op = "<=";
                            break;
                        case BinaryComparison.BinaryComparisonType.GreaterThan:
                            op = ">";
                            break;
                        case BinaryComparison.BinaryComparisonType.GreaterThanOrEqual:
                            op = ">=";
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    sql.Append(comparison.LeftValue + op + comparison.RightValue + " ");
                }
                else if (item is SqlLiteralCondition)
                {
                    sql.Append(((SqlLiteralCondition)item).SQL.Trim() + " ");
                }

                isFirst = false;
            }
            return sql.ToString();
        }
    }
}
