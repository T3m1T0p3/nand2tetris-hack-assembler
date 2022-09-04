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
            if(currChar==' ')
            {
                while(currChar==' ')
                {
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
                if(!enumerator.MoveNext()) ProcessLine();
                currChar = enumerator.Current;
                while (currChar != '\"')
                {
                    temp += currChar.ToString();
                    if(!enumerator.MoveNext()) ProcessLine();
                    currChar = enumerator.Current;
                }
                if (!enumerator.MoveNext()) ProcessLine();
                _currentToken = temp;
                _stringConst = true;
                //Console.WriteLine($"strng cnst: {_currentToken}");
                return;
            }

            else if (symbols.Contains(currChar))
            {
                //Console.WriteLine($"hit symbol processor");
                _currentToken = currChar.ToString();
                if(!enumerator.MoveNext()) ProcessLine();
                return;
            }

            else
            {
                //Console.WriteLine($"hit identifier processor");
                while (!symbols.Contains(currChar) && currChar != ' ')
                {
                    temp += currChar.ToString();
                    if (!enumerator.MoveNext()) ProcessLine();
                    currChar = enumerator.Current;
                }

                if (currChar == ' ')
                {
                    if (!enumerator.MoveNext()) ProcessLine();
                }
                _currentToken = temp;
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
            if (_reader.EndOfStream)
            {
                enumerator.MoveNext();
                return;
            }

            line = _reader.ReadLine();
            line = line.Trim();
            
            //Console.WriteLine(line);
            if (line.StartsWith("//"))
            {
                while (line.StartsWith("//")|| String.IsNullOrEmpty(line) && !_reader.EndOfStream) 
                {
                    line = _reader.ReadLine();
                    //Console.WriteLine(line);
                }
            }
            if (line.StartsWith("/**"))
            {
                //Console.WriteLine(line);
                if (line.EndsWith("*/"))
                {
                   // Console.WriteLine(line);
                    line = _reader.ReadLine();
                }
                else
                {
                    while (!line.EndsWith("*/"))
                    {
                        
                        line = _reader.ReadLine();
                        line = line.Trim();
                        //Console.WriteLine(line);
                    }
                }
            }
            if (String.IsNullOrEmpty(line))
            {
                while (String.IsNullOrEmpty(line)&&!_reader.EndOfStream)
                {
                    line = _reader.ReadLine();
                }
             }
            line = line.Split("//")[0];
            line = line.Trim();
            //Console.WriteLine("Processed Line:" + line);
            enumerator = line.GetEnumerator();
            enumerator.MoveNext();
        }

        public void Close()
        {
            _reader.Close();
        }
    }
}
