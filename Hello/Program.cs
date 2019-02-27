using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World!");
            using (new ChangeColor.Back(ConsoleColor.Green)) {
                Console.WriteLine("Hello World!");
                Console.WriteLine("Hello World!");
                using (new ChangeColor.Fore(ConsoleColor.Yellow)) {
                    Console.WriteLine("Hello World!");
                }
            }
        }
    }
}
