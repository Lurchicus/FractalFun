using System;
using System.Windows.Forms;
using System.IO;

/// <summary>
/// 
/// </summary>
namespace FractalFun
{
    /// <summary>
    /// A specialized simple editor for editing the json file PredefinedAttractors.json.
    /// The editor should be reusabe as it loads a file passed in the constructor... 
    /// this allows editing and the ability to save the file. 
    /// When the editor is exited (runs as a modal dialog)
    /// the file is reprocessed and used to load the dropdown in the Attractory form.
    /// </summary>
    public partial class EditPredef : Form
    {
        private readonly string PredefineFile;
        /// <summary>
        /// Initialize the form... nothing to see here... move along
        /// </summary>
        public EditPredef(string PredefFile)
        {
            InitializeComponent();
            PredefineFile = PredefFile;
        }

        /// <summary>
        /// On load, open and load the PredefinedAttractors file
        /// </summary>
        /// <param name="sender">EditPrefs form</param>
        /// <param name="e">any parameters</param>
        private void EditPredef_Load(object sender, EventArgs e)
        {
            TxtEdit.Clear();
            using (StreamReader r = new StreamReader(PredefineFile))
            {
                string json = r.ReadToEnd();
                TxtEdit.Text = json;
                TxtEdit.Select(0, 0);
                r.Close();
            }
        }

        /// <summary>
        /// Save whatever edits we may have been made 
        /// filename
        /// </summary>
        /// <param name="sender">Save button</param>
        /// <param name="e">Params if any</param>
        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            string json = TxtEdit.Text;
            using (StreamWriter j = new StreamWriter(PredefineFile, false))
            {
                j.Write(json);
                j.Flush();
                j.Close();
            }
        }
    }
}
