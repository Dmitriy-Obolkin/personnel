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
    public partial class FormAdd : Form
    {
        OleDbConnection con;
        OleDbCommand cmd;
        OleDbDataReader rd;
        private string pathFile = "personnel.mdb.crypt";
        public FormAdd()
        {
            InitializeComponent();
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

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtkey.Text.Length == 8)
            {
                if (txtlogin.Text != String.Empty && txtpass.Text != String.Empty && txtpib.Text != String.Empty)
                {
                    string newPathFile = pathFile.Replace(".crypt", "");
                    Crypt(pathFile, newPathFile, false);

                    con = new OleDbConnection(@"Provider=Microsoft.ACE.Oledb.12.0;Data Source=personnel.mdb");
                    int c = 0;

                    try
                    {
                        c = 0;
                        OleDbCommand cmd = new OleDbCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "INSERT INTO userlist ( login, pass, PIB, access, [key]) VALUES (@txtlogin, @txtpass, @txtpib, @comboBox1, [@txtkey])";
                        cmd.Connection = con;
                        cmd.Parameters.AddWithValue("@txtlogin", txtlogin.Text);
                        cmd.Parameters.AddWithValue("@txtpass", txtpass.Text);
                        cmd.Parameters.AddWithValue("@txtpib", txtpib.Text);
                        cmd.Parameters.AddWithValue("@comboBox1", comboBox1.Text);
                        cmd.Parameters.AddWithValue("@txtkey", txtkey.Text);

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
                            Crypt(@"personnel.mdb", "personnel.mdb.crypt", false);
                            File.Delete(@"personnel.mdb");
                        }
                        if (c == 0)
                            MessageBox.Show("Користувач " + txtpib.Text + " успішно внесений!", "Повідомлення!");
                    }
                }
                else
                    MessageBox.Show("Заповніть всі поля!", "Відмова!");
            }
            else
                MessageBox.Show("Персональний ключ повинен мати 8 символів!", "Увага!"); 
        }
    }
}
