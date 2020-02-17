using System;

namespace CapsLockSharpPrototype.Helper
{
    public static class Logger
    {
        public static void Info(string text)
        {
            Console.WriteLine(DateTime.Now.ToString("T")+" > " + text);
        }
    }
}
