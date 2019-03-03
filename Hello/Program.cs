using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello {
    class Program {
        static void Main(string[] args) {
            if (args.Length >= 2) {
                System.Console.WriteLine("Hello World!{0} is {1}", args[0], args[1]);
            } else {
                System.Console.WriteLine("Hello World! command line parameter does not meet the requirement!");
            }
            using (new ChangeColor.Back(ConsoleColor.Green)) {
                Console.WriteLine("Hello World!");
                Console.WriteLine("Hello World!");
                using (new ChangeColor.Fore(ConsoleColor.Yellow)) {
                    Console.WriteLine("Hello World!");
                }
                Console.WriteLine("If you use ResetColor(), you will have to reassign background color here.");
            }
        }
    }
}
