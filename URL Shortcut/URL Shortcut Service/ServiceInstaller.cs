using System.ComponentModel;

namespace URL_Shortcut_Service
{
    [RunInstaller(true)]
    public partial class ServiceInstaller : System.Configuration.Install.Installer
    {
        public ServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
