using System;
using System.Collections.Generic;
using System.Text;

namespace mpp_lab1.PrintResult
{
    public class ConsolePrinter : IPrinter
    {
        public void Print(string data)
        {
            Console.WriteLine(data);
        }
    }
}
