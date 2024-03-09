
using ITLang.Frontend;
using static ITLang.Runtime.Values;
using ITLang.Util;
using ITLang.Runtime.NativeFunctions;

namespace ITLang.Runtime
{  
    public class ITObject : ScopeContainer{
        
        //ideally when we create a class in code, we can define it as one of these 
        //then just call clone when we create a new one, then use constructors to 
        //assign variables (initially)
        //functions created inside objects should take that object as the first parameter, 
        //probably defined as this?
        public ITObject Clone(){
            ITObject iT = new();

            List<KeyValuePair<string, RuntimeVal>> pairs = Variables.ToList();
            foreach(KeyValuePair<string, RuntimeVal> pair in pairs){
                iT.AssignVar(pair.Key, pair.Value);
            }
            return iT;
        }
    }
}