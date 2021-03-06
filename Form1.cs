﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SklepGenerator.OracleDataManager;
using SklepGenerator.Connection;



namespace SklepGenerator
{
    public partial class Form1 : Form
    {
        int count = 0;
        Table currentTable;

        public OracleDB Oracle { get; set; }
        public Form1() {

            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (currentTable != null && count > 0)
            {
                    Oracle.InsertRandValues(currentTable.Name, count);
            }
        }

        void UpdateDataGridSize()
        {
            Size size = dataGridView1.PreferredSize;
            if (size.Width + dataGridView1.Left < ClientSize.Width &&
                size.Height + dataGridView1.Top < ClientSize.Height)
            {
                dataGridView1.Size = size;
            }
            else
            {
                size.Height = (ClientSize.Height - dataGridView1.Top);
                size.Width = (ClientSize.Width - dataGridView1.Left);
                dataGridView1.Size = size;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            currentTable = Oracle.GetDataScheme()[comboBox1.Text];
            if(currentTable.Name == null) currentTable.Name = comboBox1.Text;
            string name = "";
            for (int i = 0; i < currentTable.Columns.Count; i++)
            {
                name = currentTable.Columns[i].Name;
                dataGridView1.Columns.Add(name, name);
            }
            UpdateDataGridSize();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            count = (int)numericUpDown1.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Connecting...");
            string conString = ""; 

            OracleDB DataBase = new OracleDB(conString);
            DataBase.Connect();
            DataBase.LoadData();
            Oracle = DataBase;
            foreach (string s in Oracle.GetDataScheme().Keys)
            {
                comboBox1.Items.Add(s);
            }
        }
    }
}
