using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDB
{
    public interface IConjunction
    {
        ICondition And();
        ICondition Or();
        IConjunction And(Action<ICondition> comparisons);
        IConjunction Or(Action<ICondition> comparisons);
    }
}
