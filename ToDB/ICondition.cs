using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ToDB
{
    public interface ICondition
    {
        IConjunction IsNull(string column);
        IConjunction IsNull(Expression<Func<object>> column);
        IConjunction IsEqual(string column, Expression<Func<object>> paramater);        
        IConjunction AreEqual(string left, string right);
        IConjunction IsEqual(Expression<Func<object>> column, Expression<Func<object>> paramater);
        IConjunction IsEqual(Expression<Func<object>> columnAndParameter);
        IConjunction LiteralCondition(string sql);


        


    }
}
