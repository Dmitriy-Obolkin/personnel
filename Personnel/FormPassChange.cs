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
    public partial class FormPassChange : Form
    {
        string i;
        public FormPassChange(string i)
        {
            InitializeComponent();
            this.i = i;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!passchange())
                MessageBox.Show("Неправильний поточний пароль!", "Помилка!");
            else
            {
                Random rnd = new Random();
                string s = "";
                for (int j=0; j<10; j++)
                {
                    s+= rnd.Next(0,9);
                }
                FormAdmin a = new FormAdmin(s);
                this.Hide();
                MessageBox.Show("Пароль адміністротора успішно змінено! \n Новий пароль: " + s + "", "Повідомлення!");
                a.Show();
            }
        }
        public bool passchange ()
        {

            if (textBox1.Text == i)
            {
                return true;
            }
            else
                return false;

        }
        private void FormPassChange_FormClosed(object sender, FormClosingEventArgs e)
        {
            string s = "123";
            FormAdmin a = new FormAdmin(s);
        }
    }
}
