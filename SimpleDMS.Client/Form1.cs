using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleDMS.Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon1.Icon = new System.Drawing.Icon(@"Content/favicon.ico");
            notifyIcon1.Text = "SimpleDMS";
            notifyIcon1.Visible = true;
            notifyIcon1.ContextMenu = CreateTrayContextMenu();

            //ServerConnection server = new ServerConnection();
            //server.Start();

            NancyServer server = new NancyServer();
            server.Start();



        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private ContextMenu CreateTrayContextMenu()
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(CreateMenuItem(0, "Starte Server", new System.EventHandler(this.menuItem1_Click)));
            contextMenu.MenuItems.Add(CreateMenuItem(0, "Stoppe Server", new System.EventHandler(this.menuItem1_Click)));
            contextMenu.MenuItems.Add(CreateMenuItem(1, "Beenden", new System.EventHandler(this.menuItem1_Click)));

            return contextMenu;
        }

        private MenuItem CreateMenuItem(int index, string text, System.EventHandler eventHandler)
        {
            MenuItem menuItem = new MenuItem
            {
                Index = index,
                Text = text
            };
            menuItem.Click += eventHandler;

            return menuItem;
        }

        private void _startServer(object Sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuItem1_Click(object Sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
