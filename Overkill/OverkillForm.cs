using System;
using System.Windows.Forms;

namespace Overkill
{
    public partial class OverkillForm : Form
    {

        private readonly Overkill.Options _options;
        private bool _bDontClose; 
        public OverkillForm(Overkill.Options opts)
        {
            _options = opts;
            InitializeComponent();
        }

        private void OverkillForm_Load(object sender, EventArgs e)
        {
                 _options.LoadValues();
                cbCombineEndToEnd.Checked = _options.bCombineEndToEnd;
                cbCombileOverlappings.Checked = _options.bCombinePartialOverlaps;
                cbIgnorePolylineWidths.Checked = _options.bIgnorePolylineWidths;
                cbMaintainAssoc.Checked = _options.bMaintainAssociativities;
                cbDontBreakPolylines.Checked = _options.bMaintainPolylines;
                cbOptimizePolylines.Checked = _options.bOptimizePolylines;
                tbTolerance.Text = _options.StrTolerance;
                cbColor.Checked = _options.IgnoreColor;
                cbLayer.Checked = _options.IgnoreLayer;
                cbLinetype.Checked = _options.IgnoreLinetype;
                cbLinetypeScale.Checked = _options.IgnoreLinetypeScale;
                cbLineweight.Checked = _options.IgnoreLineweight;
                cbMaterial.Checked = _options.IgnoreMaterial;
                cbPlotStyle.Checked = _options.IgnorePlotStyle;
                cbThickness.Checked = _options.IgnoreThickness;
                cbTransparency.Checked = _options.IgnoreTransparency;

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                double val = double.Parse(tbTolerance.Text.Replace(".",","));
                if (val < 0) throw new Exception();
                _options.Tolerance = val;
                _options.StrTolerance = tbTolerance.Text;
            }
            catch (Exception)
            {
                MessageBox.Show("The tolerance value must be numeric and nonnegative", "Autocad", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                _bDontClose = true;
                return;
            }
            _options.IgnoreOptions =
                (cbColor.Checked ? (int) EOptions.EIgnoreColor : 0) |
                (cbLayer.Checked ? (int) EOptions.EIgnoreLayer : 0) |
                (cbLinetype.Checked ? (int) EOptions.EIgnoreLinetype : 0) |
                (cbLinetypeScale.Checked ? (int) EOptions.EIgnoreLinetypeScale : 0) |
                (cbLineweight.Checked ? (int) EOptions.EIgnoreLineweight : 0) |
                (cbMaterial.Checked ?(int) EOptions.EIgnoreMaterial:0)|
                (cbLineweight.Checked ? (int) EOptions.EIgnoreLineweight : 0) |
                (cbLineweight.Checked ? (int) EOptions.EIgnoreLineweight : 0) |
                (cbPlotStyle.Checked ? (int) EOptions.EIgnorePlotStyle : 0) |
                (cbThickness.Checked ? (int) EOptions.EIgnoreThickness : 0) |
                (cbTransparency.Checked ? (int) EOptions.EIgnoreTransparency : 0);
            _options.bIgnorePolylineWidths = cbIgnorePolylineWidths.Checked;
            _options.bMaintainAssociativities = cbMaintainAssoc.Checked;
            _options.bMaintainPolylines = cbDontBreakPolylines.Checked;
            _options.bIgnorePolylineWidths = cbIgnorePolylineWidths.Checked;
            _options.bOptimizePolylines = cbOptimizePolylines.Checked;
            _options.bCombineEndToEnd = cbCombileOverlappings.Checked;
            _options.SaveValues();
        }

        private void OverkillForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_bDontClose)
            { 
                e.Cancel = true;
                _bDontClose = false;
            }
        }
    }
}
