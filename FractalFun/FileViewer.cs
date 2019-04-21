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
    public partial class FileViewer : Form
    {
        private string ViewerFile = "";
        /// <summary>
        /// Initialize the form... nothing to see here... move along
        /// </summary>
        public FileViewer(string FileToView)
        {
            InitializeComponent();
            ViewerFile = FileToView;
        }

        private void FileViewer_Load(object sender, EventArgs e)
        {
            TxtView.Clear();
            using (StreamReader r = new StreamReader(ViewerFile ))
            {
                string GNU3 = r.ReadToEnd();
                TxtView.Text = GNU3;
                TxtView.Select(0, 0); // Unselect all
                r.Close();
            }
        }
    }
}
