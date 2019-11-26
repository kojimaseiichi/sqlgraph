using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Antlr4.Runtime.Tree;

namespace sqlp
{
    public enum ScriptVertexType
    {
        Script,
        Package,
        PackageBody,
        CursorDecl,
        Function,
        Procedure,
        Select,
        Update,
        Insert,
        Delete,
        Query
    }

    [DebuggerDisplay("Name={Name}, Value={Value}")]
    public class PlsAttr
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public PlsAttr(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    [DebuggerDisplay("ID={ID}, Name={Name}, VertexType={VertexType}")]
    public class PlsVertex
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public ScriptVertexType VertexType { get; private set; }

        private List<PlsAttr> _attrs = new List<PlsAttr>();

        public IList<PlsAttr> Attrs { get { return _attrs; } }

        public PlsVertex(string id, string name, ScriptVertexType vertexType)
        {
            this.ID = id;
            this.Name = name;
            this.VertexType = vertexType;
        }
    }

    [DebuggerDisplay("ID={ID}")]
    public class PlsEdge
    {
        public string ID { get; private set; }
        public PlsVertex Source { get; private set; }
        public PlsVertex Target { get; private set; }


        public PlsEdge(string id, PlsVertex source, PlsVertex target)
        {
            this.ID = id;
            this.Source = source;
            this.Target = target;
        }
    }

    public class Walk
    {
        public IParseTree Current { get; set; }
        public IParseTree Parent { get; set; }
        public int Depth { get; set; }
    }

    public enum Pred
    {
        Next,
        Skip,
        Stop,
        End
    }

    /// <summary>
    /// 探索
    /// </summary>
    public static class TreeSearch
    {
        /// <summary>
        /// 深さ優先探索(depth first search)
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static IEnumerable<Walk> dfs(IParseTree root, Func<Walk, Pred> predicate  = null, int within = int.MaxValue)
        {
            if (within < 0)
                yield break;
            List<int> stack = new List<int>();
            stack.Add(0);
            IParseTree parent = root;
            while(stack.Count > 0)
            {
                int depth = stack.Count - 1;
                int index = stack[depth];
                if (index < parent.ChildCount)
                {
                    var child = parent.GetChild(index);
                    Walk walk = new Walk { Current = child, Parent = parent, Depth = depth };
                    if (predicate == null)
                        yield return walk;
                    else
                    {
                        Pred pred = predicate(walk);
                        if (pred == Pred.Stop)
                            yield break;
                        if (pred == Pred.Next || pred == Pred.End)
                            yield return walk;
                        if (pred == Pred.End)
                            yield break;
                    }
                    if (child.ChildCount > 0 && depth < within)
                    {
                        // 子ノードの１番目に移動
                        stack.Add(0);
                        parent = child;
                    }
                    else
                    {
                        // 次の兄弟ノードに移動
                        stack[depth]++;
                    }
                }
                else
                {
                    // 親ノードに移動
                    if (parent != null)
                        parent = parent.Parent;
                    stack.RemoveAt(depth);
                    depth--;
                    // 次の兄弟ノードに移動
                    if (depth >= 0)
                        stack[depth]++;
                }
            }
        }

        /// <summary>
        /// 幅優先探索(breadth first search)
        /// </summary>
        /// <param name="root"></param>
        /// <param name="predicate"></param>
        /// <param name="within"></param>
        /// <returns></returns>
        public static IEnumerable<Walk> bfs(IParseTree root, Func<Walk, Pred> predicate = null, int within = int.MaxValue)
        {
            List<IParseTree> sibling = new List<IParseTree>();
            sibling.Add(root);
            int depth = 1;
            while (sibling.Count > 0 && depth <= within)
            {
                List<IParseTree> nextSibling = new List<IParseTree>();
                foreach (IParseTree parent in sibling)
                {
                    for (int n = 0; n < parent.ChildCount; n++)
                    {
                        var child = parent.GetChild(n);
                        nextSibling.Add(child);
                        Walk walk = new Walk { Current = child, Parent = parent, Depth = depth };
                        if (predicate == null)
                            yield return walk;
                        else
                        {
                            Pred pred = predicate(walk);
                            if (pred == Pred.Stop)
                                yield break;
                            if (pred == Pred.Next || pred == Pred.End)
                                yield return walk;
                            if (pred == Pred.End)
                                yield break;
                        }
                    }
                }
                sibling = nextSibling;
                depth++;
            }
        }

        /// <summary>
        /// 幅優先探索(breadth first search)
        /// </summary>
        /// <param name="root"></param>
        /// <param name="within"></param>
        /// <returns></returns>
        public static IEnumerable<Walk> bfs(IParseTree root, int within = int.MaxValue)
        {
            return bfs(root, null, within);
        }

        /// <summary>
        /// 祖先頂点を列挙
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<Walk> ancestors(IParseTree node)
        {
            IParseTree current = node.Parent;
            int depth = -1;
            while (current != null)
            {
                yield return new Walk { Current = current, Parent = current.Parent, Depth = depth };
                current = current.Parent;
                depth--;
            }
        }

        public static bool _dfs(IParseTree v, PlsVertex source, int depth, Func<IParseTree, PlsVertex, bool> handler)
        {
            if (depth == 0)
                return false;
            depth--;
            bool next = handler.Invoke(v, source);
            if (next == false)
                return false;
            for (int n = 0; n < v.ChildCount; n++)
            {
                if (_dfs(v.GetChild(n), source, depth, handler) == false)
                    return false;
            }
            return true;
        }

        public static bool _dfs(IParseTree v, PlsVertex source, Func<IParseTree, PlsVertex, bool> handler)
        {
            return _dfs(v, source, -1, handler);
        }

        public static bool _dfs(IParseTree v, Func<IParseTree, bool> handler)
        {
            bool next = handler.Invoke(v);
            if (next == false)
                return false;
            for (int n = 0; n < v.ChildCount; n++)
            {
                if (_dfs(v.GetChild(n), handler) == false)
                    return false;

            }
            return true;
        }

        public static void _dfs(IParseTree t, int depth, Action<IParseTree> handler)
        {
            if (depth == 0)
                return;
            depth--;
            handler.Invoke(t);
            for (int n = 0; n < t.ChildCount; n++)
            {
                _dfs(t.GetChild(n), depth, handler);
            }
        }

        public static void _dfs(IParseTree t, Action<IParseTree> handler)
        {
            _dfs(t, -1, handler);
        }

        public static void _dfsDebug(IParseTree t, int depth = 0)
        {
            for (int n = 0; n < depth; n++)
                Console.Write(' ');
            Console.WriteLine("{0}: {1}", t.GetType().Name, t.GetText());
            depth++;
            for (int n = 0; n < t.ChildCount; n++)
                _dfsDebug(t.GetChild(n), depth);
        }

        public static IParseTree _dfsFirstTreeNode(IParseTree t, Type target)
        {
            IParseTree found = null;
            _dfs(t, (IParseTree tt) => {
                if (target == tt.GetType())
                {
                    found = tt;
                    return false;
                }
                return true;
            });
            return found;
        }

        public static List<IParseTree> _dfsTypeMatchedTreeNode(IParseTree t, List<Type> targetTypes)
        {
            List<IParseTree> found = new List<IParseTree>();
            _dfs(t, (IParseTree tt) => {
                if (targetTypes.Any(x => { return x == tt.GetType(); }))
                    found.Add(tt);
                return true;
            });
            return found;
        }

        public static bool _bfs(IParseTree t, PlsVertex source, int depth, Func<IParseTree, PlsVertex, bool> hander)
        {
            List<IParseTree> bag = new List<IParseTree>();
            bag.Add(t);
            while (bag.Count > 0)
            {
                if (depth == 0)
                    return false;
                List<IParseTree> newBag = new List<IParseTree>();
                foreach (var e in bag)
                {
                    if (hander.Invoke(e, source) == false)
                        return false; // handerが探索の打ち切りを判定する
                    for (int n = 0; n < e.ChildCount; n++)
                    {
                        newBag.Add(e.GetChild(n));
                    }
                }
                depth--;
                bag = newBag;
            }
            return true;
        }

        public static bool _bfs(IParseTree t, PlsVertex source, Func<IParseTree, PlsVertex, bool> hander)
        {
            return _bfs(t, source, -1, hander);
        }

        public static bool _bfs(IParseTree t, int depth, Func<IParseTree, bool> handler)
        {
            List<IParseTree> bag = new List<IParseTree>();
            bag.Add(t);
            while (bag.Count > 0)
            {
                if (depth == 0)
                    return false;
                List<IParseTree> newBag = new List<IParseTree>();
                foreach (var e in bag)
                {
                    if (handler.Invoke(e) == false)
                        return false; // handerが探索の打ち切りを判定する
                    for (int n = 0; n < e.ChildCount; n++)
                        newBag.Add(e.GetChild(n));
                }
                depth--;
                bag = newBag;
            }
            return true;
        }

        public static bool _bfs(IParseTree t, Func<IParseTree, bool> handler)
        {
            return _bfs(t, -1, handler);
        }

        public static IParseTree _bfsFirstTreeNode(IParseTree t, Type target)
        {
            IParseTree found = null;
            _bfs(t, (IParseTree tt) =>
            {
                if (target == tt.GetType())
                {
                    found = tt;
                    return false;
                }
                return true;
            });
            return found;
        }

        public static List<IParseTree> _bfsTypeMatchedTreeNode(IParseTree t, List<Type> targetTypes)
        {
            List<IParseTree> found = new List<IParseTree>();
            _bfs(t, (IParseTree tt) =>
            {
                if (targetTypes.Any(x => { return x == tt.GetType(); }))
                    found.Add(tt);
                return true;
            });
            return found;
        }


    }
}
