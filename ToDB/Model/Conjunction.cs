using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDB.Model
{
    class Conjunction
    {
        public ConjuntionType TypeOfConjunction { get; set; }
        public enum ConjuntionType
        {
            And,
            Or
        }
    }
}
