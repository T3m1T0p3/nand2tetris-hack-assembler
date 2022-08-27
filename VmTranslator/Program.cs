using System;
using System.IO;

namespace VmTranslator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Program");
            string sourcefile = args[0];
            VmCodeParser parser = new VmCodeParser(sourcefile);
            string outputFile = sourcefile.Replace(".vm", ".asm");
            Console.WriteLine(outputFile);
            CodeGenerator codeGenerator = new CodeGenerator(outputFile);
            
            Console.WriteLine("Objects initilized");
            while (!parser.HasMoreCommand())
            {
                parser.Advance();
                string arg0 = parser.Arg0();
                Console.WriteLine(arg0);
                if (parser.CommandType(arg0) == "C_ARITHMETIC")
                {
                    codeGenerator.WriteArithmetic(arg0,parser.lineCount);
                }
                else
                {
                    //Console.WriteLine(arg0+" Mem commmand");
                    var command = new PushPopCommand { Index = parser.Arg2(), Segment = parser.Arg1(), PushPop = arg0 };
                    codeGenerator.WritePushPop(command,parser.lineCount);
                }
            }
            parser.Close();
            codeGenerator.Close();
            
        }
    }
}
