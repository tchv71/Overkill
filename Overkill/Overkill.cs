#if NCAD
    using Teigha.DatabaseServices;
    using Teigha.Runtime;
    using HostMgd.ApplicationServices;
    using HostMgd.EditorInput;
    using Application = HostMgd.ApplicationServices.Application;
    using Platform = HostMgd;
    using PlatformDb = Teigha;
#else
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Platform = Autodesk.AutoCAD;
using PlatformDb = Autodesk.AutoCAD;
#endif

#if NCAD
#else 
[assembly: ExtensionApplication(null)]
[assembly: CommandClass(typeof(Overkill.Overkill))]
#endif

namespace Overkill
{
    using RTree;
    using System.Diagnostics;
    using System.Windows.Forms;
    using Microsoft.Win32;
    using Registry = Microsoft.Win32.Registry;
    using RegistryKey = Microsoft.Win32.RegistryKey;
    public class DbEntity
    {
        private DBObject _ptr;
        public DbEntity(DBObject ptr)
        {
            _ptr = ptr;
        }

        public DBObject Ptr { get { return _ptr; } }
       
    }

    public enum EOptions
    {
        EIgnoreColor = 0x1,
        EIgnoreLayer = 0x2,
        EIgnoreLinetype = 0x4,
        EIgnoreLinetypeScale = 0x8,
        EIgnoreLineweight = 0x10,
        EIgnoreThickness = 0x20,
        EIgnoreTransparency = 0x40,
        EIgnorePlotStyle = 0x80,
        EIgnoreMaterial = 0x100
    }
    public class Overkill
    {
        private RTree<DbEntity> _tree;
        // Точность сравнения примитивов
        public class Options
        {
            private const string OVERKILL_KEY =
                //@"SOFTWARE\Autodesk\AutoCAD\R21.0\ACAD-0001:409\Profiles\<<Unnamed Profile>>\Dialogs\Overkill";
#if NCAD
                @"\Profile\Dialogs\Overkill";
#else
                @"\Profiles\<<Unnamed Profile>>\Dialogs\Overkill";
#endif
            private const string OVERKILL_KEY_ALT =
                @"\Profiles\<<Профиль без имени>>\Dialogs\Overkill";
            public int IgnoreOptions;
            public Transaction Tr;
            // Количество удаленных дубликатов
            public int DupCount;
            // Количество удаленных перекрывающихся сегментов
            public int OverlappedCount;
            public double Tolerance = 0.000001;

            public bool bCombineEndToEnd;
            public bool bCombinePartialOverlaps;
            public bool bIgnorePolylineWidths;
            public bool bMaintainAssociativities;
            public bool bMaintainPolylines;
            public bool bOptimizePolylines;
            public string StrTolerance;

            public Options()
            {
                bMaintainAssociativities = true;
            }

            private RegistryKey GetRegKey(bool bWritable)
            {
#if NCAD
                string keyName = HostApplicationServices.Current.RegistryProductRootKey;
#else
                string keyName = HostApplicationServices.Current.UserRegistryProductRootKey;
#endif
                RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName+OVERKILL_KEY, bWritable);
                if (key == null)
                {
                    key = Registry.CurrentUser.OpenSubKey(keyName + OVERKILL_KEY_ALT, bWritable);
                }
                return key;
            }

            public void LoadValues()
            {
               RegistryKey key = GetRegKey(false);

                using (key)
                {
                    IgnoreOptions = (int)key.GetValue("Ignore");
                    bCombineEndToEnd = (int) key.GetValue("CombineEndToEnd") != 0;
                    bCombinePartialOverlaps = (int) key.GetValue("CombinePartialOverlaps") != 0;
                    bIgnorePolylineWidths = (int) key.GetValue("IgnorePolylineWidths") != 0;
                    bMaintainAssociativities = (int) key.GetValue("MaintainAssociativities") != 0;
                    bMaintainPolylines = (int) key.GetValue("MaintainPolylines") != 0;
                    bOptimizePolylines = (int) key.GetValue("OptimizePolylines") != 0;
                    StrTolerance = (string) key.GetValue("Tolerance");
                    //Tolerance = Double.Parse(_strTolerance.Replace(".", ","));
                }
            }
            public void SaveValues()
            {
                RegistryKey key = GetRegKey(true);

                using (key)
                {
                    key.SetValue("Ignore", IgnoreOptions, RegistryValueKind.DWord);
                    key.SetValue("CombineEndToEnd", bCombineEndToEnd ? 1:0, RegistryValueKind.DWord);
                    key.SetValue("CombinePartialOverlaps", bCombinePartialOverlaps ? 1:0, RegistryValueKind.DWord);
                    key.SetValue("IgnorePolylineWidths", bIgnorePolylineWidths ? 1 : 0, RegistryValueKind.DWord);
                    key.SetValue("MaintainAssociativities", bMaintainAssociativities ? 1 : 0,RegistryValueKind.DWord);
                    key.SetValue("MaintainPolylines", bMaintainPolylines ? 1 : 0, RegistryValueKind.DWord);
                    key.SetValue("OptimizePolylines", bOptimizePolylines ? 1 : 0, RegistryValueKind.DWord);
                    key.SetValue("Tolerance", StrTolerance,RegistryValueKind.String);
                }
            }
            public bool IgnoreColor   {get { return (IgnoreOptions & (int)EOptions.EIgnoreColor) !=0;}}
            public bool IgnoreLayer   {get { return (IgnoreOptions & (int) EOptions.EIgnoreLayer) != 0;}}
            public bool IgnoreLinetype  {get { return (IgnoreOptions & (int)EOptions.EIgnoreLinetype) != 0;}}
            public bool IgnoreLinetypeScale   {get { return  (IgnoreOptions & (int)EOptions.EIgnoreLinetypeScale) != 0;}}
            public bool IgnoreLineweight   {get { return  (IgnoreOptions & (int)EOptions.EIgnoreLineweight) != 0;}}
            public bool IgnoreMaterial   {get { return  (IgnoreOptions & (int)EOptions.EIgnoreMaterial) != 0;}}
            public bool IgnorePlotStyle   {get { return  (IgnoreOptions & (int)EOptions.EIgnorePlotStyle) != 0;}}
            public bool IgnoreThickness { get { return (IgnoreOptions & (int)EOptions.EIgnoreThickness) != 0; } }
            public bool IgnoreTransparency   {get { return  (IgnoreOptions & (int)EOptions.EIgnoreTransparency) != 0;}}

        }

        public static Options _options = new Options();
        [CommandMethod("OVERKILL2", CommandFlags.UsePickSet)]
        public void OverkillCmd()
        {
            Editor ed = Platform.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
            PromptSelectionResult res = ed.GetSelection();
            if (res.Status == PromptStatus.OK)
            {
                Database db = Platform.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
                PlatformDb.DatabaseServices.TransactionManager tm = db.TransactionManager;
                using (Transaction tr = tm.StartTransaction())
                {
                    using (OverkillForm frm = new OverkillForm(_options))
                    {
                        DialogResult dlgResult = Platform.ApplicationServices.Application.ShowModalDialog(frm);
                        if (dlgResult == DialogResult.Cancel)
                            return;
                    }
                    FillRTree(res, tr);
                    int nOverlapped;
                    int nDublicates;
                    do
                    {
                        nOverlapped = _options.OverlappedCount;
                        nDublicates = _options.DupCount;
                        ProcessObjects(res, tr);
                    } while (_options.OverlappedCount != nOverlapped || _options.DupCount != nDublicates); 
                    tr.Commit();
                }

                ed.WriteMessage("{0} duplicate(s) deleted\n", _options.DupCount);
                ed.WriteMessage("{0} overlapping object(s) or segment(s) deleted\n", _options.OverlappedCount);
            }
        }

        private void ProcessObjects(PromptSelectionResult res, Transaction tr)
        {
            _options.Tr = tr;
            foreach (var id in res.Value.GetObjectIds())
            {
                Entity obj = tr.GetObject(id, OpenMode.ForWrite) as Entity;
                if (obj.IsErased)
                    continue;
                IEntityProxy proxy = Util.MakeProxy(obj, _options, _tree);
                proxy.Process();
            }
        }



        // Заполнение R-дерева
        private void FillRTree(PromptSelectionResult res, Transaction tr)
        {
            _tree = new RTree<DbEntity>();
            _options.DupCount = 0;
            _options.OverlappedCount = 0;
            foreach (var id in res.Value.GetObjectIds())
            {
                DBObject obj = tr.GetObject(id, OpenMode.ForWrite);
                if (obj is Polyline)
                {
                    Polyline p = obj as Polyline;
                    Debug.Assert(p != null, "p != null");
                    for (int i = 0; i < p.NumberOfVertices - 1; i++)
                    {
                        _tree.Add(Util.GetRect(p.GetLineSegmentAt(i)), new DbEntity(p));
                    }
                }
                else if (obj is Line)
                {
                    Line l = obj as Line;
                    Debug.Assert(l != null, "l != null");
                    _tree.Add(Util.GetRect(l), new DbEntity(l));
                }
                else if (obj is Entity)
                {
                    Entity entity = obj as Entity;
                    Debug.Assert(entity != null, "br != null");
                    _tree.Add(Util.GetRect(entity.GeometricExtents), new DbEntity(entity));
                }
            }
        }

    }
}
