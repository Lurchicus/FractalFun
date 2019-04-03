using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

/// <summary>
/// 
/// </summary>
namespace FractalFun
{
    /// <summary>
    /// A specialized simple editor for editing the json file PredefinedAttractors.json.
    /// The editor should be reusabe as it loads a hard coded file, allows editing and 
    /// the ability to save the file. When the editor is exited (runs as a modal dialog)
    /// the file is reprocessed and used to load the dropdown in the Attractory form.
    /// </summary>
    public partial class EditPredef : Form
    {
        /// <summary>
        /// Initialize the form... nothing to see here... move along
        /// </summary>
        public EditPredef()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On load, open and load the PredefinedAttractors file
        /// </summary>
        /// <param name="sender">EditPrefs form</param>
        /// <param name="e">any parameters</param>
        private void EditPredef_Load(object sender, EventArgs e)
        {
            TxtEdit.Clear();
            // At some point in the future, this will get the filename from the 
            // parent form since hard coding is almost evil :)
            using (StreamReader r = new StreamReader("PredefinedAttractors.json"))
            {
                string json = r.ReadToEnd();
                TxtEdit.Text = json;
                TxtEdit.Select(0, 0);
                r.Close();
            }
        }

        /// <summary>
        /// Save whatever edits we may have been made... again with the hardcoded 
        /// filename
        /// </summary>
        /// <param name="sender">Save button</param>
        /// <param name="e">Params if any</param>
        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            string json = TxtEdit.Text;
            using (StreamWriter j = new StreamWriter("PredefinedAttractors.json", false))
            {
                j.Write(json);
                j.Flush();
                j.Close();
            }
        }
    }
}
