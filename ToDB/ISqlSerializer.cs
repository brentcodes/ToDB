using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDB
{
    public interface ISqlSerializer
    {
        string ToSql(Command command);
    }
}
