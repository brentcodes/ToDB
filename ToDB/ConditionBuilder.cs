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

        public IConjunction IsNull(Expression<Func<object>> column)
        {
            return IsNull(column.ToSqlParameter());
        }
        public IConjunction IsNull(string column)
        {
            Items.Add(new UninaryComparison { Column = column, TypeOfComparion = UninaryComparison.UninaryComparisonType.IsNull });
            return this;
        }

        public IConjunction IsNotNull(Expression<Func<object>> column)
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

        public IConjunction AreEqual(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.Equal);
        }
        public IConjunction IsEqual(string column, Expression<Func<object>> parameter)
        {
            return AreEqual(column, parameter.ToSqlParameter());
        }
        public IConjunction IsEqual(Expression<Func<object>> column, Expression<Func<object>> parameter)
        {
            return IsEqual(column.GetPropertyName(), parameter);
        }
        public IConjunction IsEqual(Expression<Func<object>> columnAndParameter)
        {
            return IsEqual(columnAndParameter, columnAndParameter);
        }
        public IConjunction AreEqual(string leftColumn, Expression<Func<object>> rightColumn)
        {
            return AreEqual(leftColumn, rightColumn.GetPropertyName());
        }
        public IConjunction AreEqual(Expression<Func<object>> leftColumn, Expression<Func<object>> rightColumn)
        {
            return AreEqual(leftColumn.GetPropertyName(), rightColumn);
        }
        public IConjunction AreEqual(Expression<Func<object>> columns)
        {
            return AreEqual(columns, columns);
        }

        public IConjunction IsLike(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.Like);
        }
        public IConjunction IsLike(string column, Expression<Func<object>> parameter)
        {
            return IsLike(column, parameter.ToSqlParameter());
        }
        public IConjunction IsLike(Expression<Func<object>> column, Expression<Func<object>> parameter)
        {
            return IsLike(column.GetPropertyName(), parameter);
        }
        public IConjunction IsLike(Expression<Func<object>> columnAndParameter)
        {
            return IsLike(columnAndParameter, columnAndParameter);
        }

        public IConjunction IsGreater(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.GreaterThan);
        }
        public IConjunction IsGreater(string column, Expression<Func<object>> parameter)
        {
            return IsGreater(column, parameter.ToSqlParameter());
        }
        public IConjunction IsGreater(Expression<Func<object>> column, Expression<Func<object>> parameter)
        {
            return IsGreater(column.GetPropertyName(), parameter);
        }
        public IConjunction IsGreater(Expression<Func<object>> columnAndParameter)
        {
            return IsGreater(columnAndParameter, columnAndParameter);
        }

        public IConjunction IsGreaterOrEqual(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.GreaterThanOrEqual);
        }
        public IConjunction IsGreaterOrEqual(string column, Expression<Func<object>> parameter)
        {
            return IsGreaterOrEqual(column, parameter.ToSqlParameter());
        }
        public IConjunction IsGreaterOrEqual(Expression<Func<object>> column, Expression<Func<object>> parameter)
        {
            return IsGreaterOrEqual(column.GetPropertyName(), parameter);
        }
        public IConjunction IsGreaterOrEqual(Expression<Func<object>> columnAndParameter)
        {
            return IsGreaterOrEqual(columnAndParameter, columnAndParameter);
        }

        public IConjunction IsLess(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.LessThan);
        }
        public IConjunction IsLess(string column, Expression<Func<object>> parameter)
        {
            return IsLess(column, parameter.ToSqlParameter());
        }
        public IConjunction IsLess(Expression<Func<object>> column, Expression<Func<object>> parameter)
        {
            return IsLess(column.GetPropertyName(), parameter);
        }
        public IConjunction IsLess(Expression<Func<object>> columnAndParameter)
        {
            return IsLess(columnAndParameter, columnAndParameter);
        }

        public IConjunction IsLessOrEqual(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.LessThanOrEqual);
        }
        public IConjunction IsLessOrEqual(string column, Expression<Func<object>> parameter)
        {
            return IsLessOrEqual(column, parameter.ToSqlParameter());
        }
        public IConjunction IsLessOrEqual(Expression<Func<object>> column, Expression<Func<object>> parameter)
        {
            return IsLessOrEqual(column.GetPropertyName(), parameter);
        }
        public IConjunction IsLessOrEqual(Expression<Func<object>> columnAndParameter)
        {
            return IsLessOrEqual(columnAndParameter, columnAndParameter);
        }

        public IConjunction NotEqual(string left, string right)
        {
            return Compare(left, right, BinaryComparison.BinaryComparisonType.NotEqual);
        }
        public IConjunction NotEqual(string column, Expression<Func<object>> parameter)
        {
            return NotEqual(column, parameter.ToSqlParameter());
        }
        public IConjunction NotEqual(Expression<Func<object>> column, Expression<Func<object>> parameter)
        {
            return NotEqual(column.GetPropertyName(), parameter);
        }
        public IConjunction NotEqual(Expression<Func<object>> columnAndParameter)
        {
            return NotEqual(columnAndParameter, columnAndParameter);
        }
    }
}
