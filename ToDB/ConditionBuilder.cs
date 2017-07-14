using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ToDB.Model;

namespace ToDB
{
    public class ConditionBuilder : ICondition, IConjunction
    {
        protected List<object> _items = new List<object>();
        internal List<object> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public ICondition And()
        {
            Items.Add(new Conjunction { TypeOfConjunction = Conjunction.ConjuntionType.And });
            return this;
        }
        public IConjunction And(Action<ICondition> comparisons)
        {
            And();
            var where = new ConditionBuilder();
            Items.Add(where);
            comparisons(where);
            return this;
        }        

        public IConjunction IsNull<T>(Expression<Func<T>> column)
        {
            return IsNull(column.ToSqlParameter());
        }
        public IConjunction IsNull(string column)
        {
            Items.Add(new UninaryComparison { Column = column, TypeOfComparion = UninaryComparison.UninaryComparisonType.IsNull });
            return this;
        }

        public IConjunction IsNotNull<T>(Expression<Func<T>> column)
        {
            return IsNotNull(column.ToSqlParameter());
        }
        public IConjunction IsNotNull(string column)
        {
            Items.Add(new UninaryComparison { Column = column, TypeOfComparion = UninaryComparison.UninaryComparisonType.IsNotNull });
            return this;
        }

        public IConjunction LiteralCondition(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new NullReferenceException("sql cannot be null, empty, or whitespace");
            Items.Add(new SqlLiteralCondition { SQL = sql });
            return this;
        }

        public ICondition Or()
        {
            Items.Add(new Conjunction { TypeOfConjunction = Conjunction.ConjuntionType.Or });
            return this;
        }
        public IConjunction Or(Action<ICondition> comparisons)
        {
            Or();
            var where = new ConditionBuilder();
            Items.Add(where);
            comparisons(where);
            return this;
        }

        IConjunction Compare(string left, string right, BinaryComparison.BinaryComparisonType type)
        {
            Items.Add(new BinaryComparison { LeftValue = left, RightValue = right, TypeOfComparison = type });
            return this;
        }

        public IConjunction IsEqual(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.Equal);
        }
        public IConjunction IsEqual<T>(string column, Expression<Func<T>> parameter)
        {
            return IsEqual(column, parameter.ToSqlParameter());
        }
        public IConjunction IsEqual<T1, T2>(Expression<Func<T1>> column, Expression<Func<T2>> parameter)
        {
            return IsEqual(column.GetPropertyName(), parameter);
        }
        public IConjunction IsEqual<T>(Expression<Func<T>> columnAndParameter)
        {
            return IsEqual(columnAndParameter, columnAndParameter);
        }
        public IConjunction AreEqual<T>(string leftColumn, Expression<Func<T>> rightColumn)
        {
            return IsEqual(leftColumn, rightColumn.GetPropertyName());
        }
        public IConjunction AreEqual<T1, T2>(Expression<Func<T1>> leftColumn, Expression<Func<T2>> rightColumn)
        {
            return AreEqual(leftColumn.GetPropertyName(), rightColumn);
        }
        public IConjunction AreEqual<T>(Expression<Func<T>> columns)
        {
            return AreEqual(columns, columns);
        }

        public IConjunction IsLike(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.Like);
        }
        public IConjunction IsLike<T>(string column, Expression<Func<T>> parameter)
        {
            return IsLike(column, parameter.ToSqlParameter());
        }
        public IConjunction IsLike<T1, T2>(Expression<Func<T1>> column, Expression<Func<T2>> parameter)
        {
            return IsLike(column.GetPropertyName(), parameter);
        }
        public IConjunction IsLike<T>(Expression<Func<T>> columnAndParameter)
        {
            return IsLike(columnAndParameter, columnAndParameter);
        }

        public IConjunction IsGreater(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.GreaterThan);
        }
        public IConjunction IsGreater<T>(string column, Expression<Func<T>> parameter)
        {
            return IsGreater(column, parameter.ToSqlParameter());
        }
        public IConjunction IsGreater<T1, T2>(Expression<Func<T1>> column, Expression<Func<T2>> parameter)
        {
            return IsGreater(column.GetPropertyName(), parameter);
        }
        public IConjunction IsGreater<T>(Expression<Func<T>> columnAndParameter)
        {
            return IsGreater(columnAndParameter, columnAndParameter);
        }

        public IConjunction IsGreaterOrEqual(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.GreaterThanOrEqual);
        }
        public IConjunction IsGreaterOrEqual<T>(string column, Expression<Func<T>> parameter)
        {
            return IsGreaterOrEqual(column, parameter.ToSqlParameter());
        }
        public IConjunction IsGreaterOrEqual<T>(Expression<Func<T>> column, Expression<Func<T>> parameter)
        {
            return IsGreaterOrEqual(column.GetPropertyName(), parameter);
        }
        public IConjunction IsGreaterOrEqual<T>(Expression<Func<T>> columnAndParameter)
        {
            return IsGreaterOrEqual(columnAndParameter, columnAndParameter);
        }

        public IConjunction IsLess(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.LessThan);
        }
        public IConjunction IsLess<T>(string column, Expression<Func<T>> parameter)
        {
            return IsLess(column, parameter.ToSqlParameter());
        }
        public IConjunction IsLess<T>(Expression<Func<T>> column, Expression<Func<T>> parameter)
        {
            return IsLess(column.GetPropertyName(), parameter);
        }
        public IConjunction IsLess<T>(Expression<Func<T>> columnAndParameter)
        {
            return IsLess(columnAndParameter, columnAndParameter);
        }

        public IConjunction IsLessOrEqual(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.LessThanOrEqual);
        }
        public IConjunction IsLessOrEqual<T>(string column, Expression<Func<T>> parameter)
        {
            return IsLessOrEqual(column, parameter.ToSqlParameter());
        }
        public IConjunction IsLessOrEqual<T>(Expression<Func<T>> column, Expression<Func<T>> parameter)
        {
            return IsLessOrEqual(column.GetPropertyName(), parameter);
        }
        public IConjunction IsLessOrEqual<T>(Expression<Func<T>> columnAndParameter)
        {
            return IsLessOrEqual(columnAndParameter, columnAndParameter);
        }

        public IConjunction NotEqual(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.NotEqual);
        }
        public IConjunction NotEqual<T>(string column, Expression<Func<T>> parameter)
        {
            return NotEqual(column, parameter.ToSqlParameter());
        }
        public IConjunction NotEqual<T>(Expression<Func<T>> column, Expression<Func<T>> parameter)
        {
            return NotEqual(column.GetPropertyName(), parameter);
        }
        public IConjunction NotEqual<T>(Expression<Func<T>> columnAndParameter)
        {
            return NotEqual(columnAndParameter, columnAndParameter);
        }
    }
}
