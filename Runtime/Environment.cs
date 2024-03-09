using ITLang.Frontend;
using static ITLang.Runtime.Values;
using ITLang.Util;
using ITLang.Runtime.NativeFunctions;

namespace ITLang.Runtime
{   
    /*
    *   Basically the scope of wherever you are in the source code.
    */
    public class Enviornment : ScopeContainer{
        /*
        *   We never use this, but may in the future.
        */
        private bool global;

        /*
        *   Creates an environment with a parent environment (if one is passed in).
        */
        public Enviornment(Enviornment? parent = null) : base(){
            global = parent == null ? true : false;
            this.parent = parent;
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