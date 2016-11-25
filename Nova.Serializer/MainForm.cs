using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Nova.Serializer.Models;
using Nova.Serializer.Properties;

namespace Nova.Serializer
{
    public partial class MainForm : Form
    {
        private string _assemblyFile;
        private TypeModel _selectedType;

        public MainForm()
        {
            InitializeComponent();
            chkCollection.Checked = true;
        }

        public string AssemblyFile
        {
            get { return _assemblyFile; }
            set
            {
                txtAssembly.Text = value ?? "<not selected>";
                if (_assemblyFile == value)
                    return;

                _assemblyFile = value;
                PopulateTypes();
            }
        }

        public TypeModel SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (_selectedType == value)
                    return;

                _selectedType = value;
                UpdatePropertyGrid();
            }
        }
        
        private void UpdatePropertyGrid()
        {
            var instance = Activator.CreateInstance(SelectedType.Type);
            propertyGrid1.SelectedObject = instance;
        }
        
        private void btnSerialize_Click(object sender, EventArgs e)
        {
            // var type = GetInstanceType();
            var type = chkCollection.Checked ? typeof(ListModel<>).MakeGenericType(SelectedType.Type) : SelectedType.Type;
            
            var instance = chkCollection.Checked
                ? Activator.CreateInstance(type, propertyGrid1.SelectedObject)
                : propertyGrid1.SelectedObject;
            
            var serializer = new XmlSerializer(type);
            var sb = new StringBuilder();
            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, instance);
            }
            rtbSerialized.Text = sb.ToString();
        }

        private void PopulateTypes()
        {
            errorProvider1.Clear();
            cboType.Items.Clear();
            if (string.IsNullOrEmpty(AssemblyFile))
                return;

            try
            {
                var asm = Assembly.LoadFrom(AssemblyFile);
                var types = asm.DefinedTypes;
                foreach (var type in types)
                {
                    var model = new TypeModel(type);
                    cboType.Items.Add(model);
                }

                if (cboType.Items.Count == 0)
                    throw new TypeLoadException("No types found in selected assembly");

                cboType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                errorProvider1.SetError(cboType, ex.Message);
            }
        }

        private void btnAssemblyBrowse_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog()
            {
                Filter = Resources.AssemblyFilter,
                Title = Resources.Locate_Assembly,
                Multiselect = false,
            };
            var result = dlg.ShowDialog(this);
            if (result != DialogResult.OK)
                return;

            AssemblyFile = dlg.FileName;
        }

        private void cboType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedType = cboType.SelectedItem as TypeModel;
        }

    }
}