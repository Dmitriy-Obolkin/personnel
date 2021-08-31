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
using Microsoft.Win32;

namespace Personnel
{
    public partial class FormEdit : Form
    {
        OleDbConnection con;
        OleDbCommand cmd;
        OleDbDataReader rd;
        private string pathFile = "personnel.mdb.crypt";
        int i;

        public FormEdit(int i)
        {
            InitializeComponent();
            LoadData();
            Load();
            this.i = i;
            txtkey.MaxLength = 8;
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
            finally
            {
                //File.Delete(@"personnel.mdb");
            }


            if (c == 0)
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

        public void Load()
        {
            txtlogin.Text = dataGridView1[1, 0].Value.ToString();
            txtpass.Text = dataGridView1[2, 0].Value.ToString();
            txtpib.Text = dataGridView1[3, 0].Value.ToString();
            txtkey.Text = dataGridView1[5, 0].Value.ToString();
            comboBox1.Text = dataGridView1[4, 0].Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtkey.Text.Length == 8)
            {
                if (txtlogin.Text != String.Empty && txtpass.Text != String.Empty && txtpib.Text != String.Empty)
                {
                    string newPathFile = pathFile.Replace(".crypt", "");
                    Crypt(pathFile, newPathFile, false);

                    var result = new DialogResult();
                    result = MessageBox.Show("Підтвердити редагування?", "Увага!",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        string login = txtlogin.Text;
                        string pass = txtpass.Text;
                        string pib = txtpib.Text;
                        int key = Convert.ToInt32(txtkey.Text);
                        string access = comboBox1.Text;
                        con = new OleDbConnection(@"Provider=Microsoft.ACE.Oledb.12.0;Data Source=personnel.mdb");
                        cmd = new OleDbCommand();
                        con.Open();
                        cmd.Connection = con;


                        int c = 0;
                        try
                        {
                            c = 0;
                            string str = "(UPDATE userlist SET login='" + login + "', pass='" + pass + "', PIB='" + pib + "', access='" + access + "', [key]=" + key + "  WHERE [ID]=" + i + ")";
                            cmd.CommandText = str;

                            cmd.ExecuteReader();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Помилка редагування запису!");
                            c = 1;
                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                                Crypt(@"personnel.mdb", "personnel.mdb.crypt", false);
                                File.Delete(@"personnel.mdb");
                            }
                            if (c == 0)
                                MessageBox.Show("Успішно збережено!", "Повідомлення!");
                        }

                        Thread.Sleep(500);
                        dataGridView1.Rows.Clear();
                        LoadData();
                        Load();
                    }
                }
                else
                    MessageBox.Show("Заповніть всі поля!", "Відмова!");
            }
            else
                MessageBox.Show("Персональний ключ повинен мати 8 символів!", "Увага!");
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtkey_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtpib_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtpass_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtlogin_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
