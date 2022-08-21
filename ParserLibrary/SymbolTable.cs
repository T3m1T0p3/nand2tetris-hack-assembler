using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserLibrary
{
    public class SymbolTable
    {
        public Dictionary<string, int> LabelTable;// = new Dictionary<string, string>();
        public Dictionary<string, string> ComputeTable { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DestTable { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> JumpTable { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, int> Symbols { get; set; } = new Dictionary<string, int>();
        public int[] Addresses = new int[2];
        private int _nextAvalableAddress=16;
        public SymbolTable()
        {
            ComputeTable.Add("0", "0101010");
            ComputeTable.Add("1", "0111111");
            ComputeTable.Add("-1", "0111010");
            ComputeTable.Add("D", "0001100");
            ComputeTable.Add("A", "0110000");
            ComputeTable.Add("M", "1110000");
            ComputeTable.Add("!D", "0001101");
            ComputeTable.Add("!A", "0110001");
            ComputeTable.Add("!M", "1110001");
            ComputeTable.Add("-D", "0001111");
            ComputeTable.Add("-A", "0110011");
            ComputeTable.Add("-M", "1110011");
            ComputeTable.Add("D+1", "0011111");
            ComputeTable.Add("A+1", "0110111");
            ComputeTable.Add("M+1", "1110111");
            ComputeTable.Add("D-1", "0001110");
            ComputeTable.Add("A-1", "0110010");
            ComputeTable.Add("M-1", "1110010");
            ComputeTable.Add("D+A", "0000010");
            ComputeTable.Add("D+M", "1000010");
            ComputeTable.Add("D-A", "0010011");
            ComputeTable.Add("D-M", "1010011");
            ComputeTable.Add("A-D", "0000111");
            ComputeTable.Add("M-D", "1000111");
            ComputeTable.Add("D&A", "0000000");
            ComputeTable.Add("D&M", "1000000");
            ComputeTable.Add("D|A", "0010101");
            ComputeTable.Add("D|M", "1010101");


            DestTable.Add("null", "000");
            DestTable.Add("M", "001");
            DestTable.Add("D", "010");
            DestTable.Add("MD", "011");
            DestTable.Add("A", "100");
            DestTable.Add("AM", "101");
            DestTable.Add("AD", "110");
            DestTable.Add("AMD", "111");

            
            JumpTable.Add("null", "000");
            JumpTable.Add("JGT", "001");
            JumpTable.Add("JEQ", "010");
            JumpTable.Add("JGE", "011");
            JumpTable.Add("JLT", "100");
            JumpTable.Add("JNE", "101");
            JumpTable.Add("JMP", "111");
            JumpTable.Add("JLE", "110");

            Symbols.Add("SCREEN", 16384);
            Symbols.Add("KBD", 24570);
            Symbols.Add("SP", 0);
            Symbols.Add("LCL", 1);
            Symbols.Add("THIS", 3);
            Symbols.Add("THAT", 4);
            Symbols.Add("ARG", 2);
            /*
            Symbols.Add("R1", 1);
            Symbols.Add("R2", 2);
            Symbols.Add("R3", 3);
            Symbols.Add("4", 4);
            Symbols.Add("R5", 5);
            Symbols.Add("R6", 6);
            Symbols.Add("R7", 7);
            Symbols.Add("R8", 8);
            Symbols.Add("R9", 9);
            Symbols.Add("R10", 10);
            Symbols.Add("R11", 11);
            Symbols.Add("R12", 12);
            Symbols.Add("R13", 13);
            Symbols.Add("14", 14);
            Symbols.Add("R15", 15);*/

            LabelTable = new Dictionary<string, int>();

            for (int i = 0; i < 16; i++)
            {
                Symbols.Add($"R{i}", i);
            }
        }

        public void ParseLabel(string label, int nextLine)
        {
             Symbols.TryAdd(label, nextLine);
        }
        public void ParseSymbol(string symbol,int lineCount)
        {
            bool isAddress = int.TryParse(symbol, out int Addr);
            if (!isAddress&&!Symbols.ContainsKey(symbol))
            {
                Console.WriteLine($"{symbol}:{ _nextAvalableAddress}");
                Symbols.TryAdd(symbol, _nextAvalableAddress++);
            }
            
        }

        public void UpdateLableTable(string label,int lineNumber)
        {
            LabelTable.TryAdd(label, lineNumber + 1);
        }
        public string GetMemoryAddress(string address)
        {
            string res;
            var isAddress = int.TryParse(address, out int addr);
            if(isAddress)
            {
                 res = Convert.ToString(addr,2);
                res= res.PadLeft(16, '0');
                return res;
            }
            var symbolAddress= Symbols[address];
            var bin = Convert.ToString(symbolAddress, 2);
            bin = bin.PadLeft(16, '0');
            return bin;

        }

        public string GetCompCode(string ins)
        {
            return ComputeTable[ins];
        }

        public string GetJmpCode(string ins)
        {
            return JumpTable[ins];
        }

        public string GetDestCode(string ins)
        {
            return DestTable[ins];
        }
    }
}
