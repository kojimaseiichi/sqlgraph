using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace sqlparser
{
    public class PlsScriptGraph : Graph
    {
        protected PlSqlParser.Sql_scriptContext _sqlScriptContext;

        public PlsScriptGraph(GraphContext context, string fileName) :base(context)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            var stream = new Antlr4.Runtime.AntlrInputStream(File.ReadAllText(fileName).ToUpper());
            var lexer = new sqlparser.PlSqlLexer(stream);
            var commonTokenStream = new Antlr4.Runtime.CommonTokenStream(lexer);
            sqlparser.PlSqlParser parser = new sqlparser.PlSqlParser(commonTokenStream);
            _sqlScriptContext = parser.sql_script();

            PlsVertex script = new PlsVertex(context._NextSeq, fileInfo.Name, ScriptVertexType.Script);
            _vertexSet.Add(script.ID, script);

            
            foreach (var v in TreeSearch.dfs(_sqlScriptContext))
            {
                for (int n = 0; n < v.depth; n++)
                    Console.Write(" ");
                Console.WriteLine(string.Format("{0}: {1}", v.current.GetType().Name, v.current.GetText()));
            }
                
        }
    }
}
