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
        private readonly string _filePath;
        private string _curentLine;
        StreamReader _reader;
        Dictionary<string, string> commandTypes = new Dictionary<string, string>();
        public int lineCount = 0;
        public VmCodeParser(string filePath)
        {
            _filePath = filePath;

            _reader = new StreamReader(filePath);

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

            Console.WriteLine("Parser initialized");

        }

        public string Parser(string command)
        {
            throw new Exception();
        }

        public bool HasMoreCommand()
        {
            Console.WriteLine(_reader.EndOfStream);
            return _reader.EndOfStream;
        }

        public void Advance()
        {
            _curentLine = _reader.ReadLine();
            if (String.IsNullOrEmpty(_curentLine)||_curentLine.StartsWith('/')) Advance();
            Console.WriteLine(_curentLine);
            _curentLine =_curentLine.TrimStart(' ').TrimEnd(' ');
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
            //string arg1 = args.Length == 1 ? args[0] : args.Length == 3 ? args[1] : throw new Exception("Invalid Command");
            return args[1];//commandTypes[arg1] == "C_ARITHMETIC" ? arg1 : commandTypes[arg1];
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
