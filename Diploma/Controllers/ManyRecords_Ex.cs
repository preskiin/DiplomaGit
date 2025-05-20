using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diploma.Controllers
{
    internal class ManyRecords_Ex : Exception
    {
        public ManyRecords_Ex() { }
        public ManyRecords_Ex(string message) : base(message) { }
    }

}
