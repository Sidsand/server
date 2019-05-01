using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
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
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace WpfApp2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int port = 55555;
        const string address = "127.0.0.1";
        TcpClient client = null;
        NetworkStream stream;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                if (name != null)
                {
                    string nam = name.Text;
                    string mes = message.Text;
                    dialog.Items.Add(name + ": " + mes);

                    byte[] data = Encoding.Unicode.GetBytes(mes);
                    //  stream.Write(data, 0, data.Length);
                }
            }
            catch { MessageBox.Show("Пожалуйста, введите имя"); }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            client = new TcpClient(address, port);
            stream = client.GetStream();

            Thread myThread1 = new Thread(new ThreadStart(Victory));
            myThread1.Start();
        }

        public void Victory()
        {
            while (true)
            {
                byte[] data = new byte[64];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                }
                while (stream.DataAvailable);

                string mes = builder.ToString();

                Dispatcher.BeginInvoke(new Action(() => dialog.Items.Add("server: " + mes)));
            }
        }

        

        private void Dconnect_Click_1(object sender, RoutedEventArgs e)
        {
             string mes = "Отключился";
            byte[] data = Encoding.Unicode.GetBytes(mes);
            stream.Write(data, 0, data.Length);
            stream.Close();
            client.Close();
        }
    }
}
