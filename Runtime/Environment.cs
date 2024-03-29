using ITLang.Frontend;
using static ITLang.Runtime.Values;
using ITLang.Util;
using ITLang.Runtime.NativeFunctions;

namespace ITLang.Runtime
{   
    /*
    *   Basically the scope of wherever you are in the source code.
    */
    public class Enviornment{
        private Enviornment? parent;
        private Dictionary<string, RuntimeVal> variables;
        private List<string> constants;

        /*
        *   We never use this, but may in the future.
        */
        private bool global;

        /*
        *   Creates an environment with a parent environment (if one is passed in).
        */
        public Enviornment(Enviornment? parent = null){
            global = parent == null ? true : false;
            this.parent = parent;
            this.variables = new Dictionary<string, RuntimeVal>();
            this.constants = new List<string>();
        }

        /*
        *   Declares a variable if it doesnt exist in the current 
        *   environment or any of its parents.
        */
        public RuntimeVal DeclareVar(string varName, RuntimeVal value, bool constant = true){
            if (this.variables.ContainsKey(varName)) {
                throw new Exception ($"Cannot declare variable {varName}. As it already is defined.");
            }
            this.variables.Add(varName, value);
            if (constant) {
                this.constants.Add(varName);
            }
            return value;
        }

        /*
        *   Assigns the passed in value to the variable 
        *   with the passed in varName (if it exits)
        */
        public RuntimeVal AssignVar(string varName, RuntimeVal value) {
            Enviornment env = this.Resolve(varName);

            // Cannot assign to constant
            if (env.constants.Contains(varName)) {
                throw new Exception("Cannot reasign to variable ${varName} as it was declared constant.");
            } else if(env.variables.ContainsKey(varName)){
                env.variables[varName] = value;
            }
            else env.variables.Add(varName, value);
            return value;
        }

        /*
        *   Uses resolve to get the environment of a runtime value, 
        *   then returns that value if it exists..
        */
        public RuntimeVal lookupVar(string varName) {
            var env = this.Resolve(varName);
            return env.variables[varName] ;
        }

        /*
        *   Returns the environment that contains a runtime value of the passed in strings name.
        */
        public Enviornment Resolve(string varName) {
            if (this.variables.ContainsKey(varName)) {
                return this;
            }

            if (this.parent == null) {
                throw new Exception($"Cannot resolve {varName} as it does not exist.");
            }

            return this.parent.Resolve(varName);
        }

        /*
        *   Adds a function collection to the current environment, usually the global environment.
        */
        public void AddFunctionCollection(BaseFunctionCollection collection){
            foreach (var item in collection)
            {
                DeclareVar(item.Key, MK_NATIVE_FN(item.Value));
            }
        }

        /*
        *   Creates the first, global environment.
        *   Initializes some varuables that will be used in 
        *   all other environments.
        */
        public static Enviornment CreateGlobalEnv(){
            var env = new Enviornment();
            // Create Default Global Enviornment
            env.DeclareVar("true", MK_BOOL(true), true);
            env.DeclareVar("false", MK_BOOL(false), true);
            env.DeclareVar("null", MK_NULL(), true);

            env.AddFunctionCollection(new ITLangConsole());
            
            
            return env;
        }
    }

}