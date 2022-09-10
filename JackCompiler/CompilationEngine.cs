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
        //StreamWriter _writer;
        //StreamWriter _tokenWriter;
        //StreamReader _reader;
        Tokenizer _tokenizer;
        string fileName;
        SymbolTable symbolTable;
        VmWriter _vmWriter;
        int _labelRun = 0;
        string _className;
        string _subroutineName;
        string _subroutineType;
        public CompilationEngine(string inputFile)//,string outputFile)

        {
            fileName = inputFile;
   //         string outputFile = inputFile.Replace(".jack", ".xml");

     //       string tokenFile = inputFile.Replace(".jack", "T.xml");
            _tokenizer = new Tokenizer(inputFile);
            //_writer = new StreamWriter(outputFile, false);
            //_tokenWriter = new StreamWriter(tokenFile, false);
            _vmWriter = new VmWriter(fileName.Replace(".jack",".vm"));
            CompileClass();

        }
        public void CompileClass()
        {
            _tokenizer.Advance();
            //Console.WriteLine("compileClass");
            WriteGrammarType("class");
            WriteTokenTag("token");
            Console.WriteLine("Staring new file:" + fileName);
            InsertClass();

            symbolTable = new SymbolTable();
            _className = _tokenizer.Identifier();
            InsertIdentifier(_className, "class", true);
            InsertLeftCurly();
            CompileClassVarDec();
            while (_tokenizer.Identifier() == "method" || _tokenizer.Identifier() == "function" || _tokenizer.Identifier() == "constructor" && !_tokenizer.HasMoreToken())
            {
                _subroutineType = _tokenizer.Identifier();
                symbolTable.StartSubroutine();
                CompileSubroutineDec();
            }
            Console.WriteLine("Closing with right curly:" + _tokenizer.HasMoreToken());
            InsertRightCurly();
            Console.WriteLine("right curly inserted");
            WriteGrammarType("/class");
            WriteTokenTag("/token");
            _tokenizer.Close();
            //_writer.Close();
            _vmWriter.Close();
            //_tokenWriter.Close();
            //_tokenWriter.Dispose();
            //_writer.Dispose();

            Console.WriteLine("Closed All");

        }

        //sorted
        public void CompileClassVarDec()
        {

            //WriteGrammarType("ClassVarDec");
            Console.WriteLine("ClassVarDec");
            while (_tokenizer.Identifier() == "static" || _tokenizer.Identifier() == "field")
            {
                WriteGrammarType("classVarDec");

                string kind;
                string name;
                string type;

                kind = _tokenizer.Identifier();//static or field

                InsertStaticField();

                type = _tokenizer.Identifier();

                InsertIdentifier();

                name = _tokenizer.Identifier();



                InsertIdentifier(name, type, kind, true); //true==definging

                char symbol = _tokenizer.Symbol();
                while (symbol != ';')
                {
                    InsertComma();
                    name = _tokenizer.Identifier();

                    InsertIdentifier(name, type, kind, true);
                    symbol = _tokenizer.Symbol();
                }
                InsertSemiColon();
                WriteGrammarType("/classVarDec");
            }

            //WriteGrammarType("/ClassVarDec");
            Console.WriteLine("/ClassVarDec");
        }
        //Sorted
        public void CompileSubroutineDec()
        {
            //function void main()
            Console.WriteLine("CompileSubDec");
            WriteGrammarType("subroutineDec");
            string returnType;
            string subroutineType = _tokenizer.Identifier();
            InsertConMethodFunction();
            returnType = _tokenizer.Identifier();
            if (_tokenizer.TokenType() == "KEYWORD")
            {
                Console.WriteLine("Keyword:" + _tokenizer.Identifier());
                InsertKeyword();
            }
            else
            {
                InsertIdentifier();
            }
            _subroutineName = _tokenizer.Identifier();

            if (subroutineType == "constructor")
            {
                InsertIdentifier("new");
                InsertLeftBrace();
                CompileParameterList();
                InsertRightBrace();
                int classSize = symbolTable.VarCount("field");// ? symbolTable.VarCount("field") : symbolTable.VarCount("var");
                _vmWriter.WriteFunction(_className + "." + _subroutineName, symbolTable.VarCount("var"));
                _vmWriter.WritePush("constant", classSize);
                _vmWriter.WriteCall("Memory.alloc", 1);
                _vmWriter.WritePop("pointer", 0);
                CompileSubroutineBody();
            }
            else
            {
                if (subroutineType == "method") symbolTable.Define("this",_className,"arg");
                InsertIdentifier(_subroutineName, "subroutine", true);
                InsertLeftBrace();
                //symbolTable.Define("this", _className, "arg");//define this for each method declaration
                
                CompileParameterList();
                InsertRightBrace();
                CompileSubroutineBody();
            }
            WriteGrammarType("/subroutineDec");
            Console.WriteLine("All subroutines successfully paresed");

        }

        //Sorted
        public void CompileSubroutineBody()
        {
            Console.WriteLine("CompileSubBody");
            WriteGrammarType("subroutineBody");
            InsertLeftCurly();
            string identifier = _tokenizer.Identifier();
            if (identifier == "var")
            {
                CompileVarDec();
            }
            if(_subroutineType!="constructor")_vmWriter.WriteFunction(_className + "." + _subroutineName, symbolTable.VarCount("var"));
            if (_subroutineType == "method")//method compilation
            {
                _vmWriter.WritePush("argument", 0);//push arg 0
                _vmWriter.WritePop("pointer", 0);//pop pointer 0
            }
            CompileStatements();
            InsertRightCurly();
            WriteGrammarType("/subroutineBody");
            Console.WriteLine("/CompileSubBody");

        }
        public void CompileVarDec()
        {
            //var SquareGame game;
            Console.WriteLine("CompileVarDec");
            //WriteGrammarType("varDec");
            string kind = "var";
            string name;
            string type;
            while (_tokenizer.Identifier() == "var")
            {
                WriteGrammarType("varDec");
                Console.WriteLine(_tokenizer.Identifier());
                InsertVar();//var

                type = _tokenizer.Identifier();
                Console.WriteLine("type:" + type);

                InsertIdentifier();//type
                name = _tokenizer.Identifier();
                InsertIdentifier(name, type, kind,true);
                char symbol = _tokenizer.Symbol();
                while (symbol == ',')
                {
                    InsertComma();
                    name = _tokenizer.Identifier();
                    InsertIdentifier(name, type, kind,true);
                    symbol = _tokenizer.Symbol();
                }
                InsertSemiColon();
                WriteGrammarType("/varDec");
            }

            //WriteGrammarType("/varDec");
            Console.WriteLine("/CompileVarDec");

        }

        public void CompileStatements()
        {
            Console.WriteLine("CompileStatements");
            string identifier = _tokenizer.Identifier();
            WriteGrammarType("statements");
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
            Console.WriteLine("Exiting compilestatements while with identifier:" + identifier);
            Console.WriteLine("/CompileStatements");
            WriteGrammarType("/statements");
        }



        //Sorted
        public void CompileParameterList()
        {
            string identifier;
            WriteGrammarType("parameterList");
            Console.WriteLine("parameterList");
            identifier = _tokenizer.Identifier();
            string name, type, kind;
            kind = "arg";
            while (identifier != ")")
            {
                type = _tokenizer.Identifier();
                InsertIdentifier();
                Console.WriteLine("Identifier in paramList:" + _tokenizer.Identifier());
                name = _tokenizer.Identifier();
                InsertIdentifier(name, type, kind, true);
                identifier = _tokenizer.Identifier();
                if (identifier != ")") InsertComma();
                identifier = _tokenizer.Identifier();
            }
            WriteGrammarType("/parameterList");
            Console.WriteLine("/parameterList");
        }


        //Sorted
        public void CompileLetStatement()
        {
            Console.WriteLine("CompileLetStatement");
            WriteGrammarType("letStatement");
            InsertLet();
            string name = _tokenizer.Identifier();
            string ops,kindof;
            kindof = symbolTable.KindOf(name);
            int varIndex = symbolTable.IndexOf(name);
            InsertIdentifier(name);
            char symbol = _tokenizer.Symbol();
            if (symbol == '[')
            {
                _vmWriter.WritePush(kindof,varIndex);
                InsertLeftSquareBrace();
                CompileExpression();
                _vmWriter.WriteArithmentic("+");
                InsertRightSquareBrace();
            }
            ops = _tokenizer.Identifier();
            InsertOperator();
            CompileExpression();
            if (symbol == '[')
            {
                _vmWriter.WritePop("temp", 0);
                _vmWriter.WritePop("pointer", 1);
                _vmWriter.WritePush("temp", 0);
                _vmWriter.WritePop("that", 0);
            }
            else
            {
                if (ops != "=") _vmWriter.WriteArithmentic(ops);
                _vmWriter.WritePop(kindof, varIndex);
                //Console.WriteLine("statment compiled in let");
            }
            if (_tokenizer.Symbol() == ';') InsertSemiColon();
            WriteGrammarType("/letStatement");
            Console.WriteLine("/CompileLetStatement");

        }

        public void CompileIfStatement()
        {
            Console.WriteLine("CompileIfStaement");
            WriteGrammarType("ifStatement");
            InsertIf();
            InsertLeftBrace();
            CompileExpression();
            int labelNo=_labelRun++;
            _vmWriter.WriteArithmentic("~");//not
            _vmWriter.WriteIf($"{_className}.{_subroutineName}_LABEL_IF_FALSE_{labelNo}"); //if-goto L1

            InsertRightBrace();
            InsertLeftCurly();
            CompileStatements();
            _vmWriter.WriteGoto($"{_className}.{_subroutineName}_LABEL_IF_TRUE_{labelNo}");//goto l2
            
            

            InsertRightCurly();
            string identifier = _tokenizer.Identifier();
            if (identifier == "else")

            {
                while (identifier == "else")
                {
                    _vmWriter.WriteLabel($"{_className}.{_subroutineName}_LABEL_IF_FALSE_{labelNo}");
                    InsertElse();
                    InsertLeftCurly();
                    CompileStatements();
                    InsertRightCurly();
                    identifier = _tokenizer.Identifier();
                }
                

            }
            else
            {
                _vmWriter.WriteLabel($"{_className}.{_subroutineName}_LABEL_IF_FALSE_{labelNo}");
            }
            _vmWriter.WriteLabel($"{_className}.{_subroutineName}_LABEL_IF_TRUE_{labelNo}");
            Console.WriteLine("/ifStatement");
            WriteGrammarType("/ifStatement");

        }

        public void CompileWhileStatement()
        {
            Console.WriteLine("CompileWhileStatement");
            WriteGrammarType("whileStatement");
            InsertWhile();
            InsertLeftBrace();
            int labelNo = _labelRun++;
            _vmWriter.WriteLabel($"{_className}.{_subroutineName}_LABEL_WHILE_{labelNo}");//label
            CompileExpression();
            _vmWriter.WriteArithmentic("~");//not

            InsertRightBrace();
            InsertLeftCurly();

            _vmWriter.WriteIf($"{_className}.{_subroutineName}_LABEL_NOT_WHILE_{labelNo}");
            CompileStatements();
            _vmWriter.WriteGoto($"{_className}.{_subroutineName}_LABEL_WHILE_{labelNo}");
            _vmWriter.WriteLabel($"{_className}.{_subroutineName}_LABEL_NOT_WHILE_{labelNo}");

            InsertRightCurly();
            Console.WriteLine("/WhileStatement");
            WriteGrammarType("/whileStatement");
        }

        public void CompileDoStatement()
        {
            Console.WriteLine("CompileDostatement");
            WriteGrammarType("doStatement");
            InsertDo();
            string symbol;
            string className;
            string funcName="";
            int nArgs=0;
            string kindofClass;
            string typeofClass="";
            int varIndex;

            className=_tokenizer.Identifier();
            kindofClass = symbolTable.KindOf(className);
            typeofClass= symbolTable.TypeOf(className);
            Console.WriteLine("className in do statement:" + className);
            InsertIdentifier(className,"class");
            
            //pushes object to stack first
            if (kindofClass != null)
            {
                varIndex = symbolTable.IndexOf(className);
                _vmWriter.WritePush(kindofClass, varIndex);
            }

            //replace class name e.g game.run() with the name of base class
            className = typeofClass == null ? className : typeofClass;

            symbol = _tokenizer.Identifier();
            if (symbol == ".")
            {
                InsertFullStop();
                funcName =_tokenizer.Identifier();
                nArgs = symbolTable.VarCount(funcName);
                InsertIdentifier(funcName, "subroutine");
                funcName = "."+funcName;   
            }

            //method lies within the class
            else
            {
                _vmWriter.WritePush("pointer", 0);
                className = _className + "." + className;
                nArgs++; //this
            }
            InsertLeftBrace();
            nArgs+=CompileExpressionList();
            InsertRightBrace();

            if (kindofClass != null) nArgs++;//Object plus all other arguments

            _vmWriter.WriteCall(className + funcName, nArgs);
            _vmWriter.WritePop("temp",0);//do statment is expected to return void; dispose garbage returned to stack
            InsertSemiColon();
            WriteGrammarType("/doStatement");
            Console.WriteLine("/doStatement");

        }

        public void CompileReturnStatement()
        {

            WriteGrammarType("returnStatement");
            Console.WriteLine("returnStatement");
            InsertReturn();
            Console.WriteLine("return insert. Next identifier: " + _tokenizer.Identifier());
            string identifier = _tokenizer.Identifier();
           if (identifier == "this")
            {
                InsertIdentifier(identifier);
                _vmWriter.WritePush("pointer", 0);
                _vmWriter.WriteReturn();
                InsertSemiColon();
                Console.WriteLine("/returnStatement -this");
                WriteGrammarType("/returnStatement");
                return;
            }
            if (identifier != ";")
            {
                CompileExpression();
            }
            
            else if (identifier == ";")//take care of void method?
            {
                _vmWriter.WritePush("constant", 0);
            }
 
            _vmWriter.WriteReturn();
            InsertSemiColon();
            Console.WriteLine("/returnStatement");
            WriteGrammarType("/returnStatement");
        }

        //Sorted
        public void CompileTerm()
        {
            Console.WriteLine("CompileTerm");
            string tokenType;
            string identifier;
            WriteGrammarType("term");
            tokenType = _tokenizer.TokenType();
            identifier = _tokenizer.Identifier();
            string name = identifier;
            int konst;
            int nArgs;
            string kindof,typeOf;
            int varIndex;

            if (tokenType == "INT_CONST")
            {
                konst = _tokenizer.IntVal();
                Console.WriteLine("Inserting int constant:" + konst);
                _vmWriter.WritePush("constant", konst);
                InsertIntConst();
            }

            else if (tokenType == "STRING_CONST")
            {
                int strnLen = identifier.Length;
                _vmWriter.WritePush("constant", strnLen);
                _vmWriter.WriteCall("String.new", 1);
                foreach(char c in identifier)
                {
                    _vmWriter.WritePush("constant", (int)c);
                    _vmWriter.WriteCall("String.appendChar", 2);
                }

                InsertStringConst();
            }

            else if (tokenType == "KEYWORD")
            {
                switch (identifier)
                {
                    case "true":
                        _vmWriter.WritePush("constant", 0);
                        _vmWriter.WriteArithmentic("~");
                        break;
                    case "false":
                        _vmWriter.WritePush("constant", 0);
                        break;
                    case "null":
                        _vmWriter.WritePush("constant", 0);
                        break;
                    case "this":
                        _vmWriter.WritePush("pointer", 0);
                        break;
                    default:
                        break;
                }
                InsertKeyword();
            }

            else if (tokenType == "SYMBOL")
            {
                switch (identifier)
                {
                    case "-":
                        Console.WriteLine("inserting unary op:" + identifier);
                        InsertOperator();
                        CompileTerm();
                        _vmWriter.WriteArithmentic("neg");
                        break;
                    case "~":
                        Console.WriteLine("inserting unary op:" + identifier);

                        InsertOperator();
                        CompileTerm();
                        _vmWriter.WriteArithmentic(identifier);
                        break;
                    case "=":
                        Console.WriteLine("inserting equality:" + identifier);

                        InsertOperator();
                        //CompileTerm();
                        _vmWriter.WriteArithmentic(identifier);
                        break;
                    default:
                        break;
                }
                if (_tokenizer.Identifier() == "(")
                {
                    InsertLeftBrace();
                    CompileExpression();
                    InsertRightBrace();
                }
            }

            else
            {
                //could be a class or subroutine name e.g Foo.bar() or bar() or bar;
                //Insert identifier without advance because of token lookahead
                string temp = _tokenizer.Identifier();
                kindof = symbolTable.KindOf(temp);
                typeOf= symbolTable.TypeOf(temp);
                if (kindof == "local" || kindof == "var" || kindof == "argument" || kindof == "field" || kindof == "static"||kindof=="this")
                {
                    varIndex = symbolTable.IndexOf(temp);
                    _vmWriter.WritePush(kindof, varIndex);
                }

                if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
                char symbol = _tokenizer.Symbol();

                if (symbol == '.')
                {
                    InsertIdentifierSpecial(temp, "class");
                }
                else if (symbol == '(')
                {
                    InsertIdentifierSpecial(temp, "subroutine");
                }

                Console.WriteLine("Compile term lookeahed " + symbol);
                switch (symbol)
                {
                    //a[i]=b[j]
                    case '[':
                        InsertLeftSquareBrace();
                        CompileExpression(); //write push i
                        InsertRightSquareBrace();
                        _vmWriter.WriteArithmentic("+");
                        _vmWriter.WritePop("pointer", 1);
                        _vmWriter.WritePush("that", 0);
                        //_vmWriter.WritePop("temp", 0);
                        break;
                    case '.':
                        //CompileSubroutineCall();
                        //case foo.Bar()
                        InsertFullStop();
                        name = _tokenizer.Identifier();
                        InsertIdentifier(name, "subroutine");
                        InsertLeftBrace();
                        nArgs = CompileExpressionList();
                        InsertRightBrace();
                        //call class.new 0
                        if (kindof == null) _vmWriter.WriteCall(temp + "." + name, nArgs);

                        else
                        {
                            temp = typeOf == null ? temp : typeOf; //nArg+ 1 because;push object to the stack before method call
                            _vmWriter.WriteCall(temp + "." + name, nArgs+1);// symbolTable.VarCount(kindof));//call class.method varcount
                        }
                        break;
                    case '(':
                        InsertLeftBrace();
                        nArgs = CompileExpressionList();
                        InsertRightBrace();
                        temp = typeOf == null ? temp : typeOf;
                        _vmWriter.WriteCall(temp + "." + name, nArgs+1);
                        break;
                    case ';':
                        break;
                    default:
                        break;
                }
            }
                    
            WriteGrammarType("/term");

            Console.WriteLine("Exititng term with the identifier:" + _tokenizer.Identifier());
            Console.WriteLine("/term");
        }

        public void CompileExpression()
        {
            Console.WriteLine("Expression");
            WriteGrammarType("expression");
            CompileTerm();
            List<string> pq= new List<string>();
            int operatorPriority = -1;
            string op = _tokenizer.Identifier();
            while (op == "-" || op == "+" || op == "~" || op == "/" || op == "*" || op == "&" || op == "<" || op == ">" || op == "|" || op == "=")
            {
                pq.Add(op);
                operatorPriority++;
                InsertOperator();
                CompileTerm();
                op = _tokenizer.Identifier();
            }

            while (pq.Count > 0)
            {
                 _vmWriter.WriteArithmentic(pq[operatorPriority]);
                pq.RemoveAt(operatorPriority--);
            }

            WriteGrammarType("/expression");
            Console.WriteLine("/Expressiom");
        }

        public int CompileExpressionList()
        {
            string identifier = _tokenizer.Identifier();
            Console.WriteLine("expressionList");
            WriteGrammarType("expressionList");
            int nArgs = 0;
            while (identifier != ")")
            {
                nArgs++;
                if (identifier == "=")
                {
                    CompileTerm();
                }
                else CompileExpression();
                if (_tokenizer.Identifier() == ",") InsertComma();
                identifier = _tokenizer.Identifier();
            }
            WriteGrammarType("/expressionList");
            Console.WriteLine("/expressionList");
            return nArgs;
        }

        public void InsertSemiColon()
        {
            char c = _tokenizer.Symbol();
            if (c != ';') throw new Exception("char not semicolon: " + c);
            //_writer.WriteLine($"<symbol> {c} </symbol>");
            //_tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertLeftCurly()
        {
            char c = _tokenizer.Symbol();
            if (c != '{') throw new Exception("char not left curly: " + c);
            //_writer.WriteLine($"<symbol> {c} </symbol>");
            //_tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertRightCurly()
        {
            //Console.WriteLine("Right urly invoked");
            char c = _tokenizer.Symbol();
            if (c != '}') throw new Exception("char not right curly: " + c);
            //_writer.WriteLine($"<symbol> {c} </symbol>");
            //_tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            // Console.WriteLine("Goint for advance");
            if (!_tokenizer.HasMoreToken())
            {
                Console.WriteLine("Advancing:" + _tokenizer.HasMoreToken());
                _tokenizer.Advance();
                Console.WriteLine("Advancing:");
            }
        }

        public void InsertLeftBrace()
        {
            // Console.WriteLine("Char on landing in insertleft brace:" + _tokenizer.Symbol());
            char c = _tokenizer.Symbol();
            if (c != '(') throw new Exception("char not left brace: " + c);
            //_writer.WriteLine($"<symbol> {c} </symbol>");
            //_tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertRightBrace()
        {
            char c = _tokenizer.Symbol();
            if (c != ')') throw new Exception("char not right brace: " + c);
            //_writer.WriteLine($"<symbol> {c} </symbol>");
            //_tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertComma()
        {
            char c = _tokenizer.Symbol();
            if (c != ',') throw new Exception("char notcomma: " + c);
            //_writer.WriteLine($"<symbol> {c} </symbol>");
            //_tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertFullStop()
        {
            char c = _tokenizer.Symbol();
            if (c != '.') throw new Exception("char not full stop: " + c);
            //_writer.WriteLine($"<symbol> {c} </symbol>");
            //_tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertIdentifier()
        {
            string reff = _tokenizer.Identifier();
            if (_tokenizer.TokenType() == "KEYWORD") InsertKeyword();
            else
            {
                //_writer.WriteLine($"<identifier> {reff} </identifier>");
                //_tokenWriter.WriteLine($"<identifier> {reff} </identifier>");
                if (!_tokenizer.HasMoreToken())
                {
                    _tokenizer.Advance();
                    //Console.WriteLine("next identifier is:" + _tokenizer.Identifier());
                }
            }

        }

        public void InsertIdentifier(string name)
        {
            string reff = _tokenizer.Identifier();
            var idHandler = IdentifierHandler(reff);
            if (_tokenizer.TokenType() == "KEYWORD") InsertKeyword();
            else
            {
                //_writer.WriteLine($"<identifier> {reff} </identifier>" + $":{idHandler}");
                //_tokenWriter.WriteLine($"<identifier> {reff} </identifier>");
                if (!_tokenizer.HasMoreToken())
                {
                    _tokenizer.Advance();
                    //Console.WriteLine("next identifier is:" + _tokenizer.Identifier());
                }
            }

        }
        public void InsertIdentifier(string name, string type, string kind, bool defining = false)
        {
            string reff = _tokenizer.Identifier();
            Console.WriteLine("define symbolTbale with name: " + name+" type: "+type+" kind:" + kind);
            symbolTable.Define(name, type, kind);
            Console.WriteLine("Invoking idHandler with name:" + name);
            var idHandler = IdentifierHandler(name,defining);
            if (_tokenizer.TokenType() == "KEYWORD") InsertKeyword();
            else
            {
                //_writer.WriteLine($"<identifier> {reff} </identifier>" + $":{idHandler}");
                //_tokenWriter.WriteLine($"<identifier> {reff} </identifier>");
                if (!_tokenizer.HasMoreToken())
                {
                    _tokenizer.Advance();
                    //Console.WriteLine("next identifier is:" + _tokenizer.Identifier());
                }
            }

        }

        //subroutine type
        public void InsertIdentifier(string name, string ClassOrSub, bool defining = false)
        {
            string reff = _tokenizer.Identifier();

            //symbolTable.Define(name, type, kind);
            if (_tokenizer.TokenType() == "KEYWORD") InsertKeyword();
            else
            {
                //_writer.WriteLine($"<identifier> {reff} </identifier>" +$":{name} {ClassOrSub} callORdef={defining}");
                //_tokenWriter.WriteLine($"<identifier> {reff} </identifier>");
                if (!_tokenizer.HasMoreToken())
                {
                    _tokenizer.Advance();
                    //Console.WriteLine("next identifier is:" + _tokenizer.Identifier());
                }
            }

        }

        public void InsertIdentifierSpecial(string name, string ClassOrSub, bool defining = false)
        {
            //string reff = _tokenizer.Identifier();

            //symbolTable.Define(name, type, kind);
            if (_tokenizer.TokenType() == "KEYWORD") InsertKeyword();
            else
            {
                //_writer.WriteLine($"<identifier> {name} </identifier>" + $":{name} {ClassOrSub} callORdef={defining}");
                //_tokenWriter.WriteLine($"<identifier> {name} </identifier>");
                //if (!_tokenizer.HasMoreToken())
                //{
                //    _tokenizer.Advance();
                    //Console.WriteLine("next identifier is:" + _tokenizer.Identifier());
               // }
            }

        }
        public void InsertIdentifierSpecial(string name, bool defining = false)
        {
            //string reff = _tokenizer.Identifier();
            var idHandler = IdentifierHandler(name);
            //symbolTable.Define(name, type, kind);
            if (_tokenizer.TokenType() == "KEYWORD") InsertKeyword();
            else
            {
                //_writer.WriteLine($"<identifier> {name} </identifier>" + $":{idHandler}");
                //_tokenWriter.WriteLine($"<identifier> {name} </identifier>");
                //if (!_tokenizer.HasMoreToken())
                //{
                //    _tokenizer.Advance();
                //Console.WriteLine("next identifier is:" + _tokenizer.Identifier());
                // }
            }

        }

        public void InsertLeftSquareBrace()
        {
            char c = _tokenizer.Symbol();
            if (c != '[') throw new Exception("char not[: " + c);
            //_writer.WriteLine($"<symbol> {c} </symbol>");
            //_tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertRightSquareBrace()
        {
            char c = _tokenizer.Symbol();
            if (c != ']') throw new Exception("char not ]: " + c);
            //_writer.WriteLine($"<symbol> {c} </symbol>");
            //_tokenWriter.WriteLine($"<symbol> {c} </symbol>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertOperator()
        {
            string op = _tokenizer.Identifier();
            if (op == "-" || op == "+" || op == "~" || op == "/" || op == "*" || op == "&" || op == "<" || op == ">" || op == "|" || op == "=")
            {
                if (op == "<") op = "&lt;";
                if (op == ">") op = "&gt;";
                if (op == "&") op = "&amp;";
                //_writer.WriteLine($"<symbol> {op} </symbol>");
                //_tokenWriter.WriteLine($"<symbol> {op} </symbol>");
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
            //_writer.WriteLine($"<keyword> {keyword} </keyword>");
            //_tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertIf()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword != "if") throw new Exception("Keyword is not if: " + keyword);
            //_writer.WriteLine($"<keyword> {keyword} </keyword>");
            //_tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertElse()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword != "else") throw new Exception("Keyword is not else: " + keyword);
            //_writer.WriteLine($"<keyword> {keyword} </keyword>");
            //_tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertDo()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword != "do") throw new Exception("Keyword is not do: " + keyword);
            //_writer.WriteLine($"<keyword> {keyword} </keyword>");
            //_tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertWhile()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword != "while") throw new Exception("Keyword is not while: " + keyword);
           // _writer.WriteLine($"<keyword> {keyword} </keyword>");
            //_tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertReturn()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword != "return") throw new Exception("Keyword is not return: " + keyword);
            //_writer.WriteLine($"<keyword> {keyword} </keyword>");
            //_tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertVar()
        {
            string keyword = _tokenizer.Identifier();
            if (keyword != "var") throw new Exception("Keyword is not var: " + keyword);
            //_writer.WriteLine($"<keyword> {keyword} </keyword>");
            //_tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertConMethodFunction()
        {
            string identifier = _tokenizer.Identifier();
            if (identifier == "constructor" || identifier == "method" || identifier == "function")
            {
                //_writer.WriteLine($"<keyword> {identifier} </keyword>" );
                //_tokenWriter.WriteLine($"<keyword> {identifier} </keyword>");
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
                //_writer.WriteLine($"<keyword> {keyword} </keyword>");
                //_tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
                if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();

            }

            else
            {
                throw new Exception("Keyword is not field/static:" + keyword);
            }

        }

        public void InsertStaticField(bool def)
        {
            string keyword = _tokenizer.Identifier();
            if (keyword == "field" || keyword == "static")
            {
                //_writer.WriteLine($"<keyword> {keyword} </keyword>");
                //_tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
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
            //_writer.WriteLine($"  <keyword> {keyword} </keyword>");
            //_tokenWriter.WriteLine($"<keyword> {keyword}</keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertStringConst()
        {
            string identifier = _tokenizer.Identifier();
            //_writer.WriteLine($"<stringConstant> {identifier} </stringConstant>");
            //_tokenWriter.WriteLine($"<stringConstant> {identifier} </stringConstant>");
            //Console.WriteLine("written string constant: " + identifier);
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }
        public void InsertIntConst()
        {
            int identifier = _tokenizer.IntVal();
            //_writer.WriteLine($"<integerConstant> {identifier} </integerConstant>");
            //_tokenWriter.WriteLine($"<integerConstant> {identifier} </integerConstatnt>");
            //Console.WriteLine("written int constant: " + identifier);
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void InsertKeyword()
        {
            string keyword = _tokenizer.Identifier();
            if (_tokenizer.TokenType() != "KEYWORD") throw new Exception("current token not a keyword: " + keyword);
            //_writer.WriteLine($"<keyword> {keyword} </keyword>");
            //_tokenWriter.WriteLine($"<keyword> {keyword} </keyword>");
            if (!_tokenizer.HasMoreToken()) _tokenizer.Advance();
        }

        public void WriteGrammarType(string s)
        {
            //_writer.WriteLine($"<{s}>");
        }
        public void WriteTokenTag(string s)
        {
            //_tokenWriter.WriteLine($"<{s}>");
        }

        public (string, string, int, bool) IdentifierHandler(string name, bool defining = false)
        {

            string kind = symbolTable.KindOf(name);
            string type = symbolTable.TypeOf(name);
            int varCount;
            if (kind == "var" || kind == "field" || kind == "static" || kind == "arg")
            {
                varCount = symbolTable.IndexOf(name);
                return (type, kind, varCount, defining);
            }
            else
            {
                return (type, kind,default,defining);
            }

        }
    }
}
