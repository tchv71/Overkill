using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using RTree;
using System.Diagnostics;

namespace Overkill
{
    public class PolylineProxy : EntityProxy
    {
        public PolylineProxy(Polyline p, Overkill.Options opts, RTree<DbEntity> tree) : base(p, opts, tree)
        {
        }

        public  override void Process()
        {
            Polyline p = Ent as Polyline;
            Debug.Assert(p != null, "p != null");
            for (int i = 0; i < (p.Closed ? p.NumberOfVertices : p.NumberOfVertices - 1); i++)
            {
                LineSegment3d seg = p.GetLineSegmentAt(i);
                if (seg != null)
                {
                    var list = Tree.Intersects(Util.GetRect(seg));
                    foreach (var ent in list)
                    {
                        if (ent.Ptr == p || ent.Ptr.IsErased)
                            continue;
                        var ptr = ent.Ptr as Polyline;
                        if (ptr != null)
                        {
                            bool bDelFirst;
                            if (Util.IsEqual(p, ptr, options, out bDelFirst))
                            {
                                DelEntity(bDelFirst? new DbEntity(p) : ent );
                                continue;
                            }
                            //p.JoinEntity(ptr);
                        }
                        else if (ent.Ptr is Line)
                        {
                            Line l = (Line)ent.Ptr;
                            if (!seg.Direction.IsParallelTo(l.Delta, new Tolerance(options.Tolerance, options.Tolerance)))
                                continue;
                            if (Util.IsPointLiesOnLine(seg, l.StartPoint, options.Tolerance))
                            {
                                if (Util.IsPointLiesOnLine(seg, l.EndPoint, options.Tolerance))
                                {
                                    // case b1.6
                                    DelEntity(ent, false);
                                    continue;
                                }
                                ProcessPolylineAndLine(p, seg, l, true, i, ent);
                            }
                            else if (Util.IsPointLiesOnLine(seg, l.EndPoint, options.Tolerance))
                            {
                                ProcessPolylineAndLine(p, seg, l, false, i, ent);
                            }
                            else
                            {
                                ProcessTangentLine(p, seg, l, i);
                            }
                        }
                    }
                }
            }
        }

        // Изменить полилинию и линию, которая является касательной к ней
        protected virtual void ProcessTangentLine(Polyline p, LineSegment3d seg, Line l, int i)
        {
            Line3d segLine = seg.GetLine();
            if (segLine.GetClosestPointTo(l.StartPoint).Point.DistanceTo(l.StartPoint) < options.Tolerance &&
                segLine.GetClosestPointTo(l.EndPoint).Point.DistanceTo(l.EndPoint) <options.Tolerance)
            {
                // cases b1.7-1.9
                if (i == 0)
                {
                    // case b1.8
                    p.RemoveVertexAt(0);
                }
                else if (i == p.NumberOfVertices - 2)
                {
                    // case b1.9
                    p.RemoveVertexAt(i + 1);
                }
                else
                {
                    // case 1.7
                    BreakPolyline(p, i, l.StartPoint);
                }
            }
        }

 
        protected virtual void ProcessPolylineAndLine(Polyline p, LineSegment3d seg, Line l, bool bModifyStart, int i, DbEntity ent)
        {
            if (seg.GetParameterOf(bModifyStart ? l.EndPoint : l.StartPoint) < 0)
            {
                if (bModifyStart)
                    l.StartPoint = seg.EndPoint;
                else
                    l.EndPoint = seg.EndPoint;
                if (i == 0)
                {
                    // case b1.1
                    p.RemoveVertexAt(i);
                    options.OverlappedCount++;
                }
                else
                {
                    // case b1.2, b1.5
                    BreakPolyline(p, i, l.EndPoint);
                }
            }
            else if (i == p.NumberOfVertices - 2)
            {
                // case b1.4
                if (bModifyStart)
                    l.StartPoint = seg.StartPoint;
                else
                    l.EndPoint = seg.StartPoint;
                p.RemoveVertexAt(i + 1);
                options.OverlappedCount++;
            }
            else
            {
                // case b1.3
                BreakPolyline2(p, i, l.EndPoint);
                DelEntity(ent, false);
            }
        }

        // Разбить полилинию на 2, удалив сегмент i между ними
        private void BreakPolyline(Polyline p, int i, Point3d pt)
        {
            Polyline pnew = (Polyline)p.Clone();
            bool bLastSeg = i == p.NumberOfVertices - 2;
            for (int j = 0; j < (bLastSeg ? i : i + 1); j++)
                pnew.RemoveVertexAt(0);
            if (bLastSeg)
            {
                pnew.Extend(true, pt);
            }
            AddToDatabase(p.Database.BlockTableId, pnew);
            for (int j = i + 1; j < p.NumberOfVertices;)
                p.RemoveVertexAt(p.NumberOfVertices - 1);
            options.OverlappedCount++;
        }

        // Разбить полилинию на 2, не удаляя сегмент (разрезать по вершине i+1) и продлить конец первой полилинии до указанной точки
        private void BreakPolyline2(Polyline p, int i, Point3d pt)
        {
            Polyline pnew = (Polyline)p.Clone();
            for (int j = 0; j < i + 1; j++)
                pnew.RemoveVertexAt(0);
            ObjectId blockTableId = p.Database.BlockTableId;
            AddToDatabase(blockTableId, pnew);
            for (int j = i + 2; j < p.NumberOfVertices;)
                p.RemoveVertexAt(p.NumberOfVertices - 1);
            p.Extend(false, pt);
            options.OverlappedCount++;
        }

        protected void AddToDatabase(ObjectId blockTableId, Polyline pnew)
        {
            BlockTable acBlkTbl = options.Tr.GetObject(blockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord ms = options.Tr.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
            Debug.Assert(ms != null, "ms != null");
            ms.AppendEntity(pnew);
            options.Tr.AddNewlyCreatedDBObject(pnew, true);
        }
    }
}