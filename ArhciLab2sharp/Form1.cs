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
    public partial class Form1 : Form {
        // делегат для получения ответа
        private delegate void Answer();

        string IP;
        string Port;
        Socket socket;
        EndPoint end;

        public Form1(string ip, string port) {
            InitializeComponent();
            this.IP = ip;
            this.Port = port;
            // чтобы не ругалось на то, что компоненты формы
            // используются в другом потоке
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        // кнопка закрытия формы
        private void button1_Click(object sender, EventArgs e) {
            socket.Close();
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e) {
            // инициализация сокета
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // создание параметра для подключения к серверу
            IPAddress ip = IPAddress.Parse(IP);
            IPEndPoint ipe = new IPEndPoint(ip, int.Parse(Port));
            end = (EndPoint)ipe;
            try {
                socket.Connect(ipe);
                this.Text += " - Соединение установлено";
            }
            catch // на случай каких-либо проблем
            {
                MessageBox.Show("Проблемы с установкой соединения.\nПриложение будет закрыто.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
        // отправка сообщения
        private void button2_Click(object sender, EventArgs e) {
            string str = richTextBox1.Text;
            byte[] buffer = Encoding.Default.GetBytes(str);
            socket.Send(buffer, buffer.Length, 0);
            // ожидание ответа от сервера
            new Answer(delegate () { Answ(); }).BeginInvoke(null, null);
        }
        // метод для получения ответа
        private void Answ() {
            byte[] answer = new byte[64];
            socket.ReceiveFrom(answer, 0, ref end);
            textBox1.Text = Encoding.Default.GetString(answer);
        }
        // очистка окна сообщений от сервера
        private void richTextBox1_MouseClick(object sender, MouseEventArgs e) {
            textBox1.Text = "";
        }
    }
}
