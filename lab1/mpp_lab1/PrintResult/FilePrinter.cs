using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace mpp_lab1.PrintResult
{
    public class FilePrinter : IPrinter
    {
        public string Path { get; private set; }

        public FilePrinter(string path)
        {
            Path = path;
        }

        public void Print(string data)
        {
            try
            {
                StreamWriter sw = new StreamWriter(Path);
                sw.WriteLine(data);
                sw.Close();
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
