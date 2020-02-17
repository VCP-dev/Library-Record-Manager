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
using FirebaseDatabasePrac;
using Microsoft.Office.Interop;



using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;


namespace FirebaseDatabasePrac
{
    public partial class Form1 : Form
    {
        string startdate, returndate;

        string FilePath = @"bookslist.txt";

        List<string> booklist = new List<string>();

        DataTable dt = new DataTable();

        readonly Requiredfunctions rf = new Requiredfunctions();


        //IFirebaseConfig config = new FirebaseConfig
        //{
        //    AuthSecret = "yncOPq9XYsTfOf4QTJmNWzo90FArKs61xuNw5V1R",
        //    BasePath = "https://librarysystem-49e06.firebaseio.com/"
        //};

        IFirebaseClient client = null;


        public Form1()
        {
            InitializeComponent();            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<string> configitems = File.ReadAllLines(@"firebaseconfig.config").ToList();


            client = new FireSharp.FirebaseClient(new FirebaseConfig()
            {
                AuthSecret = configitems[0],
                BasePath = configitems[1]
            });

            if(client != null)
            {
                MessageBox.Show("Connection to database established");
            }

            dt.Columns.Add("Name of book");
            dt.Columns.Add("Issue date");
            dt.Columns.Add("Return date");
            dataGridView1.DataSource = dt;            
            dates();                       
            rf.exporttodatagridview(dt,booklist,FilePath,client);
        }

       
        private async void button1_Click(object sender, EventArgs e)        ////  inserting records
        {
            if (!rf.CheckForInternetConnection())
            {
                MessageBox.Show("Please check internet connection before performing any task", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(textBox1.Text=="")
            {
                MessageBox.Show("Please enter the name of the book");
                return;
            }
            var bookdetails = new Book
            {
                name = textBox1.Text,
                issuedate = textBox2.Text,
                returndate = textBox3.Text
            };

            SetResponse response = await client.SetTaskAsync("Books lended/"+bookdetails.name+"/",bookdetails);
            Book result = response.ResultAs<Book>();

            File.AppendAllText(FilePath, bookdetails.name + Environment.NewLine);

            MessageBox.Show(result.name+" has been issued");

            textBox1.Text = "";
            dates();
            rf.exporttodatagridview(dt, booklist, FilePath, client);
        }

        void dates()
        {           
            rf.setdates(ref startdate, ref returndate);
            textBox2.Text = startdate;
            textBox3.Text = returndate;
        }

        private void button2_Click(object sender, EventArgs e)            /////   for exprting to excel
        {
            //throw new NotImplementedException();
            if(dataGridView1.Rows.Count > 0)
            {
                var lines = new List<string>();

                string[] columnNames = dt.Columns
                    .Cast<DataColumn>()
                    .Select(column => column.ColumnName)
                    .ToArray();

                var header = string.Join(",", columnNames.Select(name => $"\"{name}\""));
                lines.Add(header);

                var valueLines = dt.AsEnumerable()
                    .Select(row => string.Join(",", row.ItemArray.Select(val => $"\"{val}\"")));

                lines.AddRange(valueLines);

                File.WriteAllLines("Books Issued as of " + startdate + ".csv", lines);

                MessageBox.Show("Exported to excel document");                
            }
        }

        private async void button3_Click(object sender, EventArgs e)   /////  for deleting all records
        {
            if (!rf.CheckForInternetConnection())
            {
                MessageBox.Show("Please check internet connection before performing any task", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (MessageBox.Show("Do you really wish to delete all records ?","Confirmation",MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                File.WriteAllText(FilePath, string.Empty);
                FirebaseResponse response = await client.DeleteTaskAsync("Books lended");
                rf.exporttodatagridview(dt, booklist, FilePath, client);
            }
            else
            {
                return;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            rf.exporttodatagridview(dt, booklist, FilePath, client);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!rf.CheckForInternetConnection())
            {
                MessageBox.Show("Please check internet connection before performing any task", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(e.KeyCode == Keys.Enter)
            {
                button1_Click(this,new EventArgs());
            }
        }

        private async void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
            ConfirmationBox newbox = new ConfirmationBox(row.Cells["Name of book"].Value.ToString(),dt,booklist, FilePath, client);
            newbox.Show();
            
            //FirebaseResponse response = await client.DeleteTaskAsync("Books lended/" + dataGridView1.Rows[e.RowIndex].Cells["Name of book"].Value.ToString());
            //exporttodatagridview();
        }
                     
        //private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if(e.RowIndex >= 0)
        //    {
        //        DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
        //        ConfirmationBox newbox = new ConfirmationBox(row.Cells["Name of book"].Value.ToString(),FilePath,client);
        //        exporttodatagridview();
        //    }
        //}

                
    }
}
