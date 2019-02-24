using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
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

namespace SimpleInternetControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        enum InternetStatus
        {
            Disconnected = 0,
            Connecting = 1,
            Connected = 2,
            Disconnecting = 3,
            HardwareNotPresent = 4,
            HardwareDisabled = 5,
            HardwareMalfunction = 6,
            MediaDisconnected = 7,
            Authenticating = 8,
            AuthenticationSucceeded = 9,
            AuthenticationFailed = 10,
            InvalidAddress = 11,
            CredentialsRequired = 12,
            Other
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SelectQuery wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(wmiQuery);
            foreach (ManagementObject item in searchProcedure.Get())
            {
                combobox_interface.Items.Add((string)item["NetConnectionId"]);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string strSelectedItem = combobox_interface.SelectedValue.ToString();
            if (GetInterfaceStatus(strSelectedItem))
            {
                DisableInternet(strSelectedItem);
                ChangeButtonStatus(false);
            }
            else
            {
                EnableInternet(strSelectedItem);
                ChangeButtonStatus(true);
            }
        }

        private void Combobox_interface_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string strSelectedItem = combobox_interface.SelectedValue.ToString();
            ChangeButtonStatus(GetInterfaceStatus(strSelectedItem));
        }

        private void ChangeButtonStatus(bool status)
        {
            btn_switch.Content = (status) ? "Disable" : "Enable";
        }

        private bool GetInterfaceStatus(string interface_name)
        {
            SelectQuery wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(wmiQuery);
            foreach (ManagementObject item in searchProcedure.Get())
            {
                if (((string)item["NetConnectionId"]) == interface_name)
                {
                    if (Convert.ToInt32(item["NetConnectionStatus"]) == (int)InternetStatus.Connected)
                    {
                        return true;
                    }
                    return false;
                }
            }

            return false;
        }

        private void EnableInternet(string interface_name)
        {
            SelectQuery wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(wmiQuery);
            foreach (ManagementObject item in searchProcedure.Get())
            {
                if (((string)item["NetConnectionId"]) == interface_name)
                {
                    item.InvokeMethod("Enable", null);
                    break;
                }
            }
        }

        private void DisableInternet(string interface_name)
        {
            SelectQuery wmiQuery = new SelectQuery("SELECT * FROM Win32_NetworkAdapter WHERE NetConnectionId != NULL");
            ManagementObjectSearcher searchProcedure = new ManagementObjectSearcher(wmiQuery);
            foreach (ManagementObject item in searchProcedure.Get())
            {
                if (((string)item["NetConnectionId"]) == interface_name)
                {
                    item.InvokeMethod("Disable", null);
                    break;
                }
            }
        }

        
    }
}
