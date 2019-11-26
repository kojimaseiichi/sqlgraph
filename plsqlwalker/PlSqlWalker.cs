using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Antlr4.Runtime;


namespace sqlgraph
{
    public class PlSqlWalker
    {
        //_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
        // フィールド
        //_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
        private string _file_name;
        private sqlparser.PlSqlParser.Sql_scriptContext _ctx_script;

        //_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
        // プロパティ
        //_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
        public string FileName
        {
            get { return _file_name; }
        }


        public PlSqlWalker(string sql_file)
        {
            var s = new Antlr4.Runtime.AntlrInputStream(File.ReadAllText(sql_file).ToUpper());
            var l = new sqlparser.PlSqlLexer(s);
            var cm = new Antlr4.Runtime.CommonTokenStream(l);
            sqlparser.PlSqlParser p = new sqlparser.PlSqlParser(cm);
            _ctx_script = p.sql_script();

            _file_name = Path.GetFileName(sql_file);
        }

        public List<OutlineTreeElement> build_script_outline()
        {
            return null;
        }
    }
}
