using System;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Samba.Infrastructure.Messaging;
using Samba.Presentation.Common.ErrorReport;
using Samba.Presentation.Services;
using Samba.Services.Common;
using System.Globalization;

namespace Samba.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var aes = new AES();
            var licenseType = RegistryHelper.readRegistryKey("licenseType");
            if (licenseType == null)
            {
                RegistryHelper.writeRegistryKey("licenseType", aes.EncryptToString("trial"));
                RegistryHelper.writeRegistryKey("trialExpirationDate", aes.EncryptToString(DateTime.UtcNow.AddDays(15).ToString("dd:MM:yyyy")));
                RegistryHelper.writeRegistryKey("lastCheckDate", aes.EncryptToString(DateTime.UtcNow.ToString("dd:MM:yyyy")));
            }
            else if (RegistryHelper.readRegistryKey("licenseType") == aes.EncryptToString("trial"))
            {

                var trialExpirationDate = DateTime.ParseExact(aes.DecryptString(RegistryHelper.readRegistryKey("trialExpirationDate")), "dd:MM:yyyy", new CultureInfo("en-US"));
                var lastCheckDate = DateTime.ParseExact(aes.DecryptString(RegistryHelper.readRegistryKey("lastCheckDate")), "dd:MM:yyyy", new CultureInfo("en-US"));

                if (lastCheckDate > DateTime.UtcNow || trialExpirationDate < DateTime.UtcNow)
                {
                    MessageBox.Show("Problem with the license, please contact the technical support on the following eamil: senlite.tech@gmail.com.");
                    return;
                }
            }
            else if (RegistryHelper.readRegistryKey("licenseType") == aes.EncryptToString("full version"))
            {

            }
            else
            {
                MessageBox.Show("Problem with the license, please contact the technical support on the following eamil: senlite.tech@gmail.com.");
                return;
            }
#if (DEBUG)
            RunInDebugMode();
#else
            RunInReleaseMode();
#endif
            ServiceLocator.Current.GetInstance<IApplicationState>().NotifyEvent(RuleEventNames.ApplicationStarted, new { Arguments = string.Join(" ", e.Args) });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            MessagingClient.Stop();
            ServiceLocator.Current.GetInstance<ITriggerService>().CloseTriggers();
        }

        private static void RunInDebugMode()
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private static void RunInReleaseMode()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
            try
            {
                var bootstrapper = new Bootstrapper();
                bootstrapper.Run();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }

        private static void HandleException(Exception ex)
        {
            if (ex == null) return;
            ExceptionReporter.Show(ex);
            Environment.Exit(1);
        }
    }
}
