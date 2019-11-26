using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace sqlp
{
    static class SqlParser
    {
        public static sqlparser.PlSqlParser.Sql_scriptContext script(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            var stream = new Antlr4.Runtime.AntlrInputStream(File.ReadAllText(fileName).ToUpper());
            var lexer = new sqlparser.PlSqlLexer(stream);
            var commonTokenStream = new Antlr4.Runtime.CommonTokenStream(lexer);
            sqlparser.PlSqlParser parser = new sqlparser.PlSqlParser(commonTokenStream);
            return parser.sql_script();
        }
    }
}
