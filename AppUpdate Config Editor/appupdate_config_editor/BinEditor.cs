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

namespace Versioner
{
    [Serializable]
    public partial class BinEditor : Form
    {
        // Initalizing variables
        private int version;
        private string url;
        private string launch;

        // Creative instances
        FileManager filemanager = new FileManager();
        JSON json = new JSON();
        Stream stream;

        // Class constructor
        public BinEditor()
        {
            InitializeComponent();
        }

        // Puts field values in to variables
        public bool saveVariables()
        {
            try
            {
                this.version = Convert.ToInt32(lblVERSION.Text);
                this.url = (string)lblURL.Text;
                this.launch = (string)lblLAUNCH.Text;
            }
            catch(FormatException){
                MessageBox.Show("ERR: You can only enter a number in to the version field", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            if (!saveVariables())
                return;

            saveFileDialog.Filter = "bin files (*.bin)|*.bin";
            saveFileDialog.FilterIndex = 1;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (filemanager.CreateConfig(json.Encode(url, launch, version), saveFileDialog.FileName.ToString()))
                {
                    lblLAUNCH.Text = "";
                    lblURL.Text = "";
                    lblVERSION.Text = "0";
                }
                else
                {
                    MessageBox.Show("ERR: There was a problem when saving.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "bin files (*.bin)|*.bin";
            openFileDialog.FilterIndex = 1;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string data = filemanager.ReadConfig(openFileDialog.FileName.ToString());
                if (!json.Decode(data))
                {
                    MessageBox.Show("ERR: The file could not be read.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    lblLAUNCH.Text = json.whenFinishedLaunch;
                    lblURL.Text = json.updateUrl;
                    lblVERSION.Text = Convert.ToString(json.updateVersion);
                }

            }
        }

        private void BinEditor_Load(object sender, EventArgs e)
        {

        }
    }
}
