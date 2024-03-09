//#define DEBUG

using ITLang.Frontend;
using ITLang.Runtime;
using ITLang.Util;
using static ITLang.Runtime.Interpreter;
namespace ITLang{
    
    public class MainPrg{
        public static void Main(string[] args){
            // DEBUG
            FileStream fileStream = new FileStream("./timed.txt", FileMode.Truncate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileStream);
            Console.SetOut(writer);

            //SpeedTest();

            if(args.Length == 0){
                RunTests();
            }else{
                string fpath = Directory.GetCurrentDirectory() + "\\Tests\\"+ args[0];
                string input = File.ReadAllText(fpath);
                Console.WriteLine(Run(input));
            }

            Console.ReadLine();

            writer.Close();
            fileStream.Close();
        }

        public static void SpeedTest(){
            double avg = 0;
            
            const int RUNS = 2000;
            List<double> vals = new();
            for(int i = 0; i < RUNS; i++){
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();

                RunTests();

                stopwatch.Stop();
                double val = stopwatch.Elapsed.TotalSeconds;
                avg += val;
                vals.Add(val);
            }
            for(int index = 0; index < vals.Count; index++)
                Console.WriteLine($"Time taken for run {(1+index)*RUNS}: {vals[index]:0.00} seconds.");
            Console.WriteLine($"Average time over {RUNS} runs: {(avg/RUNS).ToString("0.00")} seconds.");
            
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
            // catch{
            //     Console.WriteLine("Exit Code: -1");
            //     return -1;
            // }
        }
    }
}