using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace ArhciLab2sharp {
    public partial class Server : Form {
        // делегат для ожидания соединения
        private delegate void Connect();
        // делегат для приема сообщений
        private delegate void Send();

        Socket socket;
        Socket client;
        EndPoint end;
        byte[] buffer;

        public Server() {
            InitializeComponent();
            // чтобы не ругалось на то, что компоненты формы
            // используются в другом потоке
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e) {
            // инициализация сокета
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // говорим, чтобы сервер использовал любой ip
            IPAddress ip = IPAddress.Any;
            // созаем параметр для привязки сокета к адресу
            IPEndPoint ipe = new IPEndPoint(ip, 8080);
            end = (EndPoint)ipe;
            // привязка сокета к адресу
            socket.Bind(ipe);
            // указываем количество запросов на соединение в очереди
            socket.Listen(1);
            label1.Text = "Порт в режиме ожидания соединения";
            // выполняется асинхронно в потоке, тем самым программа не зависает
            //new Form1("192.168.1.131", "8080");
            new Connect(delegate () { Conect(); }).BeginInvoke(null, null);
        }
        // подключение клиента к серверу
        private void Conect() {
            client = socket.Accept();
            label1.Text = "Соединение установлено";
        }
        // метод для получения сообщений и отправки ответа
        private void SendMessage() {
            try {
                // получение сообщения
                buffer = new byte[256];
                client.ReceiveFrom(buffer, ref end);
                Mess(buffer);
                // отправка ответа
                string str = "Сообщение получено";
                byte[] service = Encoding.Default.GetBytes(str);
                client.Send(service, service.Length, 0);
            }
            catch // если соединение разовано, то завешение приложения
            {
                Quit();
            }
        }
        // кнопка выхода
        private void button1_Click(object sender, EventArgs e) {
            Quit();
        }

        private void button2_Click(object sender, EventArgs e) {
            // ожидание сообщения не вызывает зависание программы
            new Send(delegate () { Connected(); }).BeginInvoke(null, null);
        }
        // метод для запуска автоматического приема сообщений
        private void Connected() {
            while (true) {
                SendMessage();
            }
        }
        // обновление информации в текстовом окне
        private void Mess(byte[] buf) {
            richTextBox1.Text += Encoding.Default.GetString(buf);
        }
        // метод для завершения приложения
        private void Quit() {
            socket.Close();
            Application.Exit();
        }
    }
}
