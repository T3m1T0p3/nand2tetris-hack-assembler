using ParserLibrary;
using System;
using System.IO;

namespace HackAssembler
{
    public class Assembler
    {
        static void Main(string[] args)
        {
            string filePath = args[0];
            string assembly = filePath.Replace(".asm", ".hack");
            int lineCount = 0;
            string line;

            Parser parser = new Parser();
            SymbolTable symbolTable = new ParserLibrary.SymbolTable();


            using var writer = new StreamWriter(assembly);
            using (StreamReader reader = new StreamReader(filePath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.TrimStart(' ');
                    line = line.TrimEnd(' ');
                    if (lineCount == 29) Console.WriteLine($"{lineCount} {line}");
                    if (line.StartsWith('/') || String.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    if (line.StartsWith('(') &&line.EndsWith(')'))
                    {
                        var label = line.TrimStart('(').TrimEnd(')');
                        if (lineCount == 29) Console.WriteLine($"{lineCount} {line}");
                        symbolTable.ParseLabel(label, lineCount);
                        continue;
                    }
                    lineCount+=1;
                }
                
            }

            using(StreamReader reader=new StreamReader(filePath))
            {
                lineCount = 0;
                while ((line = reader.ReadLine()) != null)
                {

                    line = line.TrimStart(' ');
                    line=line.TrimEnd(' ');
                    if (line.StartsWith('/') || String.IsNullOrWhiteSpace(line) || line.StartsWith('('))
                    {
                        continue;
                    }
                    string[] lines = line.Split('/');
                    line = lines[0];
                    //Console.WriteLine(lines[0]);
                    if (line.StartsWith('@'))
                    {
                        line = line.TrimStart(' ');
                        line = line.TrimEnd(' ');
                        //Console.WriteLine(line);
                        var symbol = line.TrimStart('@');
                        symbolTable.ParseSymbol(symbol,lineCount);
                        string address = symbolTable.GetMemoryAddress(symbol);
                        //Console.WriteLine(address);
                        writer.WriteLine(address);
                        lineCount++;
                    }
                    else
                    {
                        line = line.TrimStart(' ');
                        line = line.TrimEnd(' ');
                        //Console.WriteLine(line);
                        parser.Parse(line);
                        string dest = parser.Destination();
                        string jmp = parser.Jump();
                        string comp = parser.Compute();
                        string compCode = symbolTable.GetCompCode(comp);
                        string destCode = symbolTable.GetDestCode(dest);
                        string jmpCode = symbolTable.GetJmpCode(jmp);
                        //Console.WriteLine($"{compCode} {destCode}:{dest} {jmpCode}");
                        writer.WriteLine("111" + compCode + destCode + jmpCode);
                        lineCount++;
                    }
                    
                }
            }
            writer.Close();
        }
    }
}
