using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDB.Model
{
    class UninaryComparison
    {
        public enum UninaryComparisonType
        {
            IsNull,
            IsNotNull
        }

        public string Column { get; set; }
        public UninaryComparisonType TypeOfComparion { get; set; }

    }
}
