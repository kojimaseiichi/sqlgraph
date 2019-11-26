using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;

namespace sqlp
{
    [Verb("tree", HelpText = "スクリプトファイルをパースしてツリー表示")]
    class TreeSubCommand
    {
        [Option('f', "file", Required = true, HelpText = "解析するSQLファイルのパスを指定します。")]
        public string FileName { get; set; }

        [Option('o', "output", HelpText = "結果出力先を任意で指定します。")]
        public string OutputDest { get; set; }

    }
}
