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
using static GameCoCaro.SocketManager;


namespace GameCoCaro
{
    public partial class Chat : Form
    {
        UdpClient pc = new UdpClient(7777);
        
        string data = "";
        private string _message;
        private string _message1;

       

        public Chat()
        {
            InitializeComponent();
        }

        

        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        public string Message1
        {
            get { return _message1; }
            set { _message1 = value; }
        }

        string IP = "";

        byte[] ChuyenDoi(string tb)
        {
            byte[] array = Encoding.UTF8.GetBytes(tb);
            return array;

        }

        void SendData()
        {


            IPEndPoint IP1 = new IPEndPoint(IPAddress.Parse(tb_IP.Text), 7777);
            
            pc.Send(ChuyenDoi(textBox1.Text), ChuyenDoi(textBox1.Text).Length, IP1);

        }

        void ReceiveData(IAsyncResult res)
        {
            try
            {
                IP = _message1;
                IPEndPoint IP1 = new IPEndPoint(IPAddress.Parse(IP), 7777);
                byte[] received = pc.EndReceive(res, ref IP1);
                data = Encoding.UTF8.GetString(received);

                this.Invoke(new MethodInvoker(delegate
                {

                    richTextBox1.Text += tb_IP.Text + ": " + data.ToString() + "\n";

                }));
            }
            catch { }
            

        }

        private void bt_send_Click_1(object sender, EventArgs e)
        {
            

            richTextBox1.Text += _message1 + ": " + textBox1.Text + "\n";

            SendData();

            textBox1.Text = string.Empty;
        }

        private void bt_Close_Click(object sender, EventArgs e)
        {
            pc.Close();
            Chat.ActiveForm .Close();
        }

        private void Chat_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < _message.Length; i++)
            {
                if (_message[i] == ':')
                {
                    tb_IP.Text = _message.Remove(i);
                }
            }

            

        }

        private void Chat_Activated(object sender, EventArgs e)
        {
            int k=0;
            while (k!= 1000)
            {
                try
                {
                    pc.BeginReceive(new AsyncCallback(ReceiveData), null);
                }
                catch (Exception ex)
                {
                    richTextBox1.Text = ex.Message.ToString();
                }
                k++;
            }

        }
    }
}
