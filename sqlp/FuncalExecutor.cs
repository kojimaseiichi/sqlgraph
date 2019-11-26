using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace sqlp
{
    class FuncalExecutor
    {


        protected void OutputResult(string filename, string modName, ISet<string> set)
        {
            using (var w = new OutputWriter(filename))
            {
                foreach (var s in set)
                {
                    var sp = s.Split('.');
                    w.WriteLine("{0}\t{2}\t{1}", modName, sp[0], sp.Length > 1 ? sp[1] : "");
                }
            }
        }

        public void Execute(FuncallSubCommand opt)
        {
            var files = opt.GetFiles();
            foreach (var file in files)
            {
                SortedSet<string> set = ExtractFunctionCaller(file);
                string modName = Path.GetFileNameWithoutExtension(file);
                OutputResult(opt.OutputDest, modName, set);
            }
        }

        private static SortedSet<string> ExtractFunctionCaller(string fileName)
        {
            SortedSet<string> set = new SortedSet<string>();
            var script = SqlParser.script(fileName);

            var q = from x in TreeSearch.dfs(script)
                    where
                        x.Parent is sqlparser.PlSqlParser.Function_callContext &&
                        x.Current is sqlparser.PlSqlParser.Routine_nameContext
                    select
                        string.Join("", TreeSearch.bfs(x.Current, 1).Select(y => y.Current.GetText()));

            foreach (var s in q) set.Add(s);
            return set;
        }
    }
}
