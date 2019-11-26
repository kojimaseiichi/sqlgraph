using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;

namespace sqlparser
{
    public class PlsPackageGraph : Graph
    {

        private string _getFullQualifiedName(string schemaName, string objectName)
        {
            if (string.IsNullOrEmpty(schemaName))
                return objectName;
            return schemaName + "." + objectName;
        }

        public PlsPackageGraph(GraphContext context, IParseTree v, string name) : base(context)
        {
            PlsVertex script = new PlsVertex(context._NextSeq, name, ScriptVertexType.Script);
            _vertexSet.Add(script.ID, script);
            TreeSearch._dfs(v, script, _walkScript);
            TreeSearch._dfsDebug(v);
        }

        private bool _walkScript(IParseTree v, PlsVertex source)
        {
            if (v is sqlparser.PlSqlParser.Create_packageContext)
                TreeSearch._dfs(v, source, _walkPackage);
            if (v is PlSqlParser.Create_package_bodyContext)
                return true;
            return true;
        }

        private bool _walkPackage(IParseTree t, PlsVertex source)
        {
            if (t is sqlparser.PlSqlParser.Create_packageContext)
            {
                string schemaName = "";
                string packageName = "";
                TreeSearch._dfs(t, source, 1, (IParseTree tt, PlsVertex ss) =>
                {
                    if (tt is PlSqlParser.Schema_nameContext)
                    {
                        var cc = (PlSqlParser.Schema_nameContext)tt;
                        schemaName = cc.GetText();
                    }
                    if (tt is PlSqlParser.Package_nameContext)
                    {
                        var cc = (PlSqlParser.Package_nameContext)tt;
                        packageName = cc.GetText();
                    }
                    return true;
                });
                string qn = _getFullQualifiedName(schemaName, packageName);
                TreeSearch._dfs(t, _addNode(source, qn, ScriptVertexType.Package), _walkPackageDecl);
            }
            return true;
        }

        private bool _walkPackageBody(IParseTree t, PlsVertex source)
        {
            if (t is PlSqlParser.Create_package_bodyContext)
            {

            }
            return true;
        }

        private bool _walkPackageDecl(IParseTree t, PlsVertex source)
        {
            if (t is PlSqlParser.Package_obj_specContext)
            {
                TreeSearch._dfs(t, source, (IParseTree t2, PlsVertex sv) =>
                {
                    if (t2 is PlSqlParser.Cursor_declarationContext)
                        _walkCursorDecl(t2, source);
                    if (t2 is PlSqlParser.Function_specContext)
                        _walkFunctionSpec(t2, source);
                    if (t2 is PlSqlParser.Procedure_specContext)
                        _walkProcedureSpec(t2, source);
                    return true;
                });
            }

            return true;
        }

        private bool _walkFunctionSpec(IParseTree t, PlsVertex source)
        {
            if (t is PlSqlParser.Function_specContext)
            {
                IParseTree funcNameNode = TreeSearch._dfsFirstTreeNode(t, typeof(PlSqlParser.IdentifierContext));
                string funcName = funcNameNode != null ? funcNameNode.GetText() : "";
                PlsVertex vertex_func = _addNode(source, funcName, ScriptVertexType.Function);
                TreeSearch._dfs(t, source, (IParseTree tt, PlsVertex v) =>
                {
                    if (tt is PlSqlParser.ParameterContext)
                    {
                        List<string> param =
                        TreeSearch._dfsTypeMatchedTreeNode(tt, new List<Type> { typeof(TerminalNodeImpl) })
                            .Select(x => x.GetText())
                            .ToList();
                        string paramexp = string.Join(" ", param);
                        vertex_func.Attrs.Add(new PlsAttr("param", paramexp));
                    }
                    return true;
                });
                IParseTree returnTypeNode = TreeSearch._bfsFirstTreeNode(t, typeof(PlSqlParser.Type_specContext));
                string returnTypeName = returnTypeNode != null ? returnTypeNode.GetText() : "";
                vertex_func.Attrs.Add(new PlsAttr("return", returnTypeName));
            }
            return true;
        }

        private bool _walkProcedureSpec(IParseTree t, PlsVertex source)
        {
            if (t is PlSqlParser.Procedure_specContext)
            {
                IParseTree procNameNode = TreeSearch._dfsFirstTreeNode(t, typeof(PlSqlParser.IdentifierContext));
                string procName = procNameNode != null ? procNameNode.GetText() : "";
                PlsVertex vertex = _addNode(source, procName, ScriptVertexType.Procedure);
                TreeSearch._dfs(t, source, (IParseTree tt, PlsVertex v) =>
                {
                    if (tt is PlSqlParser.ParameterContext)
                    {
                        List<string> param =
                        TreeSearch._dfsTypeMatchedTreeNode(tt, new List<Type> { typeof(TerminalNodeImpl) })
                            .Select(x => x.GetText())
                            .ToList();
                        string paramexp = string.Join(" ", param);
                        vertex.Attrs.Add(new PlsAttr("param", paramexp));
                    }
                    return true;
                });
            }
            return true;
        }

        private bool _walkCursorDecl(IParseTree t, PlsVertex source)
        {
            if (t is PlSqlParser.Cursor_declarationContext)
            {
                TreeSearch._dfs(t, source, (IParseTree tt, PlsVertex sv) =>
                {
                    if (tt is PlSqlParser.Select_statementContext)
                    {
                        //_walkDML(tt, source, null);
                    }
                    return true;
                });
            }

            return true;
        }

    }
}
