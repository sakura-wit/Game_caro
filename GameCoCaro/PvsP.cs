using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GameCoCaro.ChessBoardManager;
using System.Net;
using System.Net.Sockets;


namespace GameCoCaro
{
    public partial class PvsP : Form
    {

        

        //Lấy dữ liệu từ Form1
        private string messF1;
        public string MessageF1
        {
            get { return messF1; }
            set { messF1 = value; }
        }

        string IPLocal = "";
        string text= " Đối Thủ ";



        #region Properties
        ChessBoardManager ChessBoard;
        SocketManager socket;
        #endregion
        public PvsP()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            ChessBoard = new ChessBoardManager(pnlChessBoard,txbPlayerName, pctbMark,text);
            ChessBoard.EndedGame += ChessBoard_EndedGame;
            ChessBoard.PlayerMarked += ChessBoard_PlayerMarked;

            prcbCountDown.Step = Cons.CountDown_Step;
            prcbCountDown.Maximum = Cons.CountDown_Time;
            prcbCountDown.Value = 0;
            tmCountDown.Interval = Cons.CountDown_Interval;

            socket = new SocketManager();
                      

            NewGame();
            pnlChessBoard.Enabled = false;
            newGameToolStripMenuItem.Enabled = false;
            
        }

        

        #region Methods
        void EndGame()
        {
            int check = 0;
            if (ChessBoard.CurrentPlayer == 0) check = 1;
            tmCountDown.Stop();
            pnlChessBoard.Enabled = false;
            MessageBox.Show(ChessBoard.Player[check].Name,"Winn" +
                "er");
        }

        void NewGame()
        {
            prcbCountDown.Value = 0;
            tmCountDown.Stop();
            ChessBoard.DrawChessBoard();
        }

        void QuitGame()
        {
            Dispose();
        }

        void ChessBoard_PlayerMarked(object sender, ButtonClickEvent e)
        {
            tmCountDown.Start();
            pnlChessBoard.Enabled = false;
            prcbCountDown.Value = 0;
            socket.Send(new SocketData((int)SocketCommand.SEND_POINT, e.ClickedPoint, ""));
        }

        void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame();
        }

        private void tmCountDown_Tick(object sender, EventArgs e)
        {
            prcbCountDown.PerformStep();

            if (prcbCountDown.Value >= prcbCountDown.Maximum)
            {
                EndGame();
            }
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            socket.Send(new SocketData((int)SocketCommand.RQ_NEW_GAME, new Point(), ""));
        }

        private void quitGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuitGame();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn thoát", "Thông báo", MessageBoxButtons.OKCancel) != DialogResult.OK)
                e.Cancel = true;
            else
            {
                try
                {
                    socket.Send(new SocketData((int)SocketCommand.QUIT, new Point(), "")); // quit sẽ gửi thông báo cho phía kia
                }
                catch { }
            }
        }
        
        private void Form1_Shown(object sender, EventArgs e)
        {
            txbIP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);

            txbPlayerName.Text = messF1;

            IPLocal = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);

            if (string.IsNullOrEmpty(txbIP.Text))
            {
                txbIP.Text = socket.GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            }
            
            
        }
        public void Listen()
        {
            Thread listenThread = new Thread(() =>
            {
                try //tránh lỗi 1 bên thoát 
                {
                    SocketData data = (SocketData)socket.Receive();
                    
                    ProcessData(data);
                }
                catch (Exception e)
                {
                }
            });
            listenThread.IsBackground = true;
            listenThread.Start();

        }
        public void ProcessData(SocketData data)
        {
            switch (data.Command)
            {
                case (int)SocketCommand.NEW_GAME:
                    this.Invoke((MethodInvoker)(() =>
                    {
                        NewGame();
                        pnlChessBoard.Enabled = true;
                    }));
                    if (!socket.isServer)
                    {
                        ChessBoard.CurrentPlayer = 1;
                        ChessBoard.ChangePlayer();
                    }
                    break;
                case (int)SocketCommand.RQ_NEW_GAME:
                    if (MessageBox.Show("Đối thủ muốn chơi ván mới", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) != DialogResult.OK)
                        socket.Send(MessageBox.Show("Bắt đầu ván đầu mới"));
                    else
                    {
                        socket.Send(new SocketData((int)SocketCommand.NEW_GAME, new Point(), ""));
                        this.Invoke((MethodInvoker)(() =>
                        {
                            NewGame();
                            pnlChessBoard.Enabled = false;
                        }));
                        if (socket.isServer)
                        {
                            ChessBoard.CurrentPlayer = 1;
                            ChessBoard.ChangePlayer();
                        }
                    }
                    break;
                case (int)SocketCommand.SEND_POINT:
                    this.Invoke((MethodInvoker)(() =>   //thay đổi giao diện
                    {
                        prcbCountDown.Value = 0;
                        pnlChessBoard.Enabled = true;
                        tmCountDown.Start();
                        ChessBoard.OtherPlayerMark(data.Point);
                    }));
                    newGameToolStripMenuItem.Enabled = true;
                    break;
                case (int)SocketCommand.QUIT:
                    tmCountDown.Stop();
                    MessageBox.Show("Đối thủ đã thoát", "Thông báo");
                    this.Invoke((MethodInvoker)(() =>
                    {
                        NewGame();
                        pnlChessBoard.Enabled = false;
                    }));
                    if (socket.isServer)
                    {
                        socket.CloseConnect();
                    }
                    btnLan.Enabled = true;
                    newGameToolStripMenuItem.Enabled = false;
                    break;
                case (int)SocketCommand.START:
                    MessageBox.Show(data.Messege);
                    newGameToolStripMenuItem.Enabled = true;
                    pnlChessBoard.Enabled = true;
                    break;
                default:
                    break;
            }

            Listen();
        }
        private void btnLan_Click(object sender, EventArgs e)
        {
            socket.IP = txbIP.Text; //lấy địa chỉ IP ở textbox
            
            
            
            if (!socket.ConnectToServer())
            {
                socket.isServer = true;
                pnlChessBoard.Enabled = false;
                btnLan.Enabled = false;
                btnPlay.Enabled = true;
                ChessBoard.Player[0].Name = txbPlayerName.Text;
                pctbMark.BackgroundImage = ChessBoard.Player[0].Mark;
                socket.CreateServer();
                MessageBox.Show("Nhấn PLAY đến khi tìm được đối thủ", "Hướng dẫn");
                
            }
            else
            {
                socket.isServer = false;
                pnlChessBoard.Enabled = false;
                btnLan.Enabled = false;
                ChessBoard.Player[1].Name = txbPlayerName.Text;
                txbPlayerName.Text = ChessBoard.Player[1].Name;
                pctbMark.BackgroundImage = ChessBoard.Player[1].Mark;
                MessageBox.Show("Đã kết nối", "Thông báo");
                socket.Send(new SocketData((int)SocketCommand.START, new Point(), "Đã tìm thấy đối thủ"));
                Listen();
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
           
            Listen();
        }
        #endregion

        private void bt_Chat_Click(object sender, EventArgs e)
        {
            Chat c = new Chat();
            
            c.Message1 = IPLocal;
            c.Message = socket.GetIPClient();

            c.Show();
            
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void rulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("-Hai bên lần lượt đánh vào từng\nô trên bàn cờ.\n-Bên nào đạt được 5 con trên 1\nhàng ngang, dọc hoặc chéo\ntrước sẽ thắng.\n-Nếu đánh hết tất cả các ô trên\nbàn cờ mà vẫn chưa có người\nchiến thắng thì xem như hòa.", "Rules");
        }
    }
}



