using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VmTranslator
{
    public class VmCodeParser
    {
        public string sourceFile;
        private string _curentLine;
        StreamReader _reader;
        Dictionary<string, string> commandTypes = new Dictionary<string, string>();
        public int lineCount = 0;

        public void SetFileName(string file)
        {
            sourceFile = file;
            _reader = new StreamReader(sourceFile);
        }
        public VmCodeParser()
        {
            commandTypes.Add("push", "C_PUSH");
            commandTypes.Add("pop", "C_POP");
            commandTypes.Add("add", "C_ARITHMETIC");
            commandTypes.Add("sub", "C_ARITHMETIC");
            commandTypes.Add("neg", "C_ARITHMETIC");
            commandTypes.Add("eq", "C_ARITHMETIC");
            commandTypes.Add("lt", "C_ARITHMETIC");
            commandTypes.Add("gt", "C_ARITHMETIC");
            commandTypes.Add("and", "C_ARITHMETIC");
            commandTypes.Add("or", "C_ARITHMETIC");
            commandTypes.Add("not", "C_ARITHMETIC");
            commandTypes.Add("label", "LABEL");
            commandTypes.Add("if-goto", "IF-GOTO");
            commandTypes.Add("goto", "GOTO");
            commandTypes.Add("function", "FUNC_DEFINITION");
            commandTypes.Add("call", "FUNC_CALL");
            commandTypes.Add("return", "RETURN");
            Console.WriteLine("Parser initialized");

        }

        public string Parser(string command)
        {
            throw new Exception();
        }

        public bool HasMoreCommand()
        {
            return _reader.EndOfStream;
        }

        public void Advance()
        {
            _curentLine = _reader.ReadLine();
            if (String.IsNullOrEmpty(_curentLine)||_curentLine.StartsWith('/')) Advance();
            _curentLine =_curentLine.TrimStart(' ').TrimEnd(' ');
            _curentLine = _curentLine.Split("//")[0];
            lineCount++;
        }

        public string Arg0()
        {
            if (commandTypes.ContainsKey(_curentLine) && commandTypes[_curentLine] == "C_ARITHMETIC") return _curentLine;
            string[] args = _curentLine.Split(' ');
            return args[0];
        }
        public string Arg1()
        {
            if (commandTypes.ContainsKey(_curentLine) && commandTypes[_curentLine] == "C_ARITHMETIC") return _curentLine;
            string[] args = _curentLine.Split(' ');
            return args[1];
        }

        public int Arg2()
        {
            string[] args = _curentLine.Split(' ');
            string arg2 = args[2];
            int.TryParse(arg2,out int res);
            return res;
        }

        public string CommandType(string command)
        {
            return commandTypes[command];
        }
        public void Close()
        {
            try
            {
                _reader.Dispose();
            }
            catch(Exception e)
            {

            }
        }
    }
}
