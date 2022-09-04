using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackCompiler
{
    public interface ICompilationEngine
    {
        public void CompileClass();
        public void CompileClassVarDec();
        public void CompileSubroutineDec();
        public void CompileParameterList();
        public void CompileSubroutineBody();
        public void CompileVarDec();
        public void CompileStatements();

        public void CompileLetStatement();
        public void CompileIfStatement();
        public void CompileWhileStatement();
        public void CompileDoStatement();
        public void CompileReturnStatement();
    }
}
