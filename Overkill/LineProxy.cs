using System.Diagnostics;
using System;
#if NCAD
using Teigha.DatabaseServices;
using Teigha.Geometry;
using Teigha.Runtime;
using Platform = HostMgd;
using PlatformDb = Teigha;
#else
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Platform = Autodesk.AutoCAD;
using PlatformDb = Autodesk.AutoCAD;
# endif
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
                    ObjectIdCollection list1 = l1.GetPersistentReactorIds();
                    if (list1.Count != 0 && options.bMaintainAssociativities)
                        continue;

                    bool bDelFirst;
                    if (Util.IsEqual(l1, l2, options, out bDelFirst))
                    {
                        DelEntity(bDelFirst ? new DbEntity(l1) : ent);
                        continue;
                    }
                    if (CheckUnequal(l1, l2))
                        continue;
                    if (Util.AreLinesParrallelAndItersects(l1, l2, options.Tolerance))
                    {
                        bool startPointLiesOnLine = Util.IsPointLiesOnLine(l1, l2.StartPoint, options.Tolerance);
                        bool endPointLiesOnLine = Util.IsPointLiesOnLine(l1, l2.EndPoint, options.Tolerance);
                        if (startPointLiesOnLine && endPointLiesOnLine)
                        {
                            DelEntity(ent, false);
                            continue;
                        }
                        Debug.Assert(l1 != null, "l1 != null");
                        JoinLines(l1, l2, ent, startPointLiesOnLine);
                    }
                }
            }

        }

        /// <summary>
        /// Объединение двух линий
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <param name="ent"></param>
        /// <param name="startPointLiesOnLine"></param>
        private void JoinLines(Line l1, Line l2, DbEntity ent, bool startPointLiesOnLine)
        {
            if (l2.GetPersistentReactorIds().Count > 0)
            {
                Line3d l3D2 = new Line3d(l2.StartPoint, l2.EndPoint);
                Point3d pt2 = Util.IsPointLiesOnLine(l2, l1.StartPoint,options.Tolerance) ? l1.EndPoint : l1.StartPoint;
                if (l3D2.GetClosestPointTo(pt2).Parameter < 0)
                    l2.StartPoint = pt2;
                else
                    l2.EndPoint = pt2;
                DelEntity(new DbEntity(l1), false);
                return;
            }
            Line3d l = new Line3d(l1.StartPoint, l1.EndPoint);
            Point3d pt = startPointLiesOnLine ? l2.EndPoint : l2.StartPoint;
            if (l.GetClosestPointTo(pt).Parameter < 0)
                l1.StartPoint = pt;
            else
                l1.EndPoint = pt;
            DelEntity(ent,false);
        }
    }
}