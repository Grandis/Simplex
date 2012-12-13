using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Simplex
{
    public partial class Form1 : Form
    {
        public static Example example;
        public static Count count;

        public Form1()
        {
            InitializeComponent();
            example = null;
            count = null;
        }

        //Изменение размеров формы
        private void Form1_Resize(object sender, EventArgs e)
        {

            //Изменение размера самостоятельно (из-за MenuStrip)
            if (example != null)
            {
                example.Location = new Point(0, menuStrip1.Height);
                example.Size = new Size(ClientSize.Width, ClientSize.Height - menuStrip1.Height);
            }
            
            if (count != null)
            {
                count.Location = new Point(0, menuStrip1.Height);
                count.Size = new Size(ClientSize.Width, ClientSize.Height - menuStrip1.Height);
            }
            

        }

        private void demoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (count != null)
            {
                count.Dispose();
                count = null;
            }
            if (example != null) return;

            example = new Example();
            example.Parent = this;

            Text = "Демонстрационный пример";
            Form1_Resize(null, null);
        }

        private void countToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (example != null)
            {
                example.Dispose();
                example = null;
            }
            if (count != null) return;

            count = new Count();
            count.Parent = this;

            Text = "Решение";
            Form1_Resize(null, null);
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "help.chm");
        }

        
    }
}
