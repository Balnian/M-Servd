using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M_Servd
{
    class Log
    {
        /// <summary>
        /// Log a message
        /// </summary>
        /// <param name="message">Message that will be log</param>
        public static void Message(String message)
        {
            ConsoleColor bkp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ForegroundColor = bkp;
        }

        /// <summary>
        /// Log a success message
        /// </summary>
        /// <param name="message">Message that will be log</param>
        public static void Success(String message)
        {
            ConsoleColor bkp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ForegroundColor = bkp;
        }

        /// <summary>
        /// Log a warning message
        /// </summary>
        /// <param name="message">Message that will be log</param>
        public static void Warning(String message)
        {
            ConsoleColor bkp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = bkp;
        }

        /// <summary>
        /// Log an error message
        /// </summary>
        /// <param name="message">Message that will be log</param>
        public static void Error(String message)
        {
            ConsoleColor bkp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = bkp;
        }

        /// <summary>
        /// Log a debug message
        /// </summary>
        /// <param name="str">Message that will be log</param>
        public static void Debug(String message)
        {
            ConsoleColor bkp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(message);
            Console.ForegroundColor = bkp;
        }
    }

}
