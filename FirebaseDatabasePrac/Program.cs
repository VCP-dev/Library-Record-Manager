using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirebaseDatabasePrac
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            List<string> configitems = null;
            Requiredfunctions rf = new Requiredfunctions();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (rf.CheckForInternetConnection())
            {
                rf.createtextfile();
                rf.createtextforconfigfile();
                configitems = File.ReadAllLines(@"firebaseconfig.config").ToList();
                if(configitems.Count != 2)
                {
                    //ConfigWindow configWindow = new ConfigWindow(@"firebaseconfig.txt");
                    //configWindow.Show();
                    Application.Run(new ConfigWindow(@"firebaseconfig.config"));
                }
                else
                {
                    Application.Run(new Form1());
                }
                //Application.Run(new Form1());                
            }
            else
            {
                MessageBox.Show("Please check internet connection before running the application","Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

       

    }
}
