using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDB.Model
{
    class Union
    {
        public enum UnionType
        {
            Union,
            UnionAll,
            Intersect,
            Except
        }

        public UnionType TypeOfUnion { get; set; }
        public Command Query { get; set; }
    }
}
