using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DHCPv6
{
    public partial class eIDForm : Form
    {
        public eIDForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
           
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
           
        }

        private void picBtn_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
            {
                MessageBox.Show("请输入姓名");
                return;
            }
            if (txtNo.Text.Trim() == "")
            {
                MessageBox.Show("请输入身份证号");
                return;
            }
            if (txtPIN.Text.Trim() == "")
            {
                MessageBox.Show("请输入PIN码");
                return;
            }

            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void ptnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            this.Close();
        }
 
    }
}
