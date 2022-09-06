using System;
using System.Collections.Generic;
using System.IO;

namespace JackCompiler
{
    public class Tokenizer
    {
        string _file;
        Dictionary<string,string> keywords;
        List<char> symbols;
        List<int> intergerConstants;
        StreamReader _reader;
        string _currentToken=null;
        bool _stringConst;
        string line;
        CharEnumerator enumerator;
        public Tokenizer(string file)
        {
            _file = file;
            _reader = new StreamReader(_file);
            keywords = new Dictionary<string, string>();
            symbols = new List<char>();
           // enumerator = new CharEnumerator();
             //line = _reader.ReadLine();
           // enumerator = line.GetEnumerator();
            ProcessLine();
            keywords.Add("class", "CLASS");
            keywords.Add("constructor", "CONSTRUCTOR");
            keywords.Add("function", "FUNCTION");
            keywords.Add("method", "METHOD");
            keywords.Add("field", "FIELD");
            keywords.Add("static", "STATIC");
            keywords.Add("var", "VAR");
            keywords.Add("boolean", "BOOLEAN");
            keywords.Add("void", "VOID");
            keywords.Add("true", "TRUE");
            keywords.Add("false", "FALSE");
            keywords.Add("null", "NULL");
            keywords.Add("this", "THIS");
            keywords.Add("let", "LET");
            keywords.Add("return", "RETURN");
            keywords.Add("while", "WHILE");
            keywords.Add("char", "CHAR");
            keywords.Add("else", "ELSE");
            keywords.Add("int", "INT");
            keywords.Add("do", "DO");
            keywords.Add("if", "IF");
            //keywords.Add("", "");


            string symbls = "'{' | '}' | '(' | ')' | '[' | ']' | '.' | ',' | ';' | '+' | '-' | '*' | '/' | '&' | '|' | '<' | '>' | '=' | '~'".Replace("' | '", "").Replace("\'","").Trim();
            symbols.AddRange(symbls.ToCharArray());
            intergerConstants = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }

        public bool HasMoreToken()
        {
            return _reader.EndOfStream;
        }

        public void Advance()
        {
            string temp = "";
            char currChar = enumerator.Current;
            Console.WriteLine(1);
            if(currChar==' ')
            {
                Console.WriteLine(2);
                while (currChar==' ')
                {
                    Console.WriteLine("processing empty char");
                    if (!enumerator.MoveNext())
                    {
                        ProcessLine();
                    }
                    currChar = enumerator.Current;
                }
            }
            //prcess string const
            if (currChar == '\"')
            {
                Console.WriteLine(3);
                if (!enumerator.MoveNext()) ProcessLine();
                currChar = enumerator.Current;
                while (currChar != '\"')
                {
                    Console.WriteLine(4);
                    temp += currChar.ToString();
                    if(!enumerator.MoveNext()) ProcessLine();
                    currChar = enumerator.Current;
                }
                Console.WriteLine(5);
                if (!enumerator.MoveNext()) ProcessLine();
                Console.WriteLine(6);
                _currentToken = temp;
                _stringConst = true;
                //Console.WriteLine($"strng cnst: {_currentToken}");
                return;
            }

            else if (symbols.Contains(currChar))
            {
                //Console.WriteLine($"hit symbol processor");
                Console.WriteLine(7);
                _currentToken = currChar.ToString();
                Console.WriteLine("possibly last char:" + _currentToken);
                if(!enumerator.MoveNext()) ProcessLine();
                Console.WriteLine(8);
                return;
            }

            else
            {
                Console.WriteLine(9);
                //Console.WriteLine($"hit identifier processor");
                while (!symbols.Contains(currChar) && currChar != ' ')
                {
                    Console.WriteLine(10);
                    temp += currChar.ToString();
                    if (!enumerator.MoveNext()) ProcessLine();
                    Console.WriteLine(11);
                    currChar = enumerator.Current;
                }

                if (currChar == ' ')
                {
                    Console.WriteLine(12);
                    if (!enumerator.MoveNext()) ProcessLine();
                    Console.WriteLine(13);
                }
                _currentToken = temp;
                Console.WriteLine(14);
                //Console.WriteLine($"identifier: {_currentToken}");
                return;
            }
        }

        public string TokenType()
        {
            char currChar;
            
            if (keywords.ContainsKey(_currentToken))
            {
                return "KEYWORD";
            }
            else if (int.TryParse(_currentToken, out int currInt))
            {
                return "INT_CONST";
            }

            else if(Char.TryParse(_currentToken, out currChar))
            {
                if (symbols.Contains(currChar)) return "SYMBOL";
                return "IDENTIFIER";
            }
            

            else if (_stringConst)
            {
                _stringConst = false;
                return "STRING_CONST";
            }

            else
            {
                return "IDENTIFIER";
            }
        }

        public string Keyword()
        {
            keywords.TryGetValue(_currentToken,out string temp);
            return temp;
        }

        public string Identifier()
        {
            //Console.WriteLine($"Identifier: {_currentToken}");
            return _currentToken;
        }

        public int IntVal()
        {
            int temp;
            int.TryParse(_currentToken, out temp);
            //Console.WriteLine($"int val: {temp}");
            return temp;
        }

        public string StringVal()
        {
            //Console.WriteLine($"current token: {_currentToken}");
            return _currentToken;
        }

        public char Symbol()
        {
            char temp;
            char.TryParse(_currentToken, out temp);
            //Console.WriteLine($"current token: {temp}");
            return temp;
        }
        public void ProcessLine()
        {
            Console.WriteLine("Processline called");
            if (_reader.EndOfStream)
            {
                Console.WriteLine(15);
                enumerator.MoveNext();
                Console.WriteLine(16);
                return;
            }
            Console.WriteLine("Not eof");
            line = _reader.ReadLine();
            if (line == null && _reader.EndOfStream)
            {
                Console.WriteLine("Hit EOF");
                return;
                
            }
            Console.WriteLine("line not null");
            if (!String.IsNullOrEmpty(line)) line = line.Trim();
            Console.WriteLine("line not empty");
            if (line.StartsWith("//")|| line.StartsWith("/**")|| String.IsNullOrEmpty(line))
            {
                Console.WriteLine("calling purge");
                Purge();
                Console.WriteLine("purged");
            }
            Console.WriteLine(line);
            line = line.Split("//")[0];
            line = line.Trim();
            Console.WriteLine("Processed Line:" + line);
            enumerator = line.GetEnumerator();
            enumerator.MoveNext();
        }

        public void Purge()
            
        {
            Console.WriteLine("current line:" + _reader.ToString() + " Eof status:" + _reader.EndOfStream);
            if (_reader.EndOfStream) return;
            if (!line.StartsWith("//") && !line.StartsWith("/**") && !String.IsNullOrEmpty(line)&&!line.StartsWith("*")) return;
            if (line.StartsWith("//"))
            {
                while (line.StartsWith("//"))
                {
                    line = _reader.ReadLine();
                    line = line.Trim();
                }
                Purge();

            }
            if (String.IsNullOrEmpty(line))
            {
                while (String.IsNullOrEmpty(line)&&!_reader.EndOfStream)
                {
                    line = _reader.ReadLine();
                    if (!String.IsNullOrEmpty(line)) line=line.Trim();
                }
                Purge();
            }
            if (line.StartsWith("/**"))
            {
                if (line.EndsWith("*/"))
                {
                    line = _reader.ReadLine();
                    if (!String.IsNullOrEmpty(line)) line = line.Trim();
                    return;
                }
                while (!line.EndsWith("*/"))
                {
                    line = _reader.ReadLine();
                    if (!String.IsNullOrEmpty(line))  line = line.Trim();
                    Console.WriteLine(line);
                }
                line = _reader.ReadLine();
                Purge();
            }
            
        }

        public void Close()
        {
            
            _reader.Close();
            _reader.Dispose();
        }
    }
}
