using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sqlp
{
    class TreeExecutor
    {
        public void Execute(TreeSubCommand opt)
        {
            var script = SqlParser.script(opt.FileName);
            using (var w = new OutputWriter(opt.OutputDest))
            {
                foreach (var x in TreeSearch.dfs(script))
                {
                    for (int n = 0; n < x.Depth; n++)
                        w.Write(" ");
                    w.WriteLine(x.Current.GetType().Name);
                }
            }
        }
    }
}
