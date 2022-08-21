using ParserLibrary;
using System;
using System.IO;

namespace HackAssembler_Console
{
    class Program
    {
        static void Main(string[] args)
        { 
            Parser parser = new Parser();
            using var writer = new StreamWriter(@"C:\Users\Oyinlola\Desktop\nand2tetris\nand2tetris\nand2tetris\projects\06\pong\PongL.hack", false);
            using (StreamReader reader=new StreamReader(@"C:\Users\Oyinlola\Desktop\nand2tetris\nand2tetris\nand2tetris\projects\06\pong\PongL.asm"))
            {
                SymbolTable symbolTable = new ParserLibrary.SymbolTable();
                string line;
                while ((line= reader.ReadLine())!=null)
                {
                    if (line.StartsWith('/') || String.IsNullOrWhiteSpace(line)) continue;
                    if (line.StartsWith('@'))
                    {
                        var ins = line.TrimStart('@');
                        string address=symbolTable.MemoryAddress(ins);
                        Console.WriteLine(address);
                        writer.WriteLine(address);
                    }
                    else
                    {
                        parser.Parse(line);
                        string dest = parser.Destination();
                        string jmp = parser.Jump();
                        string comp = parser.Compute();
                        string compCode = symbolTable.GetCompCode(comp);
                        string destCode = symbolTable.GetDestCode(dest);
                        string jmpCode = symbolTable.GetJmpCode(jmp);
                        Console.WriteLine($"{compCode} {destCode}:{dest} {jmpCode}");
                        writer.WriteLine("111"+ compCode + destCode + jmpCode);
                    }
                    
                }
            }
            writer.Close();
        }
    }
}
