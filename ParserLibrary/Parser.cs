using System;

namespace ParserLibrary
{
    public class Parser
    {
        string _instruction;
        string Dest { get; set; } = "null";
        string Comp
        {
            get; set;
        } = "null";
        string Jmp
        {
            get; set;
        } = "null";

        public Parser()
        {
        }

        public void Parse(string instruction)
        {
            
            _instruction = instruction.Trim(';');
            int index;
            string tmp = "";
            for (index = 0; index < _instruction.Length; index++)
            {
                if (_instruction[index] == '=')
                {
                    Dest = tmp;
                    tmp = "";
                }
                else if (_instruction[index] == ';')
                {
                    Comp = tmp;
                    Jmp = _instruction[(index + 1)..];
                    break;
                }
                else
                {
                    tmp += _instruction[index].ToString();
                    if (index == _instruction.Length - 1) Comp = tmp;
                }
                
            }

        }

        public string Destination()
        {
            string res = Dest;
            Dest = "null";
            return res;
        }

        public string Compute()
        {
            string res = Comp;
            Comp = "null";
            return res;
        }

        public string Jump()
        {
            string res = Jmp;
            Jmp = "null";
            return res;
        }
    }
}
