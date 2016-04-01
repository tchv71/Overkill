using System;
using Autodesk.AutoCAD.DatabaseServices;
using RTree;

namespace Overkill
{
    public interface IEntityProxy
    {
        void Process();
    }

    public class EntityProxy : IEntityProxy
    {
        protected readonly Entity Ent;
        protected readonly Overkill.Options options;
        protected readonly RTree<DbEntity> Tree; 

        public EntityProxy(Entity ent, Overkill.Options opts, RTree<DbEntity> tree)
        {
            Ent = ent;
            options = opts;
            Tree = tree;
        }

        // Удаление примитива из чертежа и дерева поиска
        protected void DelEntity(DbEntity ent, bool bDuplicate = true)
        {
            ent.Ptr.Erase();
            try
            {
                Tree.Delete(Util.GetRect(ent.Ptr as Entity), ent);
            }
            catch (System.Exception)
            {
                // ignored
            }
            if (bDuplicate)
                options.DupCount++;
            else
                options.OverlappedCount++;
        }

        public virtual void Process()
        {
            Entity ent1 = Ent;
            var list = Tree.Intersects(Util.GetRect(ent1.GeometricExtents));
            foreach (var ent in list)
            {
                if (ent.Ptr == ent1 || ent.Ptr.IsErased)
                    continue;
                if (ent1.GetType() == ent.Ptr.GetType())
                {
                    Entity ent2 = ent.Ptr as Entity;
                    bool bDelFirst;
                    if (Util.IsEqual(ent1, ent2, options, out bDelFirst))
                    {
                        DelEntity(bDelFirst ? new DbEntity(ent1) : ent);
                    }
                }
            }
        }

        protected bool CheckUnequal(Entity ent1, Entity ent2)
        {
            bool bResult = (!options.IgnoreLayer && ent1.Layer != ent2.Layer) ||
                    (!options.IgnoreLinetype && ent1.Linetype != ent2.Linetype) ||
                    (!options.IgnoreLinetypeScale &&
                     Math.Abs(ent1.LinetypeScale - ent2.LinetypeScale) > options.Tolerance) ||
                    (!options.IgnoreMaterial && ent1.Material != ent2.Material) ||
                    (!options.IgnorePlotStyle && ent1.PlotStyleNameId != ent2.PlotStyleNameId) ||
                    (!options.IgnoreColor && ent1.Color != ent2.Color) ||
                    (!options.IgnoreTransparency && ent1.Transparency != ent2.Transparency) ||
                    (!options.IgnoreLineweight && ent1.LineWeight != ent2.LineWeight);
            Polyline p1 = ent1 as Polyline;
            Line l1 = ent2 as Line;
            if (p1 != null && l1 != null)
                bResult = bResult || (!options.IgnoreThickness && Math.Abs(p1.Thickness - l1.Thickness) > options.Tolerance);
            return bResult;
        }

    }
}
