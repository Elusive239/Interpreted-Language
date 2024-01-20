using ITLang.Util;
using static ITLang.Frontend.Lexer;
/**
 * Frontend for producing a valid AST from sourcode
 */
namespace ITLang.Frontend{
	public class Parser{
		private Util.TokenStack tokens;
		private string sourceCode = "";
		public Parser(string sourceCode){
			this.tokens = new TokenStack(Tokenize(sourceCode));
			this.sourceCode = sourceCode;
		}

		public void ReParse(string sourceCode){
			if(this.sourceCode.Equals(sourceCode)){
				this.tokens.Restart();
			}else{
				tokens = new TokenStack(Tokenize(sourceCode));
				this.sourceCode = sourceCode;
			}
		}

		public Program ProduceAST() {
			Program program = new Program();

			// Parse until end of file
			while (tokens.Not_eof()) {
                program.body.Add(this.Parse_Stmt());
            }

			return program;
		}

		// Handle complex statement types
		private Stmt Parse_Stmt() {
			// skip to Parse_Expr
			switch (tokens.At().type) {
				case TokenType.Let:
				case TokenType.Const:
					return this.Parse_Var_Declaration();
				case TokenType.Fn:
					return this.Parse_Fn_Declaration();
				case TokenType.If: //WRITE THIS NOW
					//throw new Exception("Not yet implemented!");
					return this.Parse_Branch();
				default:
					return this.Parse_Expr();
			}
		}

		private Stmt Parse_Branch(){
			Token tk = tokens.Eat();
			if(tk.type == TokenType.If){
				return Parse_If_Stmt();
			}else if(tk.type == TokenType.Elif){
				return Parse_ElIf_Stmt();
			}else if(tk.type == TokenType.Else){
				return Parse_Else_Stmt();
			}
			throw new Exception("Not yet implemented!");
		}

		private Stmt Parse_Else_Stmt(){

			IfStmt stmt = new IfStmt(new BooleanExpr(true));

			tokens.Expect(
				TokenType.OpenBrace,
				"Need open brace in if statement"
			);
			
			List<Stmt> body = new List<Stmt>();
			while (
				tokens.At().type != TokenType.EOF &&
				tokens.At().type != TokenType.CloseBrace
			) {
				body.Add(this.Parse_Stmt());
			}

			tokens.Expect(
				TokenType.CloseBrace,
				"Need close brace at end of statement"
			);

			stmt.body = body;

			return stmt;
		}

		private Stmt Parse_ElIf_Stmt(){
			IfStmt stmt = (IfStmt) this.Parse_If_Stmt();
			return stmt;
		}

		private Stmt Parse_If_Stmt(){
			tokens.Expect(
				TokenType.OpenParen,
				"Expected open parenthesis following if/elif keyword"
			);
			Expr expr = this.Parse_Expr();

			tokens.Expect(
				TokenType.CloseParen,
				"Need close paren in if statement"
			);

			tokens.Expect(
				TokenType.OpenBrace,
				"Need open brace in if statement"
			);
			
			List<Stmt> body = new List<Stmt>();
			while (
				tokens.At().type != TokenType.EOF &&
				tokens.At().type != TokenType.CloseBrace
			) {
				body.Add(this.Parse_Stmt());
			}

			tokens.Expect(
				TokenType.CloseBrace,
				"Need close brace at end of statement"
			);

			IfStmt? els = null;
			if(tokens.At().type == TokenType.Elif || tokens.At().type == TokenType.Else){
				els = (IfStmt) this.Parse_Branch();
			}

			return new IfStmt(expr){
				body = body,
                elseStmt = els
			};
		}

		private Stmt Parse_Fn_Declaration() {
			tokens.Eat(); // Eat fn keyword
			string name = tokens.Expect(
				TokenType.Identifier,
				"Expected function name following fn keyword"
			).value;

			Expr[] args = this.Parse_Args();
			List<string> parms = new List<string>();
			//string[] parms = new string[0];
			foreach (Expr arg in args) {
				if (arg.kind != NodeType.Identifier) {
					Console.WriteLine(arg);
					throw new Exception("Inside function declaration expected parameters to be of type string.");
				}

                parms.Add(( (Identifier)arg).symbol);
            }

			tokens.Expect(
				TokenType.OpenBrace,
				"Expected function body following declaration"
			);
			List<Stmt> body = new List<Stmt>();
			while (
				tokens.At().type != TokenType.EOF &&
				tokens.At().type != TokenType.CloseBrace
			) {
				body.Add(this.Parse_Stmt());
			}

			tokens.Expect(
				TokenType.CloseBrace,
				"Closing brace expected inside function declaration"
			);
			FunctionDeclaration fn = new FunctionDeclaration();
			fn.body = body.ToArray();
			fn.name = name;
			fn.parameters = parms.ToArray();

			return fn;
		}

		// LET IDENT;
		// ( LET | CONST ) IDENT = EXPR;
		private Stmt Parse_Var_Declaration() {
			bool isConstant = tokens.Eat().type == TokenType.Const;
			string identifier = tokens.Expect(
				TokenType.Identifier,
				"Expected identifier name following let | const keywords."
			).value;

			if (tokens.At().type == TokenType.Semicolon) {
				tokens.Eat(); // Expect semicolon
				if (isConstant) {
					throw new Exception("Must assigne value to constant expression. No value provided.");
				}

                VarDeclaration varDec = new VarDeclaration
                {
                    constant = false,
                    identifier = identifier
                };
                return varDec;
			}

			tokens.Expect(
				TokenType.Equals,
				"Expected equals token following identifier in var declaration."
			);

			Expr value = this.Parse_Expr();

			VarDeclaration declaration;
			if(tokens.At().type == TokenType.BooleanOperator){
				string op = tokens.Eat().value;
				Expr rhs = this.Parse_Expr();
				declaration = new VarDeclaration{
					value = new BinaryExpr{
						left = value,
						right = rhs,
						op = op
					},
					constant = isConstant,
					identifier = identifier
				};
			}else declaration = new VarDeclaration
            {
                value = value,
                constant = isConstant,
                identifier = identifier
            };

			tokens.Expect(
				TokenType.Semicolon,
				"Variable declaration statment must end with semicolon."
			);

			return declaration;
		}

		private Expr Parse_Expr() {
			return this.Parse_Assignment_Expr();
		}

		private Expr Parse_Assignment_Expr() {
			Expr left = this.Parse_Object_Expr();

			if (tokens.At().type == TokenType.Equals) {
				tokens.Eat(); // advance past equals
				Expr value = this.Parse_Assignment_Expr();
                AssignmentExpr expr = new AssignmentExpr
                {
                    value = value,
                    assigne = left
                };
                return expr;
			}

			return left;
		}

		private Expr Parse_Object_Expr() {
			// { Prop[] }
			if (tokens.At().type != TokenType.OpenBrace) {
				return this.Parse_Additive_Expr();
			}

			tokens.Eat(); // advance past open brace.
			List<Property> properties = new List<Property>();
			//Property[] properties = new Property[0];

			while (tokens.Not_eof() && tokens.At().type != TokenType.CloseBrace) {
				string key = tokens.Expect(
					TokenType.Identifier,
					"Object literal key expected"
				).value;

				// Allows shorthand key: pair -> { key, }
				if (tokens.At().type == TokenType.CloseBrace || tokens.At().type == TokenType.Comma) {
					if(tokens.At().type == TokenType.Comma){ //eat the comma because we need the close brace to break from the loop
						tokens.Eat();
					}
                    Property prop = new Property
                    {
                        key = key,
						value = null
                    };
                    properties.Add(prop);
					continue;
				}

				// { key: val }
				tokens.Expect(
					TokenType.Colon,
					"Missing colon following identifier in ObjectExpr"
				);
				Expr value = this.Parse_Expr();

                Property property = new Property
                {
                    key = key,
                    value = value
                };
                properties.Add(property);
				if (tokens.At().type != TokenType.CloseBrace) {
					tokens.Expect(
						TokenType.Comma,
						"Expected comma or closing bracket following property"
					);
				}
			}

			tokens.Expect(TokenType.CloseBrace, "Object literal missing closing brace.");
            return new ObjectLiteral
            {
                properties = properties.ToArray()
            };
		}

		private Expr Parse_Additive_Expr() {
			Expr left = this.Parse_Multiplicitave_Expr();

			while (tokens.At().value == "+" || tokens.At().value == "-") {
				string op = tokens.Eat().value;
				Expr right = this.Parse_Multiplicitave_Expr();
                BinaryExpr tmp = new BinaryExpr
                {
                    left = left,
                    right = right,
                    op = op
                };
                left = tmp;
			}

			while(tokens.At().type == TokenType.UnaryOperator){
				//is the left value a number?
				string op = tokens.Eat().value;				
				left = new UnaryExpr{
					value = left,
					op = op
				};

			}

			return left;
		}

		private Expr Parse_Multiplicitave_Expr() { //ADD PARSE BOOLEAN EXPRESSION IN BETWEEN THIS METHOD AND CALL.
			Expr left = this.Parse_Boolean_Expr();

			while (
				tokens.At().value == "/" ||
				tokens.At().value == "*" ||
				tokens.At().value == "%"
			) {
				string op = tokens.Eat().value;
				Expr right = this.Parse_Call_Member_Expr();
                BinaryExpr tmp = new BinaryExpr
                {
                    left = left,
                    right = right,
                    op = op
                };
                left = tmp;
			}

			return left;
		}

		private Expr Parse_Boolean_Expr() { //ADD PARSE BOOLEAN EXPRESSION IN BETWEEN THIS METHOD AND CALL.
			Expr left = this.Parse_Call_Member_Expr();

			while (
				tokens.At().value == "==" ||
				tokens.At().value == ">=" ||
				tokens.At().value == "<=" ||
				tokens.At().value == ">" ||
				tokens.At().value == "<" ||
				tokens.At().value == "&&" ||
				tokens.At().value == "||"
			) {
				string op = tokens.Eat().value;
				Expr right = this.Parse_Call_Member_Expr();
                BinaryExpr tmp = new BinaryExpr
                {
                    left = left,
                    right = right,
                    op = op
                };
                left = tmp;
			}

			return left;
		}

		private Expr Parse_Call_Member_Expr() {
			Expr member = this.Parse_Member_Expr();

			if (tokens.At().type == TokenType.OpenParen) {
				return this.Parse_Call_Expr(member);
			}

			return member;
		}

		private Expr Parse_Call_Expr(Expr caller) {
            CallExpr? call_expr = new CallExpr
            {
                caller = caller,
                args = this.Parse_Args()
            };


            if (tokens.At().type == TokenType.OpenParen) {
				call_expr = (CallExpr) this.Parse_Call_Expr(call_expr) ;
			}

			if(call_expr == null){
				throw new Exception("Call Exception is null. How did this happen?");
			}

			return call_expr;
		}

		private Expr[] Parse_Args() {
			tokens.Expect(TokenType.OpenParen, "Expected open parenthesis");
			Expr[] args =
				tokens.At().type == TokenType.CloseParen ? Array.Empty<Expr>() : this.Parse_Arguments_List();

			tokens.Expect(
				TokenType.CloseParen,
				$"Missing closing parenthesis inside arguments list, received {tokens.At().Stringify()}"
			);
			return args;
		}

		private Expr[] Parse_Arguments_List() {
			List<Expr> args = new List<Expr>
            {
                this.Parse_Assignment_Expr()
            };

			while (tokens.At().type == TokenType.Comma && tokens.Eat() != null) {
				args.Add(this.Parse_Assignment_Expr());
			}

			return args.ToArray();
		}

		private Expr Parse_Member_Expr() {
			Expr obj = this.Parse_Primary_Expr();

			while (
				tokens.At().type == TokenType.Dot ||
				tokens.At().type == TokenType.OpenBracket
			) {
				Token op = tokens.Eat();
				Expr property;
				bool computed;

				// non-computed values aka obj.expr
				if (op.type == TokenType.Dot) {
					computed = false;
					// get identifier
					property = this.Parse_Primary_Expr();
					if(property.kind == NodeType.NumericLiteral && obj.kind == NodeType.NumericLiteral){
						float valObj = ((NumericLiteral)obj).value;
						float valProp = ((NumericLiteral)property).value;
						if($"{valObj} ${valProp}".Contains('.')){
							throw new Exception("NumberLiteral has two decimal places.");
						}

						float num = float.Parse($"{valObj}.{valProp}");
						NumericLiteral lit = new NumericLiteral(num);
						return lit;
					}
					else if (property.kind != NodeType.Identifier) {
						throw new Exception ("Can not use dot op without right hand side being a identifier");
					}
				} else {
					// this allows obj[computedValue]
					computed = true;
					property = this.Parse_Expr();
					tokens.Expect(
						TokenType.CloseBracket,
						"Missing closing bracket in computed value."
					);
				}

                MemberExpr tmp = new MemberExpr
                {
                    obj = obj,
                    property = property,
                    computed = computed
                };

                return tmp;
			}

			return obj;
		}

		private Expr Parse_Unary_Operator_Expr(){
			string op = tokens.At().value;

			return new UnaryExpr{
				value = null,
				op = op
			};
		}

		// Parse Literal Values & Grouping Expressions
		private Expr Parse_Primary_Expr() {
			TokenType tk = tokens.At().type;

			// Determine which token we are currently At and return literal value
			switch (tk) {
				// User defined values.
				case TokenType.Identifier:
                    Identifier identifier = new Identifier
                    {
                        symbol = tokens.Eat().value
                    };
                    return identifier;

				// Constants and Numeric Constants
				case TokenType.Number:
                    NumericLiteral numericLiteral = new NumericLiteral
                    {
                        value = float.Parse(tokens.Eat().value)
                    };
                    return numericLiteral;

				// Grouping Expressions
				case TokenType.OpenParen: 
					tokens.Eat(); // Eat the opening paren
					Expr value = this.Parse_Expr();
					tokens.Expect(
						TokenType.CloseParen,
						"Unexpected token found inside parenthesised expression. Expected closing parenthesis."
					); // closing paren
					return value;
				
				case TokenType.Apostrophe:
				case TokenType.Quote:
					bool isQuote = tk == TokenType.Quote;
					tokens.Eat();
					StringLiteral str = new StringLiteral(tokens.Eat().value);
					string expected = isQuote ? "\"" : "\'";
					tokens.Expect(
						isQuote ? TokenType.Quote : TokenType.Apostrophe,
						$"Unexpected token found inside string expression. Expected ({expected})"
					);
					return str;
				
				case TokenType.UnaryOperator:
					return this.Parse_Unary_Operator_Expr();

				// Unidentified Tokens and Invalid Code Reached
				default:
					throw new Exception( $"Unexpected token found during parsing at token {tokens.CurrentToken}! {tokens.At().Stringify()}" );
			}
		}
    }
}
