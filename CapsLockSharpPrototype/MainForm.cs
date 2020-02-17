using System;
using System.Windows.Forms;
using CapsLockSharpPrototype.Helper;
using CapsLockSharpPrototype.Runtime;
using Microsoft.Win32;

namespace CapsLockSharpPrototype
{
    public partial class MainForm : Form
    {


        public static NotifyIcon NotifyIcon { get; set; }
        public MainForm()
        {
            InitializeComponent();
            WindowState = FormWindowState.Minimized;
            Helper.KeyDefRuntime.KeyDefs = Helper.KeyDefRuntime.SetUpKeyDefs();
            TrayIcon.RefleshIcon(notifyIcon);
        }

        private void QuitMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            foreach (var item in KeyDefRuntime.KeyDefs)
            {
                if(item.Type == KeyDefRuntime.FuncType.Replace)
                {
                    keysListView.Items.Add(new ListViewItem(new[] { $"[CapsLock] + {item.AdditionKey.ToString()}", item.ReplacingKey.ToString() }));
                }
                
            }
            NotifyIcon = notifyIcon;
            new Controller().SetupKeyboardHooks((x,y)=> {
                TrayIcon.RefleshIcon(notifyIcon);
            });
            CheckStartWithSystem();
        }
        private void CheckStartWithSystem()
        {
            startWithSystem.Checked = AutoStart.CheckEnabled(Text);            
        }


        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            var x = new About();
            x.ShowDialog();
            x.Dispose();
        }
        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Visible = true;
            ShowInTaskbar = true;
            Activate();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            WindowState = FormWindowState.Minimized;
            Hide();
            e.Cancel = true;
            //notifyIcon.Visible = false;
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ShowInTaskbar = false;
            }
        }

        private void HelpMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            Activate();
        }

        private void startWithSystem_CheckedChanged(object sender, EventArgs e)
        {
            var currentEnabled = AutoStart.CheckEnabled(Text);
            if(startWithSystem.Checked == currentEnabled)
            {
                return;
            }
            if (startWithSystem.Checked)
            {
               AutoStart.Enable(Text, "\"" + Application.ExecutablePath + "\"");
            }
            else
            {
                AutoStart.Disable(Text);
            }
        }
    }
}
