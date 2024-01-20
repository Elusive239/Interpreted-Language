namespace ITLang.Runtime.NativeFunctions
{

    /*
    *   Base Function Collection for the environment variable.
    *   Gives a really easy way to add new natively implemented 
    *   functions in an organzied manor.
    */
    public abstract class BaseFunctionCollection : Dictionary<string, Func<RuntimeVal[], Enviornment, RuntimeVal>>{

    } 
}