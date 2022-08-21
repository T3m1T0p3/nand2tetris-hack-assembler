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

        private readonly int _nextAvalableAddress=16;
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

            LabelTable= new Dictionary<string, int>();

            for (int i = 0; i < 16; i++)
            {
                Symbols.Add($"R{i}", i);
            }
        }

        public void UpdateLableTable(string label,int lineNumber)
        {
            LabelTable.TryAdd(label, lineNumber + 1);
        }
        public string MemoryAddress(string address)
        {
            int.TryParse(address, out int bin);

            var res=Convert.ToString(bin, 2);
            res=res.PadLeft(16,'0');
            return res;
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
