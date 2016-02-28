using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 小游戏
{
    public partial class Form2 : Form
    {
        public string s_Name;
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.s_Name = textBox1.Text;
            this.Hide();
        }

        private void textBox1_Enter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                button1_Click(sender, e);
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.textBox1.KeyDown += new KeyEventHandler(textBox1_Enter);
        }
        
    }
}
