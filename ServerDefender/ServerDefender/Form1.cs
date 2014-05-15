using Alta.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServerDefender
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            if (this.label2.Text != Ressources.server.Clients.Count().ToString())
            {
                this.label2.Text = Ressources.server.Clients.Count().ToString();
                this.listBox1.Items.Clear();
                try
                {
                    foreach (ClientData client in Ressources.server.Clients)
                    {
                        this.listBox1.Items.Add(((Player)(client.ClientState)).Pseudo);
                    }
                }
                catch {}
            }
            timer1.Start();
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            int currentIndex = listBox1.SelectedIndex;
            if (currentIndex > -1)
            {
                contextMenuStrip1.Show(new Point(this.Location.X + listBox1.Location.X + e.X, this.Location.Y + listBox1.Location.Y + e.Y));
            }
        }

        private void hgtfhfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Ressources.kick(listBox1.Items[listBox1.SelectedIndex].ToString());
        }
    }
}
