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

namespace Personnel
{
    public partial class Form1 : Form
    {
        OleDbConnection con;
        private string pathFile = "personnel.mdb.crypt";
        public Form1()
        {
            InitializeComponent();
        }

        public void Crypt(string inputFilePath, string outputFilePath)
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


                        for (int i = 0; i < bytesAsInts.Length; i++)
                        {
                            bytesAsInts[i] = ((bytesAsInts[i] * (-1) + ln * 2) + 128);
                        }
                        


                        byte[] bytes1 = bytesAsInts.Select(x => (byte)x).ToArray();
                        fileStream.Write(bytes1, 0, bytesRead);
                        fileStream.Flush();
                    }
                    //MessageBox.Show("Декодування виконано!", "Повідомлення");
                    File.SetAttributes(outputFilePath, FileAttributes.Hidden);
                    fileStream.Close();


                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string newPathFile = pathFile.Replace(".crypt", "");
            
            Crypt(pathFile, newPathFile);

            int c = 0;
            string usr = txtLogin.Text;
            string psw = txtPass.Text;
            con = new OleDbConnection(@"Provider=Microsoft.ACE.Oledb.12.0;Data Source=personnel.mdb");
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter("Select access From userlist where login ='" + usr + "' and pass ='" + psw + "'", con);
            DataTable dt = new DataTable();

            try
            {
                c = 0;
                dataAdapter.Fill(dt);
            }
            catch(Exception ex)
            {
                c = 1;
                MessageBox.Show(ex.Message, "Помилка доступу!");
            }
            finally
            {
                File.Delete(@"personnel.mdb");
            }


            if (c == 0)
            {
                // Проверяем, что количество строк из БД больше нуля
                if (dt.Rows.Count > 0)
                {
                    string access = dt.Rows[0][0].ToString();
                    if (access == "admin")
                    {
                        this.Hide();
                        FormAccept fa = new FormAccept();
                        fa.Show();
                        File.Delete(@"personnel.mdb");
                    }
                    else
                    {
                        this.Hide();
                        FormMain fm = new FormMain(usr, psw);
                        fm.Show();
                        File.Delete(@"personnel.mdb");
                    }
                }
                else
                {
                    MessageBox.Show("Неправильне ім'я користувача або пароль!");
                }
            }
            

            con.Close();
        }
    }
}
