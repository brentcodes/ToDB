using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ToDB.Model;

namespace ToDB
{
    public class ConditionBuilder
    {
        protected List<object> _items = new List<object>();
        internal List<object> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public ConditionBuilder And()
        {
            Items.Add(new Conjunction { TypeOfConjunction = Conjunction.ConjuntionType.And });
            return this;
        }
        public ConditionBuilder And(Action<ConditionBuilder> comparisons)
        {
            And();
            var where = new ConditionBuilder();
            Items.Add(where);
            comparisons(where);
            return this;
        }        

        public ConditionBuilder IsNull<T>(Expression<Func<T>> column)
        {
            return IsNull(column.ToSqlParameter());
        }
        public ConditionBuilder IsNull(string column)
        {
            Items.Add(new UninaryComparison { Column = column, TypeOfComparion = UninaryComparison.UninaryComparisonType.IsNull });
            return this;
        }

        public ConditionBuilder IsNotNull<T>(Expression<Func<T>> column)
        {
            return IsNotNull(column.ToSqlParameter());
        }
        public ConditionBuilder IsNotNull(string column)
        {
            Items.Add(new UninaryComparison { Column = column, TypeOfComparion = UninaryComparison.UninaryComparisonType.IsNotNull });
            return this;
        }

        public ConditionBuilder LiteralCondition(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                throw new NullReferenceException("sql cannot be null, empty, or whitespace");
            Items.Add(new SqlLiteralCondition { SQL = sql });
            return this;
        }

        public ConditionBuilder Or()
        {
            Items.Add(new Conjunction { TypeOfConjunction = Conjunction.ConjuntionType.Or });
            return this;
        }
        public ConditionBuilder Or(Action<ConditionBuilder> comparisons)
        {
            Or();
            var where = new ConditionBuilder();
            Items.Add(where);
            comparisons(where);
            return this;
        }

        ConditionBuilder Compare(string left, string right, BinaryComparison.BinaryComparisonType type)
        {
            Items.Add(new BinaryComparison { LeftValue = left, RightValue = right, TypeOfComparison = type });
            return this;
        }

        public ConditionBuilder IsEqual(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.Equal);
        }
        public ConditionBuilder IsEqual<T>(string column, Expression<Func<T>> parameter)
        {
            return IsEqual(column, parameter.ToSqlParameter());
        }
        public ConditionBuilder IsEqual<T1, T2>(Expression<Func<T1>> column, Expression<Func<T2>> parameter)
        {
            return IsEqual(column.GetPropertyName(), parameter);
        }
        public ConditionBuilder IsEqual<T>(Expression<Func<T>> columnAndParameter)
        {
            return IsEqual(columnAndParameter, columnAndParameter);
        }
        public ConditionBuilder AreEqual<T>(string leftColumn, Expression<Func<T>> rightColumn)
        {
            return IsEqual(leftColumn, rightColumn.GetPropertyName());
        }
        public ConditionBuilder AreEqual<T1, T2>(Expression<Func<T1>> leftColumn, Expression<Func<T2>> rightColumn)
        {
            return AreEqual(leftColumn.GetPropertyName(), rightColumn);
        }
        public ConditionBuilder AreEqual<T>(Expression<Func<T>> columns)
        {
            return AreEqual(columns, columns);
        }

        public ConditionBuilder IsLike(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.Like);
        }
        public ConditionBuilder IsLike<T>(string column, Expression<Func<T>> parameter)
        {
            return IsLike(column, parameter.ToSqlParameter());
        }
        public ConditionBuilder IsLike<T1, T2>(Expression<Func<T1>> column, Expression<Func<T2>> parameter)
        {
            return IsLike(column.GetPropertyName(), parameter);
        }
        public ConditionBuilder IsLike<T>(Expression<Func<T>> columnAndParameter)
        {
            return IsLike(columnAndParameter, columnAndParameter);
        }

        public ConditionBuilder IsGreater(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.GreaterThan);
        }
        public ConditionBuilder IsGreater<T>(string column, Expression<Func<T>> parameter)
        {
            return IsGreater(column, parameter.ToSqlParameter());
        }
        public ConditionBuilder IsGreater<T1, T2>(Expression<Func<T1>> column, Expression<Func<T2>> parameter)
        {
            return IsGreater(column.GetPropertyName(), parameter);
        }
        public ConditionBuilder IsGreater<T>(Expression<Func<T>> columnAndParameter)
        {
            return IsGreater(columnAndParameter, columnAndParameter);
        }

        public ConditionBuilder IsGreaterOrEqual(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.GreaterThanOrEqual);
        }
        public ConditionBuilder IsGreaterOrEqual<T>(string column, Expression<Func<T>> parameter)
        {
            return IsGreaterOrEqual(column, parameter.ToSqlParameter());
        }
        public ConditionBuilder IsGreaterOrEqual<T>(Expression<Func<T>> column, Expression<Func<T>> parameter)
        {
            return IsGreaterOrEqual(column.GetPropertyName(), parameter);
        }
        public ConditionBuilder IsGreaterOrEqual<T>(Expression<Func<T>> columnAndParameter)
        {
            return IsGreaterOrEqual(columnAndParameter, columnAndParameter);
        }

        public ConditionBuilder IsLess(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.LessThan);
        }
        public ConditionBuilder IsLess<T>(string column, Expression<Func<T>> parameter)
        {
            return IsLess(column, parameter.ToSqlParameter());
        }
        public ConditionBuilder IsLess<T>(Expression<Func<T>> column, Expression<Func<T>> parameter)
        {
            return IsLess(column.GetPropertyName(), parameter);
        }
        public ConditionBuilder IsLess<T>(Expression<Func<T>> columnAndParameter)
        {
            return IsLess(columnAndParameter, columnAndParameter);
        }

        public ConditionBuilder IsLessOrEqual(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.LessThanOrEqual);
        }
        public ConditionBuilder IsLessOrEqual<T>(string column, Expression<Func<T>> parameter)
        {
            return IsLessOrEqual(column, parameter.ToSqlParameter());
        }
        public ConditionBuilder IsLessOrEqual<T>(Expression<Func<T>> column, Expression<Func<T>> parameter)
        {
            return IsLessOrEqual(column.GetPropertyName(), parameter);
        }
        public ConditionBuilder IsLessOrEqual<T>(Expression<Func<T>> columnAndParameter)
        {
            return IsLessOrEqual(columnAndParameter, columnAndParameter);
        }

        public ConditionBuilder NotEqual(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.NotEqual);
        }
        public ConditionBuilder NotEqual<T>(string column, Expression<Func<T>> parameter)
        {
            return NotEqual(column, parameter.ToSqlParameter());
        }
        public ConditionBuilder NotEqual<T>(Expression<Func<T>> column, Expression<Func<T>> parameter)
        {
            return NotEqual(column.GetPropertyName(), parameter);
        }
        public ConditionBuilder NotEqual<T>(Expression<Func<T>> columnAndParameter)
        {
            return NotEqual(columnAndParameter, columnAndParameter);
        }
    }
}
