using System;
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
                if (p.GetSegmentType(i) != SegmentType.Line)
                    continue;
                if (seg != null)
                {
                    var list = Tree.Intersects(Util.GetRect(seg));
                    foreach (var ent in list)
                    {
                        if  (i>= (p.Closed ? p.NumberOfVertices : p.NumberOfVertices - 1))
                            continue;
                        seg = p.GetLineSegmentAt(i);
                        if (p.GetSegmentType(i) != SegmentType.Line)
                            continue;
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
                            if (!seg.Direction.IsParallelTo(l.Delta, new Tolerance(options.Tolerance, options.Tolerance)) ||
                                CheckUnequal(p, l))
                                continue;
                            if (Util.IsPointLiesOnLine(seg, l.StartPoint, options.Tolerance))
                            {
                                if (Util.IsPointLiesOnLine(seg, l.EndPoint, options.Tolerance))
                                {
                                    // case b1.6
                                    DelEntity(ent, false);
                                    continue;
                                }
                                if (options.bMaintainPolylines)
                                    continue;
                                Point3d pt = l.StartPoint;
                                if (pt.DistanceTo(seg.StartPoint)<options.Tolerance || pt.DistanceTo(seg.EndPoint)<options.Tolerance)
                                    ProcessTangentLine(p,seg,l, true,i, ent);
                                else
                                    ProcessPolylineAndLine(p, seg, l, true, i, ent);
                            }
                            else if (Util.IsPointLiesOnLine(seg, l.EndPoint, options.Tolerance))
                            {
                                if (options.bMaintainPolylines)
                                    continue;
                                Point3d pt = l.EndPoint;
                                if (pt.DistanceTo(seg.StartPoint) < options.Tolerance || pt.DistanceTo(seg.EndPoint) < options.Tolerance)
                                    ProcessTangentLine(p, seg, l, false, i, ent);
                                else
                                    ProcessPolylineAndLine(p, seg, l, false, i, ent);
                            }
                            else
                            {
                                if (options.bMaintainPolylines)
                                    continue;
                                ProcessTangentLine(p, seg, l, true, i, ent);
                            }
                        }
                    }
                }
            }
        }


        // Изменить полилинию и линию, которая является касательной к ней
        protected virtual void ProcessTangentLine(Polyline p, LineSegment3d seg, Line l, bool bModifyFirst, int i, DbEntity ent)
        {
            Line3d segLine = seg.GetLine();
            if (segLine.GetClosestPointTo(l.StartPoint).Point.DistanceTo(l.StartPoint) < options.Tolerance &&
                segLine.GetClosestPointTo(l.EndPoint).Point.DistanceTo(l.EndPoint) <options.Tolerance)
            {
                if (IsAdjacent(p, seg, l, bModifyFirst, i, ent)) return;
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

        protected bool IsAdjacent(Polyline p, LineSegment3d seg, Line l, bool bModifyFirst, int i, DbEntity ent)
        {
            double parameterOfStart = seg.GetParameterOf(l.StartPoint);
            double parameterOfEnd = seg.GetParameterOf(l.EndPoint);
            if (parameterOfStart*parameterOfEnd > 0 || (Math.Abs(parameterOfEnd)<options.Tolerance && parameterOfStart<0) || (Math.Abs(parameterOfStart) < options.Tolerance && parameterOfEnd < 0))
            {
                ProcessPolylineAndLine(p, seg, l, bModifyFirst, i, ent);
                return true;
            }
            return false;
        }


        protected virtual void ProcessPolylineAndLine(Polyline p, LineSegment3d seg, Line l, bool bModifyStart, int i, DbEntity ent)
        {
            Point3d pt = bModifyStart ? l.EndPoint : l.StartPoint;
            bool b1 = seg.GetParameterOf(pt) < 0;
            bool b2 = seg.Direction.Y>-seg.Direction.X;
            if (!(b1^b2))
            {
                if (bModifyStart)
                    l.StartPoint = !b1?seg.StartPoint:seg.EndPoint;
                else
                    l.EndPoint = !b1 ? seg.StartPoint : seg.EndPoint;
                if (i == 0)
                {
                    // case b1.1
                    if (p.NumberOfVertices==2)
                        DelEntity(new DbEntity(p));
                    else
                        p.RemoveVertexAt(i);
                    options.OverlappedCount++;
                }
                else if (i != p.NumberOfVertices - 2)
                {
                    // case b1.3
                    BreakPolyline(p, i, l.EndPoint);
                }
                else
                {
                    // case b1.5
                    if (p.NumberOfVertices == 2)
                        DelEntity(new DbEntity(p));
                    else
                        p.RemoveVertexAt(i+1);
                    options.OverlappedCount++;
                }
            }
            else if (i == 0)
            {
                if (b1)
                    p.Extend(true, pt);
                else
                    BreakPolyline2(p, i, pt, false);
                DelEntity(ent, false);
            }
            else if (i == p.NumberOfVertices - 2)
            {
                // case b1.2, 1.6
                if (!b1)
                    p.Extend(i==0,pt);
                else
                    BreakPolyline2(p, i, pt, true);

                DelEntity(ent, false);
                 return;
            }
            else
            {
                // case 1.4
                BreakPolyline2(p, i, pt, b1);
                DelEntity(ent, false);
            }
        }

        // Разбить полилинию на 2, удалив сегмент i между ними
        private void BreakPolyline(Polyline p, int i, Point3d pt)
        {
            if (p.NumberOfVertices == 2)
            {
                DelEntity(new DbEntity(p), false);
                return;
            }
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
        private void BreakPolyline2(Polyline p, int i, Point3d pt, bool bExtendStart)
        {
            if (p.NumberOfVertices > 2)
            {
                Polyline pnew = (Polyline) p.Clone();
                for (int j = 0; j < (bExtendStart? i : i + 1); j++)
                    pnew.RemoveVertexAt(0);
                ObjectId blockTableId = p.Database.BlockTableId;
                if (bExtendStart)
                    pnew.Extend(true, pt);
                AddToDatabase(blockTableId, pnew);
                for (int j = (bExtendStart? i+1 : i + 2); j < p.NumberOfVertices;)
                    p.RemoveVertexAt(p.NumberOfVertices - 1);
            }
            if (!bExtendStart)
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