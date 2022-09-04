using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackCompiler
{
    public class JackAnalyzer
    {
        public static void Main(string[] args)
        {
            string path = args[0];
            //Tokenizer tokenizer = new Tokenizer(path);
            CompilationEngine engine = new CompilationEngine(path);
        }
    }
}
