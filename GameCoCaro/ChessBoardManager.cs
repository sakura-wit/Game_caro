using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace GameCoCaro
{
    

    public class ChessBoardManager
    {

      

        #region Properties
        private Panel chessBoard;
        public Panel ChessBoard
        {
            get { return chessBoard; }
            set { chessBoard = value; }
        }

        
        private List<Player> player;
        public List<Player> Player 
        { 
            get => player;
            set => player = value;
        }

        private int currentPlayer;

        public int CurrentPlayer 
        {
            get => currentPlayer; 
            set => currentPlayer = value; 
        }

        private TextBox playerName;

        public TextBox PlayerName 
        { 
            get => playerName; 
            set => playerName = value;
        }

        private PictureBox playerMark;

        public PictureBox PlayerMark 
        { 
            get => playerMark; 
            set => playerMark = value;
        }
       

        private List<List<Button>> matrix;

        public List<List<Button>> Matrix 
        { 
            get => matrix; 
            set => matrix = value;
        }
        
        private event EventHandler<ButtonClickEvent> playerMarked;
        public event EventHandler<ButtonClickEvent> PlayerMarked
        {
            add
            {
                playerMarked += value;
            }
            remove
            {
                playerMarked -= value;
            }
        }

        private event EventHandler endedGame;
        public event EventHandler EndedGame
        {
            add
            {
                endedGame += value;
            }
            remove
            {
                endedGame -= value;
            }
        }
        #endregion

       

        #region Initialize
        public ChessBoardManager(Panel chessBoard, TextBox playerName, PictureBox mark,string text)
        {


            this.ChessBoard = chessBoard;
            this.PlayerName = playerName;
            this.PlayerMark = mark;
        //    this.Name = text;
            this.Player = new List<Player>()
            {
                //Tính năng còn thiếu là tên cho người dùng nhập và random xem ai đi trước
                new Player(text,Image.FromFile(Application.StartupPath + "\\Resources\\Icon X.png")),
                new Player(text,Image.FromFile(Application.StartupPath + "\\Resources\\Icon O.png"))
            };
        }
        #endregion

        #region Methods
        public void DrawChessBoard()
        {
            ChessBoard.Enabled = true;
            ChessBoard.Controls.Clear();    
            CurrentPlayer = 0;
            ChangePlayer();

            Matrix = new List<List<Button>>();

            Button oldButton = new Button() { Width = 0, Location = new Point(0, 0)};
            for(int i = 0; i < Cons.ChessBoard_Height; i++)
            {
                Matrix.Add(new List<Button>());

                for(int j = 0; j <= Cons.ChessBoard_Width; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Cons.ChessBoard_Width,
                        Height = Cons.ChessBoard_Height,
                        Location = new Point(oldButton.Location.X + oldButton.Width, oldButton.Location.Y),
                        BackgroundImageLayout = ImageLayout.Stretch,
                        Tag = i.ToString()
                    };

                    btn.Click += btn_Click;

                    ChessBoard.Controls.Add(btn);

                    Matrix[i].Add(btn);

                    oldButton = btn;
                }
                oldButton.Location = new Point(0, oldButton.Location.Y + Cons.ChessBoard_Height);
                oldButton.Width = 0;
                oldButton.Height = 0;
            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn.BackgroundImage != null)
                return;

            Mark(btn);

            CurrentPlayer = (CurrentPlayer == 0) ? 1 : 0;

            ChangePlayer();

            if(playerMarked != null)
            {
                playerMarked(this,new ButtonClickEvent(GetChessPoint(btn)));
            }

            if(IsEndGame(btn))
            {
                EndGame();
            }
        }

        public void OtherPlayerMark(Point point)
        {
            Button btn = Matrix[point.Y][point.X];

            if (btn.BackgroundImage != null)
                return;

            Mark(btn);

            CurrentPlayer = (CurrentPlayer == 0) ? 1 : 0;

            ChangePlayer();

            if (IsEndGame(btn))
            {
                EndGame();
            }
        }
        public void EndGame()
        {
            if(endedGame != null)
            {
                endedGame(this, new EventArgs());
            }
        }
        private bool IsEndGame(Button btn)
        {

            return IsEndHorizontal(btn) || IsEndVertical(btn) || IsEndPrimary(btn) || IsEndSub(btn);
        }
        private void Mark(Button btn)
        {
            btn.BackgroundImage = Player[CurrentPlayer].Mark;
        }
        public void ChangePlayer()
        {
            PlayerName.Text = Player[CurrentPlayer].Name;

            PlayerMark.Image = Player[CurrentPlayer].Mark;
        }
        private Point GetChessPoint(Button btn)
        {
            int vertical = Convert.ToInt32(btn.Tag);

            int horizontal = Matrix[vertical].IndexOf(btn);

            Point point = new Point(horizontal,vertical);

            return point;
        }

        private bool IsEndHorizontal(Button btn) //Kết thúc bằng hàng ngang
        {
            Point point = GetChessPoint(btn);
            int CountLeft = 0;
            for(int i = point.X; i >= 0; i-- )
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    CountLeft++;
                }
                else
                    break;
            }
            int CountRight = 0;
            for(int i = point.X + 1; i < Cons.ChessBoard_Width; i++)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    CountRight++;
                }
                else
                    break;
            }
            return CountLeft + CountRight == 5;
        }
        private bool IsEndVertical(Button btn) //Kết thúc bằng hàng dọc
        {
            Point point = GetChessPoint(btn);

            int countTop = 0;

            for (int i = point.Y; i >= 0; i--)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }

            int countBottom = 0;

            for (int i = point.Y + 1; i < Cons.ChessBoard_Height; i++)
            {
                if (Matrix[i][point.X].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }

            return countTop + countBottom == 5;
        }
        private bool IsEndPrimary(Button btn) // Kết thúc bằng đường chéo chính
        {
            Point point = GetChessPoint(btn);
            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.X - i < 0 || point.Y - i < 0)//xử lí ra khỏi bảng
                    break;
                if (Matrix[point.Y - i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }

            int countBottom = 0;
            for (int i = 1; i <= Cons.ChessBoard_Width - point.X; i++)
            {
                if (point.Y + i >= Cons.ChessBoard_Height || point.X + i >= Cons.ChessBoard_Width)
                    break;
                if (Matrix[point.Y + i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }
            return countTop + countBottom == 5;
        }
        private bool IsEndSub(Button btn) //Kết thúc bằng đường chéo phụ
        {
            Point point = GetChessPoint(btn);
            int countTop = 0;
            for (int i = 0; i <= point.X; i++)
            {
                if (point.X + i > Cons.ChessBoard_Width || point.Y - i < 0)//xử lí ra khỏi bảng
                    break;
                if (Matrix[point.Y - i][point.X + i].BackgroundImage == btn.BackgroundImage)
                {
                    countTop++;
                }
                else
                    break;
            }

            int countBottom = 0;
            for (int i = 1; i <= Cons.ChessBoard_Width - point.X; i++)
            {
                if (point.Y + i >= Cons.ChessBoard_Height || point.X - i < 0)
                    break;
                if (Matrix[point.Y + i][point.X - i].BackgroundImage == btn.BackgroundImage)
                {
                    countBottom++;
                }
                else
                    break;
            }
            return countTop + countBottom == 5;
        }
        public class ButtonClickEvent : EventArgs
        {
            private Point clickedPoint;

            public Point ClickedPoint { get => clickedPoint; set => clickedPoint = value; }

            public ButtonClickEvent(Point point)
            {
                this.ClickedPoint = point;
            }
        }
        #endregion

    }
    
}
