using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using sqlparser;

namespace sqlparser_test
{
    class Program
    {
        static void Main(string[] args)
        {
            const string fileName = @"C:\dev\example.sql";

            GraphContext context = new GraphContext();
            PlsScriptGraph scriptGraph = new PlsScriptGraph(context, fileName);
        }
    }
}
