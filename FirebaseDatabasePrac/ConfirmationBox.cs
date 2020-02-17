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

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace FirebaseDatabasePrac
{
    public partial class ConfirmationBox : Form
    {

        string booktitle;
        string FilePath;
        IFirebaseClient client;
        DataTable dt;
        List<string> booklist;


        Book newbook;

        Requiredfunctions rf = new Requiredfunctions();


        public ConfirmationBox(string booktitle,DataTable dt,List<string> booklist,string FilePath,IFirebaseClient client)
        {
            InitializeComponent();
            this.booktitle = booktitle;
            this.FilePath = FilePath;
            this.client = client;
            this.dt = dt;
            this.booklist = booklist;
            newbook = null;
            label1.Text = "Menu for "+booktitle;
        }

        private void button1_Click(object sender, EventArgs e)           //// for updating
        {
            if (!rf.CheckForInternetConnection())
            {
                MessageBox.Show("Please check internet connection before performing any task", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //MessageBox.Show("Update function not made yet", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            UpdateForm newform = new UpdateForm(dt,booktitle,booklist,FilePath,client);
            newform.Show();
            this.Close();
        }

        private async void button2_Click(object sender, EventArgs e)          /////  for deleteing
        {
            if(!rf.CheckForInternetConnection())
            {
                MessageBox.Show("Please check internet connection before performing any task", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            FirebaseResponse response = await client.DeleteTaskAsync("Books lended/" + booktitle);
            booklist.Clear();
            booklist = File.ReadAllLines(FilePath).ToList();
            File.WriteAllText(FilePath, string.Empty);
            foreach(string book in booklist)
            {
                if(book!=booktitle)
                {
                    File.AppendAllText(FilePath,book + Environment.NewLine);
                }
            }
            rf.exporttodatagridview(dt, booklist, FilePath, client);
            this.Close();
        }
    }
}
