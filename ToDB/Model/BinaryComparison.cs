using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDB.Model
{
    class BinaryComparison
    {
        public enum BinaryComparisonType
        {
            Equal,
            NotEqual,
            Like,
            LessThan,
            LessThanOrEqual,
            GreaterThan,
            GreaterThanOrEqual            
        }

        public string LeftValue { get; set; }
        public string RightValue { get; set; }
        public BinaryComparisonType TypeOfComparison { get; set; }
    }
}
