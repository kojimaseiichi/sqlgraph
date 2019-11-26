using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace sqlp
{
    [Verb("crud", HelpText = "CRUD情報を抽出します")]
    class CrudSubCommand
    {
        private const string helpFile =
@"解析するSQLファイルのパスを指定します。
dオプションを指定する場合は、ファイル名を指定します。";

        private const string helpOutput =
@"出力結果を任意で指定します。";

        private const string helpDictionary =
@"解析するSQLファイルのディレクトリを指定します。
fオプションにファイル名を指定します。
ワイルドカード使用可能。";

        [Option('f', "file", HelpText = helpFile)]
        public string FileName { get; set; }

        [Option('o', "output", HelpText = helpOutput)]
        public string OutputDest { get; set; }

        [Option('d', "directory", HelpText = "解析するSQLファイルのディレクトリを指定します。")]
        public string Directory { get; set; }

        public List<string> GetFiles()
        {
            var li = new List<string>();
            if (string.IsNullOrEmpty(Directory))
                li.Add(FileName);
            else
                li.AddRange(System.IO.Directory.GetFiles(Directory, FileName));
            return li;
        }

    }
}
