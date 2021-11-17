using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Client
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Connect();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            Send();
            AddMessage(txbMessage.Text);

        }

        IPEndPoint IP;
        Socket client;
        // Kết nối tới server
        void Connect()
        {
            //IP: Địa chỉ của server
            IP = new IPEndPoint(IPAddress.Parse("127.0.0.11"), 9999);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            try
            {
                client.Connect(IP);
            }
            catch {
                MessageBox.Show("Khong the ket noi den SERVER!");
                return;
                    }

            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }

        // Đóng kết nối hiện thời
        void Close()
        {
            client.Close();
        }

        // Gửi tin
        void Send()
        {
            if (txbMessage.Text != string.Empty)
                client.Send(Serialize(txbMessage.Text));
        }

        // Nhận tin
        void Receive()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);
                    string message = (string)Deserialize(data);
                    AddMessage(message);
                }

            }
            catch {
                Close(); } }
       
        // ADD MESSAGE vào khung chat
        void AddMessage(string s)
        {
            lsvMessage.Items.Add(new ListViewItem() { Text = s }) ;
            txbMessage.Clear();

        }

        // Phân mảnh
        byte[] Serialize(object obj)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            return stream.ToArray();

        }

        // Gom mảnh lại 
        object Deserialize(byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(stream);
            
        }

        // Đóng kết nối khi đóng form
        private void Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }
    }
}
