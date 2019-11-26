using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sqlparser
{
    public class GraphContext
    {
        private int _seq;

        public string _NextSeq
        {
            get { return String.Format("{0:00000}", _seq++); }
        }
    }

    public class Graph
    {
        protected GraphContext _context;
        protected Dictionary<string, PlsVertex> _vertexSet = new Dictionary<string, PlsVertex>();
        protected Dictionary<string, PlsEdge> _edgeSet = new Dictionary<string, PlsEdge>();

        public Graph(GraphContext context)
        {
            _context = context;
        }

        protected PlsVertex _addNode(PlsVertex source, string name, ScriptVertexType vertexType)
        {
            if (source != null && _vertexSet.ContainsKey(source.ID) == false)
                throw new Exception();
            var id = _context._NextSeq;
            var v = new PlsVertex(id, name, vertexType);
            _vertexSet.Add(v.ID, v);
            if (source != null)
            {
                var e = new PlsEdge(_context._NextSeq, source, v);
                _edgeSet.Add(e.ID, e);
            }
            return v;
        }

    }
}
