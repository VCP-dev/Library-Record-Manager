using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FirebaseDatabasePrac
{
    public partial class ConfigWindow : Form
    {

        string filepath;

        public ConfigWindow(string filepath)
        {
            InitializeComponent();
            this.filepath = filepath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text!="" && textBox2.Text!="")
            {
                File.AppendAllText(filepath, textBox1.Text + Environment.NewLine);
                File.AppendAllText(filepath, textBox2.Text);
                //Application.Run(new Form1());
                Application.ExitThread();
                Thread _thread = new Thread(() =>
                {
                    Application.Run(new Form1());
                });
                _thread.SetApartmentState(ApartmentState.STA);
                _thread.Start();
                //Form1 form1 = new Form1();
                //form1.Show();                
            }
            else
            {
                MessageBox.Show("Please enter valid config id's", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
