#define DEBUG

using ITLang.Frontend;
using ITLang.Runtime;
using ITLang.Util;
using static ITLang.Runtime.Interpreter;
namespace ITLang{
    
    public class MainPrg{
        public static void Main(string[] args){
            #if DEBUG
            double avg = 0;
            
            const int tests = 20;
            List<double> vals = new();
            for(int i = 0; i < tests; i++){
                double val = TimeTaken();
                avg += val;
                vals.Add(val);
            }
            for(int index = 0; index < vals.Count; index++)
                Console.WriteLine($"Time taken for run {index}: {vals[index]:0.00} seconds.");
            Console.WriteLine($"Average time over {RUNS} runs: {(avg/tests).ToString("0.00")} seconds.");
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

        #if DEBUG
        public const int RUNS = 10;
        public static double TimeTaken(){
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            for(int i = 0; i < RUNS; i++)
                RunTests();

            stopwatch.Stop();
            return stopwatch.Elapsed.TotalSeconds;
        }
        #endif

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