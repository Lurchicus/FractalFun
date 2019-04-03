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

namespace FractalFun
{
    public partial class EditPredef : Form
    {
        public EditPredef()
        {
            InitializeComponent();
        }

        private void EditPredef_Load(object sender, EventArgs e)
        {
            TxtEdit.Clear();
            using (StreamReader r = new StreamReader("PredefinedAttractors.json"))
            {
                string json = r.ReadToEnd();
                TxtEdit.Text = json;
                TxtEdit.Select(0, 0);
                r.Close();
            }
        }

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
