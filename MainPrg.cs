#define DEBUG

using ITLang.Frontend;
using ITLang.Runtime;
using ITLang.Util;
using static ITLang.Runtime.Interpreter;
namespace ITLang{
    
    public class MainPrg{
        public static void Main(string[] args){
            #if DEBUG
            TimeTaken();
            #elif DEBUG
            if(args.Length == 0){
                RunTests();
            }else{
                string fpath = Directory.GetCurrentDirectory() + "\\Tests\\"+ args[0];
                string input = File.ReadAllText(fpath);
                Console.WriteLine(Run(input));
            }
            #endif

            Console.ReadLine();
        }

        public const int RUNS = 10000000;
        public static void TimeTaken(){
            float startTime = DateTime.Now.Nanosecond;

            for(int i = 0; i < RUNS; i++)
                RunTests();

            float endTime = DateTime.Now.Nanosecond;
            Console.WriteLine($"Time taken over {RUNS} runs: {endTime - startTime}");
        }

        public static void RunTests(){
            string[] filePaths = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\Tests", "*.itl");
            foreach(string path in filePaths){
                if(!File.Exists(path)){
                    Console.WriteLine($"File at {path} does not exist?");
                    continue;
                }

                Console.WriteLine("File: " + path);
                string input = File.ReadAllText(path);
                Run(input);
                Console.WriteLine();
            }
        }

        public static int Run(string input) {
            if(input.Length == 0) {
                Console.WriteLine();
                return 0;
            }

            try{
                Parser parser = new Parser(input);
                Enviornment env = Enviornment.CreateGlobalEnv();
                
                Stmt program = parser.ProduceAST();

                RuntimeVal result = Evaluate(program, env);
                Console.WriteLine("Exit Code: 0");
                return 0;
            }catch(ExitException e){
                Console.WriteLine(e.Message);
                return e.exitCode;
            }
        }
    }
}