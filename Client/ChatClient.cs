using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Client
{
    public class ChatClient : INotifyPropertyChanged
    {
        public int count = 0;
        public int line = 0;
        private Dispatcher _dispatcher;
        private Thread _thread;
        private Socket _socket;
        public int countt = 0;
        public int countf = 0;
        public bool run = false;
        public bool errCheck = false;
        public bool Ready = false;
        private IPAddress _ipAddress;
        public int ErrCount = 0;
        public byte[] inf;
        public string IpAddress
        {
            get
            {
                return _ipAddress.ToString();
            }
            set
            {
                if (this.IsClientConnected)
                    throw new Exception("Can't change this property when server is active");
                _ipAddress = IPAddress.Parse(value);
            }
        }

        private string _CountT;
        public string CountT
        {
            get
            {
                return _CountT;
            }
            set
            {
               
                this._CountT = value;
            }
        }
        private string _CountF;
        public string CountF
        {
            get
            {
                return _CountF;
            }
            set
            {
               
                this._CountF = value;
            }
        }


        private ushort _port;
        public ushort Port
        {
            get
            {
                return _port;
            }
            set
            {
                if (this.IsClientConnected)
                    throw new Exception("Can't change this property when server is active");
                this._port = value;
            }
        }


        private IPEndPoint _ipEndPoint => new IPEndPoint(_ipAddress, _port);

        private bool _isClientConnected;
        public bool IsClientConnected
        {
            get
            {
                return _isClientConnected;
            }
            private set
            {
                this._isClientConnected = value;

                this.NotifyPropertyChanged("IsClientConnected");
                this.NotifyPropertyChanged("IsClientDisconnected");
            }
        }
        public bool IsClientDisconnected => !this.IsClientConnected;

        private string _username;
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                this._username = value;
                if (this.IsClientConnected)
                {
                    //this.SetUsername(value);
                }
            }
        }

        public BindingList<String> lstChat { get; set; }

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        #endregion
        public ChatClient()
        {
            this._dispatcher = Dispatcher.CurrentDispatcher;
            this.lstChat = new BindingList<string>();
            this.CountT = "0";
            this.CountF = "0";
            this.IpAddress = "127.0.0.1";
            this.Port = 8010;
            this.Username = "Client" + new Random().Next(0, 99).ToString(); // random username
        }
        public static bool IsSocketConnected(Socket s)
        {
            if (!s.Connected)
                return false;

            if (s.Available == 0)
                if (s.Poll(1000, SelectMode.SelectRead))
                    return false;

            return true;
        }
        public void SwitchClientState()
        {
            if (!this.IsClientConnected)
                this.Connect();
            else
                this.Disconnect();
        }
        private void Connect()
        {
            if (this.IsClientConnected) return;

            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._socket.Connect(this._ipEndPoint);

            //SetUsername(this.Username);

            this._thread = new Thread(() => this.ReceiveMessages());
            this._thread.Start();

            this.IsClientConnected = true;
        }
        private void Disconnect()
        {
            if (!this.IsClientConnected) return;
            if (this._socket != null && this._thread != null)
            {
                //this._thread.Abort(); MainThread = null;
                this._socket.Shutdown(SocketShutdown.Both);
                //this._socket.Disconnect(false);
                this._socket.Dispose();
                this._socket = null;
                this._thread = null;
            }
            this.lstChat.Clear();

            this.IsClientConnected = false;
        }

        public int randomStep(Random rnd, int min, int max)
        {
            //Random rnd = new Random();
            int val = rnd.Next(min, max);
            return val;
        }

        Random rndom = new Random();

        public void ReceiveMessages()
        {
            int num = 0;
            while (true)
            {
                inf = new byte[1024];
                try
                {
                    if (!IsSocketConnected(this._socket))
                    {
                        this._dispatcher.Invoke(new Action(() =>
                        {
                            this.Disconnect();
                        }));
                        return;
                    }
                    int x = this._socket.Receive(inf);
                    if (x > 0)
                    {
                        string message = Encoding.Default.GetString(inf);
                        this._dispatcher.Invoke(new Action(() =>
                        {
                            line = this.lstChat.Count;
                            if (line > 30)
                            {
                                this.lstChat.Clear();
                            }
                        }));
                        if (run)
                        {

                            if (message.Contains("[VOL,"))
                            {
                                Task.Run(() =>
                                {
                                    this._dispatcher.Invoke(new Action(() =>
                                {
                                    this.lstChat.Add("VOL");
                                }));
                                    Thread.Sleep(1000);
                                    SendMessageTo("", "<CTACT,OFF>");
                                });
                            }
                            if (message.Contains("[PINCHECK,"))
                            {
                                Task.Run(() =>
                                {
                                    this._dispatcher.Invoke(new Action(() =>
                                {
                                    countt += 1;
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                    //    ((MainWindow)System.Windows.Application.Current.MainWindow).CountT.Text = string.Format("{0}", countt);
                                    });
                                    this.lstChat.Add("CHECK");
                                }));
                                    Thread.Sleep(1000);
                                    SendMessageTo("", "<CTACT,VOL>");
                                });
                            }

                            if (message.Contains("[CTACT,ON,OK"))
                            {
                                Task.Run(() =>
                                {
                                    this._dispatcher.Invoke(new Action(() =>
                                    {
                                        this.lstChat.Add("ON");
                                    }));
                                    Thread.Sleep(1000);
                                    SendMessageTo("", "<CTACT,PINCHECK>");
                                    //SendMessageTo("", "<CTACT,OFF>");
                                });
                            }
                            else if (message.Contains("[CTACT,ON,ERR"))
                            {
                                Task.Run(() =>
                                {
                                    var str = message.Split(',');
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        ((MainWindow)System.Windows.Application.Current.MainWindow).MoveHMsg();
                                    });
                                    Thread.Sleep(8000);
                                    SendMessageTo("", "<CTACT,ON>");
                                });
                            }
                            //else if (message.Contains("[CTACT,PINCHECK,ERR"))
                            //{
                            //    this._dispatcher.Invoke(new Action(() =>
                            //    {
                            //        countf += 1;
                            //        Application.Current.Dispatcher.Invoke(() =>
                            //        {
                            //            ((MainWindow)System.Windows.Application.Current.MainWindow).CountF.Text = string.Format("{0}", countf);
                            //        });
                            //        this.lstChat.Add("PINCHECK,ERR");
                            //    }));
                            //    Thread.Sleep(1000);
                            //    SendMessageTo("", "<CTACT,VOL>");

                            //}
                            else if (message.Contains("[CTACT,OFF,ERR"))
                            {
                                this._dispatcher.Invoke(new Action(() =>
                                {
                                    this.lstChat.Add("OFFERR");
                                }));
                                Thread.Sleep(4000);
                                SendMessageTo("", "<CTACT,OFF>");
                            }
                            else if (message.Contains("[CTACT,OFF,OK"))
                            {
                                Task.Run(() =>
                                {
                                    this._dispatcher.Invoke(new Action(() =>
                                    {
                                        this.lstChat.Add("OFF");
                                    }));

                                    int StepX = randomStep(rndom, 100, 6000);
                                    int StepY = randomStep(rndom, 100, 6000);
                                    int StepT = randomStep(rndom, 100, 12000);

                                    string cmd = "";
                                    if (StepX > 3000)
                                    {
                                        cmd = string.Format("{0},1,", (StepX - 3000));
                                    }
                                    else
                                    {
                                        cmd = string.Format("{0},0,", (StepX));
                                    }

                                    if (StepY > 3000)
                                    {
                                        cmd = cmd + string.Format("{0},1,", (StepY - 3000));
                                    }
                                    else
                                    {
                                        cmd = cmd + string.Format("{0},0,", (StepY));
                                    }

                                    if (StepT > 6000)
                                    {
                                        cmd = cmd + string.Format("{0},1", (StepT - 6000));
                                    }
                                    else
                                    {
                                        cmd = cmd + string.Format("{0},0", (StepT));
                                    }

                                    cmd = "<MOM,A," + cmd + ">";

                                    Thread.Sleep(500);

                                    SendMessageTo("", cmd);
                                });
                            }
                            else if (message.Contains("[MOM,OK"))
                            {
                                this._dispatcher.Invoke(new Action(() =>
                                {
                                    this.lstChat.Add("MOM,ALL");
                                }));
                                Thread.Sleep(1000);
                                SendMessageTo("", "<CTACT,ON>");
                            }
                        }//if(run)

                        if (message.Contains("[CTACT,BOTLEDON]"))
                        {
                            Task.Run(() =>
                            {
                                SendMessageTo("", "<CTACT,BOTLEDON,OK>");
                            });
                        }

                        if (message.Contains("<CTACT,12V,ON>"))
                        {
                            Thread.Sleep(2000);
                            SendMessageTo("", "[CTACT,12V,OK] ");
                        }
                        if (message.Contains("<CTACT,12V,OFF>"))
                        {
                            SendMessageTo("", "[CTACT,12V,OFF,OK]");
                        }
                        else if (message.Contains("<CTACT,PINCHECK,"))
                        {
                            SendMessageTo("", "[CTACT,PINCHECK,OK]");
                        }


                    }
                    
                }
                catch (Exception)
                {
                    this._dispatcher.Invoke(new Action(() =>
                    {
                        this.Disconnect();
                    }));

                    return;
                }
            }
        }

        //private void SetUsername(string newUsername)
        //{
        //    string cmd = string.Format("/setname {0}", newUsername);

        //    this._socket.Send(Encoding.Unicode.GetBytes(cmd));
        //}
        public void SendMessageTo(string targetUsername, string message)
        {
            try
            {
                string cmd = string.Format("{0}", message);
                byte[] StrByte = Encoding.UTF8.GetBytes(message);
                this._socket.Send(StrByte);
            }catch(Exception ex)
            {
                Connect();
                Debug.WriteLine(ex.ToString());
            }
        }
        
    }
}
