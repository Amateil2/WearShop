using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WearShop
{
    public partial class RepairAndImport : Form
    {
        private bool IsAdmin;
        public RepairAndImport(bool IsAdmin)
        {
            InitializeComponent();
            this.IsAdmin = IsAdmin;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (IsAdmin)
            {
                Administrator am = new Administrator();
                am.Show();
                this.Hide();
            }
            else
            {
                Authorization authorization = new Authorization();
                this.Close();
                authorization.Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RepairBase RepairBase = new RepairBase(IsAdmin);
            this.Close();
            RepairBase.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ImportData ImportData = new ImportData(IsAdmin);
            this.Close();
            ImportData.Show();
        }
    }
}
