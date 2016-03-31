using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using RTree;

namespace Overkill
{
    public class ClosedPolylineProxy : PolylineProxy
    {
        public ClosedPolylineProxy(Polyline p, Overkill.Options opts, RTree<DbEntity> tree) : base(p, opts, tree)
        {
        }

        protected override void ProcessPolylineAndLine(Polyline p, LineSegment3d seg, Line l, bool bModifyStart, int i,
            DbEntity ent)
        {
            if (seg.GetParameterOf(bModifyStart ? l.EndPoint : l.StartPoint) < 0)
            {
                if (bModifyStart)
                    l.StartPoint = seg.EndPoint;
                else
                    l.EndPoint = seg.EndPoint;
                if (i < p.NumberOfVertices - 2)
                {
                    // case b2.1 and b2.3
                    RemoveSegmentAt(p, i);
                }
                else if (i == p.NumberOfVertices - 2)
                {
                    // case b2.5
                    RotatePolyline(p, i - 1);
                    p.Extend(true, bModifyStart ? l.EndPoint : l.StartPoint);
                    DelEntity(ent, false);
                }
                else
                {
                    // case 2.8
                    Polyline pnew = (Polyline) p.Clone();
                    int nVert = p.NumberOfVertices - 1;
                    for (int j = 1; j < nVert; j++)
                        pnew.RemoveVertexAt(1);
                    pnew.Closed = false;
                    pnew.Extend(false, bModifyStart ? l.EndPoint : l.StartPoint);
                    AddToDatabase(p.Database.BlockTableId, pnew);
                    p.Closed = false;
                    DelEntity(ent, false);
                }
            }
            else if (i == 0)
            {
                // case b2.2
                Polyline pnew = (Polyline) p.Clone();
                pnew.Closed = false;
                for (int j = 0; j < p.NumberOfVertices - 2; j++)
                {
                    pnew.RemoveVertexAt(pnew.NumberOfVertices - 1);
                }
                pnew.Extend(false, bModifyStart ? l.EndPoint : l.StartPoint);
                AddToDatabase(p.Database.BlockTableId, pnew);
                RemoveSegmentAt(p, i);
            }
            else if (i < p.NumberOfVertices - 2)
            {
                // case b2.4
                RotatePolyline(p, i);
                p.Extend(false, bModifyStart ? l.EndPoint : l.StartPoint);
                DelEntity(ent, false);
            }
            else if (i == p.NumberOfVertices - 2)
            {
                // case 2.6
                if (bModifyStart)
                    l.StartPoint = seg.StartPoint;
                else
                    l.EndPoint = seg.StartPoint;
                RemoveSegmentAt(p, i);
            }
            else
            {
                // case b2.7
                if (bModifyStart)
                    l.StartPoint = seg.StartPoint;
                else
                    l.EndPoint = seg.StartPoint;
                p.Closed = false;
                Options.OverlappedCount++;
            }
        }

        private void RemoveSegmentAt(Polyline p, int i)
        {
            Point2d[] pt = new Point2d[i + 1];
            Point2d[] widths = new Point2d[i + 1];
            p.Closed = false;
            for (int j = 0; j <= i; j++)
            {
                pt[j] = p.GetPoint2dAt(0);
                widths[j] = new Point2d(p.GetStartWidthAt(0), p.GetEndWidthAt(0));
                p.RemoveVertexAt(0);
            }
            for (int j = 0; j <= i; j++)
                p.AddVertexAt(p.NumberOfVertices, pt[j], 0, widths[j].X, widths[j].Y);
            Options.OverlappedCount++;
        }

        private static void RotatePolyline(Polyline p, int i)
        {
            Point2d[] pt = new Point2d[i + 2];
            Point2d[] widths = new Point2d[i + 2];
            p.Closed = false;
            for (int j = 0; j <= i + 1; j++)
            {
                pt[j] = p.GetPoint2dAt(0);
                widths[j] = new Point2d(p.GetStartWidthAt(0), p.GetEndWidthAt(0));
                if (j != i + 1)
                    p.RemoveVertexAt(0);
            }
            for (int j = 0; j <= i + 1; j++)
                p.AddVertexAt(p.NumberOfVertices, pt[j], 0, widths[j].X, widths[j].Y);
        }

        protected override void ProcessTangentLine(Polyline p, LineSegment3d seg, Line l, int i)
        {
            Line3d segLine = seg.GetLine();
            if (segLine.GetClosestPointTo(l.StartPoint).Point.DistanceTo(l.StartPoint) < Options.Tolerance &&
                segLine.GetClosestPointTo(l.EndPoint).Point.DistanceTo(l.EndPoint) < Options.Tolerance)
            {
                if (i < p.NumberOfVertices - 1)
                {
                    RemoveSegmentAt(p, i);
                }
                else
                {
                    p.Closed = false;
                    Options.OverlappedCount++;
                }
            }
        }
    }
}