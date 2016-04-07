#if NCAD
using Teigha.DatabaseServices;
using Teigha.Geometry;
#else
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
# endif
using RTree;
using System;

namespace Overkill
{
    public static class Util
    {
        public static IEntityProxy MakeProxy(Entity ent, Overkill.Options opts, RTree<DbEntity> tree)
        {
            if (ent is Line)
                return new LineProxy((Line) ent, opts, tree);
            if (ent is Polyline)
                return (ent as Polyline).Closed ?
                    new ClosedPolylineProxy((Polyline) ent, opts, tree) : 
                    new PolylineProxy((Polyline) ent, opts, tree);
            return new EntityProxy(ent, opts, tree);
        }
            
        // Проверка двух примитивов на идентичность
        public static bool IsEqual(Entity ent1, Entity ent2, Overkill.Options options, out bool bDelFirst)
        {
            ResultBuffer rb1 = AcadLib.ObjectARX._acdbEntGet(new UIntPtr((ulong)ent1.Id.OldIdPtr.ToInt64()));
            ResultBuffer rb2 = AcadLib.ObjectARX._acdbEntGet(new UIntPtr((ulong)ent2.Id.OldIdPtr.ToInt64()));
            TypedValue[] arr1 = rb1.AsArray();
            TypedValue[] arr2 = rb2.AsArray();
            long h1 = ent1.Handle.Value;
            long h2 = ent2.Handle.Value;
            bDelFirst = false;
            if (arr1.Length != arr2.Length)
            {
                bDelFirst = true;
                if (arr1.Length > arr2.Length)
                {
                    TypedValue[] tmp = arr1;
                    arr1 = arr2;
                    arr2 = tmp;
                    bDelFirst = false;
                }
                for (int i = 0; i < arr2.Length; i++)
                {
                    if (arr2[i].TypeCode == (short)DxfCode.ControlString)
                    {
                        int n = 0;
                        for (int j = i + 1; j<arr2.Length; j++)
                        {
                            if (arr2[j].TypeCode == (short)DxfCode.ControlString)
                            {
                                if (++n != 3)
                                    continue;
                                TypedValue[] arr2Copy = arr2;
                                arr2 = new TypedValue[arr2.Length-(j-i+1)];
                                for (int k=0; k<i; k++)
                                    arr2[k] = arr2Copy[k];
                                for (int k = j + 1; k < arr2Copy.Length; k++)
                                    arr2[k-j + i-1] = arr2Copy[k];
                                i = j;
                                break;
                            }
                        }
                    }
                }
                //if (arr1.Length != arr2.Length)
                //    return false;
            }
            int index1 = 1;
            int index2 = 1; 
            for ( ; index1 < arr1.Length && index2<arr2.Length; index1++, index2++)
            {
                TypedValue v1 = arr1[index1];
                TypedValue v2 = arr2[index2];
                if (v1.TypeCode == (short)DxfCode.Handle ||
                    v1.TypeCode ==(short)DxfCode.AttributeTag ||
                    v1.TypeCode == (short)DxfCode.SoftPointerId)
                    continue;
                CheckOption(options.IgnoreColor, ref v1, arr1, ref index1, DxfCode.Color);
                CheckOption(options.IgnoreColor, ref v2, arr2, ref index2, DxfCode.Color);
                CheckOption(options.IgnoreLayer, ref v1, arr1, ref index1, DxfCode.LayerName);
                CheckOption(options.IgnoreLayer, ref v2, arr2, ref index2, DxfCode.LayerName);
                CheckOption(options.IgnoreLinetype, ref v1, arr1, ref index1, DxfCode.LinetypeName);
                CheckOption(options.IgnoreLinetype, ref v2, arr2, ref index2, DxfCode.LinetypeName);
                CheckOption(options.IgnoreLinetypeScale, ref v1, arr1, ref index1, DxfCode.LinetypeScale);
                CheckOption(options.IgnoreLinetypeScale, ref v2, arr2, ref index2, DxfCode.LinetypeScale);
                CheckOption(options.IgnoreLineweight, ref v1, arr1, ref index1, DxfCode.LineWeight);
                CheckOption(options.IgnoreLineweight, ref v2, arr2, ref index2, DxfCode.LineWeight);
                CheckOption(options.IgnoreMaterial, ref v1, arr1, ref index1, (DxfCode)347);
                CheckOption(options.IgnoreMaterial, ref v2, arr2, ref index2, (DxfCode)347);
                CheckOption(options.IgnorePlotStyle, ref v1, arr1, ref index1, DxfCode.PlotStyleNameId);
                CheckOption(options.IgnorePlotStyle, ref v2, arr2, ref index2, DxfCode.PlotStyleNameId);
                CheckOption(options.IgnoreThickness, ref v1, arr1, ref index1, DxfCode.Thickness);
                CheckOption(options.IgnoreThickness, ref v2, arr2, ref index2, DxfCode.Thickness);
                CheckOption(options.IgnoreTransparency, ref v1, arr1, ref index1, DxfCode.Alpha);
                CheckOption(options.IgnoreTransparency, ref v2, arr2, ref index2, DxfCode.Alpha);

                if (!CompareTypedValue(v1, v2, Overkill._options.Tolerance))
                {
                    // В случае линии проверка совпадений начало 1-й - конец 2-й и наоборот
                    if (ent1 is Line && v1.TypeCode == 10)
                    {
                        TypedValue v = new TypedValue(10, arr2[index2 + 1].Value);
                        if (!CompareTypedValue(v, v1, Overkill._options.Tolerance))
                            return false;
                        v = new TypedValue(10, arr1[index1+1].Value);
                        if (!CompareTypedValue(v, v2, Overkill._options.Tolerance))
                            return false;
                        index1++;
                        index2++;
                        continue;
                    }
                    return false;
                }
            }
            if (index1!=arr1.Length || index2!=arr2.Length)
                return false;
             return true;
        }

        private static bool CompareTypedValue(TypedValue v1, TypedValue v2, double tolerance)
        {
            if (v1.TypeCode != v2.TypeCode)
                return false;
            if (v1.Value is int)
                return (int) v1.Value == (int)v2.Value;
            if (v1.Value is short)
                return (short) v1.Value == (short) v2.Value;
            if (v1.Value is double)
                return Math.Abs((double) v1.Value - (double) v2.Value) < tolerance;
            if (v1.Value is Point3d)
                return ((Point3d) v1.Value).DistanceTo((Point3d) v2.Value) < tolerance;
            if (v1.Value is string)
                return (string) v1.Value == (string) v2.Value;
            return false;
        }

        private static void CheckOption(bool option, ref TypedValue v, TypedValue[] arr, ref int index, DxfCode dxfCode)
        {
            if (option && v.TypeCode == (short) dxfCode)
                v = arr[++index];
        }

        // Проверка двух линий на идентичность
        public static bool IsEqual(Line l1, Line l2, double tolerance)
        {
            return l1.StartPoint.DistanceTo(l2.StartPoint) < tolerance &&
                   l1.EndPoint.DistanceTo(l2.StartPoint) < tolerance ||
                   l1.StartPoint.DistanceTo(l2.EndPoint) < tolerance &&
                   l1.EndPoint.DistanceTo(l2.StartPoint) < tolerance;
        }

        private static Rectangle GetRect(Point3d p1, Point3d p2)
        {
            return new Rectangle((float)p1.X-(float)Overkill._options.Tolerance, (float)p1.Y - (float)Overkill._options.Tolerance, (float)p2.X + (float)Overkill._options.Tolerance,
                (float)p2.Y + (float)Overkill._options.Tolerance, (float)p1.Z - (float)Overkill._options.Tolerance, (float)p2.Z + (float)Overkill._options.Tolerance);
        }

        public static Rectangle GetRect(Line l1)
        {
            return GetRect(l1.StartPoint, l1.EndPoint);
        }

        public static Rectangle GetRect(LineSegment3d l1)
        {
            return GetRect(l1.StartPoint, l1.EndPoint);
        }

        public static Rectangle GetRect(Extents3d ext)
        {
            return GetRect(ext.MinPoint, ext.MaxPoint);
        }

        public static Rectangle GetRect(Entity ent)
        {
            return GetRect(ent.GeometricExtents);
        }

        public static bool AreLinesParrallelAndItersects(Line l1, Line l2, double tolerance)
        {
            Vector3d vecL1 = l1.StartPoint.GetVectorTo(l1.EndPoint);
            Vector3d vecL2 = l2.StartPoint.GetVectorTo(l2.EndPoint);

            if (!vecL1.IsParallelTo(vecL2, new Tolerance(tolerance, tolerance)))
                return false;
            return IsPointLiesOnLine(l1, l2.StartPoint, tolerance) || IsPointLiesOnLine(l1, l2.EndPoint, tolerance);
        }

        // Проверка, лежит ли точка на отрезке
        public static bool IsPointLiesOnLine(Curve l1, Point3d startPoint, double tolerance)
        {
            Point3d p = l1.GetClosestPointTo(startPoint, false);
            return p.DistanceTo(startPoint) < tolerance;
        }

        public static bool IsPointLiesOnLine(Curve3d l1, Point3d startPoint, double tolerance)
        {
            return l1.GetDistanceTo(startPoint) < tolerance;
        }

    }
}
