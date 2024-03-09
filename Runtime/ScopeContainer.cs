

using ITLang.Frontend;
using static ITLang.Runtime.Values;
using ITLang.Util;
using ITLang.Runtime.NativeFunctions;

namespace ITLang.Runtime
{   
    /*
    *   Basically the scope of wherever you are in the source code.
    */
    public class ScopeContainer{
        public Enviornment? parent;
        public Dictionary<string, RuntimeVal> Variables {get{return variables;}}
        public List<string> Constants {get{return constants;}}

        private Dictionary<string, RuntimeVal> variables;
        private List<string> constants;

        /*
        *   Creates an environment with a parent environment (if one is passed in).
        */
        public ScopeContainer(){
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
            ScopeContainer env = this.Resolve(varName);

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

        public ScopeContainer Resolve(string varName) {
            if (this.Variables.ContainsKey(varName)) {
                return this;
            }

            if (this.parent == null) {
                throw new Exception($"Cannot resolve {varName} as it does not exist.");
            }

            return this.parent.Resolve(varName);
        }
    }

}