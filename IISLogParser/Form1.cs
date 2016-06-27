using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IISLogParser
{
    public partial class Form1 : Form
    {
        public class IISLogEntry
        {
            public int LineNumber { get; set; }
            public string DateTime { get; set; }
            public string Method { get; set; }
            public string Request { get; set; }
            public string RequestIP { get; set; }

            public string Query { get; set; }

            public string TimeTaken { get; set; }

            public string Win32Status { get; set; }

            public string SubStatus { get; set; }

            public string Status { get; set; }

            public string Raw { get; set; }
        }
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dl = new OpenFileDialog();

            if (dl.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.logs = ParseIISLogFile(dl.FileName);
                DisplayLog(this.logs, textBox1.Text, textBox2.Text, textBox3.Text);
                this.Text = "IIS Loger Parser [" + dl.FileName + "]";
            }
        }

        private void DisplayLog(List<IISLogEntry> list, string from = "", string to = "", string request="")
        {
            var query = list.AsEnumerable();
            if (!string.IsNullOrEmpty(from))
            {
                query = query.Where(p => string.Compare(p.DateTime , from) >=0 );
            }

            if (!string.IsNullOrEmpty(to))
            {
                query = query.Where(p => string.CompareOrdinal(p.DateTime, to) <= 0);
            }
            if (!string.IsNullOrEmpty(request))
            {
                query = query.Where(p => p.Request.ToLower() == request.ToLower());
            }

            this.dataGridView1.DataSource = query.ToList();
            this.dataGridView1.AutoGenerateColumns = true;
            this.dataGridView1.AutoResizeColumns();
        }

        List<IISLogEntry> logs;

        private List<IISLogEntry> ParseIISLogFile(string filename)
        {
            int linenumber = 0;
            var list = new List<IISLogEntry>();
            using (FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BufferedStream bs = new BufferedStream(fs))
            using (StreamReader sr = new StreamReader(bs))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    linenumber++;
                    if (line.StartsWith("#")) continue;
                    try
                    {
                        IISLogEntry entry = ParseLog(line, linenumber);
                        list.Add(entry);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return list;
        }

        private IISLogEntry ParseLog(string line, int linenumber)
        {
            var arr = line.Split(new char[] { ' ' });
            return new IISLogEntry()
            {
                LineNumber = linenumber,
                DateTime = arr[0] + ' ' + arr[1],
                RequestIP = arr[2],
                Method = arr[3],
                Request = arr[4],
                Query= arr[5],
                TimeTaken = arr[arr.Length-1] ,
                Win32Status = arr[arr.Length - 2],
                SubStatus = arr[arr.Length - 3],
                Status = arr[arr.Length - 4]   ,
                Raw = line
            };
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DisplayLog(this.logs, textBox1.Text, textBox2.Text, textBox3.Text);
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            foreach (DataGridViewRow Myrow in dataGridView1.Rows)
            {            //Here 2 cell is target value and 1 cell is Volume
                if (Myrow.Cells[9].Value.ToString() !="200")// Or your condition 
                {
                    Myrow.DefaultCellStyle.BackColor = Color.Red;
                    Myrow.DefaultCellStyle.ForeColor = Color.White;
                }
               
            }
        }
    }
}
