using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ToDB.Model;

namespace ToDB
{
    public class ToDBCommand : ICloneable
    {

        internal List<object> SelectItems { get; set; } = new List<object>();
        internal List<Join> Joins { get; set; } = new List<Join>();
        internal List<Union> Unions { get; set; } = new List<Model.Union>();
        internal ConditionBuilder WhereClause { get; set; } = new ConditionBuilder();
        internal ToDBCommand UnionOriginator { get; set; }
        internal TableItem FromClause { get; set; }
        internal List<OrderBy> OrderBys { get; set; } = new List<Model.OrderBy>();
        internal List<GroupBy> GroupBys { get; set; } = new List<Model.GroupBy>();
        internal ConditionBuilder HavingClause { get; set; } = new ConditionBuilder();
        internal Insert InsertClause { get ; set; }
        internal List<Values> ValuesClause { get; set; } = new List<Model.Values>();
        internal TableItem UpdateClause { get; set; }
        internal List<SetItem> SetItems { get; set; } = new List<SetItem>();
        internal TableItem DeleteClause { get; set; }

        public ToDBCommand Select(params string[] columns)
        {
            foreach (var item in columns)
            {
                SelectItems.Add(new SelectItem { Item = item.Trim() });
            }
            return this;
        }

        public ToDBCommand Select(Action<ToDBCommand> subQuery, string alias = null)
        {
            ToDBCommand cmd = new ToDBCommand();
            subQuery(cmd);
            SelectItems.Add(new SelectSubQuery { Alias = alias, SubQuery = cmd });
            return this;
        }

        public ToDBCommand Select<T>()
        {
            return Select(
                            Utility.GetFieldsAndProperties(typeof(T))
                                   .Select(x => x.Name)
                                   .ToArray()
                         );
        }

        public ToDBCommand Select(string columns)
        {
            return Select(columns.Split(','));
        }

        public ToDBCommand From(string table)
        {
            FromClause = new TableItem { Item = table };
            return this;
        }

        public ToDBCommand SelectAllFrom(string table)
        {
            return Select("*")
                    .From(table);
        }

        public ToDBCommand SelectFrom<T>()
        {
            From(typeof(T).Name);
            return Select<T>();
        }

        public ToDBCommand Where(Action<ConditionBuilder> where)
        {
            where(WhereClause);
            return this;
        }
        public ToDBCommand WhereAreEqual(string left, string right)
        {
            return Where(where => where.IsEqual(left, right));
        }

        public ToDBCommand NaturalJoin<Table1,Table2>()
        {
            var t1 = typeof(Table1);
            var t2 = typeof(Table2);
            var intersection = Utility.GetFieldsAndProperties(t1)
                    .Intersect(Utility.GetFieldsAndProperties(t2));
            if (intersection.Count() != 1)
                throw new ToDBException("To do an automatic natural join, the types " + t1.Name + " and " + t2.Name + " must have exactly one property or field with the same name and type; instead, they have " + intersection.Count() + " in common. Use an explicit inner join instead.");
            string intersectingColumn = intersection.Single().Name;
            if (FromClause == null)
                From(t1.Name);
            return InnerJoin(t2.Name, where => where.IsEqual(t1.Name + "." + intersectingColumn, t2.Name + "." + intersectingColumn));            
        }

        public ToDBCommand InnerJoin(string table, Action<ConditionBuilder> on)
        {
            return Join(table, on, Model.Join.JoinType.Inner);
        }
        public ToDBCommand InnerJoin(string table, string leftColumn, string rightColumn)
        {
            return InnerJoin(table, on => on.IsEqual(leftColumn, rightColumn));
        }

        public ToDBCommand LeftJoin(string table, Action<ConditionBuilder> on)
        {
            return Join(table, on, Model.Join.JoinType.Left);
        }
        public ToDBCommand LeftJoin(string table, string leftColumn, string rightColumn)
        {
            return LeftJoin(table, on => on.IsEqual(leftColumn, rightColumn));
        }

        public ToDBCommand RightJoin(string table, Action<ConditionBuilder> on)
        {
            return Join(table, on, Model.Join.JoinType.Right);
        }
        public ToDBCommand RightJoin(string table, string leftColumn, string rightColumn)
        {
            return RightJoin(table, on => on.IsEqual(leftColumn, rightColumn));
        }

        public ToDBCommand FullJoin(string table, Action<ConditionBuilder> on)
        {
            return Join(table, on, Model.Join.JoinType.Full);
        }
        public ToDBCommand FullJoin(string table, string leftColumn, string rightColumn)
        {
            return FullJoin(table, on => on.IsEqual(leftColumn, rightColumn));
        }

        ToDBCommand Join(string table, Action<ConditionBuilder> where, Join.JoinType joinType)
        {
            ConditionBuilder whereBuilder = new ConditionBuilder();
            where(whereBuilder);
            Joins.Add(new Join { Table = new TableItem { Item = table }, On = whereBuilder, TypeOfJoin = joinType });
            return this;
        }

        public ToDBCommand Union(Action<ToDBCommand> query)
        {
            return Union(query, Model.Union.UnionType.Union);
        }

        public ToDBCommand UnionAll(Action<ToDBCommand> query)
        {
            return Union(query, Model.Union.UnionType.UnionAll);
        }
        public ToDBCommand Except(Action<ToDBCommand> query)
        {
            return Union(query, Model.Union.UnionType.Except);
        }
        public ToDBCommand Intersect(Action<ToDBCommand> query)
        {
            return Union(query, Model.Union.UnionType.Intersect);
        }

        static ToDBCommand UnionRoot(ToDBCommand cmd) { return cmd.UnionOriginator == null ? cmd : UnionRoot(cmd.UnionOriginator); }
        ToDBCommand Union(Action<ToDBCommand> query, Union.UnionType unionType)
        {
            ToDBCommand cmd = new ToDBCommand();
            cmd.UnionOriginator = this;
            query(cmd);
            ToDBCommand unionRoot = UnionRoot(this);
            unionRoot.Unions.Add(new Model.Union { Query = cmd, TypeOfUnion = unionType });
            return this;
        }

        public ToDBCommand OrderByDesc<T>(Expression<Func<T>> column)
        {
            return OrderByDesc(column.GetPropertyName());
        }
        public ToDBCommand OrderByDesc(string column)
        {
            return OrderBy(column, true);
        }
        public ToDBCommand OrderByAsc<T>(Expression<Func<T>> column)
        {
            return OrderByAsc(column.GetPropertyName());
        }
        public ToDBCommand OrderByAsc(string column)
        {
            return OrderBy(column, false);
        }
        ToDBCommand OrderBy(string column, bool desc)
        {
            UnionRoot(this).OrderBys.Add(new Model.OrderBy { Column = column, Desc = desc });
            return this;
        } 

        public ToDBCommand GroupBy<T>(Expression<Func<T>> column)
        {
            return GroupBy(column.GetPropertyName());
        }
        public ToDBCommand GroupBy(string column)
        {
            UnionRoot(this).GroupBys.Add(new Model.GroupBy { Column = column });
            return this;
        }

        ToDBCommand Having(Action<ConditionBuilder> having)
        {
            having(UnionRoot(this).HavingClause);
            return this;
        }

        public ToDBCommand Insert(string table, params string[] columns)
        {
            InsertClause = new Model.Insert { Columns = columns, Table = new TableItem { Item = table } };
            return this;
        }
        public ToDBCommand Insert(string table)
        {
            return Insert(table, new string[0]);
        }
        public ToDBCommand Insert(string table, string columns)
        {
            return Insert(table, columns.Split(','));
        }
        public ToDBCommand Insert(string table, Type columns)
        {
            return Insert(table, Utility.GetFieldsAndProperties(columns).Select(x => x.Name).ToArray());
        }
        public ToDBCommand Insert<T>(string table)
        {
            return Insert(table, typeof(T));
        }
        public ToDBCommand Insert<T>()
        {
            return Insert<T>(typeof(T).Name);
        }
       
        public ToDBCommand Values(params string[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                throw new ToDBException("No parameters passed");
            ValuesClause.Add(new Model.Values { Parameters = parameters });
            return this;
        }
        public ToDBCommand Values(object obj)
        {
            return Values(Utility.GetFieldsAndProperties(obj.GetType()).Select(x => "@" + x.Name));
        }

        public ToDBCommand InsertValues<TColumns>(string table, object values)
        {
            return Insert(table, typeof(TColumns))
                    .Values(values);
        }
        public ToDBCommand InsertValues(string table, object columnsAndValues)
        {
            return Insert(table, columnsAndValues.GetType())
                    .Values(columnsAndValues);
        }

        public ToDBCommand Update(string table)
        {
            UpdateClause = new TableItem { Item = table };
            return this;
        }

        public ToDBCommand Set(string left, string right)
        {
            SetItems.Add(new SetItem { Left = left, Right = right });
            return this;
        }
        public ToDBCommand Set<T1, T2>(Expression<Func<T1>> column, Expression<Func<T2>> parameter)
        {
            return Set(Utility.GetPropertyName(column), Utility.ToSqlParameter(parameter));
        }
        public ToDBCommand Set<T>(Expression<Func<T>> columnAndParameter)
        {
            return Set(columnAndParameter, columnAndParameter);
        }
        public ToDBCommand Set(object obj, string except)
        {
            foreach (string col in Utility.ExtractColumnNames(obj))
            {
                if (string.Equals(col, except, StringComparison.OrdinalIgnoreCase))
                    continue;
                Set(col, "@" + col);
            }
            return this;
        }
        public ToDBCommand Set(object obj)
        {
            return Set(obj, (string)null);
        }
        public ToDBCommand Set<T>(object obj, Expression<Func<T>> except)
        {
            return Set(obj, except.GetPropertyName());
        }

        public ToDBCommand Delete(string table)
        {
            DeleteClause = new TableItem { Item = table };
            return this;
        }

        object ICloneable.Clone()
        {
            ToDBCommand clone = new ToDBCommand
            {
                FromClause = FromClause,
                Joins = new List<Model.Join>(Joins),
                Unions = new List<Model.Union>(Unions),
                SelectItems = new List<object>(SelectItems),
                WhereClause = new ConditionBuilder { Items = new List<object>(WhereClause.Items) },
                OrderBys = new List<Model.OrderBy>(OrderBys),
                GroupBys = new List<Model.GroupBy>(GroupBys),
                HavingClause = new ConditionBuilder { Items = new List<object>(HavingClause.Items) },
                InsertClause = new Model.Insert { Columns = new List<string>(InsertClause.Columns), Table = InsertClause.Table },
                ValuesClause = new List<Model.Values>(ValuesClause)
            };
            return clone;
        }
        public ToDBCommand Clone()
        {
            return (ToDBCommand)((ICloneable)this).Clone();
        }

    }
}