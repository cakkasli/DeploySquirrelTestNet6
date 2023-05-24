using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Squirrel;

namespace SquirrelTestNet6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            AddVersionNumber();
            CheckForUpdates();

        }

        private void AddVersionNumber()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            Dispatcher.Invoke(() =>
            {
                this.Title += $" v{ versionInfo.FileVersion }";
            });
        }

        private async void CheckForUpdates()
        {
            try
            {

                ReleaseEntry release = null;

                using (var mgr = await UpdateManager.GitHubUpdateManager("https://github.com/cakkasli/DeploySquirrelTestNet6"))
                {
                    UpdateInfo updateInfo = await mgr.CheckForUpdate();
                    if (updateInfo.ReleasesToApply.Any()) // Check if we have any update
                    {
                        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);


                        string msg = "New version available!" +
                                        "\n\nCurrent version: " + updateInfo.CurrentlyInstalledVersion.Version +
                                        "\nNew version: " + updateInfo.FutureReleaseEntry.Version +
                                        "\n\nUpdate application now?";
                        MessageBoxResult dialogResult = MessageBox.Show(msg, "Version Update", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (dialogResult == MessageBoxResult.Yes)
                        {
                            // Do the update
                            release = await mgr.UpdateApp();

                        }
                    }
                }

                // Restart the app
                if (release != null)
                {
                    UpdateManager.RestartApp();
                }
            }

            catch (Exception e)
            {
                Debug.WriteLine("Failed to check updates: " + e.ToString());
                ////MessageBox.Show(e.ToString());
                ///
                string subject = e.Message.ToString();
                //MessageBox.Show(e.ToString());

            }
        }
    }
}
