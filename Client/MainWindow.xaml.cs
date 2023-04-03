using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RegistryKey reg;
        string g_sRecvData = String.Empty;
        SerialPort serial = new SerialPort();
        private ChatClient cc;
        public MainWindow()
        {
            reg = Registry.CurrentUser.CreateSubKey("Software").CreateSubKey("Contact").CreateSubKey("SETTING");
            InitializeComponent();
            cc = new ChatClient();
            this.DataContext = cc;
            Loaded += new RoutedEventHandler(InitSerialPort);
        }
        void InitSerialPort(object sender, EventArgs e)
        {
            serial.DataReceived += new SerialDataReceivedEventHandler(serial_DataReceived);

            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                //cbComPort.Items.Add(port);
            }
            
        }
        private void bSwitchClientState_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cc.SwitchClientState();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    //btnAging.IsEnabled = true;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void bSend_Click(object sender, RoutedEventArgs e)
        {
            cc.SendMessageTo(tbTargetUsername.Text, tbMessage.Text);
        }

        private void btnAging_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                //btnAging.IsEnabled = false;
                ((MainWindow)System.Windows.Application.Current.MainWindow).cc.count = Int32.Parse(Count.Text);
                cc.run = true;
                cc.SendMessageTo(tbTargetUsername.Text, "<CTACT,ON>");
            });
        }

        private void btnAgingStop_Click(object sender, RoutedEventArgs e)
        {
            //ZMotorCheck();
            Task.Run(() =>
            {
                while (true)
                {

                    cc.SendMessageTo("", "<CTACT,READY>");
                    Thread.Sleep(2000);
                }
            });
        }
        public static async Task ZMotorCheck()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ((MainWindow)System.Windows.Application.Current.MainWindow).cc.SendMessageTo("", "<CTACT,READY>");
                            Thread.Sleep(1000);
                        });
                    }
                    catch (Exception e)
                    {

                    }

                }
            });
        }
        internal void SendMessageTo(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        private void VBY1_Click(object sender, RoutedEventArgs e)
        {
            
            if (reg.GetValue("MASK") != null)
            {
                string str = reg.GetValue("MASK").ToString();
                reg.SetValue("MASK", str + ",VBY1");
            }
            else
            {
                reg.SetValue("MASK","VBY1");
            }
        }

        private void VBY2_Click(object sender, RoutedEventArgs e)
        {
            if (reg.GetValue("MASK") != null)
            {
                string str = reg.GetValue("MASK").ToString();
                reg.SetValue("MASK", str + ",VBY2");
            }
            else
            {
                reg.SetValue("MASK", "VBY2");
            }
        }

        private void VBY3_Click(object sender, RoutedEventArgs e)
        {
            if (reg.GetValue("MASK") != null)
            {
                string str = reg.GetValue("MASK").ToString();
                reg.SetValue("MASK", str + ",VBY3");
            }
            else
            {
                reg.SetValue("MASK", "VBY3");
            }
        }

        private void VBY4_Click(object sender, RoutedEventArgs e)
        {
            if (reg.GetValue("MASK") != null)
            {
                string str = reg.GetValue("MASK").ToString();
                reg.SetValue("MASK", str + ",VBY4");
            }
            else
            {
                reg.SetValue("MASK", "VBY4");
            }
        }

        private void DP1_Click(object sender, RoutedEventArgs e)
        {
            if (reg.GetValue("MASK") != null)
            {
                string str = reg.GetValue("MASK").ToString();
                reg.SetValue("MASK", str + ",DP1");
            }
            else
            {
                reg.SetValue("MASK", "DP1");
            }
        }

        private void DP2_Click(object sender, RoutedEventArgs e)
        {
            if (reg.GetValue("MASK") != null)
            {
                string str = reg.GetValue("MASK").ToString();
                reg.SetValue("MASK", str + ",DP2");
            }
            else
            {
                reg.SetValue("MASK", "DP2");
            }
        }

        private void LAN_Click(object sender, RoutedEventArgs e)
        {
            if (reg.GetValue("MASK") != null)
            {
                string str = reg.GetValue("MASK").ToString();
                reg.SetValue("MASK", str + ",LAN");
            }
            else
            {
                reg.SetValue("MASK", "LAN");
            }
        }

        private void PIN_Click(object sender, RoutedEventArgs e)
        {
            if (reg.GetValue("MASK") != null)
            {
                reg.DeleteValue("MASK");
            }
        }

        private void btnPin_Click(object sender, RoutedEventArgs e)
        {
            //cc.SendMessageTo(tbTargetUsername.Text, "<CTACT,VOL>");
            cc.SendMessageTo(tbTargetUsername.Text, "<CTACT,PINCHECK>");
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                //btnAging.IsEnabled = true;
            });
        }

        private void btnCountReset_Click(object sender, RoutedEventArgs e)
        {
            cc.countf = 0;
            cc.countt = 0;
            Application.Current.Dispatcher.Invoke(() =>
            {
                //CountF.Text = "0";
                //CountT.Text = "0";
            });
        }

        private void btging_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                cc.SendMessageTo(tbTargetUsername.Text, "<CTACT,ON>");
            });
        }

        private void btgingStop_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                cc.SendMessageTo(tbTargetUsername.Text, "<CTACT,OFF>");
            });
        }

        private void btStop_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                cc.SendMessageTo(tbTargetUsername.Text, "<CTACT,STOP>");
            });
        }

        private void RsConnect_Click(object sender, RoutedEventArgs e)
        {
            //string[] names = cbComSpeed.SelectedItem.ToString().Split(':');
            if (serial.IsOpen)
            {
                //RsConnect.Content = "Connect";
                serial.Close();
            }
            else
            {
                //serial.PortName = cbComPort.SelectedItem.ToString();
                serial.BaudRate = 115200;
                try
                {
                    serial.Open();
                    //RsConnect.Content = "DisConnect";
                    serialSend("<VER>");
                }
                catch (Exception ex)
                {

                }
            }
        }
        void serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                g_sRecvData = serial.ReadExisting();
                if ((g_sRecvData != string.Empty)) // && (g_sRecvData.Contains('\n')))
                {
                    Debug.WriteLine(g_sRecvData);
                }
            }
            catch (TimeoutException)
            {
                g_sRecvData = string.Empty;
            }
        }

        void serialSend(string cmd)
        {
            try
            {
                serial.Write(cmd);
            }
            catch (Exception e)
            {

            }
        }

        private void btnxr_Click(object sender, RoutedEventArgs e)
        {
            serialSend("<DIR,X,1><STP,X,333><RUN>");
        }

        private void btnxl_Click(object sender, RoutedEventArgs e)
        {
            serialSend("<DIR,X,0><STP,X,333><RUN>");
        }

        private void btnyr_Click(object sender, RoutedEventArgs e)
        {
            serialSend("<DIR,Y,1><STP,Y,333><RUN>");
        }

        private void btnyl_Click(object sender, RoutedEventArgs e)
        {
            serialSend("<DIR,Y,0><STP,Y,333><RUN>");
        }

        private void btntr_Click(object sender, RoutedEventArgs e)
        {
            serialSend("<DIR,R,1><STP,R,333><RUN>");
        }

        private void btntl_Click(object sender, RoutedEventArgs e)
        {
            serialSend("<DIR,R,0><STP,R,333><RUN>");
        }

        private void btnzu_Click(object sender, RoutedEventArgs e)
        {
            serialSend("<DIR,Z,0><STP,Z,333><RUN>");
        }

        private void btnzd_Click(object sender, RoutedEventArgs e)
        {
            serialSend("<DIR,Z,1><STP,Z,333><RUN>");
        }

        public int randomStep(Random rnd, int min, int max)
        {
            //Random rnd = new Random();
            int val = rnd.Next(min, max);
            return val;
        }

        Random rndom = new Random();

        public void MoveMsg()
        {
            //Random rnd = new Random();

            int StepX = randomStep(rndom, 100, 12000);
            
            int StepY = randomStep(rndom, 100, 12000);

            int StepT = randomStep(rndom, 100, 3000);

            string cmd = "";

            if(StepX > 6000)
            {
                cmd = string.Format("<DIR,X,1><STP,X,{0}>", (StepX - 6000));
            }
            else
            {
                cmd = string.Format("<DIR,X,0><STP,X,{0}>", (StepX));
            }

            if (StepY > 6000)
            {
                cmd += string.Format("<DIR,Y,1><STP,Y,{0}>", (StepY - 6000));
            }
            else
            {
                cmd += string.Format("<DIR,Y,0><STP,Y,{0}>", (StepY));
            }

            if (StepT > 1500)
            {
                cmd += string.Format("<DIR,R,1><STP,R,{0}>", (StepT - 1500));
            }
            else
            {
                cmd += string.Format("<DIR,R,0><STP,R,{0}>", (StepT));
            }

            cmd += "<RUN>";
            
            serialSend(cmd);
        }

        private void btncenter_Click(object sender, RoutedEventArgs e)
        {
            string cmd = "";
            cmd = string.Format("<HOM,Z>");
            serialSend(cmd);
            Thread.Sleep(3000);
            cmd = string.Format("<BPO,X,26468><BPO,Y,32000><BPO,R,7659><RUN>");
            serialSend(cmd);
            Thread.Sleep(1000);
            cmd = string.Format("<HOM,A,B>");
            serialSend(cmd);

        }
        public void MoveHMsg()
        {
            Task.Run(() =>
            {
                string cmd = "";
                cmd = string.Format("<HOM,Z>");
                serialSend(cmd);
                Thread.Sleep(3000);
                cmd = string.Format("<HOM,A,B>");
                serialSend(cmd);
            });
        }

        private void btns1_Click(object sender, RoutedEventArgs e)
        {
            cc.SendMessageTo(tbTargetUsername.Text, "<CTACT,PINID>");
        }

        private void btns2_Click(object sender, RoutedEventArgs e)
        {
            cc.SendMessageTo(tbTargetUsername.Text, "<CTACT,PINUSEDCOUNT>");
        }

        private void btns3_Click(object sender, RoutedEventArgs e)
        {
            cc.SendMessageTo(tbTargetUsername.Text, "<CTACT,AUTOTEACHING>");
        }

        private void Btns4_Click(object sender, RoutedEventArgs e)
        {
            //cc.SendMessageTo(tbTargetUsername.Text, "<MOM,A,1000,0,1000,0,1000,0>");
            cc.SendMessageTo(tbTargetUsername.Text, "<CTACT,PINPERCENT>");
        }


        private void Btn11_Click(object sender, RoutedEventArgs e)
        {
            cc.SendMessageTo(tbTargetUsername.Text, "[CTACT,12V,OK]");
        }

        private void Btn11_Copy_Click(object sender, RoutedEventArgs e)
        {
            cc.SendMessageTo(tbTargetUsername.Text, "[CTACT,12V,NG]");
        }

        private void Btn22_Click(object sender, RoutedEventArgs e)
        {
            cc.SendMessageTo(tbTargetUsername.Text, "<CTACT,PINCHECK>");
        }

        private void Btn22_Copy_Click(object sender, RoutedEventArgs e)
        {
            cc.SendMessageTo(tbTargetUsername.Text, "[CTACT,PINCHECK,NG]");
        }

        private void Btn33_Click(object sender, RoutedEventArgs e)
        {
            //cc.SendMessageTo(tbTargetUsername.Text, "<CTACT,VOL>");

            //cc.SendMessageTo(tbTargetUsername.Text, "<CTACT,PINCOUNT>");

            cc.SendMessageTo(tbTargetUsername.Text, "<CTACT,PINCOUNT>");
        }
    }
}