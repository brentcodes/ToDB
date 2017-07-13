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
    public class Command : ICloneable
    {

        internal List<object> SelectItems { get; set; } = new List<object>();
        internal List<Join> Joins { get; set; } = new List<Join>();
        internal List<Union> Unions { get; set; } = new List<Model.Union>();
        internal ConditionBuilder WhereClause { get; set; } = new ConditionBuilder();
        internal Command UnionOriginator { get; set; }
        internal TableItem FromClause { get; set; }
        internal List<OrderBy> OrderBys { get; set; } = new List<Model.OrderBy>();
        internal List<GroupBy> GroupBys { get; set; } = new List<Model.GroupBy>();
        internal ConditionBuilder HavingClause { get; set; } = new ConditionBuilder();
        internal Insert InsertClause { get ; set; }
        internal List<Values> ValuesClause { get; set; } = new List<Model.Values>();
        internal TableItem UpdateClause { get; set; }
        internal List<SetItem> SetItems { get; set; } = new List<SetItem>();
        internal TableItem DeleteClause { get; set; }

        public Command Select(params string[] columns)
        {
            foreach (var item in columns)
            {
                SelectItems.Add(new SelectItem { Item = item.Trim() });
            }
            return this;
        }

        public Command Select(Action<Command> subQuery, string alias = null)
        {
            Command cmd = new Command();
            subQuery(cmd);
            SelectItems.Add(new SelectSubQuery { Alias = alias, SubQuery = cmd });
            return this;
        }

        public Command Select<T>()
        {
            return Select(
                            Utility.GetFieldsAndProperties(typeof(T))
                                   .Select(x => x.Name)
                                   .ToArray()
                         );
        }

        public Command Select(string columns)
        {
            return Select(columns.Split(','));
        }

        public Command From(string table)
        {
            FromClause = new TableItem { Item = table };
            return this;
        }

        public Command SelectFrom<T>()
        {
            From(typeof(T).Name);
            return Select<T>();
        }

        public Command Where(Action<ConditionBuilder> where)
        {
            where(WhereClause);
            return this;
        }
        public Command WhereAreEqual(string left, string right)
        {
            return Where(where => where.AreEqual(left, right));
        }

        public Command NaturalJoin<Table1,Table2>()
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
            return InnerJoin(t2.Name, where => where.AreEqual(t1.Name + "." + intersectingColumn, t2.Name + "." + intersectingColumn));            
        }

        public Command InnerJoin(string table, Action<ConditionBuilder> on)
        {
            return Join(table, on, Model.Join.JoinType.Inner);
        }
        public Command InnerJoin(string table, string leftColumn, string rightColumn)
        {
            return InnerJoin(table, on => on.AreEqual(leftColumn, rightColumn));
        }

        public Command LeftJoin(string table, Action<ConditionBuilder> on)
        {
            return Join(table, on, Model.Join.JoinType.Left);
        }
        public Command LeftJoin(string table, string leftColumn, string rightColumn)
        {
            return LeftJoin(table, on => on.AreEqual(leftColumn, rightColumn));
        }

        public Command RightJoin(string table, Action<ConditionBuilder> on)
        {
            return Join(table, on, Model.Join.JoinType.Right);
        }
        public Command RightJoin(string table, string leftColumn, string rightColumn)
        {
            return RightJoin(table, on => on.AreEqual(leftColumn, rightColumn));
        }

        public Command FullJoin(string table, Action<ConditionBuilder> on)
        {
            return Join(table, on, Model.Join.JoinType.Full);
        }
        public Command FullJoin(string table, string leftColumn, string rightColumn)
        {
            return FullJoin(table, on => on.AreEqual(leftColumn, rightColumn));
        }

        Command Join(string table, Action<ConditionBuilder> where, Join.JoinType joinType)
        {
            ConditionBuilder whereBuilder = new ConditionBuilder();
            where(whereBuilder);
            Joins.Add(new Join { Table = new TableItem { Item = table }, On = whereBuilder, TypeOfJoin = joinType });
            return this;
        }

        public Command Union(Action<Command> query)
        {
            return Union(query, Model.Union.UnionType.Union);
        }

        public Command UnionAll(Action<Command> query)
        {
            return Union(query, Model.Union.UnionType.UnionAll);
        }
        public Command Except(Action<Command> query)
        {
            return Union(query, Model.Union.UnionType.Except);
        }
        public Command Intersect(Action<Command> query)
        {
            return Union(query, Model.Union.UnionType.Intersect);
        }

        static Command UnionRoot(Command cmd) { return cmd.UnionOriginator == null ? cmd : UnionRoot(cmd.UnionOriginator); }
        Command Union(Action<Command> query, Union.UnionType unionType)
        {
            Command cmd = new Command();
            cmd.UnionOriginator = this;
            query(cmd);
            Command unionRoot = UnionRoot(this);
            unionRoot.Unions.Add(new Model.Union { Query = cmd, TypeOfUnion = unionType });
            return this;
        }

        public Command OrderByDesc(Expression<Func<object>> column)
        {
            return OrderByDesc(column.GetPropertyName());
        }
        public Command OrderByDesc(string column)
        {
            return OrderBy(column, true);
        }
        public Command OrderByAsc(Expression<Func<object>> column)
        {
            return OrderByAsc(column.GetPropertyName());
        }
        public Command OrderByAsc(string column)
        {
            return OrderBy(column, false);
        }
        Command OrderBy(string column, bool desc)
        {
            UnionRoot(this).OrderBys.Add(new Model.OrderBy { Column = column, Desc = desc });
            return this;
        } 

        public Command GroupBy(Expression<Func<object>> column)
        {
            return GroupBy(column.GetPropertyName());
        }
        public Command GroupBy(string column)
        {
            UnionRoot(this).GroupBys.Add(new Model.GroupBy { Column = column });
            return this;
        }

        Command Having(Action<ConditionBuilder> having)
        {
            having(UnionRoot(this).HavingClause);
            return this;
        }

        public Command Insert(string table, params string[] columns)
        {
            InsertClause = new Model.Insert { Columns = columns, Table = new TableItem { Item = table } };
            return this;
        }
        public Command Insert(string table)
        {
            return Insert(table, new string[0]);
        }
        public Command Insert(string table, string columns)
        {
            return Insert(table, columns.Split(','));
        }
        public Command Insert(string table, Type columns)
        {
            return Insert(table, Utility.GetFieldsAndProperties(columns).Select(x => x.Name).ToArray());
        }
        public Command Insert<T>(string table)
        {
            return Insert(table, typeof(T));
        }
        public Command Insert<T>()
        {
            return Insert<T>(typeof(T).Name);
        }
       
        public Command Values(params string[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                throw new ToDBException("No parameters passed");
            ValuesClause.Add(new Model.Values { Parameters = parameters });
            return this;
        }
        public Command Values(object obj)
        {
            return Values(Utility.GetFieldsAndProperties(obj.GetType()).Select(x => "@" + x.Name));
        }

        public Command InsertValues<TColumns>(string table, object values)
        {
            return Insert(table, typeof(TColumns))
                    .Values(values);
        }
        public Command InsertValues(string table, object columnsAndValues)
        {
            return Insert(table, columnsAndValues.GetType())
                    .Values(columnsAndValues);
        }

        public Command Update(string table)
        {
            UpdateClause = new TableItem { Item = table };
            return this;
        }

        public Command Set(string left, string right)
        {
            SetItems.Add(new SetItem { Left = left, Right = right });
            return this;
        }
        public Command Set(Expression<Func<object>> column, Expression<Func<object>> parameter)
        {
            return Set(Utility.GetPropertyName(column), Utility.ToSqlParameter(parameter));
        }
        public Command Set(Expression<Func<object>> columnAndParameter)
        {
            return Set(columnAndParameter, columnAndParameter);
        }
        public Command Set(object obj, string except)
        {
            foreach (string col in Utility.ExtractColumnNames(obj))
            {
                if (string.Equals(col, except, StringComparison.OrdinalIgnoreCase))
                    continue;
                Set(col, "@" + col);
            }
            return this;
        }
        public Command Set(object obj)
        {
            return Set(obj, (string)null);
        }
        public Command Set(object obj, Expression<Func<object>> except)
        {
            return Set(obj, except.GetPropertyName());
        }

        public Command Delete(string table)
        {
            DeleteClause = new TableItem { Item = table };
            return this;
        }

        object ICloneable.Clone()
        {
            Command clone = new Command
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
        public Command Clone()
        {
            return (Command)((ICloneable)this).Clone();
        }

    }
}