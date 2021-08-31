using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Personnel
{
    public partial class FormAccept : Form
    {
        public FormAccept()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pw = textBox1.Text;
            FormAdmin fa = new FormAdmin("");
            string b = fa.pass();

            if (pw == b)
            {
                this.Hide();
                fa.Show();
            }
            else
            {
                MessageBox.Show("Неправильний пароль адміністратора", "Відмова!");
            }
        }
    }
}
