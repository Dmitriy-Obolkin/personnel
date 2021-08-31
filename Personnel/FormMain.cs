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
    public partial class FormMain : Form
    {
        OleDbConnection con;
        OleDbCommand cmd;
        OleDbDataReader rd;
        private string pathFile = "data.mdb.crypt";

        string login;
        string pass;
        int key;
        public FormMain(string login, string pass)
        {
            InitializeComponent();
            this.login = login;
            this.pass = pass;
            FormAdmin fa = new FormAdmin("");
            key = fa.GetKey(login, pass);
            label2.Text = key.ToString();
            LoadData(key);
        }

        private void Crypt(string inputFilePath, string outputFilePath, bool CoderMode)
        {
            int bufferSize = 1024 * 64; //Создание буфера

            using (FileStream inStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            {

                using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {

                    int bytesRead = -1;
                    byte[] bytes = new byte[bufferSize];
                    string decodeString = "87654321";


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
                    File.SetAttributes(@"data.mdb", FileAttributes.Hidden);
                    fileStream.Close();
                }
            }
        }

        private void LoadData(int i)
        {
            string newPathFile = pathFile.Replace(".crypt", "");
            Crypt(pathFile, newPathFile, false);

            con = new OleDbConnection(@"Provider=Microsoft.ACE.Oledb.12.0;Data Source=data.mdb");
            cmd = new OleDbCommand();
            con.Open();
            cmd.Connection = con;
            string str = "Select ID, name From data WHERE [key]=" + i + "";
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


            if (c == 0)
            {
                List<string[]> dt = new List<string[]>();

                while (rd.Read())
                {
                    dt.Add(new string[2]);

                    dt[dt.Count - 1][0] = rd[0].ToString();
                    dt[dt.Count - 1][1] = rd[1].ToString();

                }

                rd.Close();

                foreach (string[] s in dt)
                    dataGridView1.Rows.Add(s);

                con.Close();
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                File.Delete(@"data.mdb");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string k = (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value.ToString());
            // FormUFiles fuf = new FormUFiles(k, key);
            //fuf.Show();

            string name = (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value.ToString());
            string newPathFile = pathFile.Replace(".crypt", "");
            Crypt(pathFile, newPathFile, false);

            con = new OleDbConnection(@"Provider=Microsoft.ACE.Oledb.12.0;Data Source=data.mdb");
            cmd = new OleDbCommand();
            con.Open();
            cmd.Connection = con;
            string str = "Select name From data WHERE [key]=" + key + "";
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

            if (c == 0)
            {
                List<string[]> dt = new List<string[]>();

                while (rd.Read())
                {
                    dt.Add(new string[1]);

                    dt[dt.Count - 1][0] = rd[0].ToString();
                }

                rd.Close();
                con.Close();
                File.Delete(@"data.mdb");

                for (int j = 0; j < dt.Count; j++)
                {
                    if (name == dt[j][0].ToString())
                    {
                        //MessageBox.Show("Доступ є!", "П");
                        string k = (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value.ToString());
                        FormFiles ff = new FormFiles(k, key);
                        ff.Show();
                    }
                }

            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 f1 = new Form1();
            f1.Show();
        }
    }
}
