using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;

namespace sqlparser
{
    public class DmlGraph : Graph
    {
        public DmlGraph(GraphContext context, IParseTree rootDml) :base(context)
        {
            _walkDML(rootDml, null, null);
        }


        private bool _walkDML(IParseTree t, PlsVertex source, Action<IParseTree> action)
        {
            if (t is PlSqlParser.Select_statementContext)
            {
                _walkDML_select(t, _addNode(source, "", ScriptVertexType.Select));
            }
            else if (t is PlSqlParser.Update_statementContext)
            {
                _walkDML_update(t, _addNode(source, "", ScriptVertexType.Update));
            }
            else if (t is PlSqlParser.Insert_statementContext)
            {
                _walkDML_insert(t, _addNode(source, "", ScriptVertexType.Insert));
            }
            else if (t is PlSqlParser.Delete_statementContext)
            {
                _walkDML_delete(t, _addNode(source, "", ScriptVertexType.Delete));
            }
            else if (t is PlSqlParser.Query_blockContext)
            {
                _walkDML_select(t, _addNode(source, "", ScriptVertexType.Query));
            }
            else
            {
                if (action != null)
                {
                    action(t);
                }
                for (int n = 0; n < t.ChildCount; n++)
                {
                    _walkDML(t.GetChild(n), source, action);
                }
            }
            return true;
        }

        private void _walkDML_select(IParseTree t, PlsVertex select)
        {
            for (int n = 0; n < t.ChildCount; n++)
            {
                bool in_from = false;
                _walkDML(t.GetChild(n), select, (IParseTree tt) =>
                {
                    if (tt is PlSqlParser.From_clauseContext)
                    {
                        in_from = true;
                    }
                    else if (in_from && tt is PlSqlParser.Tableview_nameContext)
                    {
                        select.Attrs.Add(new PlsAttr("ref", tt.GetText()));
                    }
                });
            }
        }

        private void _walkDML_update(IParseTree t, PlsVertex update)
        {
            for (int n = 0; n < t.ChildCount; n++)
            {
                _walkDML(t.GetChild(n), update, (IParseTree tt) =>
                {
                    if (tt is PlSqlParser.Tableview_nameContext)
                    {
                        update.Attrs.Add(new PlsAttr("ref", tt.GetText()));
                    }
                });
            }
        }

        private void _walkDML_insert(IParseTree t, PlsVertex insert)
        {
            for (int n = 0; n < t.ChildCount; n++)
            {
                _walkDML(t.GetChild(n), insert, (IParseTree tt) =>
                {
                    if (tt is PlSqlParser.Tableview_nameContext)
                    {
                        insert.Attrs.Add(new PlsAttr("ref", tt.GetText()));
                    }
                });
            }
        }

        private void _walkDML_delete(IParseTree t, PlsVertex delete)
        {
            for (int n = 0; n < t.ChildCount; n++)
            {
                _walkDML(t.GetChild(n), delete, (IParseTree tt) =>
                {
                    if (tt is PlSqlParser.Tableview_nameContext)
                    {
                        delete.Attrs.Add(new PlsAttr("ref", tt.GetText()));
                    }
                });
            }
        }


    }
}
