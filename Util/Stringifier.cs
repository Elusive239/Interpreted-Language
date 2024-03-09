using ITLang.Frontend;

using ITLang.Runtime;

namespace ITLang.Util{

	/*
	*	Wrote as an easy way to change how I printed different values to the console/elsewhere.
	*	ToString was taken by some classes / this just made it easier. all in one place.
	*/
    public static class Stringifier{

		/*
		*	Returns a token types string form.
		*/
        public static string Stringify(this Token token)
        {
            string tmp = "";
			switch(token.type){
				case TokenType.Number: tmp = "Number: "; break;
				case TokenType.Identifier: tmp = "Identifier: "; break;
				case TokenType.Let: tmp = "Let: "; break;
				case TokenType.Const: tmp = "Const: "; break;
				case TokenType.Fn: tmp = "Fn: "; break;
				case TokenType.BinaryOperator: tmp = "BinaryOperator: "; break;
				case TokenType.Equals: tmp = "Equals: "; break;
				case TokenType.Comma: tmp = "Comma: "; break;
				case TokenType.Dot: tmp = "Dot: "; break;
				case TokenType.Colon: tmp = "Colon: "; break;
				case TokenType.Semicolon: tmp = "Semicolon: "; break;
				case TokenType.OpenParen: tmp = "OpenParen: "; break;
				case TokenType.CloseParen: tmp = "CloseParen: "; break;
				case TokenType.OpenBrace: tmp = "OpenBrace: "; break;
				case TokenType.CloseBrace: tmp = "CloseBrace: "; break;
				case TokenType.OpenBracket: tmp = "OpenBracket: "; break;
				case TokenType.CloseBracket: tmp = "CloseBracket: "; break;
				case TokenType.EOF: tmp = "EOF: "; break;
				case TokenType.If: tmp = "If: "; break;
                case TokenType.Elif: tmp = "ElIf: "; break;
                case TokenType.Else: tmp = "Else: "; break;
				case TokenType.BooleanOperator: tmp = "BooleanOperator: "; break;
				case TokenType.UnaryOperator: tmp = "UnaryOperator: "; break;
                case TokenType.Hashtag: tmp ="#: "; break;
			}
			return tmp + token.value;
        }

		/*
		*	Returns the string version of a Runtime value.
		*/
        public static string Stringify(this RuntimeVal val){
			string tmp = "";
			switch(val.type){
				case Runtime.ValueType.NULL: tmp = "null"; break;
				case Runtime.ValueType.NUMBER: tmp = ((NumberVal) val).value + ""; break;
				case Runtime.ValueType.BOOLEAN: tmp = ((BooleanVal) val).value == true ? "true" : "false"; break;
				case Runtime.ValueType.OBJECT: 
				ObjectVal v = (ObjectVal) val;
					tmp += "object {";
					foreach (var prop in v.iTObject.Variables){
						tmp += $"\n\t{prop.Key}: {Stringify(prop.Value)}";
					}
					tmp += "\n}";
					break;
				case Runtime.ValueType.NATIVE_FN: tmp = "Native Function"; break;
				case Runtime.ValueType.FUNCTION: tmp = "Function"; break;
				case Runtime.ValueType.STRING: tmp = ((StringVal) val).value; break;				
				default: tmp = val + ""; break;
			}
			return tmp;
		}

		/*
		*	Returns the string version of a Runtimes ValueType
		*/
		public static string Stringify(this Runtime.ValueType type){
			string tmp = "ValueType.";
			switch(type){
				case Runtime.ValueType.NULL: tmp += "NULL"; break;
				case Runtime.ValueType.NUMBER: tmp += "NUMBER"; break;
				case Runtime.ValueType.BOOLEAN: tmp += "BOOLEAN"; break;
				case Runtime.ValueType.OBJECT: tmp += "OBJECT"; break;
				case Runtime.ValueType.NATIVE_FN: tmp += "NATIVE_FN"; break;
				case Runtime.ValueType.FUNCTION: tmp += "FUNCTION"; break;
				case Runtime.ValueType.STRING: tmp += "STRING"; break;				
				default: throw new Exception("Not yet implemented.");
			}
			return tmp;
		}

		/*
		*	Returns the string version of a node type.
		*/
        public static string Stringify(this NodeType kind)
        {
            switch(kind){
                case NodeType.AssignmentExpr: return  "AssignmentExpr";
                case NodeType.BinaryExpr: return  "BinaryExpr";
                case NodeType.CallExpr: return  "CallExpr";
                case NodeType.FunctionDeclaration: return  "FunctionDeclaration";
                case NodeType.Identifier: return  "Identifier";
                case NodeType.MemberExpr: return  "MemberExpr";
                case NodeType.NumericLiteral: return  "NumericLiteral";
                case NodeType.ObjectLiteral: return  "ObjectLiteral";
                case NodeType.Program: return  "Program";
                case NodeType.Property: return  "Property";
                case NodeType.StringLiteral: return  "StringLiteral";
                case NodeType.VarDeclaration: return  "VarDeclaration";
                case NodeType.BooleanExpr: return "BooleanExpr";
                default: return "Not Implemented";
            }
        }
    }
}