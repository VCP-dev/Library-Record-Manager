using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;


namespace FirebaseDatabasePrac
{
    class Requiredfunctions
    {
        public void setdates(ref string issuedate,ref string returndate)
        {
            DateTime today = DateTime.Today;
            string startdate = today.ToString("dd");
            int next = int.Parse(startdate) + 5;
            string returnd = next.ToString() + "-" + today.ToString("MM-yyyy");
            issuedate = today.ToString("dd-MM-yyyy");
            returndate = returnd;
        }


        public bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }


        public void createtextfile()
        {
            try
            {
                string filepath = @"bookslist.txt";
                if(!File.Exists(filepath))
                {
                    StreamWriter file = new StreamWriter(filepath);
                    file.Write("File content to be added");
                    MessageBox.Show("Text file which stores the list of books lended has been created.Please do not tamper with file");
                }   
                if(File.Exists(filepath))
                {
                    MessageBox.Show("File for book list already exists");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }            
        }


        public void createtextforconfigfile()
        {
            try
            {
                string filepath = @"firebaseconfig.config";
                if (!File.Exists(filepath))
                {
                    StreamWriter file = new StreamWriter(filepath);
                    file.Write("File content to be added");
                    MessageBox.Show("Text file which stores the config for firebase has been created.Please do not tamper with file");
                }
                if (File.Exists(filepath))
                {
                    MessageBox.Show("File for firebase config already exists");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + ex.Message);
            }
        }


        public async void exporttodatagridview(DataTable dt,List<string> booklist,string FilePath,IFirebaseClient client)
        {
            if (!CheckForInternetConnection())
            {
                MessageBox.Show("Please check internet connection before performing any task", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dt.Rows.Clear();
            if (booklist.Count > 0)
            {
                booklist.Clear();
            }
            booklist = File.ReadAllLines(FilePath).ToList();

            if (booklist.Count > 0)
            {
                foreach (string book in booklist)
                {
                    try
                    {
                        FirebaseResponse response = await client.GetTaskAsync("Books lended/" + book);
                        Book obj = response.ResultAs<Book>();

                        DataRow row = dt.NewRow();
                        row["Name of book"] = obj.name;
                        row["Issue date"] = obj.issuedate;
                        row["Return date"] = obj.returndate;
                        dt.Rows.Add(row);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                MessageBox.Show("Book list retrieved");
            }
            else
            {
                MessageBox.Show("No books issued");
            }
        }


    }
}
