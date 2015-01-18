using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NetworkResponse
{
    public partial class OptionsForm : Form
    {
        public OptionsForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            numericUpDownFrequency.Value = Properties.Settings.Default.CheckFrequency;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.CheckFrequency = (int)numericUpDownFrequency.Value;
            Properties.Settings.Default.Save();

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }
    }
}
