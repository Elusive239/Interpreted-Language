using ITLang.Frontend;

namespace ITLang.Runtime
{
	/*
	*	ValueType of a RuntimeVal. every RuntimeVal has one, helps prevent unsafe casting.
	*/
	public enum ValueType
	{
		NULL, NUMBER, BOOLEAN, OBJECT, NATIVE_FN, FUNCTION, STRING
	}

	/**
	* Base class that represents all values to be accessed/created at runtime by our source code.
	*/
	public class RuntimeVal
	{
		public ValueType type;
	}

	/**
	* Defines a value of undefined meaning
	*/
	public class NullVal : RuntimeVal
	{
		public string value = "null";
		public NullVal() {
			type = ValueType.NULL;
		}
	}

	/**
	* Runtime value that has access to the raw native C# bool.
	*/
	public class BooleanVal : RuntimeVal
	{
		public bool value;
		public BooleanVal(bool value = true)
		{
			type = ValueType.BOOLEAN;
			this.value = value;
		}
	}

	/**
	* Runtime value that has access to the raw native C# number.
	*/
	public class NumberVal : RuntimeVal
	{
		public float value;
		public NumberVal(float value = 0)
		{
			this.value = value;
			type = ValueType.NUMBER;
		}
	}

	/**
	* Runtime value that has access to the raw user defined object.
	*/
	public class ObjectVal : RuntimeVal
	{
		//public Dictionary<string, RuntimeVal> properties;
		public ITObject iTObject;
		public ObjectVal()
		{
			//properties = new Dictionary<string, RuntimeVal>();
			iTObject = new ITObject();
			type = ValueType.OBJECT;
		}

		public ObjectVal(ITObject other)
		{
			//properties = new Dictionary<string, RuntimeVal>();
			iTObject = other.Clone();
			type = ValueType.OBJECT;
		}
	}

	/*
	*	Runtime value that has access to a native function call.
	*	Should be created BEFORE parsing source code, not during parsing.
	*/
	public class NativeFnValue : RuntimeVal
	{
		public Func<RuntimeVal[], Enviornment, RuntimeVal> call;
		public NativeFnValue(Func<RuntimeVal[], Enviornment, RuntimeVal> call)
		{
			type = ValueType.NATIVE_FN;
			this.call = call;
		}
	}

	/*
	*	Runtime value that holds a function call defined in ITLang source code.
	*	These functions will be created when parsing source code, NOT BEFORE.
	*/
	public class FunctionValue : RuntimeVal
	{
		public string name;
		public string[] parameters;
		public Enviornment declarationEnv;
		public Stmt[] body;
		public FunctionValue(string name, string[] parameters, Enviornment declarationEnv, Stmt[] body)
		{
			type = ValueType.FUNCTION;
			this.name = name;
			this.parameters = parameters;
			this.declarationEnv = declarationEnv;
			this.body = body;
		}
	}

	/*
	*	Runtime value that has access to the raw C# string value.
	*/
	public class StringVal : RuntimeVal{
		public string value;
		public StringVal(string value = "")
		{
			this.value = value;
			type = ValueType.STRING;
		}
	}

	/*
	*	Static class used to make runtime values with MK_<Type>.
	*/
	public static class Values{

		/*
		*	Creates a runtime value that holds a null value / a string with the words "null".
		*/
		public static RuntimeVal MK_NULL() => new NullVal();

		/*
		*	Creates a runtime value that holds the value of the passed in bool.
		*/
		public static RuntimeVal MK_BOOL(bool b = true) => new BooleanVal(b);

		/*
		*	Creates a runtime value that holds the value of the passed in number.
		*	The value is also stored as a float, currently integers are unsupported.
		*/
		public static RuntimeVal MK_NUMBER(float n = 0) => new NumberVal(n);

		/*
		*	Creates a runtime value that holds a function call.
		*	The passed in Func has the following types:
		*	RuntimeVal[] => functions arguments in the ITLanguages' code.
		*	Environment => current scope within source code.
		*	RuntimeVal => return value of the source code.
		*/
		public static RuntimeVal MK_NATIVE_FN(Func<RuntimeVal[], Enviornment, RuntimeVal> call) => new NativeFnValue(call);

		/*
		*	Creates a runtime value that holds the value of the passed in string.
		*/
		public static RuntimeVal MK_STRING(string v) => new StringVal(v);
	}
}