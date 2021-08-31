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
using System.Diagnostics;

namespace Personnel
{
    public partial class FormUFiles : Form
    {
        OleDbConnection con;
        OleDbCommand cmd;
        OleDbDataReader rd;
        string s;
        int i;
        public FormUFiles(string s, int i)
        {
            InitializeComponent();
            Load(s);
            Load2(s);
        }

        private void Crypt(string inputFilePath, string outputFilePath, bool CoderMode, string dec)
        {
            int bufferSize = 1024 * 64; //Создание буфера

            using (FileStream inStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            {

                using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {

                    int bytesRead = -1;
                    byte[] bytes = new byte[bufferSize];
                    string decodeString = dec;


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

                    string extension = Path.GetExtension(outputFilePath);
                    if (extension != ".crypt")
                    {
                        File.SetAttributes(outputFilePath, FileAttributes.Hidden);
                    }
                    fileStream.Close();
                }
            }
            string extension1 = Path.GetExtension(inputFilePath);
            if (extension1 != ".crypt")
                File.Delete(inputFilePath);
        }

        private void CryptOpen(string inputFilePath, string outputFilePath, bool CoderMode, string dec)
        {
            int bufferSize = 1024 * 64; //Создание буфера

            using (FileStream inStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            {

                using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
                {

                    int bytesRead = -1;
                    byte[] bytes = new byte[bufferSize];
                    string decodeString = dec;


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

                    string extension = Path.GetExtension(outputFilePath);
                    if (extension != ".crypt")
                    {
                        File.SetAttributes(outputFilePath, FileAttributes.Hidden);
                        Open(outputFilePath);
                    }
                    fileStream.Close();
                }
            }
        }

        private void Open(string s)
        {
            var pr = new Process();
            pr.StartInfo.FileName = s;
            pr.StartInfo.UseShellExecute = true;
            pr.Start();
        }

        private void Load(string s)
        {
            string[] files = Directory.GetFiles(s);

            for (int j = 0; j < files.Count(); j++)
            {
                //string result = Path.GetFileName(files[0]);

                string extension = Path.GetExtension(files[j]);
                if (extension != ".crypt")
                {
                    string pathFile = files[j];
                    Crypt(pathFile, pathFile.ToString() + ".crypt", false, i.ToString());
                }
            }
        }

        private void Load2(string s)
        {
            string[] files = Directory.GetFiles(s);

            List<string[]> dt = new List<string[]>();
            //MessageBox.Show(files[0].ToString());
            for (int j = 0; j < files.Count(); j++)
            {

                dt.Add(new string[3]);

                dt[dt.Count - 1][0] = files[j].ToString();
                dt[dt.Count - 1][1] = Convert.ToString(File.GetLastWriteTime(files[j]));
                dt[dt.Count - 1][2] = i.ToString();
            }
            foreach (string[] a in dt)
                dataGridView1.Rows.Add(a);
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

    private void button1_Click(object sender, EventArgs e)
        {
            string pathFile = (dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString());
            string newPathFile = pathFile.Replace(".crypt", "");
            CryptOpen(pathFile, newPathFile, false, i.ToString());
        }
    }
}
