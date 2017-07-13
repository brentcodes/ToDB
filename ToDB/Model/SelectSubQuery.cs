using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDB.Model
{
    class SelectSubQuery
    {
        public ToDBCommand SubQuery { get; set; }
        public string Alias { get; set; }
    }
}
