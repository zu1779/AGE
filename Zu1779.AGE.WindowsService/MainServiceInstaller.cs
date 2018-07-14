namespace Zu1779.AGE.WindowsService
{
    using System.ComponentModel;
    using System.Configuration.Install;

    [RunInstaller(true)]
    public partial class MainServiceInstaller : Installer
    {
        public MainServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
