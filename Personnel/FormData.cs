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
    public partial class FormData : Form
    {
        OleDbConnection con;
        OleDbCommand cmd;
        OleDbDataReader rd;
        private string pathFile = "data.mdb.crypt";
        int i;
        public FormData(int i)
        {
            this.i = i;
            InitializeComponent();
            LoadData(i);
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
            string str = "Select * From data WHERE [key]=" + i + "";
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
                    dt.Add(new string[3]);

                    dt[dt.Count - 1][0] = rd[0].ToString();
                    dt[dt.Count - 1][1] = rd[1].ToString();
                    dt[dt.Count - 1][2] = rd[2].ToString();

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
            string name = (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value.ToString());
            int key = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[2].Value.ToString());

            string newPathFile = pathFile.Replace(".crypt", "");
            Crypt(pathFile, newPathFile, false);

            con = new OleDbConnection(@"Provider=Microsoft.ACE.Oledb.12.0;Data Source=data.mdb");
            cmd = new OleDbCommand();
            con.Open();
            cmd.Connection = con;
            string str = "Select name From data WHERE [key]=" + i + "";
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

                for (int j = 0; j< dt.Count; j++)
                    {
                        if (name == dt[j][0].ToString())
                        {
                            FormFiles ff = new FormFiles(name, key);
                            ff.Show();
                        }
                    }
                
            }
                

                
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);

                    txtpath.Text = fbd.SelectedPath.ToString();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int g = 0;
                if (txtpath.Text != String.Empty)
                {
                for (int j = 0; j<dataGridView1.RowCount-1; j++)
                {
                    if (txtpath.Text == dataGridView1.Rows[j].Cells[1].Value.ToString())
                    {
                        g = 1;
                        MessageBox.Show("Вказана директорія уже присутня!", "Помилка!");
                        break;
                    }
                }
                if (g == 0)
                {
                    string newPathFile = pathFile.Replace(".crypt", "");
                    Crypt(pathFile, newPathFile, false);

                    con = new OleDbConnection(@"Provider=Microsoft.ACE.Oledb.12.0;Data Source=data.mdb");
                    int c = 0;

                    try
                    {
                        c = 0;
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "INSERT INTO data (name, [key]) VALUES (@txtpath, [@txtkey])";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@txtpath", txtpath.Text);
                        cmd.Parameters.AddWithValue("@txtkey", i);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Помилка внесення запису!");
                        c = 1;
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                            Crypt(@"data.mdb", "data.mdb.crypt", false);
                            File.Delete(@"data.mdb");
                        }
                        if (c == 0)
                            MessageBox.Show("Успішно!", "Повідомлення!");
                    }
                    dataGridView1.Rows.Clear();
                    LoadData(i);
                }
            }
                else
                    MessageBox.Show("Неправильно вказана директорія!", "Помилка!");
        }


        private void button5_Click(object sender, EventArgs e)
        {
            string newPathFile = pathFile.Replace(".crypt", "");
            Crypt(pathFile, newPathFile, false);

            int a = Convert.ToInt32(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString());

            con = new OleDbConnection(@"Provider=Microsoft.ACE.Oledb.12.0;Data Source=data.mdb");
            cmd = new OleDbCommand();
            con.Open();
            cmd.Connection = con;

            var result = new DialogResult();
            result = MessageBox.Show("Видалити запис?", "Увага!",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                string str = "(DELETE FROM data WHERE ID=" + a + ")";
                cmd.CommandText = str;
                cmd.ExecuteReader();

                con.Close();
                Crypt(@"data.mdb", "data.mdb.crypt", false);
                File.Delete(@"data.mdb");
                Thread.Sleep(500);
                dataGridView1.Rows.Clear();
                LoadData(i);
            }
        }
    }
}
