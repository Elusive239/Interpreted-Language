using ITLang.Frontend;
using ITLang.Util;
using static ITLang.Runtime.Interpreter;
using static ITLang.Runtime.Values;


namespace ITLang.Runtime.Eval
{
    public static class Expressions
    {
        /*
        *   Evaluates a unary expression (++ and -- for now)
        */
        public static RuntimeVal Eval_Unary_Expr(UnaryExpr val, Enviornment env){
            RuntimeVal returnValue = MK_NULL();

            if(val.value == null){
                throw new Exception("Unary Expression had null value somehow.");
            }

        
            RuntimeVal tmp = Evaluate(val.value, env);
            if(tmp.type != ValueType.NUMBER ){
                throw new Exception("ValueType.NUMBER Expected, Got " + tmp.type.Stringify());
            }

            returnValue = val.op switch
            {
                "++" => MK_NUMBER(((NumberVal)tmp).value + 1),
                "--" => MK_NUMBER(((NumberVal)tmp).value - 1),
                _ => throw new Exception("Unary Operator not yet implemented."),
            };

            if(val.value.kind == NodeType.Identifier){
                Identifier iden = (Identifier)val.value;
                env.AssignVar(iden.symbol,returnValue );
            }

            return returnValue;
        }

        /*
        *   Evaluates a boolean expression.
        *   lhs & rhs => any type
        *   op => operator it tries to evaluate with.
        */
        public static RuntimeVal Eval_Boolean_Expr(RuntimeVal lhs, RuntimeVal rhs, string op){
            RuntimeVal val = MK_NULL();

            if(lhs.type == rhs.type){
                switch(lhs.type){
                    case ValueType.NUMBER:
                    val = Eval_Numeric_Boolean_Expr((NumberVal) lhs, (NumberVal) rhs, op);
                    break;
                    case ValueType.OBJECT:
                    val = Eval_Object_Boolean_Expr((ObjectVal) lhs, (ObjectVal) rhs, op);
                    break;
                    case ValueType.BOOLEAN:
                    val = Eval_Boolean_Value_Expr((BooleanVal) lhs, (BooleanVal) rhs, op);
                    break;
                    default: 
                    val = Eval_String_Boolean_Expr(lhs, rhs, op);
                    break;
                }
            }else val = Eval_String_Boolean_Expr(lhs, rhs, op);

            return val;
        }

        /*
        *   Evaluates a boolean expression between two boolean values.
        *   lhs & rhs => C# bools
        *   op => operator it tries to evaluate with.
        */
        public static RuntimeVal Eval_Boolean_Value_Expr(BooleanVal lhs, BooleanVal rhs, string op){
            return op switch
            {
                "==" => MK_BOOL(lhs.value == rhs.value),
                "&&" => MK_BOOL(lhs.value && rhs.value),
                "||" => MK_BOOL(lhs.value || rhs.value),
                _ => throw new Exception($"Can't evaluate these values with operator {op}."),
            };

            
        }

        /*
        *   Evaluates a boolean expression between two user defined objects.
        *   lhs & rhs => ObjectVals
        *   op => operator it tries to evaluate with.
        *   Compares two objects be first comparing their properties, 
        *   then returning that value.
        */
        public static RuntimeVal Eval_Object_Boolean_Expr(ObjectVal lhs, ObjectVal rhs, string op){
            RuntimeVal returnVal = MK_BOOL(false);
            
            switch(op){
                case "==":
                //if they have a different amount of properties there is no point in checking further.
                if(lhs.properties.Count != rhs.properties.Count){
                    break;
                }
                //compare the properties of the objects, if they match then they are probably equal.
                returnVal = MK_BOOL(lhs.properties.Equals(rhs.properties));
                break;
                default: throw new Exception($"Can't evaluate these values with operator {op}.");
            }
            return returnVal;
        }
        
        /*
        *   Evaluates a boolean expression using string values.
        *   lhs & rhs => any type
        *   op => operator it tries to evaluate with.
        *   This function is used if when comparing values, the 
        *   values we are comparing either are different types or
        *   the values do not have boolean expression explicitly defined yet.
        */
        public static RuntimeVal Eval_String_Boolean_Expr(RuntimeVal lhs, RuntimeVal rhs, string op){
            string left = lhs.Stringify(), right = rhs.Stringify();
            return op switch
            {
                "==" => MK_BOOL(left.Equals(right)),
                _ => throw new Exception($"Can't evaluate these values with operator {op}."),
            };
        }
        
        /*
        *   Evaluates a boolean expression comparing two number values
        *   lhs & rhs => number values
        *   op => operator it tries to evaluate with.
        *   Evaluates them just like you can in C#.
        */
        public static RuntimeVal Eval_Numeric_Boolean_Expr(NumberVal lhs, NumberVal rhs, string op){
            return op switch
            {
                "==" => MK_BOOL(lhs.value == rhs.value),
                ">=" => MK_BOOL(lhs.value >= rhs.value),
                "<=" => MK_BOOL(lhs.value <= rhs.value),
                ">" => MK_BOOL(lhs.value > rhs.value),
                "<" => MK_BOOL(lhs.value < rhs.value),
                _ => throw new Exception($"Can't evaluate these values with operator {op}."),
            };
        }

        /*
        *   Evaluates Numeric Binary Expressions (1+2, 2*5, etc)
        *   lhs & rhs => number values
        *   op => operator it tries to evaluate with.
        */
        public static NumberVal Eval_Numeric_Binary_Expr(NumberVal lhs, NumberVal rhs, string op)
        {
            float result = 0f;
            if (op == "+")
            {
                result = lhs.value + rhs.value;
            }
            else if (op == "-")
            {
                result = lhs.value - rhs.value;
            }
            else if (op == "*")
            {
                result = lhs.value * rhs.value;
            }
            else if (op == "/")
            {
                // TODO: Division by zero checks
                result = lhs.value / rhs.value;
            }
            else
            {
                result = lhs.value % rhs.value;
            }
            return new NumberVal(result);
            //return { value: result, type: "number" };
        }


        /*
        *   A set of symbols commonly used in boolean expressions.
        *   needed to define it somewhere to be used in certain functions, 
        *   will move later.
        */
        private static readonly string[] booleanSymbols = { ">=", "<=", "==", "<", ">", "||", "&&" };
        
        /**
        * Evaulates expressions following the binary operation type.
        * Can evaluate boolean and binary expressions.
        */
        public static RuntimeVal Eval_Binary_Expr(BinaryExpr binop, Enviornment env)
        {
            if(binop.left == null || binop.right == null){
                throw new Exception("Binary operator is null;");
            }

            var lhs = Evaluate(binop.left, env);
            var rhs = Evaluate(binop.right, env);
            
            if(booleanSymbols.Contains<string>(binop.op)){
                return Eval_Boolean_Expr(
                    lhs,
                    rhs,
                    binop.op
                );
            }
            //Only support numeric math operations, will implement strings and other types(?) later
            if (lhs.type == ValueType.NUMBER && rhs.type == ValueType.NUMBER )
            {   
                    return Eval_Numeric_Binary_Expr(
                        (NumberVal) lhs,
                        (NumberVal)rhs,
                        binop.op
                    );
            }

            // One or both are NULL
            return MK_NULL();
        }

        /**
        *   Evaulates an Identifier.
        *   searches passed in enviornment for the passed in 
        *   identifier, then returns whatever value it holds.
        */
        public static RuntimeVal Eval_Identifier(Identifier ident, Enviornment env)
        {
            var val = env.lookupVar(ident.symbol);
            return val;
        }

        /**
        * Evaulates assignment expressions.
        * Takes the Assignment Expressions assigne and uses it to 
        * create a var in the passed in enviornment.
        */
        public static RuntimeVal Eval_Assignment(AssignmentExpr node, Enviornment env)
        {
            if (node.assigne == null || node.assigne.kind != NodeType.Identifier)
            {
                throw new Exception($"Invalid LHS inaide assignment expr {node.assigne}");//${JSON.stringify(node.assigne)});
            }

            string varname = (( Identifier) node.assigne).symbol;

            return env.AssignVar(varname, Evaluate(node.value ?? throw new Exception("Value is null."), env));
        }

        /*
        *   Evaluates user defined objects in the passed in environment.
        */
        public static RuntimeVal Eval_Object_Expr(ObjectLiteral obj, Enviornment env)
        {
            var tmp = new ObjectVal();
            foreach (Property prop in obj.properties)
            {
                RuntimeVal runtimeVal =
                    prop.value == null ? env.lookupVar(prop.key) : Evaluate(prop.value, env);

                tmp.properties.Add(prop.key, runtimeVal);
            }

            return tmp;
        }

        /*
        *   Evaluates member expressions within the passed in enviornment.
        */
        public static RuntimeVal Eval_Member_Expr(MemberExpr expr, Enviornment env){       
            RuntimeVal val = MK_NULL();
            
            if(expr.obj != null){
                if(expr.property == null){
                    throw new Exception("Identifier is null.");
                }

                Identifier property = (Identifier)expr.property;
                val = (ObjectVal) Evaluate(expr.obj, env);
                return ((ObjectVal)val).properties[property.symbol];
            }

            return val;
        }

        /*
        *   Evalues a passed in function call expression in the current enviornment.
        *   Evaluates native and user defined functions.
        */
        public static RuntimeVal Eval_Call_Expr(CallExpr expr, Enviornment env)
        {
            if(expr.caller == null){
                throw new Exception("Caller is null?");
            }

            int size = expr.args != null ? expr.args.Length : 0;
            RuntimeVal[] args = new RuntimeVal[size];
            for (int i = 0; i < size; i++)
            {
                args[i] = Evaluate((expr.args ?? throw new Exception("Args is null?"))[i], env);
            }

            //expr.args.map((arg) => Evaluate(arg, env));
            var fn = Evaluate(expr.caller, env);

            if (fn.type == ValueType.NATIVE_FN)
            {                
                var result = ((NativeFnValue)fn ?? throw new Exception("fn is not a NativeFnValue but has NATIVE_FN value type")).call(args, env);
                return result;
            }

            if (fn.type == ValueType.FUNCTION)
            {
                FunctionValue func;
                func = (FunctionValue)fn ?? throw new Exception("Function value is null?");
                Enviornment scope = new Enviornment(func.declarationEnv);

                // Create the variables for the parameters list
                for (int i = 0; i < func.parameters.Length; i++)
                {
                    // TODO Check the bounds here.
                    // verify arity of function
                    string varname = func.parameters[i];
                    scope.DeclareVar(varname, args[i], false);
                }

                RuntimeVal result = MK_NULL();
                // Evaluate the function body line by line
                foreach (var stmt in func.body)
                {
                    result = Evaluate(stmt, scope);
                }

                return result;
            }

            throw new Exception($"Cannot call value that is not a function: {fn}");// + JSON.stringify(fn));
        }

    }
}