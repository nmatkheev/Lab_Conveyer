using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab_Conveyer
{
    public partial class Choice : Form
    {
        public Choice()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Form1 frm = new Form1();
                frm.Show();
                //this.FindForm().Hide();
            }

            if (radioButton2.Checked)
            {
                NetworkForm frm = new NetworkForm();
                frm.Show();
                //this.FindForm().Hide();
            }
        }
    }
}
