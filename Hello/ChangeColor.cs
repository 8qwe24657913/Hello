using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello {
    static class ChangeColor {
        public struct Back : IDisposable {
            public readonly ConsoleColor origColor;
            public Back(ConsoleColor color) {
                origColor = Console.BackgroundColor;
                Console.BackgroundColor = color;
            }
            public void Dispose() {
                Console.BackgroundColor = origColor;
            }
        }
        public struct Fore : IDisposable {
            public readonly ConsoleColor origColor;
            public Fore(ConsoleColor color) {
                origColor = Console.ForegroundColor;
                Console.ForegroundColor = color;
            }
            public void Dispose() {
                Console.ForegroundColor = origColor;
            }
        }
    }
}

