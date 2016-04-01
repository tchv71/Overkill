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
                    if (list2.Count != 0 && options.bMaintainAssociativities)
                        continue;

                    bool bDelFirst;
                    if (Util.IsEqual(l1, l2, options, out bDelFirst))
                    {
                        DelEntity(bDelFirst? new DbEntity(l1):ent);
                        continue;
                    }
                    if (CheckUnequal(l1, l2))
                        continue;
                    if (Util.AreLinesParrallelAndItersects(l1, l2, options.Tolerance))
                    {
                        if (Util.IsPointLiesOnLine(l1, l2.StartPoint, options.Tolerance) && Util.IsPointLiesOnLine(l1, l2.EndPoint, options.Tolerance))
                        {
                            DelEntity(ent, false);
                            continue;
                        }
                        try
                        {
                            Debug.Assert(l1 != null, "l1 != null");
                            l1.JoinEntity(l2);
                            options.OverlappedCount++;
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