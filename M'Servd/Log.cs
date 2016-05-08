using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_Servd
{
    class Log
    {
        public static void Message(String str)
        {
            ConsoleColor bkp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(str);
            Console.ForegroundColor = bkp;
        }
        public static void Success(String str)
        {
            ConsoleColor bkp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(str);
            Console.ForegroundColor = bkp;
        }
        public static void Warning(String str)
        {
            ConsoleColor bkp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(str);
            Console.ForegroundColor = bkp;
        }
        public static void Error(String str)
        {
            ConsoleColor bkp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
            Console.ForegroundColor = bkp;
        }
        public static void Debug(String str)
        {
            ConsoleColor bkp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(str);
            Console.ForegroundColor = bkp;
        }
    }

}
