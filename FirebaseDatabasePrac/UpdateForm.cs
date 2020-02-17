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
    public partial class UpdateForm : Form
    {

        string booktitle;
        List<string> booklist;
        string FilePath;
        IFirebaseClient client;
        DataTable dt;

        Requiredfunctions rf = new Requiredfunctions();

        public UpdateForm(DataTable dt,string booktitle,List<string> booklist,string FilePath,IFirebaseClient client)
        {
            InitializeComponent();
            this.dt = dt;
            this.booklist = booklist;
            this.FilePath = FilePath;
            this.client = client;
            this.booktitle = booktitle;
            getbook();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (!rf.CheckForInternetConnection())
            {
                MessageBox.Show("Please check internet connection before performing any task", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBox1.Text!="" && textBox2.Text!="" && textBox3.Text!="")
            {
                string newtitle = textBox1.Text;
                Book updatedbook = new Book()
                {
                    name = newtitle,
                    issuedate = textBox2.Text,
                    returndate = textBox3.Text
                };
                FirebaseResponse response = await client.DeleteTaskAsync("Books lended/" + booktitle);
                SetResponse setResponse = await client.SetTaskAsync("Books lended/"+newtitle,updatedbook);                
                if (booklist.Count > 0)
                {
                    booklist.Clear();
                }
                booklist = File.ReadAllLines(FilePath).ToList();
                File.WriteAllText(FilePath, string.Empty);
                foreach (string book in booklist)
                {
                    if (book != booktitle)
                    {
                        File.AppendAllText(FilePath, book + Environment.NewLine);
                    }
                    else
                    {
                        File.AppendAllText(FilePath, newtitle + Environment.NewLine);
                    }
                }
                rf.exporttodatagridview(dt, booklist, FilePath, client);
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid input for updating", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        async void getbook()
        {
            FirebaseResponse response = await client.GetTaskAsync("Books lended/"+booktitle);
            Book result = response.ResultAs<Book>();
            textBox1.Text = result.name;
            textBox2.Text = result.issuedate;
            textBox3.Text = result.returndate;
        }
    }
}
