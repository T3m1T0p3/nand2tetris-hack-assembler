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
        private  string _destFile;
        StreamWriter writer;
        string currLine;

        string _staticVar;
        int _retAddrCount = 0;
        int _tempCount = 0;
        int _endFrameCount = 0;
        public int lineCount = 0;
        string _funcName = "";
        string _fileName;

        int _runningInt = 0;
        Dictionary<string,string> ramMap = new Dictionary<string, string>();
        Dictionary<string, string> functionLabels = new Dictionary<string, string>();
        Dictionary<string, string> returnAddresses = new Dictionary<string, string>();

        public void SetFileName(string file)
        {
            _destFile = file;
            _staticVar = _destFile.Replace("asm","");
             _fileName = _staticVar;
            writer = new StreamWriter(_destFile, false);
        }
        public CodeGenerator()
        {
            ramMap.Add("local","LCL");
            ramMap.Add("argument","ARG");
            ramMap.Add("this", "THIS");
            ramMap.Add("that", "THAT");
            Console.WriteLine("Generator init");

        }

        public void WritePushPop(PushPopCommand command,int lineCount)
        {
            Console.WriteLine(command);
            string staticVarName = _staticVar+$".{command.Index}";

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
                    writer.WriteLine("A=D");
                    writer.WriteLine("D=M");
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M");
                    writer.WriteLine("M=D");

                    writer.WriteLine("@SP");
                    writer.WriteLine("M=M+1");
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

                    writer.WriteLine($"@5");
                    writer.WriteLine("D=A");
                    writer.WriteLine($"@{command.Index}");
                    writer.WriteLine("D=D+A");
                    writer.WriteLine($"@popTemp_Temp:{_runningInt}");
                    writer.WriteLine("M=D");
                    writer.WriteLine("@SP");
                    writer.WriteLine("AM=M-1");
                    writer.WriteLine("D=M");
                    writer.WriteLine($"@popTemp_Temp:{_runningInt++}");
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
                writer.WriteLine($"@{command.Index}");
                writer.WriteLine("D=A");
                writer.WriteLine($"@{ramMap[command.Segment]}");
                writer.WriteLine("D=M+D");

                writer.WriteLine($"@pop:{_runningInt}");
                writer.WriteLine("M=D");

                writer.WriteLine("@SP");
                writer.WriteLine("AM=M-1");
                writer.WriteLine("D=M");
                writer.WriteLine($"@pop:{_runningInt++}");
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
                    writer.WriteLine("AM=M-1");
                    writer.WriteLine("A=A-1");
                    writer.WriteLine("D=M");
                    writer.WriteLine("@SP"); //temp1
                    writer.WriteLine("A=M");
                    writer.WriteLine("D=D+M");
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("M=D");

                    break;
                case "sub":
                    writer.WriteLine($"//{command}");
                    writer.WriteLine("@SP");
                    writer.WriteLine("AM=M-1");
                    writer.WriteLine("A=A-1");
                    writer.WriteLine("D=M");
                    writer.WriteLine("@SP"); //temp1
                    writer.WriteLine("A=M");
                    writer.WriteLine("D=D-M");
                    writer.WriteLine("@SP");
                    writer.WriteLine("A=M-1");
                    writer.WriteLine("M=D");
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

        public void WriteInit()
        {
            writer.WriteLine("@256");
            writer.WriteLine("D=A");
            writer.WriteLine("@SP");
            writer.WriteLine("M=D");
            WriteCall($"Sys.init", 0);
        }

        public void WriteLabel(string label)
        {
            
            string funcLabel = $"{_funcName}${label}";
            functionLabels.TryAdd(label, funcLabel);
            writer.WriteLine($"//write label  {funcLabel}");
            writer.WriteLine($"({funcLabel})");
        }

        public void WriteGoTo(string dest)
        {
            /*if (functionLabels.ContainsKey(dest))
            {
                dest = functionLabels[dest];
                writer.WriteLine($"//goto {dest}");
                writer.WriteLine($"@{dest}");
                writer.WriteLine($"0;JMP");
                return;
            }*/
            writer.WriteLine($"//goto {dest}");
            writer.WriteLine($"@{_funcName}${dest}");
            writer.WriteLine($"0;JMP");
        }

        public void WriteIf(string label)
        {
            
            if (functionLabels.ContainsKey(label))
            {
                label = functionLabels[label];
                
            }
            writer.WriteLine($"//if-goto {label}");
            writer.WriteLine("@SP");
            writer.WriteLine("M=M-1");
            writer.WriteLine("A=M");
            writer.WriteLine("D=M");
            writer.WriteLine($"@{_funcName}${label}");
            writer.WriteLine("D;JLT");
            writer.WriteLine("D;JGT");
        }

        public void WriteFunction(string funcName,int nVars)
        {
            _funcName = funcName;
            writer.WriteLine($"// Func def {funcName}");
            writer.WriteLine($"({funcName})");
            writer.WriteLine($"   @{nVars}");
            writer.WriteLine("     D=A");
            writer.WriteLine($"   @temp:{_tempCount}");
            writer.WriteLine("    M=D");
            writer.WriteLine($"   (WHILE_LOOP:{_tempCount})");
            writer.WriteLine($"       @temp:{_tempCount}");
            writer.WriteLine("        D=M");
            writer.WriteLine($"       @END_LOOP:{_tempCount}");
            writer.WriteLine("        D;JEQ");
            writer.WriteLine("        @0");
            writer.WriteLine("        D=A");
            writer.WriteLine("        @SP");
            writer.WriteLine("        A=M");
            writer.WriteLine("        M=D");
            writer.WriteLine("        @SP");
            writer.WriteLine("        M=M+1");
            writer.WriteLine($"       @temp:{_tempCount}");
            writer.WriteLine("        M=M-1");
            writer.WriteLine($"       @WHILE_LOOP:{_tempCount}");
            writer.WriteLine("        0;JMP");
            writer.WriteLine($"(END_LOOP:{_tempCount++})");
        }

        public void WriteCall(string funcName,int nArgs)
        {
            writer.WriteLine($"//call {funcName}");
            string returnAddr = $"{_funcName}$ret.{_retAddrCount++}";
            writer.WriteLine($"@{returnAddr}");
            //writer.WriteLine("A=M");
            writer.WriteLine("D=A");
            writer.WriteLine("@SP");
            writer.WriteLine("A=M");
            writer.WriteLine("M=D");
            writer.WriteLine("@SP");
            writer.WriteLine("M=M+1");

            writer.WriteLine("@LCL");
            //writer.WriteLine("A=M");
            writer.WriteLine("D=M");
            writer.WriteLine("@SP");
            writer.WriteLine("A=M");
            writer.WriteLine("M=D");
            writer.WriteLine("@SP");
            writer.WriteLine("M=M+1");

            writer.WriteLine("@ARG");
            //writer.WriteLine("A=M");
            writer.WriteLine("D=M");
            writer.WriteLine("@SP");
            writer.WriteLine("A=M");
            writer.WriteLine("M=D");
            writer.WriteLine("@SP");
            writer.WriteLine("M=M+1");

            writer.WriteLine("@THIS");
            //writer.WriteLine("A=M");
            writer.WriteLine("D=M");
            writer.WriteLine("@SP");
            writer.WriteLine("A=M");
            writer.WriteLine("M=D");
            writer.WriteLine("@SP");
            writer.WriteLine("M=M+1");

            writer.WriteLine("@THAT");
            //writer.WriteLine("A=M");
            writer.WriteLine("D=M");
            writer.WriteLine("@SP");
            writer.WriteLine("A=M");
            writer.WriteLine("M=D");
            writer.WriteLine("@SP");
            writer.WriteLine("M=M+1");

            writer.WriteLine("@5");
            writer.WriteLine("D=A");
            writer.WriteLine("@SP");
            writer.WriteLine("D=M-D");
            writer.WriteLine($"@{nArgs}");
            writer.WriteLine("D=D-A");
            writer.WriteLine("@ARG");
            writer.WriteLine("M=D");


            writer.WriteLine("@SP");
            writer.WriteLine("D=M");
            writer.WriteLine("@LCL");
            writer.WriteLine("M=D");

            writer.WriteLine($"@{funcName}");
            writer.WriteLine("0;JMP");
            writer.WriteLine($"({returnAddr})");



        }

        public void WriteReturn()
        {
            writer.WriteLine("//return Command");
            writer.WriteLine("@LCL");
            writer.WriteLine("D=M");
            writer.WriteLine($"@endFrame:{_endFrameCount}");
            writer.WriteLine("M=D");

            writer.WriteLine("@5");
            writer.WriteLine("D=D-A");
            writer.WriteLine("A=D");
            writer.WriteLine("D=M");
            writer.WriteLine($"@returnAddress:{_runningInt}");
            writer.WriteLine("M=D");
            writer.WriteLine("@SP");
            writer.WriteLine("AM=M-1");
            writer.WriteLine("D=M");
            writer.WriteLine("@ARG");
            writer.WriteLine("A=M");
            writer.WriteLine("M=D");

            writer.WriteLine("@ARG");
            writer.WriteLine("D=M");
            writer.WriteLine("@SP");
            writer.WriteLine("M=D+1");

            writer.WriteLine($"@endFrame:{_endFrameCount}");
            writer.WriteLine("A=M-1");
            writer.WriteLine("D=M");
            writer.WriteLine("@THAT");
            writer.WriteLine("M=D");


            writer.WriteLine($"@endFrame:{_endFrameCount}");
            writer.WriteLine("A=M-1");
            writer.WriteLine("A=A-1");
            writer.WriteLine("D=M");
            writer.WriteLine("@THIS");
            writer.WriteLine("M=D");

            writer.WriteLine("@3");
            writer.WriteLine("D=A");
            writer.WriteLine($"@endFrame:{_endFrameCount}");
            writer.WriteLine("A=M-D");
            writer.WriteLine("D=M");
            writer.WriteLine("@ARG");
            writer.WriteLine("M=D");

            writer.WriteLine("@4");
            writer.WriteLine("D=A");
            writer.WriteLine($"@endFrame:{_endFrameCount++}");
            writer.WriteLine("A=M-D");
            writer.WriteLine("D=M");
            writer.WriteLine("@LCL");
            writer.WriteLine("M=D");

            writer.WriteLine($"@returnAddress:{_runningInt++}");
            writer.WriteLine("A=M");
            writer.WriteLine("0;JMP");
        }

        public void Merge(string directoryName)
        {
            string asmFinal = directoryName+"\\"+directoryName+".asm";
            Console.WriteLine("asmFile "+asmFinal);
            StreamWriter streamWriter = new StreamWriter(asmFinal, false);
            string sysFile =Directory.GetFiles(directoryName,"*Sys.asm").First();
            //sysFile = sysFile.Replace(directoryName+"\\", "");
            Console.WriteLine("Sys file "+sysFile);
            StreamReader streamReader;
            
            streamReader = new StreamReader(sysFile);
            string line;
            while (!streamReader.EndOfStream)
            {
                
                line = streamReader.ReadLine();
                streamWriter.WriteLine(line);
            }
            streamWriter.WriteLine();
            streamReader.Close();
            foreach(string file in Directory.GetFiles(directoryName, "*.asm"))
            {
                Console.WriteLine("reading asms: "+file);
                if (file==asmFinal) continue;
                streamReader = new StreamReader(file);
                while (!streamReader.EndOfStream)
                {
                    line = streamReader.ReadLine();
                    streamWriter.WriteLine(line);
                }
                streamWriter.WriteLine();
                streamReader.Close();
            }
            streamWriter.Close();
        }
    } 
}
