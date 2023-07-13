using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IcomConnect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private TcpClient _tcpClient = null;

        private const string _ipAddrString = "192.168.1.12";
        private const int _port = 50001;

        private object _lockObject = new object();

        private Thread _readThread = null;
        private Thread _workThread = null;
        private ManualResetEvent _doWork = new ManualResetEvent(false);

        private Queue<byte[]> _sendqueue = new Queue<byte[]>();

        private bool _opened = false;

        private Queue<byte[]> _initialqueue = new Queue<byte[]>();

        public bool Open()
        {
            lock (_lockObject)
            {
                if (_workThread == null)
                {
                    _workThread = new Thread(new ThreadStart(WorkThread));
                    _workThread.Name = "TCP Work thread";
                    _workThread.IsBackground = true;
                    _workThread.Start();
                }
            }
            return true;
        }



        public MainWindow()
        {

            InitializeComponent();

            UdpClient udpClientControl = new UdpClient(_ipAddrString, _port);
            //UdpClient udpClientCIV = new UdpClient(RadioPortStart + 1);
            try
            {
                // Sends a message to the host to which you have connected.
                byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there?");

                udpClientControl.Send(sendBytes, sendBytes.Length);

                // Sends a message to a different host using optional hostname and port parameters.
                //UdpClient udpClientB = new UdpClient();
                //udpClientB.Send(sendBytes, sendBytes.Length, "AlternateHostMachineName", 11000);

                //IPEndPoint object will allow us to read datagrams sent from any source.
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // Blocks until a message returns on this socket from a remote host.
                byte[] receiveBytes = udpClientControl.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);

                // Uses the IPEndPoint object to determine which of these two hosts responded.
                Console.WriteLine("This is the message you received " +
                                             returnData.ToString());
                Console.WriteLine("This message was sent from " +
                                            RemoteIpEndPoint.Address.ToString() +
                                            " on their port number " +
                                            RemoteIpEndPoint.Port.ToString());

                udpClientControl.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
