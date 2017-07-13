using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDB.Model
{
    class Insert
    {
        public TableItem Table { get; set; }
        public IEnumerable<string> Columns { get; set; }
    }
}
