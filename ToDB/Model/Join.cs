using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDB.Model
{
    class Join
    {
        public enum JoinType
        {
            Inner,
            Left,
            Right,
            Full
        }

        public TableItem Table { get; set; }
        public ConditionBuilder On { get; set; }
        public JoinType TypeOfJoin { get; set; }
    }
}
