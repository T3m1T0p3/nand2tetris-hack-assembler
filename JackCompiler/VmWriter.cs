using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackCompiler
{
    public class VmWriter
    {
        string _outputFile;

        StreamWriter _writer;

        public VmWriter(string outputfile)
        {
            _writer = new StreamWriter(outputfile,false);
        }

        public void WritePush(string segment,int index)
        {
            _writer.WriteLine($"push {segment} {index}");
        }

        public void WritePop(string segment,int index)
        {
            _writer.WriteLine($"pop {segment} {index}");
        }

        public void WriteArithmentic(string op)
        {
            switch (op)
            {
                case "+":
                    _writer.WriteLine("add");
                    break;
                case "-":
                    _writer.WriteLine("sub");
                    break;
                case "*":
                    _writer.WriteLine("call Math.multiply 2");
                    break;
                case "neg":
                    _writer.WriteLine("neg");
                    break;
                case "<":
                    _writer.WriteLine("lt");
                    break;
                case ">":
                    _writer.WriteLine("gt");
                    break;
                case "~":
                    _writer.WriteLine("not");
                    break;
                case "/":
                    _writer.WriteLine("call Math.divide 2");
                    break;
                case "&":
                    _writer.WriteLine("and");
                    break;
                case "|":
                    _writer.WriteLine("or");
                    break;
                case "=":
                    _writer.WriteLine("eq");
                    break;
                default:
                    break;
            }

        }
        public void WriteLabel(string label)
        {
            _writer.WriteLine($"label {label}");
        }

        public void WriteGoto(string label)
        {
            _writer.WriteLine($"goto {label}");
        }
        public void WriteIf(string label)
        {
            _writer.WriteLine($"if-goto {label}");
        }
        public void WriteCall(string name, int? nArgs)
        {
            _writer.WriteLine($"call {name} {nArgs}");
        }

        public void WriteFunction(string name, int nArgs)
        {
            _writer.WriteLine($"function {name} {nArgs}");
        }

        public void WriteReturn()
        {
            _writer.WriteLine("return");
        }

        public void Close()
        {
            _writer.Close();
        }
    }
}
