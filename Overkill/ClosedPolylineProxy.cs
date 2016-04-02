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
            Point3d pt = bModifyStart ? l.EndPoint : l.StartPoint;
            bool b1 = seg.GetParameterOf(pt) < 0;
            bool b2 = seg.Direction.Y > -seg.Direction.X;
            if ((b1 && b2) || (!b1 && !b2))
            {
                if (bModifyStart)
                    l.StartPoint = !b1? seg.StartPoint:seg.EndPoint;
                else
                    l.EndPoint = !b1 ? seg.StartPoint : seg.EndPoint;
                if (i <= p.NumberOfVertices - 2)
                {
                    // case b2.1, b2.3, b2.5
                    RemoveSegmentAt(p, i);
                }
                else
                {
                    // case b2.7
                    p.Closed = false;
                    options.OverlappedCount++;
                }
            }
            else if (i == 0)
            {
                if (b1)
                {
                    p.AddVertexAt(p.NumberOfVertices, p.GetPoint2dAt(0), 0, p.GetEndWidthAt(p.NumberOfVertices-1), p.GetEndWidthAt(p.NumberOfVertices) );
                    p.Closed = false;
                    p.Extend(true,pt);
                    DelEntity(ent, false);
                    return;
                }
                // case b2.2
                Polyline pnew = (Polyline) p.Clone();
                pnew.Closed = false;
                for (int j = 0; j < p.NumberOfVertices - 2; j++)
                {
                    pnew.RemoveVertexAt(pnew.NumberOfVertices - 1);
                }
                pnew.Extend(false, pt);
                AddToDatabase(p.Database.BlockTableId, pnew);
                RemoveSegmentAt(p, i);
            }
            else if (i <= p.NumberOfVertices - 2)
            {
               
                // case b2.4, b2.6
                RotatePolyline(p, b1? i-1:i);
                p.Extend(b1, pt);
                DelEntity(ent, false);
            }
            else
            {
                if (!b1)
                {
                    p.AddVertexAt(p.NumberOfVertices, p.GetPoint2dAt(0), 0, p.GetEndWidthAt(p.NumberOfVertices - 1), p.GetEndWidthAt(p.NumberOfVertices));
                    p.Closed = false;
                    p.Extend(false, pt);
                    DelEntity(ent, false);
                    return;
                }
                // case 2.8
                Polyline pnew = (Polyline)p.Clone();
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
            options.OverlappedCount++;
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
            if (segLine.GetClosestPointTo(l.StartPoint).Point.DistanceTo(l.StartPoint) < options.Tolerance &&
                segLine.GetClosestPointTo(l.EndPoint).Point.DistanceTo(l.EndPoint) < options.Tolerance)
            {
                if (i < p.NumberOfVertices - 1)
                {
                    RemoveSegmentAt(p, i);
                }
                else
                {
                    p.Closed = false;
                    options.OverlappedCount++;
                }
            }
        }
    }
}