using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Reimplement Unity debug log functions to work in standalone
namespace UnityEngine
{
    class Debug
    {
        public static void Log(string message)
        {
            Console.WriteLine(message);
        }
        public static void LogWarning(string message)
        {
            Console.WriteLine(message);
        }
        public static void LogError(string message)
        {
            Console.WriteLine(message);
        }
    }
}
