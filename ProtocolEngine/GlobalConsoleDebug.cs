using System;

namespace ProtocolEngine
{
    public class GlobalConsoleDebug
    {
        static object lockO = new object();
        static ConsoleColor expectedErrorColor = ConsoleColor.Magenta;
        static ConsoleColor internalErrorColor = ConsoleColor.Red;
        static ConsoleColor errorHeaderColor = ConsoleColor.DarkMagenta;

        public static void ShowErrorHeader(string className)
        {
            lock (lockO)
            {
                Console.ForegroundColor = errorHeaderColor;
                Console.WriteLine("++++++++++++ HANDLED IN +++++++++++++");
                Console.WriteLine(className);
                Console.ResetColor();
            }
        }

        public static void ShowInternalError(Exception e)
        {
            lock (lockO)
            {
                Console.ForegroundColor = internalErrorColor;
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("INTERNAL:\t" + e.GetType().Name);
                Console.WriteLine("MESSAGE :\t" + e.Message);
                Console.WriteLine("SHORT STACK TRACE  :\r\n");
                string[] stackTrace = e.StackTrace.Split(new string[] { "\n" }, StringSplitOptions.None);
                int i = 0;
                while (i < stackTrace.Length && i < 5)
                {
                    Console.WriteLine(stackTrace[i].Split(new string[] {" in "}, StringSplitOptions.None)[0]);
                    i++;
                }

                Console.WriteLine("-----------------------------------------------");
                Console.ResetColor();
            }
        }

        public static void ShowExpectedError(Exception e)
        {
            lock (lockO)
            {
                Console.ForegroundColor = expectedErrorColor;
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine("Expected:\t" + e.GetType().Name);
                Console.WriteLine("Message :\t" + e.Message);
                Console.WriteLine("-----------------------------------------------");
                Console.ResetColor();
            }
        }

    }
}
