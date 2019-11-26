using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace sqlp
{
    class OutputWriter : IDisposable
    {
        public void Dispose()
        {
            if (_writer != null)
                _writer.Dispose();
            _writer = null;
        }

        private TextWriter GetOutputStream(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) == false)
                return new StreamWriter(fileName, true);
            return Console.Out;
        }

        private TextWriter _writer;

        public OutputWriter(string fileName)
        {
            _writer = GetOutputStream(fileName);
        }

        public void WriteLine(string s)
        {
            _writer.WriteLine(s);
        }

        public void WriteLine(string format, params object[] args)
        {
            _writer.WriteLine(format, args);
        }

        public void Write(string s)
        {
            _writer.Write(s);
        }
    }
}
