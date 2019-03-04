//Form1.cs 4.3.2019
//by Aki Sirviö
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JypeliWinForm
{
    public partial class Form1 : Form
    {
        JypeliWinForm peli; //game class

        // constructor not in use
        public Form1()
        {
            InitializeComponent();
        }

        // constructor takes game class and save it for later use.
        public Form1(JypeliWinForm peliolio)
        {
            peli = peliolio;
            InitializeComponent();
        }

        // button click event add balloons in game class method
        private void button1_Click(object sender, EventArgs e)
        {
            peli.LisaaPallo();
            textBox1.Text = "You got the balloon!";
        }

        // button click event destroy balloons in game class method
        private void button2_Click(object sender, EventArgs e)
        {
            peli.TuhoaPallot();
            textBox1.Text = "Try again.";
        }

        // form closing event close game in game class method
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            peli.Sulje();
        }
    }
}
