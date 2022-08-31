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
            VmCodeParser parser = new VmCodeParser();
            CodeGenerator codeGenerator = new CodeGenerator();
            string outputFile;
            if (Directory.Exists(sourcefile)){
                Console.WriteLine("A Dir");
                foreach (string file in Directory.GetFiles(sourcefile,"*.vm"))
                {
                    Console.WriteLine(file);
                    outputFile = file.Replace(".vm", ".asm");
                    parser.SetFileName(file);
                    codeGenerator.SetFileName(outputFile);
                    if(file.EndsWith("Sys.vm")) {
                           codeGenerator.WriteInit();
                    }
                    MainWriter(codeGenerator, parser);
                    codeGenerator.Close();
                        
                }
                
                codeGenerator.Merge(sourcefile);
                return;
            }
            parser.SetFileName(sourcefile);
            outputFile = sourcefile.Replace(".vm", ".asm");
            codeGenerator.SetFileName(outputFile);
            if (sourcefile.EndsWith("Sys.vm"))
            {
                codeGenerator.WriteInit();
            }
            MainWriter(codeGenerator, parser);

        }

        public static void MainWriter(CodeGenerator codeGenerator, VmCodeParser parser)
        {
            while (!parser.HasMoreCommand())
            {
                parser.Advance();
                string arg0 = parser.Arg0();
                string commandType = parser.CommandType(arg0);
                if (parser.CommandType(arg0) == "C_ARITHMETIC")
                {
                    codeGenerator.WriteArithmetic(arg0, parser.lineCount);
                }
                else if (commandType == "LABEL")
                {
                    codeGenerator.WriteLabel(parser.Arg1());
                }
                else if (commandType == "IF-GOTO")
                {
                    codeGenerator.WriteIf(parser.Arg1());
                }
                else if (commandType == "GOTO")
                {
                    codeGenerator.WriteGoTo(parser.Arg1());
                }
                else if (commandType == "FUNC_CALL")
                {
                    codeGenerator.WriteCall(parser.Arg1(), parser.Arg2());
                }
                else if (commandType == "FUNC_DEFINITION")
                {
                    codeGenerator.WriteFunction(parser.Arg1(), parser.Arg2());
                }
                else if (commandType == "RETURN")
                {
                    codeGenerator.WriteReturn();
                }
                else
                {
                    //Console.WriteLine(arg0+" Mem commmand");
                    var command = new PushPopCommand { Index = parser.Arg2(), Segment = parser.Arg1(), PushPop = arg0 };
                    Console.WriteLine($"pushpop with {command.Index} {command.Segment} {command.PushPop}");
                    codeGenerator.WritePushPop(command, parser.lineCount);
                }
            }
            parser.Close();
            codeGenerator.Close();
        }
    }

    
 
}
