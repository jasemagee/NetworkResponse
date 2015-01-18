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
    public partial class MainForm : Form
    {
        private BindingList<NetworkTarget> mNetworkTargets;
        private System.Threading.Timer mTimer;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {

                mNetworkTargets = new BindingList<NetworkTarget>(NetworkTarget.GetNetworkTargetsFromProperties());

                dataGridView.DataSource = mNetworkTargets;

                mTimer = new System.Threading.Timer(CheckTargets, null, 0, 
                    (int)TimeSpan.FromSeconds(Properties.Settings.Default.CheckFrequency).TotalMilliseconds);
            }
        }

        private void CheckTargets(object state)
        {
            BeginInvoke(new Action(() => toolStripStatusLabelStatus.Text = "In Progress..."), null);

            double progress = 0;
            for (int i = 0; i < mNetworkTargets.Count; i++)
            {
                var target = mNetworkTargets[i];
                target.Update();
            

                double amt = (double)(i + 1) / mNetworkTargets.Count;
                progress = amt * 100;
                BeginInvoke(new Action(() => toolStripProgressBar.Value = (int)progress), null);
            }

            BeginInvoke(new Action(() => toolStripStatusLabelStatus.Text = ""), null);
            BeginInvoke(new Action(() => toolStripProgressBar.Value = 0), null);
            BeginInvoke(new Action(() => dataGridView.Refresh()), null);
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 1)
                return;

            var selected = (NetworkTarget)dataGridView.SelectedRows[0].DataBoundItem;

            textBoxName.Text = selected.Name;
            textBoxTarget.Text = selected.Target;   
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 1)
                return;

            var selected = (NetworkTarget)dataGridView.SelectedRows[0].DataBoundItem;

            selected.Name = textBoxName.Text;
            selected.Target = textBoxTarget.Text;
            dataGridView.Refresh();

            NetworkTarget.SaveNetworkTargetsToProperties(mNetworkTargets.ToList());
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            var newItem = new NetworkTarget();
            mNetworkTargets.Add(newItem);
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.DataBoundItem.Equals(newItem))
                {
                    row.Selected = true;
                }
            }
   
            NetworkTarget.SaveNetworkTargetsToProperties(mNetworkTargets.ToList());
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 1)
                return;

            var selected = (NetworkTarget)dataGridView.SelectedRows[0].DataBoundItem;
            mNetworkTargets.Remove(selected);

            NetworkTarget.SaveNetworkTargetsToProperties(mNetworkTargets.ToList());
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Normal;
            Show();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(500);
                Hide();
            } else if (FormWindowState.Normal == this.WindowState)
            {
                notifyIcon.Visible = false;
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var frm = new OptionsForm())
            {      
                var query= frm.ShowDialog(this);
                if (query == System.Windows.Forms.DialogResult.OK)
                {
                    int newSeconds = Properties.Settings.Default.CheckFrequency;
                    mTimer.Change(0, (int)TimeSpan.FromSeconds(newSeconds).TotalMilliseconds);
                }
            }
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
