using System.Diagnostics;
using Autodesk.AutoCAD.DatabaseServices;
using RTree;

namespace Overkill
{
    public class LineProxy : EntityProxy
    {
        public LineProxy(Line l, Overkill.Options opts, RTree<DbEntity> tree) : base(l, opts, tree)
        {
        }

        public override void Process()
        {
            Line l1 = Ent as Line;
            var list = Tree.Intersects(Util.GetRect(l1));
            foreach (var ent in list)
            {
                if (ent.Ptr == l1 || ent.Ptr.IsErased)
                    continue;
                Line l2 = ent.Ptr as Line;
                if (l2 != null)
                {
                    ObjectIdCollection list2 = l2.GetPersistentReactorIds();
                    // Если с линией что-то ассоциировано, не удалять ее
                    if (list2.Count != 0 && Options.bMaintainAssociativities)
                        continue;

                    if (Util.IsEqual(l1, l2, Options.Tolerance))
                    {
                        DelEntity(ent);
                        continue;
                    }
                    if (Util.AreLinesParrallelAndItersects(l1, l2, Options.Tolerance))
                    {
                        if (Util.IsPointLiesOnLine(l1, l2.StartPoint, Options.Tolerance) && Util.IsPointLiesOnLine(l1, l2.EndPoint, Options.Tolerance))
                        {
                            DelEntity(ent, false);
                            continue;
                        }
                        try
                        {
                            Debug.Assert(l1 != null, "l1 != null");
                            l1.JoinEntity(l2);
                        }
                        catch (Autodesk.AutoCAD.Runtime.Exception)
                        {
                        }
                    }
                }
            }

        }
    }
}