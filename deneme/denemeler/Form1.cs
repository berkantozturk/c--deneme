﻿using Microsoft.VisualBasic;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace denemeler
{
    public partial class Form1 : Form

    {
        private string currentSchema = "";
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string connString = @"C:\Users\berka\OneDrive\Masaüstü\C#\deneme\denemeler\bin\Debug\data\connstring.txt";

            if (File.Exists(connString))
            {
                using (StreamReader a = new StreamReader(connString))
                {
                    string column;
                    while ((column = a.ReadLine()) != null)
                    {
                        comboBox1.Items.Add(column);
                    }
                }
            }
            string folderFilePath = @"C:\Users\berka\OneDrive\Masaüstü\C#\deneme\denemeler\bin\Debug\data\folder.txt";

            if (File.Exists(folderFilePath))
            {
                using (StreamReader reader = new StreamReader(folderFilePath))
                {
                    string folderPath = reader.ReadLine();

                    if (!string.IsNullOrEmpty(folderPath))
                    {
                        DirectoryInfo foldersinfo = new DirectoryInfo(folderPath);
                        FileInfo[] files = foldersinfo.GetFiles();

                        List<MigFile> fileNames = new List<MigFile>();

                        foreach (FileInfo file in files)
                        {
                            MigFile migFile = new MigFile();
                            migFile.Name = file.Name;
                            fileNames.Add(migFile);
                        }

                        dataGridView2.DataSource = fileNames;

                    }
                    else
                    {
                        MessageBox.Show("Dosya yolunu bulunamadı veya boş.");
                    }
                }
            }
            else
            {
                MessageBox.Show("folder.txt dosyası bulunamadı.");
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            {
                string selectedDataSource = comboBox1.SelectedItem.ToString();
                OracleConnection conn = new OracleConnection(selectedDataSource);

                currentSchema = dataGridView1.SelectedCells[0].Value.ToString();

                try
                {
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT name FROM {currentSchema}.TNRK_MIGRATIONS";
                    cmd.CommandType = CommandType.Text;

                    OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    //if (dt.Rows.Count > 0)
                    //{
                        dataGridView3.DataSource = dt;

                    //}
                    //else
                    //{
                    //    dataGridView3.DataSource = null;
                    //    MessageBox.Show("Schema is empty!");
                    //}
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }

            }
        }

        private void addFolderButton(object sender, EventArgs e)
        {

            folderBrowserDialog1.ShowDialog();
            string folderPath = folderBrowserDialog1.SelectedPath;
            DirectoryInfo foldersinfo = new DirectoryInfo(folderPath);
            FileInfo[] folders = foldersinfo.GetFiles();

            List<MigFile> fileNames = new List<MigFile>();

            foreach (FileInfo file in folders)
            {
                MigFile migFile = new MigFile();
                migFile.Name = file.Name;
                fileNames.Add(migFile);

            }

            dataGridView2.DataSource = fileNames;
            try
            {
                File.WriteAllText("C:\\Users\\berka\\OneDrive\\Masaüstü\\C#\\deneme\\denemeler\\bin\\Debug\\data\\folder.txt", folderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dosya yolu kaydedilirken bir hata oluştu: " + ex.Message);
            }
        }


        private class MigFile //listede rakam olarak gözükmesi için class ekledik
        {
            public string Name { get; set; }
        }

        private void dataGridView4_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void ConnectButton(object sender, EventArgs e)
        {
            try
            {
                OracleConnection conn = new OracleConnection(comboBox1.Text);

                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT username AS SCHEMAS FROM all_users WHERE (username = 'MIGRATION_TEST') ORDER BY username";
                cmd.CommandType = CommandType.Text;

                OracleDataAdapter adapter = new OracleDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dt;

                }
                else
                {
                    MessageBox.Show("Not Connected!");
                }
            }
            catch (Exception ex)
            {
                    MessageBox.Show("Please add or choose a valid connection string!");
            }
            finally
            {
            }

        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {


        }
        private void addButton(object sender, EventArgs e)
        {
            {
                string newString = Interaction.InputBox("Connection String", "Add your Connection String!");

                if (!string.IsNullOrWhiteSpace(newString))
                {
                    try
                    {
                        if (!comboBox1.Items.Contains(newString))
                        {
                            string fileName = @"C:\Users\berka\OneDrive\Masaüstü\C#\deneme\denemeler\bin\Debug\data\connstring.txt";
                            File.AppendAllText(fileName, newString + Environment.NewLine);

                            comboBox1.Items.Add(newString);
                            comboBox1.Text =(newString);

                            MessageBox.Show("Successful!");
                        }
                        else
                        {
                            MessageBox.Show("The item already exists in the ComboBox.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Please write string!");
                }

            }
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            
        }
        private void compareButton(object sender, EventArgs e)
        {
            List<MigFile> grid2Data = (List<MigFile>)dataGridView2.DataSource;
            DataTable grid3Data = (DataTable)dataGridView3.DataSource;
            List<MigFile> differentFiles = new List<MigFile>();

            foreach (MigFile file in grid2Data)
            {
                bool found = false;

                if (grid3Data != null)
                {
                    foreach (DataRow row in grid3Data.Rows)
                    {
                        string fileName = row["name"].ToString();
                        if (file.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                        {
                            found = true;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    differentFiles.Add(file);
                }
            }
            dataGridView4.DataSource = differentFiles;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void runButton_Click(object sender, EventArgs e)
        {

            string a = richTextBox1.Text;
            string sql = $"BEGIN {a} END;";

            OracleConnection conn = new OracleConnection(comboBox1.Text);

            conn.Open();
            OracleCommand cmd = new OracleCommand();
            cmd.Connection = conn;


            try
            {
                cmd.CommandText = $"alter session set current_schema = {currentSchema}";
                cmd.ExecuteNonQuery();
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                richTextBox1.Clear();
                MessageBox.Show("DATA SAVED SUCCESSFULLY!");

            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata: " + ex.Message);
            }

            dataGridView1_CellContentClick(null, null);
            compareButton(sender, e);
        }


        private void showButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            foreach (DataGridViewCell cell in dataGridView4.SelectedCells)
            {
                if (cell.RowIndex >= 0 && cell.ColumnIndex == 0)
                {
                    string selectedFileName = dataGridView4.Rows[cell.RowIndex].Cells[0].Value.ToString();
                    string folderPath = @"C:\Users\berka\OneDrive\Masaüstü\C#\deneme\denemeler\bin\Debug";
                    string filePath = Path.Combine(folderPath, selectedFileName);

                    try
                    {
                        string fileContent = File.ReadAllText(filePath);
                        richTextBox1.AppendText(fileContent + Environment.NewLine);

                        int lineLength = 50;
                        string line = new string('-', lineLength);

                        richTextBox1.AppendText(line + Environment.NewLine);


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Dosya okuma hatası: " + ex.Message);
                    }
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1_CellContentClick(null,null);
            compareButton(sender, e);

        }
    }
}

    

