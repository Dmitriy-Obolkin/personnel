using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Threading;

namespace Personnel
{
    public partial class FormAdmin : Form
    {
        OleDbConnection con;
        OleDbCommand cmd;
        OleDbDataReader rd;
        private string pathFile = "personnel.mdb.crypt";
        string i;
        public FormAdmin(string i)
        {
            InitializeComponent();
            LoadData();
            this.i = i;
            if (i != String.Empty)
            {
                label1.Text = i;
            }
        }

        public void Crypt(string inputFilePath, string outputFilePath, bool CoderMode)
        {
            int bufferSize = 1024 * 64; //Создание буфера

            using (FileStream inStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            {

                using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {

                    int bytesRead = -1;
                    byte[] bytes = new byte[bufferSize];
                    string decodeString = "12345678";


                    double decodeInt = Convert.ToDouble(decodeString);
                    double l = Math.Round((decodeInt / 13998), 5);
                    double k = (l - (int)l) * 1000;
                    int ln = Convert.ToInt32((k - (int)k) * 100);

                    while ((bytesRead = inStream.Read(bytes, 0, bufferSize)) > 0)
                    {

                        int[] bytesAsInts = bytes.Select(x => (int)x).ToArray();

                        if (CoderMode == false)
                        {
                            for (int i = 0; i < bytesAsInts.Length; i++)
                            {
                                bytesAsInts[i] = ((bytesAsInts[i] - (ln * 2) - 128) * -1);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < bytesAsInts.Length; i++)
                            {
                                bytesAsInts[i] = ((bytesAsInts[i] * (-1) + ln * 2) + 128);
                            }
                        }


                        byte[] bytes1 = bytesAsInts.Select(x => (byte)x).ToArray();
                        fileStream.Write(bytes1, 0, bytesRead);
                        fileStream.Flush();
                    }
                    //MessageBox.Show("Декодування виконано!", "Повідомлення");
                    File.SetAttributes(@"personnel.mdb", FileAttributes.Hidden);
                    fileStream.Close();
                }
            }
        }

        private void LoadData()
        {
            string newPathFile = pathFile.Replace(".crypt", "");
            Crypt(pathFile, newPathFile, false);

            con = new OleDbConnection(@"Provider=Microsoft.ACE.Oledb.12.0;Data Source=personnel.mdb");
            cmd = new OleDbCommand();
            con.Open();
            cmd.Connection = con;
            string str = "Select * From userlist";
            cmd.CommandText = str;
            int c = 0;

            try
            {
                c = 0;
                rd = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                c = 1;
                MessageBox.Show(ex.Message, "Помилка доступу!");
            }

            
            if (c==0)
            {
                List<string[]> dt = new List<string[]>();

                while (rd.Read())
                {
                    dt.Add(new string[6]);

                    dt[dt.Count - 1][0] = rd[0].ToString();
                    dt[dt.Count - 1][1] = rd[1].ToString();
                    dt[dt.Count - 1][2] = rd[2].ToString();
                    dt[dt.Count - 1][3] = rd[3].ToString();
                    dt[dt.Count - 1][4] = rd[4].ToString();
                    dt[dt.Count - 1][5] = rd[5].ToString();
                }

                rd.Close();

                foreach (string[] s in dt)
                    dataGridView1.Rows.Add(s);

                con.Close();
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                File.Delete(@"personnel.mdb");
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormAdd fadd = new FormAdd();
            fadd.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString());
            FormEdit fe = new FormEdit(i);
            fe.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            LoadData();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 f1 = new Form1();
            f1.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            string newPathFile = pathFile.Replace(".crypt", "");
            Crypt(pathFile, newPathFile, false);

            int a = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString());

            con = new OleDbConnection(@"Provider=Microsoft.ACE.Oledb.12.0;Data Source=personnel.mdb");
            cmd = new OleDbCommand();
            con.Open();
            cmd.Connection = con;

            var result = new DialogResult();
            result = MessageBox.Show("Видалити запис?", "Увага!",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string str = "(DELETE FROM userlist WHERE ID=" + a + ")";
                cmd.CommandText = str;
                cmd.ExecuteReader();

                con.Close();
                Crypt(@"personnel.mdb", "personnel.mdb.crypt", false);
                File.Delete(@"personnel.mdb");
                Thread.Sleep(500);
                dataGridView1.Rows.Clear();
                LoadData();
            }
        }

        public int GetKey(string login, string pass)
        {
            string newPathFile = pathFile.Replace(".crypt", "");
            Crypt(pathFile, newPathFile, false);
            con = new OleDbConnection(@"Provider=Microsoft.ACE.Oledb.12.0;Data Source=personnel.mdb");
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter("Select key From userlist where login='" + login + "' and  pass='" + pass + "'", con);
            DataTable dt = new DataTable();
            dataAdapter.Fill(dt);
            // Проверяем, что количество строк из БД больше нуля
            if (dt.Rows.Count > 0)
            {
                string key = dt.Rows[0][0].ToString();
                if (key != String.Empty)
                {
                    return Convert.ToInt32(key);
                }
                else
                    return 0;
            }
            else
                return 0;
        }


        public string pass()
        {
            string p1 = Convert.ToString(label1.Text.ToString());
            return p1;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int i = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[5].Value.ToString());
            FormData fd = new FormData(i);
            fd.Show();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            FormPassChange pch = new FormPassChange(label1.Text);
            this.Hide();
            pch.Show();
        }
    }
}
