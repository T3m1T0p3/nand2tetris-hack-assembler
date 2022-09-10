using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackCompiler
{
    public class SymbolTable
    {
        private int _fieldRunningIndex;
        private int _staticRunningIndex;
        private int _argRunningIndex;
        private int _localRunningIndex;
        public Dictionary<string, Symbol> _classLevelSymbols;
        public Dictionary<string, Symbol> _subroutineLevelSymbols;
        public SymbolTable()
        {
            _fieldRunningIndex = 0;
            _staticRunningIndex = 0;
            _classLevelSymbols = new Dictionary<string, Symbol>();
            _subroutineLevelSymbols = new Dictionary<string, Symbol>();
        }

        public void StartSubroutine()
        {
            Console.WriteLine("Starting subroutine table");
            _subroutineLevelSymbols = new Dictionary<string, Symbol>();
            _argRunningIndex = 0;
            _localRunningIndex = 0;
        }

        public void Define(string name,string type,string kind)
        {
            switch (kind)
            {
                case "field":
                    _classLevelSymbols.Add(name, new Symbol { Index = _fieldRunningIndex++, Kind ="this", Type = type });
                    break;
                case "static":
                    _classLevelSymbols.Add(name, new Symbol { Index = _staticRunningIndex++, Kind = kind, Type = type });
                    break;
                case "arg":
                    _subroutineLevelSymbols.Add(name, new Symbol { Index = _argRunningIndex++, Kind = "argument", Type = type });
                    break;
                case "var":
                    _subroutineLevelSymbols.Add(name, new Symbol { Index = _localRunningIndex++, Kind = "local", Type = type });
                    //Console.WriteLine($"has key {name} with index:" + _subroutineLevelSymbols[name].Index);
                    break;
                default:
                    break;
            }
        }

        public int VarCount(string? kind)
        {
            switch (kind)
            {
                case "this":
                case "field":
                    return _fieldRunningIndex;
                case "static":
                    return _staticRunningIndex;
                case "arg":
                case "argument":
                    return _argRunningIndex;
                case "var":
                case "local":
                    return _localRunningIndex;
                default:
                    break;
            }
            return 0;
        }
        public string KindOf(string varName)
        {
            if (_subroutineLevelSymbols.ContainsKey(varName)) return _subroutineLevelSymbols[varName].Kind;
            else if (_classLevelSymbols.ContainsKey(varName)) return _classLevelSymbols[varName].Kind;
            
           else return null;
        }

        public string TypeOf(string varName)
        {
            
            if (_subroutineLevelSymbols.ContainsKey(varName)) return _subroutineLevelSymbols[varName].Type;  
            else if (_classLevelSymbols.ContainsKey(varName)) return _classLevelSymbols[varName].Type;
            else return null;
        }

        public int IndexOf(string varName)
        {
            
            if (_subroutineLevelSymbols.ContainsKey(varName)) return _subroutineLevelSymbols[varName].Index;
            else if (_classLevelSymbols.ContainsKey(varName)) return _classLevelSymbols[varName].Index;
            throw new Exception("invalid IndexOf");
        }
    }
}
