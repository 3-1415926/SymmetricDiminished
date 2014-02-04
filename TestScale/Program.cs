using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestScale
{
    class Program
    {
        static void Main(string[] args)
        {
            Process.Start(@"d:\Tools\coverage.cmd", Assembly.GetExecutingAssembly().Location);
        }
    }
}
