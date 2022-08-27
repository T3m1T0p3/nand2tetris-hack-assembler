using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VmTranslator
{
    public class PushPopCommand
    {
        public string PushPop { get; set; }
        public string Segment { get; set; }
        public int Index { get; set; }
    }
}
