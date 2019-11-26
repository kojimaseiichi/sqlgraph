using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommandLine;

namespace sqlp
{
    class Program
    {
        static void Main(string[] args)
        {
            ParseArguments(args);
        }

        static void ParseArguments(string[] args)
        {
            Parser.Default.ParseArguments<FuncallSubCommand, CrudSubCommand, TreeSubCommand>(args)
                .WithParsed<FuncallSubCommand>(opt => { (new FuncalExecutor()).Execute(opt); })
                .WithParsed<CrudSubCommand>(opt => { (new CrudExecutor()).Execute(opt); })
                .WithParsed<TreeSubCommand>(opt => { (new TreeExecutor()).Execute(opt); })
                .WithNotParsed(err => { Console.WriteLine(err); });
            Console.ReadKey();
        }

        static void test_extract_funcall()
        {
            const string fileName = @"C:\dev\sql\example.sql";
            var script = SqlParser.script(fileName);

            var q = from x in TreeSearch.dfs(script)
                    where
                        x.Parent is sqlparser.PlSqlParser.Function_callContext &&
                        x.Current is sqlparser.PlSqlParser.Routine_nameContext
                    select x;

            foreach (var node in TreeSearch.dfs(script))
            {
                for (int n = 0; n < node.Depth; n++)
                    Console.Write(" ");
                Console.WriteLine("{0}", node.Current.GetType().Name);
            }
            Console.WriteLine("************************");
            foreach (var node in q)
            {
                foreach (var n2 in TreeSearch.bfs(node.Current, 1))
                    Console.WriteLine("{0}", n2.Current.GetText());
                Console.WriteLine("************************");
            }
            Console.ReadKey();
        }
    }
}
