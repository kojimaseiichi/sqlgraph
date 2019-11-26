using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;
using System.IO;

namespace sqlp
{
    class CrudExecutor
    {

        private SortedSet<string> _select = new SortedSet<string>();
        private SortedSet<string> _insert = new SortedSet<string>();
        private SortedSet<string> _update = new SortedSet<string>();
        private SortedSet<string> _delete = new SortedSet<string>();

        public void Execute(CrudSubCommand opt)
        {
            var files = opt.GetFiles();
            foreach (var file in files)
            {
                var script = SqlParser.script(file);
                var modName = Path.GetFileNameWithoutExtension(file);
                ExtractInsert(script);
                ExtractSelect(script);
                ExtractUpdate(script);
                ExtractDelete(script);
                OutputResult(opt.OutputDest, modName, "create", _insert);
                OutputResult(opt.OutputDest, modName, "refer", _select);
                OutputResult(opt.OutputDest, modName, "update", _update);
                OutputResult(opt.OutputDest, modName, "delete", _delete);
            }
        }

        protected void OutputResult(string filename, string modName, string header, ISet<string> set)
        {
            using (var w = new OutputWriter(filename))
            {
                foreach (var s in set) w.WriteLine("{0}\t{1}\t{2}", modName, header, s);
            }
        }

        private void ExtractSelect(IParseTree node)
        {
            _select = new SortedSet<string>();
            var selectStatements = from x in TreeSearch.dfs(node)
                    where x.Current is sqlparser.PlSqlParser.Select_statementContext
                    select x;
            foreach (var s in selectStatements)
            {
                bool inFromClause = false;
                var walk = TreeSearch.bfs(
                     s.Current,
                     x =>
                     {
                         if (x.Current is sqlparser.PlSqlParser.Select_statementContext)
                             return Pred.Stop;
                         if (inFromClause)
                         {
                             if (x.Current is sqlparser.PlSqlParser.Tableview_nameContext)
                                 return Pred.End;
                         }
                         else
                         {
                             if (x.Current is sqlparser.PlSqlParser.From_clauseContext)
                                 inFromClause = true;
                         }
                         return Pred.Skip;
                     }).FirstOrDefault();
                if (walk != null)
                    _select.Add(walk.Current.GetText());
            }
        }

        private void ExtractInsert(IParseTree node)
        {
            _insert = new SortedSet<string>();
            var insertStatements = from x in TreeSearch.dfs(node)
                                   where x.Current is sqlparser.PlSqlParser.Insert_statementContext
                                   select x;
            foreach (var s in insertStatements)
            {
                var walk = TreeSearch.bfs(
                    s.Current,
                    x => {
                        if (x.Current is sqlparser.PlSqlParser.Tableview_nameContext)
                            return Pred.End;
                        return Pred.Skip;
                    }).FirstOrDefault();
                if (walk != null)
                    _insert.Add(walk.Current.GetText());
            }
        }

        private void ExtractUpdate(IParseTree node)
        {
            _update = new SortedSet<string>();
            var updateStatements = from x in TreeSearch.dfs(node)
                                   where x.Current is sqlparser.PlSqlParser.Update_statementContext
                                   select x;
            foreach (var s in updateStatements)
            {
                var walk = TreeSearch.bfs(
                    s.Current,
                    x => {
                        if (x.Current is sqlparser.PlSqlParser.Tableview_nameContext)
                            return Pred.End;
                        return Pred.Skip;
                    }).FirstOrDefault();
                if (walk != null)
                    _update.Add(walk.Current.GetText());
            }
        }

        private void ExtractDelete(IParseTree node)
        {
            _delete = new SortedSet<string>();
            var deleteStatements = from x in TreeSearch.dfs(node)
                                   where x.Current is sqlparser.PlSqlParser.Delete_statementContext
                                   select x;
            foreach (var s in deleteStatements)
            {
                var walk = TreeSearch.bfs(
                    s.Current,
                    x => {
                        if (x.Current is sqlparser.PlSqlParser.Tableview_nameContext)
                            return Pred.End;
                        return Pred.Skip;
                    }).FirstOrDefault();
                if (walk != null)
                    _delete.Add(walk.Current.GetText());
            }
        }

    }
}
