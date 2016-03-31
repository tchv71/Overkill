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
        protected readonly Overkill.Options Options;
        protected readonly RTree<DbEntity> Tree; 

        public EntityProxy(Entity ent, Overkill.Options opts, RTree<DbEntity> tree)
        {
            Ent = ent;
            Options = opts;
            Tree = tree;
        }

        // Удаление примитива из чертежа и дерева поиска
        protected void DelEntity(DbEntity ent, bool bDuplicate = true)
        {
            ent.Ptr.Erase();
            Tree.Delete(Util.GetRect(ent.Ptr as Entity), ent);
            if (bDuplicate)
                Options.DupCount++;
            else
                Options.OverlappedCount++;
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

                    if (Util.IsEqual(ent1, ent2))
                    {
                        DelEntity(ent);
                    }
                }
            }
        }

    }
}
