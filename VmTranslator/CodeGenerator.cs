using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VmTranslator
{
    public class CodeGenerator
    {
        private readonly string _filePath;
        StreamWriter writer;
        string currLine;
        string _staticVar;

        Dictionary<string,string> ramMap = new Dictionary<string, string>();
        public CodeGenerator(string filePath)
        {
            _filePath = filePath;
            string[] paths = _filePath.Split('\\');
            _staticVar = paths[paths.Length - 1];

            writer = new StreamWriter(filePath, false);
            ramMap.Add("local","LCL");
            ramMap.Add("argument","ARG");
            ramMap.Add("this", "THIS");
            ramMap.Add("that", "THAT");
            Console.WriteLine("Generator init");

        }

        public void WritePushPop(PushPopCommand command,int lineCount)
        {
            Console.WriteLine(command);
            string staticVarName = _staticVar.Replace(".asm", $".{command.Index}");

            if (command.PushPop.ToLower() == "push"){
                writer.WriteLine($"// {command.PushPop} {command.Segment} {command.Index}");
                if (command.Segment == "static")
                {   
                    writer.WriteLine($"@{staticVarName}");
                    writer.WriteLine("D=M");
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M");
                    writer.WriteLine("M=D");

                    writer.WriteLine("@1");
                    writer.WriteLine("D=A");
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M+D");

                    return;
                }

                if (command.Segment == "temp")
                {
                    writer.WriteLine($"@5");
                    writer.WriteLine("D=A");
                    writer.WriteLine($"@{command.Index}");
                    writer.WriteLine("D=D+A");
                    writer.WriteLine("@R6000");
                    writer.WriteLine("M=D");
                    writer.WriteLine("A=M");
                    writer.WriteLine("D=M");
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M");
                    writer.WriteLine("M=D");

                    writer.WriteLine("@1");
                    writer.WriteLine("D=A");
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M+D");
                    return;
                }
                if (command.Segment == "pointer")
                {
                    switch (command.Index)
                    {
                        case 0:
                            writer.WriteLine("@THIS");
                            writer.WriteLine("D=M");
                            writer.WriteLine("@SP");
                            writer.WriteLine("A=M");
                            writer.WriteLine("M=D");
                            

                            writer.WriteLine("@1");
                            writer.WriteLine("D=A");
                            writer.WriteLine("@SP");
                            writer.WriteLine("M=M+D");
                            break;
                        case 1:
                            writer.WriteLine("@THAT");
                            writer.WriteLine("D=M");
                            writer.WriteLine("@SP");
                            writer.WriteLine("A=M");
                            writer.WriteLine("M=D");

                            writer.WriteLine("@1");
                            writer.WriteLine("D=A");
                            writer.WriteLine("@SP");
                            writer.WriteLine("M=M+D");
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    return;

                }
                writer.WriteLine($"@{command.Index}");
                writer.WriteLine("D=A");
                if (command.Segment == "constant")
                {
                    writer.WriteLine($"@SP");
                    writer.WriteLine("A=M");
                    writer.WriteLine("M=D");

                    writer.WriteLine("@1");
                    writer.WriteLine("D=A");
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M+D");
                    return;
                }


                writer.WriteLine($"@{ramMap[command.Segment]}");
                writer.WriteLine("A=M+D");
                writer.WriteLine("D=M");
                writer.WriteLine($"@SP");
                writer.WriteLine("A=M");
                writer.WriteLine("M=D");

                writer.WriteLine("@1");
                writer.WriteLine("D=A");
                writer.WriteLine("@SP");
                writer.WriteLine("M=M+D");
                //stackPointer++;
            }

            else if(command.PushPop.ToLower()=="pop")
            {
                //stackPointer--;
                writer.WriteLine($"// {command.PushPop} {command.Segment} {command.Index}");

                if (command.Segment == "static")
                {
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M-1");
                    writer.WriteLine("A=M");
                    writer.WriteLine("D=M");
                    writer.WriteLine($"@{staticVarName}");
                    writer.WriteLine("M=D");
                    return;
                }
                if (command.Segment == "temp")
                {
                    writer.WriteLine($"@SP");
                    writer.WriteLine("M=M-1");


                    writer.WriteLine($"@5");
                    writer.WriteLine("D=A");
                    writer.WriteLine($"@{command.Index}");
                    writer.WriteLine("D=D+A");
                    writer.WriteLine("@R6000");
                    writer.WriteLine("M=D");
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M");
                    writer.WriteLine("D=M");
                    writer.WriteLine($"@R6000");
                    writer.WriteLine("A=M");
                    writer.WriteLine("M=D");
                    return;
                }
                if (command.Segment == "pointer")
                {
                    switch (command.Index)
                    {
                        case 0:
                            writer.WriteLine("@SP");
                            writer.WriteLine("M=M-1");
                            writer.WriteLine("A=M");
                            writer.WriteLine("D=M");
                            writer.WriteLine("@THIS");
                            writer.WriteLine("M=D");
                            break;
                        case 1:

                            writer.WriteLine("@SP");
                            writer.WriteLine("M=M-1");
                            writer.WriteLine("A=M");
                            writer.WriteLine("D=M");
                            writer.WriteLine("@THAT");
                            writer.WriteLine("M=D"); ;
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                    return;
                }
                writer.WriteLine($"@SP");
                writer.WriteLine("M=M-1");

                writer.WriteLine($"@{command.Index}");
                writer.WriteLine("D=A");
                
                writer.WriteLine($"@{ramMap[command.Segment]}");
                writer.WriteLine("D=M+D");
                //writer.WriteLine("D=M");
                writer.WriteLine($"@R6000");
                writer.WriteLine("M=D");
                writer.WriteLine("@SP");
                writer.WriteLine("A=M");
                writer.WriteLine("D=M");
                writer.WriteLine($"@R6000");
                writer.WriteLine("A=M");
                writer.WriteLine("M=D");
                
            }

        }

        public void WriteArithmetic(string command,int lineCount)
        {
            Console.WriteLine(command);
            switch (command)
            {
                case "add":
                    writer.WriteLine($"//{command}");
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M-1");
                    writer.WriteLine("A=M");
                    writer.WriteLine("D=M");
                    writer.WriteLine("@R6000"); //temp1
                    writer.WriteLine("M=D");
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M-1");
                    writer.WriteLine("A=M");
                    writer.WriteLine("D=M");
                    writer.WriteLine("@R6000");
                    writer.WriteLine("M=M+D");
                    writer.WriteLine("D=M");
                    writer.WriteLine("@SP");
                    //writer.WriteLine("M=M+1");
                    writer.WriteLine("A=M");
                    writer.WriteLine("M=D");
                    writer.WriteLine("@1");
                    writer.WriteLine("D=A");
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M+D");
                    break;
                case "sub":
                    writer.WriteLine($"//{command}");
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M-1");
                    writer.WriteLine("A=M");
                    writer.WriteLine("D=M");
                    writer.WriteLine("@R6000"); //temp1
                    writer.WriteLine("M=D");
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M-1");
                    writer.WriteLine("A=M");
                    writer.WriteLine("D=M");
                    writer.WriteLine("@R6000");  //temp2
                    writer.WriteLine("M=D-M");
                    writer.WriteLine("D=M");
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M");
                    writer.WriteLine("M=D");
                    writer.WriteLine("@1");
                    writer.WriteLine("D=A");
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M+D");
                    break;
                case "neg":
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("D=M");
                    writer.WriteLine("D=M-D");
                    writer.WriteLine("D=D-M");
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("M=D");
              
                    break;
                case "eq":
                    writer.WriteLine($"//{command}");
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M-1"); //next sp
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("D=M");
                    writer.WriteLine("A=A+1");
                    writer.WriteLine("D=D-M");
                    writer.WriteLine($"@TRUE:{lineCount}");
                    writer.WriteLine("D;JEQ");

                    writer.WriteLine("@0");
                    writer.WriteLine("D=A");
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("M=D");
                    writer.WriteLine($"@END:{lineCount}");
                    writer.WriteLine("0;JMP");

                    writer.WriteLine($"(TRUE:{lineCount})");
                    writer.WriteLine("\t@SP");
                    writer.WriteLine("\tA=M-1");
                    writer.WriteLine("\tM=-1");
                   writer.WriteLine($"\t@END:{lineCount}");
                   writer.WriteLine("\t0;JMP");
                   writer.WriteLine($"(END:{lineCount})");

                    break;
                case "lt": 
                    writer.WriteLine($"//{command}");
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M-1");
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("D=M");
                    writer.WriteLine("A=A+1");
                    writer.WriteLine("D=D-M");
                    writer.WriteLine($"@TRUE:{lineCount}");
                    writer.WriteLine("D;JLT");

                    writer.WriteLine("@0");
                    writer.WriteLine("D=A");
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("M=D");
                    writer.WriteLine($"@END:{lineCount}");
                    writer.WriteLine("0;JMP");

                    writer.WriteLine($"(TRUE:{lineCount})");
                    writer.WriteLine("\t@SP");
                    writer.WriteLine("\tA=M-1");
                    writer.WriteLine("\tM=-1");
                    writer.WriteLine($"\t@END:{lineCount}");
                    writer.WriteLine("\t0;JMP");
                    writer.WriteLine($"(END:{lineCount})");

                    break;
                case "gt":
                    writer.WriteLine($"//{command}");
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M-1");
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("D=M");
                    writer.WriteLine("A=A+1");
                    writer.WriteLine("D=D-M");
                    writer.WriteLine($"@TRUE:{lineCount}");
                    writer.WriteLine("D;JGT");

                    writer.WriteLine("@0");
                    writer.WriteLine("D=A");
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("M=D");
                    writer.WriteLine($"@END:{lineCount}");
                    writer.WriteLine("0;JMP");

                    writer.WriteLine($"(TRUE:{lineCount})");
                    writer.WriteLine("\t@SP");
                    writer.WriteLine("\tA=M-1");
                    writer.WriteLine("\tM=-1");
                    writer.WriteLine($"\t@END:{lineCount}");
                    writer.WriteLine("\t0;JMP");
                    writer.WriteLine($"(END:{lineCount})");

                    break;
                case "and":
                    writer.WriteLine($"//{command}");
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M-1");
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("D=M");
                    writer.WriteLine("A=A+1");
                    writer.WriteLine("D=D&M");
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("M=D");
                    break;
                case "or":
                    writer.WriteLine($"//{command}");
                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M-1"); //next sp
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("D=M");
                    writer.WriteLine("A=A+1");
                    writer.WriteLine("D=M|D");
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("M=D");
                    break;
                case "not":
                    writer.WriteLine($"//{command}");
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("D=!M");
                    writer.WriteLine("M=D");

                    break;
                default:
                    throw new Exception("Invalid Arithmetic Command");
            }
        }

        public void Close()
        {
            try
            {
                writer.Dispose();
            }
            catch(Exception e)
            {

            }
            finally
            {
                writer.Close();
            }
            
        }
    }
}
