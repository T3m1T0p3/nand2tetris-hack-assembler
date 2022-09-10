using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace JackCompiler
{
    public class JackCompiler
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Starting Program");

            string sourcefile = args[0];
            CompilationEngine compilationEngine;
            if (Directory.Exists(sourcefile))
            {
                foreach (string file in Directory.GetFiles(sourcefile, "*.jack"))
                {
                    compilationEngine = new CompilationEngine(file);
                }
                return;
            }
            compilationEngine = new CompilationEngine(sourcefile);
        }
    }
}
