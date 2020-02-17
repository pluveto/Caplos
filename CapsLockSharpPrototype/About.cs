using System.Diagnostics;
using System.Windows.Forms;

namespace CapsLockSharpPrototype
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void LinkToGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe","https://github.com/pluveto/capslock-sharp");
        }

        private void LinkToDefaultShortcuts_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("explorer.exe", "https://www.pluvet.com/archives/calos.html");
        }

        private void About_Load(object sender, System.EventArgs e)
        {
            this.lblVersion.Text = $"v{System.Reflection.Assembly.GetEntryAssembly().GetName().Version} by Pluveto ";
        }
    }
}
