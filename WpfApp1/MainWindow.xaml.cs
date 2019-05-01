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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //прослушиваемый порт
        const int port = 55555;
        //объект, прослушивающий порт
        static TcpListener listener;
        public MainWindow()
        {
            InitializeComponent();
        }

        //функция обработки сообщений от клиента
        public void Process(TcpClient tcpClient)
        {
            TcpClient client = tcpClient;
            NetworkStream stream = null;
            try
            {
                //получение потока для обмена сообщениями
                stream = client.GetStream();
                // буфер для получаемых данных
                byte[] data = new byte[64];
                //цикл обработки сообщений
                while (true)
                {
                    //объект, для формирования строк
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    //до тех пор, пока в потоке есть данные
                    do
                    {
                        //из потока считываются 64 байта и записываются в data
                        bytes = stream.Read(data, 0, data.Length);
                        //из считанных данных формируется строка
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    //преобразование сообщения
                    string message = builder.ToString();
                    //вывод сообщения в консоль сервера
                    Console.WriteLine(message);
                    //преобразование сообщения в набор байт
                    data = Encoding.Unicode.GetBytes(message);
                    //отправка сообщения обратно клиенту
                    stream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //освобождение ресурсов при завершении сеанса
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }

        public void Potok()
        {
            try
            {
                while (true)
                {
                    //принятие запроса на подключение
                    TcpClient client = listener.AcceptTcpClient();
                    //создание нового потока для обслуживания нового клиента
                    Thread clientThread = new Thread(() => Process(client));
                    Dispatcher.BeginInvoke(new Action(() => op.Items.Add("Новое соединение")));
                    clientThread.Start();
                }
            }
            catch
            {
                MessageBox.Show("Упс.. Сервер остановлен");
            }
        }


            private void Button_Click(object sender, RoutedEventArgs e)
        {
            //создание объекта для отслеживания сообщений переданных с ip адреса через порт
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            //начало прослушивания
            listener.Start();
            op.Items.Add("Сервер включен");

            
            Thread clientThread = new Thread(() => Potok());
            clientThread.Start();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            listener.Stop();
            op.Items.Add("Сервер остановлен");
        }
    }
}
