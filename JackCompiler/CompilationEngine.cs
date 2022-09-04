using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackCompiler
{
    public class CompilationEngine : ICompilationEngine
    {
        StreamWriter _writer;
        StreamWriter _tokenWriter;
        //StreamReader _reader;
        Tokenizer _tokenizer;
        //string keyword;
        public CompilationEngine(string inputFile)//,string outputFile)
        {
            string outputFile = inputFile.Replace(".jack", ".XML");
            //string tokenFile = inputFile + "T";
            string tokenFile=inputFile.Replace(".jack", "T.XML");
            _tokenizer = new Tokenizer(inputFile);
            _writer = new StreamWriter(outputFile, false);
            _tokenWriter = new StreamWriter(tokenFile, false);
            CompileClass();

        }
        public void CompileClass()
        {
            _tokenizer.Advance();
            //Console.WriteLine("compileClass");
            WriteGrammarType("class");
            WriteTokenTag("token");
            InsertClass();
            InsertIdentifier();
            InsertLeftCurly();
            CompileClassVarDec();
            while(_tokenizer.Identifier()=="method"|| _tokenizer.Identifier() == "function"|| _tokenizer.Identifier() == "constructor"&&!_tokenizer.HasMoreToken())
            {
                CompileSubroutineDec();
            }
            InsertRightCurly();
            WriteGrammarType("/class");
            WriteTokenTag("/token");
            _tokenizer.Close();
            _writer.Close();
            _tokenWriter.Close();

        }

        //sorted
        public void CompileClassVarDec()
        {

            WriteGrammarType("ClassVarDec");
          //  Console.WriteLine("ClassVarDec");
            while (_tokenizer.Identifier() == "static" || _tokenizer.Identifier() == "field")
            {
                WriteGrammarType("ClassVarDec");
                InsertStaticField();
                InsertIdentifier();
                InsertIdentifier();
                char symbol = _tokenizer.Symbol();
                while (symbol != ';')
                {
                    InsertComma();
                    InsertIdentifier();
                    symbol = _tokenizer.Symbol();
                }
                InsertSemiColon();
                WriteGrammarType("/ClassVarDec");
            }
            
            WriteGrammarType("/ClassVarDec");
            //Console.WriteLine("/ClassVarDec");
        }
        //Sorted
        public void CompileSubroutineDec()
        {
            //function void main()
            Console.WriteLine("CompileSubDec");
            WriteGrammarType("subroutineDec");
            InsertConMethodFunction();
            InsertIdentifier();
            InsertIdentifier();
            InsertLeftBrace();
            CompileParameterList();
            InsertRightBrace();
            CompileSubroutineBody();
            //Console.WriteLine("All subroutines successfully paresed");
            WriteGrammarType("/subroutineDec");

        }

        //Sorted
        public void CompileSubroutineBody()
        {
            //Console.WriteLine("CompileSubBody");
            WriteGrammarType("subroutineBody");
            InsertLeftCurly();
            string identifier = _tokenizer.Identifier();
            if (identifier == "var")
            {
                CompileVarDec();
            }
            CompileStatements();
            //Console.WriteLine("Statements succeffuly compiled in subBody");
            //CompileReturnStatement();
            //Console.WriteLine("Return Statement succeffuly compiled in subBody");
            InsertRightCurly();
            //Console.WriteLine("/CompileSubBody");
            WriteGrammarType("/subroutineBody");

        }
        public void CompileVarDec()
        {
            //var SquareGame game;
            //Console.WriteLine("CompileVarDec");
             WriteGrammarType("varDec");
            while (_tokenizer.Identifier() == "var")
            {
                WriteGrammarType("varDec");
                Console.WriteLine(_tokenizer.Identifier());
                InsertVar();
                InsertIdentifier();
                InsertIdentifier();
                char symbol = _tokenizer.Symbol();
                while (symbol == ',')
                {
                    InsertComma();
                    InsertIdentifier();
                    symbol = _tokenizer.Symbol();
                }
                InsertSemiColon();
                WriteGrammarType("/varDec");
            }
            //Console.WriteLine("/CompileVarDec");
   //       WriteGrammarType("/varDec");

        }

        public void CompileStatements()
        {
            //Console.WriteLine("CompileStatements");
            string identifier = _tokenizer.Identifier();
            WriteGrammarType("statement");
            while (identifier == "if" || identifier == "let" || identifier == "while" || identifier == "do" || identifier == "return")
            {
                switch (identifier)
                {
                    case "let":
                        CompileLetStatement();
                        identifier = _tokenizer.Identifier();
                        break;
                    case "if":
                        CompileIfStatement();
                        identifier = _tokenizer.Identifier();
                        break;
                    case "while":
                        CompileWhileStatement();
                        identifier = _tokenizer.Identifier();
                        break;
                    case "do":
                        CompileDoStatement();
                        identifier = _tokenizer.Identifier();
                        break;
                    case "return":
                        CompileReturnStatement();
                        identifier = _tokenizer.Identifier();
                        break;
                    default:
                        break;
                }
            }
            //Console.WriteLine("Exiting compilestatements while with identifier:" + identifier);
            //Console.WriteLine("/CompileStatements");
            WriteGrammarType("/statement");
        }

        

        //Sorted
        public void CompileParameterList()
        {
            string identifier;
            WriteGrammarType("parameterList");
            //Console.WriteLine("parameterList");
            identifier = _tokenizer.Identifier();

            while (identifier != ")")
            {
                InsertIdentifier();
                InsertComma();
                identifier = _tokenizer.Identifier();
            }
            WriteGrammarType("/parameterLst");
           // Console.WriteLine("/parameterList");
        }

        //Sorted
        public void CompileLetStatement()
        {
            //Console.WriteLine("CompileLetStatement");
            WriteGrammarType("letStatement");
            InsertLet();
            InsertIdentifier();
            char symbol = _tokenizer.Symbol();
            if (symbol == '[')
            {
                InsertLeftSquareBrace();
                CompileExpression();
                InsertRightSquareBrace();
            }
            InsertOperator();
            CompileExpression();
            //Console.WriteLine("statment compiled in let");
            if(_tokenizer.Symbol()==';') InsertSemiColon();
            WriteGrammarType("/letStatement");
            //Console.WriteLine("/CompileLetStatement");

        }

        public void CompileIfStatement()
        {
            //Console.WriteLine("CompileIfStaement");
            WriteGrammarType("ifStatement");

            InsertIf();
            InsertLeftBrace();
            CompileExpression();
            InsertRightBrace();
            InsertLeftCurly();
            CompileStatements();
            InsertRightCurly();
            string identifier = _tokenizer.Identifier();
            if (identifier == "else")

            {
                while (identifier == "else")
                {
                    InsertElse();
                    InsertLeftCurly();
                    CompileStatements();
                    InsertRightCurly();
                    identifier = _tokenizer.Identifier();
                }
                
            }
            //Console.WriteLine("/ifStatement");
            WriteGrammarType("/ifStatement");

        }

        public void CompileWhileStatement()
        {
            //Console.WriteLine("CompileWhileStatement");
            WriteGrammarType("whileStatement");
            InsertWhile();
            InsertLeftBrace();
            CompileExpression();
            InsertRightBrace();
            InsertLeftCurly();
            CompileStatements();
            InsertRightCurly();
            //Console.WriteLine("/WhileStatement");
            WriteGrammarType("/whileStatement");
        }

        public void CompileDoStatement()
        {
            //Console.WriteLine("CompileDostatement");
            WriteGrammarType("doStatement");
            InsertDo();
            InsertIdentifier();
            CompileSubroutineCall();
            InsertSemiColon();
            WriteGrammarType("/doStatement");
            //Console.WriteLine("/doStatement");

        }

        public void CompileReturnStatement()
        {

            WriteGrammarType("returnStatement");
            //Console.WriteLine("returnStatement");
            InsertReturn();
            //Console.WriteLine("return insert. Next identifier: " + _tokenizer.Identifier());
            string identifier = _tokenizer.Identifier();
            if (identifier != ";")
            {
                CompileExpression();
            }
            InsertSemiColon();
            //Console.WriteLine("/returnStatement");
            WriteGrammarType("/returnStatement");
        }

        //Sorted
        public void CompileTerm()
        {
            //Console.WriteLine("CompileTerm");
            string tokenType;
            string identifier;
            WriteGrammarType("term");
            tokenType = _tokenizer.TokenType();
            identifier = _tokenizer.Identifier();
            
            if (tokenType == "INT_CONST")
            {
                InsertIntConst();
            }
            else if (tokenType == "STRING_CONST")
            {
                InsertStringConst();
            }
            else if (tokenType == "KEYWORD")
            {
                InsertKeyword();
            }
            else if (tokenType == "SYMBOL")
            {
                if (_tokenizer.Identifier() == "-" || _tokenizer.Identifier() == "~")
                {
                    InsertOperator();
                    CompileTerm();
                }
                else
                {
                   if( _tokenizer.Identifier() == "(")
                    {
                        InsertLeftBrace();
                        CompileExpression();
                        InsertRightBrace();
                    }
                }
                
            }
            else
            {
                InsertIdentifier();
                char symbol = _tokenizer.Symbol();
                //Console.WriteLine("Compile term lookeahed " + symbol);
                switch (symbol)
                {
                    case '[':
                        InsertLeftSquareBrace();
                        CompileExpression();
                        InsertRightSquareBrace();
                        break;
                    case '.':
                        CompileSubroutineCall();
                        break;
                    case '(':
                        CompileSubroutineCall();
                        break;
                    case ';':
                        break;
                    default:
                        break;
                }
            }
            WriteGrammarType("/term");
            //Console.WriteLine("/term" );
            //Console.WriteLine("Exititng term with the identifier:"+_tokenizer.Identifier());
        }

        public void CompileExpression()
        {
            //Console.WriteLine("Expression");
            WriteGrammarType("expression");
            CompileTerm();
            string op = _tokenizer.Identifier();
            while (op == "-" || op == "+" || op == "~" || op == "/" || op == "*" || op == "&" || op == "<" || op == ">" || op == "|")
            {
                InsertOperator();
                CompileTerm();
                op = _tokenizer.Identifier();
            }
            //if(_tokenizer.Symbol()==';') InsertSemiColon();
            WriteGrammarType("/expression");
            //Console.WriteLine("/Expressiom");
        }

        public void CompileSubroutineCall()
        {
            //Console.WriteLine("SubroutineCall");
            string symbol;
            //InsertIdentifier();
            symbol = _tokenizer.Identifier();
            if (symbol == ".")
            {
                InsertFullStop();
                InsertIdentifier();
            }
            InsertLeftBrace();
            CompileExpressionList();
            InsertRightBrace();
            Console.WriteLine("/SubroutineCall");

        }

        public void CompileExpressionList()
        {
            string identifier = _tokenizer.Identifier();
            //Console.WriteLine("expressionList");
            WriteGrammarType("expressionList");
            while (identifier != ")")
            {
                CompileExpression();
                if(_tokenizer.Identifier()==",") InsertComma();
                identifier = _tokenizer.Identifier();
            }
            WriteGrammarType("/expressionList");
            //Console.WriteLine("/expressionList");
            
        }

        public void InsertSemiColon()
        {
            char c = _tokenizer.Symbol();
            if (c != ';') throw new Exception("char not semicolon: " + c);
            _writer.WriteLine($"<symbol> {c} </symbol>");
            _tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertLeftCurly()
        {
            char c = _tokenizer.Symbol();
            if (c != '{') throw new Exception("char not left curly: " + c);
            _writer.WriteLine($"<symbol> {c} </symbol>");
            _tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertRightCurly()
        {
            //Console.WriteLine("Right urly invoked");
            char c = _tokenizer.Symbol();
            if (c != '}') throw new Exception("char not right curly: " + c);
            _writer.WriteLine($"<symbol> {c} </symbol>");
            _tokenWriter.WriteLine($"<symbol> {c} </symbol>");
           // Console.WriteLine("Goint for advance");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
            //Console.WriteLine("Aadvance");
        }

        public void InsertLeftBrace()
        {
           // Console.WriteLine("Char on landing in insertleft brace:" + _tokenizer.Symbol());
            char c = _tokenizer.Symbol();
            if (c != '(') throw new Exception("char not left brace: " + c);
            _writer.WriteLine($"<symbol> {c} </symbol>");
            _tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertRightBrace()
        {
            char c = _tokenizer.Symbol();
            if (c != ')') throw new Exception("char not right brace: " + c);
            _writer.WriteLine($"<symbol> {c} </symbol>");
            _tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertComma()
        {
            char c = _tokenizer.Symbol();
            if (c != ',') throw new Exception("char notcomma: " + c);
            _writer.WriteLine($"<symbol> {c} </symbol>");
            _tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertFullStop()
        {
            char c = _tokenizer.Symbol();
            if (c != '.') throw new Exception("char not full stop: " + c);
            _writer.WriteLine($"<symbol> {c} </symbol>");
            _tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertIdentifier()
        {
            string reff = _tokenizer.Identifier();
            Console.WriteLine("Identifier inserted is :" + reff);
            _writer.WriteLine($"<identifier> {reff} </identifier>");
            _tokenWriter.WriteLine($"<identifier> {reff} </identifier>");
            if (!_tokenizer.HasMoreToken())
            {
                _tokenizer.Advance();
                //Console.WriteLine("next identifier is:" + _tokenizer.Identifier());
            }
        }
        public void InsertLeftSquareBrace()
        {
            char c = _tokenizer.Symbol();
            if (c != '[') throw new Exception("char not[: " + c);
            _writer.WriteLine($"<symbol> {c} </symbol>");
            _tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertRightSquareBrace()
        {
            char c = _tokenizer.Symbol();
            if (c != ']') throw new Exception("char not ]: " + c);
            _writer.WriteLine($"<symbol> {c} </symbol>");
            _tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertOperator()
        {
            string op = _tokenizer.Identifier();
            if (op == "-" || op == "+" || op == "~" || op == "/" || op == "*" || op == "&" || op == "<" || op == ">" || op == "|"||op=="=")
            {
                if (op == "<") op = "&lt;";
                if (op == ">") op = "&gt;";
                if (op == "&") op = "&amp;";
                _writer.WriteLine($"<symbol> {op} </symbol>");
                _tokenWriter.WriteLine($"<symbol> {op} </symbol>");
                if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
            }
            else
            {
                throw new Exception("char not an operator: " + op);
            }
        }

        public void InsertLet()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword != "let") throw new Exception("Keyword is not let: " + keyword);
            _writer.WriteLine($"<keyword> {keyword} </keyword>");
            _tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertIf()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword != "if") throw new Exception("Keyword is not if: " + keyword);
            _writer.WriteLine($"<keyword> {keyword} </keyword>");
            _tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertElse()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword != "else") throw new Exception("Keyword is not else: " + keyword);
            _writer.WriteLine($"<keyword> {keyword} </keyword>");
            _tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertDo()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword != "do") throw new Exception("Keyword is not do: " + keyword);
            _writer.WriteLine($"<keyword> {keyword} </keyword>");
            _tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertWhile()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword != "while") throw new Exception("Keyword is not while: " + keyword);
            _writer.WriteLine($"<keyword> {keyword} </keyword>");
            _tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertReturn()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword != "return") throw new Exception("Keyword is not return: " + keyword);
            _writer.WriteLine($"<keyword> {keyword} </keyword>");
            _tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertVar()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword != "var") throw new Exception("Keyword is not var: " + keyword);
            _writer.WriteLine($"<keyword> {keyword} </keyword>");
            _tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertConMethodFunction()
        {
            string identifier = _tokenizer.Identifier();
            if (identifier == "constructor" || identifier == "method" || identifier == "function")
            {
                _writer.WriteLine($"<keyword> {identifier} </keyword>");
                _tokenWriter.WriteLine($"<keyword> {identifier} </keyword>");
                if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
                
            }
            else
            {
                throw new Exception("Keyword is not method/function/constructor: " + identifier);
            }
            

        }
        public void InsertStaticField()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword == "field" || keyword == "static")
            {
                _writer.WriteLine($"<keyword> {keyword} </keyword>");
                _tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
                if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();

            }

            else
            {
                throw new Exception("Keyword is not field/static:" + keyword);
            }
            
        }
        public void InsertClass()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword != "class") throw new Exception("Keyword is not class: " + keyword);
            _writer.WriteLine($"  <keyword> {keyword} </keyword>");
            _tokenWriter.WriteLine($"<keyword> {keyword}</keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertStringConst()
        {
            string identifier = _tokenizer.Identifier();
            _writer.WriteLine($"<stringConstant> {identifier} </stringConstant>");
            _tokenWriter.WriteLine($"<stringConstant> {identifier} </stringConstant>");
            //Console.WriteLine("written string constant: " + identifier);
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertIntConst()
        {
            int identifier = _tokenizer.IntVal();
            _writer.WriteLine($"<integerConstant> {identifier} </integerConstant>");
            _tokenWriter.WriteLine($"<integerConstant> {identifier} </integerConstatnt>");
            //Console.WriteLine("written int constant: " + identifier);
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertKeyword()
        {
            string keyword = _tokenizer.Identifier();
            if (_tokenizer.TokenType() != "KEYWORD") throw new Exception("current token not a keyword: " + keyword);
            _writer.WriteLine($"<keyword> {keyword} </keyword>");
            _tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void WriteGrammarType(string s)
        {
            _writer.WriteLine($"<{s}>");
        }
        public void WriteTokenTag(string s)
        {
            _tokenWriter.WriteLine($"<{s}>");
        }

    }
}
