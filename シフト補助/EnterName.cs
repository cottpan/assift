﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Shiftwork.Library;
using System.Collections;

namespace Shiftwork
{
    public partial class EnterName : Form
    {
        private string[,] namelist;
        Excel.Application app;
        Excel.Workbook book;
        Excel.Sheets sheets;
        Excel.Worksheet jobsheet;

        ArrayList jobarray;
        Excel.Range activerange;
        Excel.Range selectrange;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="namelist">構成員名簿データ</param>
        /// <param name="app">操作中のExcelアプリケーション</param>
        public EnterName(string[,] namelist, Excel.Application app)
        {
            InitializeComponent();
            this.namelist = namelist;
            this.app = app;
        }

        /// <summary>
        /// 最初に開かれた時のメソッドです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnterName_Load(object sender, EventArgs e)
        {
            book = app.ActiveWorkbook;
            sheets = book.Worksheets;
            jobsheet = (Excel.Worksheet)sheets.get_Item(sheets.getSheetIndex("仕事シフト"));

            this.jobBox.SelectedIndexChanged -= new EventHandler(jobBox_SelectedIndexChanged);
            jobBox.Items.Clear();
            jobBox.Items.Add("全");
            this.jobBox.SelectedIndex = 0;
            this.jobBox.SelectedIndexChanged += new EventHandler(jobBox_SelectedIndexChanged);
            this.Activated += new EventHandler(EnterName_Activated);
            bureauTextBox.Text = "全";
            gradeTextBox.Text = "全";
            activeCellUpdate();


        }

        #region method using EventHandler
        void EnterName_Activated(object sender, EventArgs e)
        {
            nameViewUpdate(bureauTextBox.Text, gradeTextBox.Text, jobBox.SelectedItem.ToString());
            activeCellUpdate();

        }

        void jobBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            nameViewUpdate(bureauTextBox.Text, gradeTextBox.Text, jobBox.SelectedItem.ToString());
            activeCellUpdate();
        }


        #endregion

        #region method using update
        private void activeCellUpdate()
        {
            activerange = app.ActiveCell;
            selectrange = app.Selection;
            //MessageBox.Show(activerange.Columns.Count.ToString() + "\r\n" + selectrange.Column.ToString());
        }
        private void nameViewUpdate(string bureau, string grade, string job)
        {
            nameView.Rows.Clear();
            List<DataGridViewRow> row_list = new List<DataGridViewRow>();
            for (int i = 0; i <= namelist.GetUpperBound(0); i++)
            {
                if ((bureau == "全" || bureau == namelist[i,1]) && (grade == "全" || grade == namelist[i,3]) && (job == "全" || isJobContained(job, i)))
                {
                    object[] row_value = new object[] { namelist[i,1], namelist[i,2], namelist[i,3], namelist[i,4] ,isFilled(namelist[i,4])?"×":"○"};
                    DataGridViewRow row = new DataGridViewRow();
                   
                    row.CreateCells(nameView);
                    row.SetValues(row_value);
                    row_list.Add(row);
                }
            }
            nameView.Rows.AddRange(row_list.ToArray<DataGridViewRow>());
        }

        private bool isFilled(string name)
        {
            if(name == null)
            {
                throw new ArgumentNullException();
            }
            // selectなのかactiveなのか要調査
            Excel.Range tmprange = jobsheet.Cells[24, selectrange.Column];
            int jobtype = int.Parse(System.Configuration.ConfigurationManager.AppSettings["jobtype"]);
            tmprange = tmprange.get_Resize(jobtype, selectrange.Columns.Count);
            string[,] tmp = tmprange.DeepToString();

            return (isdeepContained(tmp, name));
        }
        private bool isdeepContained (string[,] array, string data)
        {
            for(int i = 0; i <= array.GetUpperBound(1); i++)
            {
                for(int j = 0; j <= array.GetUpperBound(0); j++)
                {
                    if (array[j, i] == data) return true;
                }
            }
            return false;
        }

        private bool isJobContained(string job, int index)
        {
            for (int i = 7; i <= 16; i++)
            {
                if (namelist[index,i] == job)
                {
                    return true;
                }
            }
            return false;
        }
        private void jobBoxUpdate(string bureau, string grade)
        {
            this.jobBox.SelectedIndexChanged -= new EventHandler(jobBox_SelectedIndexChanged);
            jobBox.Items.Clear();
            jobBox.Items.Add("全");
            jobBox.Items.AddRange(Util.jobSearch(namelist, bureau, grade));
            this.jobBox.SelectedIndex = 0;
            this.jobBox.SelectedIndexChanged += new EventHandler(jobBox_SelectedIndexChanged);
        }
        #endregion


        #region button clicked


        private void bulAll_Click(object sender, EventArgs e)
        {
            bureauTextBox.Text = "全";
            sendButton.Enabled = false;
            jobBoxUpdate(bureauTextBox.Text, gradeTextBox.Text);
            nameViewUpdate(bureauTextBox.Text, gradeTextBox.Text, jobBox.SelectedItem.ToString());
            activeCellUpdate();
            sendButton.Enabled = true;
        }

        private void bulShikko_Click(object sender, EventArgs e)
        {
            bureauTextBox.Text = "執行";
            sendButton.Enabled = false;
            jobBoxUpdate(bureauTextBox.Text, gradeTextBox.Text);
            nameViewUpdate(bureauTextBox.Text, gradeTextBox.Text, jobBox.SelectedItem.ToString());
            activeCellUpdate();
            sendButton.Enabled = true;
        }

        private void bulKoho_Click(object sender, EventArgs e)
        {
            bureauTextBox.Text = "広報";
            sendButton.Enabled = false;
            jobBoxUpdate(bureauTextBox.Text, gradeTextBox.Text);
            nameViewUpdate(bureauTextBox.Text, gradeTextBox.Text, jobBox.SelectedItem.ToString());
            activeCellUpdate();
            sendButton.Enabled = true;
        }

        private void bulKikaku_Click(object sender, EventArgs e)
        {
            bureauTextBox.Text = "企画";
            sendButton.Enabled = false;
            jobBoxUpdate(bureauTextBox.Text, gradeTextBox.Text);
            nameViewUpdate(bureauTextBox.Text, gradeTextBox.Text, jobBox.SelectedItem.ToString());
            activeCellUpdate();
            sendButton.Enabled = true;
        }

        private void bulSoushoku_Click(object sender, EventArgs e)
        {
            bureauTextBox.Text = "装飾";
            sendButton.Enabled = false;
            jobBoxUpdate(bureauTextBox.Text, gradeTextBox.Text);
            nameViewUpdate(bureauTextBox.Text, gradeTextBox.Text, jobBox.SelectedItem.ToString());
            activeCellUpdate();
            sendButton.Enabled = true;
        }

        private void bulShisetu_Click(object sender, EventArgs e)
        {
            bureauTextBox.Text = "施設";
            sendButton.Enabled = false;
            jobBoxUpdate(bureauTextBox.Text, gradeTextBox.Text);
            nameViewUpdate(bureauTextBox.Text, gradeTextBox.Text, jobBox.SelectedItem.ToString());
            activeCellUpdate();
            sendButton.Enabled = true;
        }

        private void bulJimu_Click(object sender, EventArgs e)
        {
            bureauTextBox.Text = "事務";
            sendButton.Enabled = false;
            jobBoxUpdate(bureauTextBox.Text, gradeTextBox.Text);
            nameViewUpdate(bureauTextBox.Text, gradeTextBox.Text, jobBox.SelectedItem.ToString());
            activeCellUpdate();
            sendButton.Enabled = true;
        }

        private void graAll_Click(object sender, EventArgs e)
        {
            gradeTextBox.Text = "全";
            sendButton.Enabled = false;
            jobBoxUpdate(bureauTextBox.Text, gradeTextBox.Text);
            nameViewUpdate(bureauTextBox.Text, gradeTextBox.Text, jobBox.SelectedItem.ToString());
            activeCellUpdate();
            sendButton.Enabled = true;
        }

        private void gra1_Click(object sender, EventArgs e)
        {
            gradeTextBox.Text = "1";
            sendButton.Enabled = false;
            jobBoxUpdate(bureauTextBox.Text, gradeTextBox.Text);
            nameViewUpdate(bureauTextBox.Text, gradeTextBox.Text, jobBox.SelectedItem.ToString());
            activeCellUpdate();
            sendButton.Enabled = true;
        }

        private void gra2_Click(object sender, EventArgs e)
        {
            gradeTextBox.Text = "2";
            sendButton.Enabled = false;
            jobBoxUpdate(bureauTextBox.Text, gradeTextBox.Text);
            nameViewUpdate(bureauTextBox.Text, gradeTextBox.Text, jobBox.SelectedItem.ToString());
            activeCellUpdate();
            sendButton.Enabled = true;
        }

        private void gra3_Click(object sender, EventArgs e)
        {
            gradeTextBox.Text = "3";
            sendButton.Enabled = false;
            jobBoxUpdate(bureauTextBox.Text, gradeTextBox.Text);
            nameViewUpdate(bureauTextBox.Text, gradeTextBox.Text, jobBox.SelectedItem.ToString());
            activeCellUpdate();
            sendButton.Enabled = true;
        }

        private void gra4_Click(object sender, EventArgs e)
        {
            gradeTextBox.Text = "4";
            sendButton.Enabled = false;
            jobBoxUpdate(bureauTextBox.Text, gradeTextBox.Text);
            nameViewUpdate(bureauTextBox.Text, gradeTextBox.Text, jobBox.SelectedItem.ToString());
            activeCellUpdate();
            sendButton.Enabled = true;
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            string buf = (string)nameView.CurrentRow.Cells[4].Value;
            if (buf == "×")
            {
                this.Activated -= new EventHandler(EnterName_Activated);
                DialogResult result = MessageBox.Show((string)nameView.CurrentRow.Cells[3].Value + "さんは存在します。\r\n続行しますか？",
            "確認",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Exclamation,
            MessageBoxDefaultButton.Button2);
                        if (result == DialogResult.OK)
                        {
                            activerange.Value2 = nameView.CurrentRow.Cells[3].Value;
                            this.Activated += new EventHandler(EnterName_Activated);
                        }
                        else if (result == DialogResult.Cancel)
                        {
                    this.Activated += new EventHandler(EnterName_Activated);
                    return;
                        }
                    }

            activerange.Value2 = nameView.CurrentRow.Cells[3].Value;
        }
        #endregion
    }
}
