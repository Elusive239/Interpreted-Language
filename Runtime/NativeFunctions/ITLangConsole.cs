using ITLang.Util;
using ITLang.Runtime;
using ITLang.Frontend;

namespace ITLang.Runtime.NativeFunctions
{
    public class ITLangConsole : BaseFunctionCollection{
        public ITLangConsole(){
            Add("print", PrintFunction);
            Add("println", PrintLNFunction);
            Add("exit", ExitFunction);
            Add("time", TimeFunction);
        }
        private RuntimeVal PrintFunction(RuntimeVal[] _args, Enviornment _env){
            string toPrint = "";
            foreach(RuntimeVal arg in _args){
                toPrint += arg.Stringify();
            }
            Console.Write(toPrint);
            return Runtime.Values.MK_NULL() ?? throw new Exception("Null is... null?");
        }

        private RuntimeVal PrintLNFunction(RuntimeVal[] _args, Enviornment _env){
            string toPrint = "";
            foreach(RuntimeVal arg in _args){
                toPrint += arg.Stringify();
            }
            Console.WriteLine(toPrint);
            return Runtime.Values.MK_NULL() ?? throw new Exception("Null is... null?");
        }

        private RuntimeVal TimeFunction(RuntimeVal[] _args, Enviornment _env) {
            var CurDate= DateTime.Now;

            ObjectLiteral obj = new ObjectLiteral()
            .AddProperty("hour", new NumericLiteral(CurDate.Hour))
            .AddProperty("minute", new NumericLiteral(CurDate.Minute))
            .AddProperty("second", new NumericLiteral(CurDate.Second))
            .AddProperty("milisecond", new NumericLiteral(CurDate.Millisecond))
            .AddProperty("microsecond", new NumericLiteral(CurDate.Microsecond))
            .AddProperty("day", new NumericLiteral(CurDate.Day))
            .AddProperty("month", new NumericLiteral(CurDate.Month))
            .AddProperty("year", new NumericLiteral(CurDate.Year));

            _env.DeclareVar("timeObj", Eval.Expressions.Eval_Object_Expr(obj, _env), true);
            return _env.lookupVar("timeObj");
        }

        private RuntimeVal ExitFunction(RuntimeVal[] _args, Enviornment _env){
            
            int exitCode = 0;

            if(_args.Length >= 1)
                exitCode = (int) ((NumberVal) (_args[0])).value;
            string message = $"Exit code: {exitCode} ";

            if(_args.Length > 2)
                message += ((StringVal) (_args[1])).value;

            throw new ExitException(message);
        }
    }
}