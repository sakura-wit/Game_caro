using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameCoCaro
{
    public partial class PvsC : Form
    {
        private string messPC;

        public string MessPC
        {
            get { return messPC; }
            set { messPC = value; }
        }
        public PvsC()
        {
            InitializeComponent();
            pvsc = new PvsC(pnlChessBoard, txbPlayerName, pctbMark);
            pvsc.DrawChessBoard();
            pvsc.EndedGame += ChessBoard_EndedGame;
        }
        #region Cons
        public class Cons
        {
            public static int Chess_Width = 30;
            public static int Chess_Height = 30;
            public static int ChessBoard_Width = 22;
            public static int ChessBoard_Height = 19;
        }
        #endregion
        #region Properties
        PvsC pvsc;
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

        public PvsC(Panel chessBoard, TextBox playerName, PictureBox mark)
        {
            this.ChessBoard = chessBoard;
            this.PlayerName = playerName;
            this.PlayerMark = mark;
            this.Player = new List<Player>()
            {
                new Player("Player1",Image.FromFile(Application.StartupPath + "\\Resources\\Icon X.png")),
                new Player("COM",Image.FromFile(Application.StartupPath + "\\Resources\\Icon O.png"))
            };
        }

        private long[] AttackPoint = new long[7] { 0, 9, 54, 162, 1458, 13112, 118008 };
        private long[] DefensePoint = new long[7] { 0, 3, 27, 99, 729, 6561, 59049 };
        #endregion

        #region Exception
        public void DrawChessBoard()
        {
            ChessBoard.Controls.Clear();
            CurrentPlayer = 0;
            ChangePlayer();

            Matrix = new List<List<Button>>();

            Button oldButton = new Button() { Width = 0, Location = new Point(0, 0) };
            for (int i = 0; i < Cons.ChessBoard_Height; i++)
            {
                Matrix.Add(new List<Button>());

                for (int j = 0; j <= Cons.ChessBoard_Width; j++)
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

            CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;

            ChangePlayer();

            
            if (IsEndGame(btn))
            {
                EndGame();
            }
            else
            {
                CurrentPlayer = CurrentPlayer == 1 ? 0 : 1;
                ChangePlayer();
                StartCom(btn);
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
            if (endedGame != null)
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

            Point point = new Point(horizontal, vertical);

            return point;
        }

        private bool IsEndHorizontal(Button btn) //Kết thúc bằng hàng ngang
        {
            Point point = GetChessPoint(btn);
            int CountLeft = 0;
            for (int i = point.X; i >= 0; i--)
            {
                if (Matrix[point.Y][i].BackgroundImage == btn.BackgroundImage)
                {
                    CountLeft++;
                }
                else
                    break;
            }
            int CountRight = 0;
            for (int i = point.X + 1; i < Cons.ChessBoard_Width; i++)
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
        #region Method
        void EndGame_PvsC()
        {
            int check = 0;
            if (pvsc.CurrentPlayer == 0) check = 1;
            tmCountDown.Stop();
            pnlChessBoard.Enabled = false;
            MessageBox.Show("Chúc mừng " + pvsc.Player[check].Name + " đã giành được chiến thắng", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void NewGame()
        {
            //prcbCountDown.Value = 0;
            //tmCountDown.Stop();
            pvsc.DrawChessBoard();
            //tmCountDown.Start();
            pnlChessBoard.Enabled = true;
        }

        void QuitGame()
        {
            Dispose();
        }

        void ChessBoard_EndedGame(object sender, EventArgs e)
        {
            EndGame_PvsC();
        }


        #endregion

        #region AI
        public void StartCom(Button btn)
        {
            Point point = Find();
            btn = Matrix[point.Y][point.X];
            btn.BackgroundImage = Player[1].Mark;
            if (IsEndGame(btn))
            {
                EndGame();
            }
        }

        public Point Find()
        {
            Point Result = new Point();
            long Max = 0;
            for (int i = 0; i < Cons.ChessBoard_Width; i++)
            {
                for (int j = 0; j < Cons.ChessBoard_Height; j++)
                    if (Matrix[j][i].BackgroundImage == null)
                    {
                        long Attack = Attack_Doc(i, j) + Attack_Ngang(i, j) + Attack_CheoChinh(i, j) + Attack_CheoPhu(i, j);
                        long Defense = Defense_Doc(i, j) + Defense_Ngang(i, j) + Defense_CheoChinh(i, j) + Defense_CheoPhu(i, j);
                        long DiemTam = Attack > Defense ? Attack : Defense;
                        long DiemTong = (Attack + Defense) > DiemTam ? (Attack + Defense) : DiemTam;
                        if (DiemTong > Max)
                        {
                            Max = DiemTong;
                            Result = new Point(i, j);
                        }

                    }
            }
            return Result;
        }

        private long Attack_Doc(int dong, int cot)
        {
            long DiemTong = 0;

            int SoQuanTa = 0;
            int SoQuanDich = 0;
            int SoQuanTa2 = 0;
            int SoQuanDich2 = 0;

            for (int dem = 1; dem < 6 && dem + dong < Cons.ChessBoard_Height; dem++)
            {
                if (Matrix[cot][dong + dem].BackgroundImage == Player[1].Mark)
                    SoQuanTa++;
                else
                {
                    if (Matrix[cot][dong + dem].BackgroundImage == Player[0].Mark)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && dong + dem2 < Cons.ChessBoard_Height; dem2++)
                            if (Matrix[cot][dong + dem2].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                            }
                            else if (Matrix[cot][dong + dem2].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                                break;
                            }
                            else
                                break;
                        break;
                    }
                }
            }
            for (int dem = 1; dem < 6 && dong - dem >= 0; dem++)
            {
                if (Matrix[cot][dong - dem].BackgroundImage == Player[1].Mark)
                    SoQuanTa++;
                else
                {
                    if (Matrix[cot][dong - dem].BackgroundImage == Player[0].Mark)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && dong - dem2 >= 0; dem2++)
                            if (Matrix[cot][dong - dem2].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                            }
                            else if (Matrix[cot][dong - dem2].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                                break;
                            }
                            else
                                break;
                        break;
                    }
                }
            }



            if (SoQuanDich == 2)
                return 0;
            if (SoQuanDich == 0)
                DiemTong += AttackPoint[SoQuanTa] * 2;
            else
                DiemTong += AttackPoint[SoQuanTa];
            if (SoQuanDich2 == 0)
                DiemTong += AttackPoint[SoQuanTa2] * 2;
            else
                DiemTong += AttackPoint[SoQuanTa2];
            if (SoQuanTa >= SoQuanTa2)
                DiemTong -= 1;
            else
                DiemTong -= 2;
            if (SoQuanTa == 4)
                DiemTong *= 2;
            if (SoQuanTa == 0)
                DiemTong += DefensePoint[SoQuanDich] * 2;
            else
                DiemTong += DefensePoint[SoQuanDich];
            if (SoQuanTa2 == 0)
                DiemTong += DefensePoint[SoQuanDich2] * 2;
            else
                DiemTong += DefensePoint[SoQuanDich2];

            return DiemTong;
        }
        private long Attack_Ngang(int dong, int cot)
        {
            long DiemTong = 0;

            int SoQuanTa = 0;
            int SoQuanDich = 0;
            int SoQuanTa2 = 0;
            int SoQuanDich2 = 0;

            for (int dem = 1; dem < 6 && dem + cot < Cons.ChessBoard_Height; dem++)
            {
                if (Matrix[cot + dem][dong].BackgroundImage == Player[1].Mark)
                    SoQuanTa++;
                else
                {
                    if (Matrix[cot + dem][dong].BackgroundImage == Player[0].Mark)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && cot + dem2 < Cons.ChessBoard_Height; dem2++)
                            if (Matrix[cot + dem2][dong].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                            }
                            else if (Matrix[cot + dem2][dong].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                                break;
                            }
                            else
                                break;
                        break;
                    }
                }

            }
            for (int dem = 1; dem < 6 && cot - dem >= 0; dem++)
            {
                if (Matrix[cot - dem][dong].BackgroundImage == Player[1].Mark)
                    SoQuanTa++;
                else
                {
                    if (Matrix[cot - dem][dong].BackgroundImage == Player[0].Mark)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && cot - dem2 >= 0; dem2++)
                            if (Matrix[cot - dem2][dong].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                            }
                            else if (Matrix[cot - dem2][dong].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                                break;
                            }
                            else
                                break;
                        break;
                    }
                }
            }

            if (SoQuanDich == 2)
                return 0;
            if (SoQuanDich == 0)
                DiemTong += AttackPoint[SoQuanTa] * 2;
            else
                DiemTong += AttackPoint[SoQuanTa];
            if (SoQuanDich2 == 0)
                DiemTong += AttackPoint[SoQuanTa2] * 2;
            else
                DiemTong += AttackPoint[SoQuanTa2];
            if (SoQuanTa >= SoQuanTa2)
                DiemTong -= 1;
            else
                DiemTong -= 2;
            if (SoQuanTa == 4)
                DiemTong *= 2;
            if (SoQuanTa == 0)
                DiemTong += DefensePoint[SoQuanDich] * 2;
            else
                DiemTong += DefensePoint[SoQuanDich];
            if (SoQuanTa2 == 0)
                DiemTong += DefensePoint[SoQuanDich2] * 2;
            else
                DiemTong += DefensePoint[SoQuanDich2];

            return DiemTong;
        }
        private long Attack_CheoChinh(int dong, int cot)
        {
            long DiemTong = 0;

            int SoQuanTa = 0;
            int SoQuanDich = 0;
            int SoQuanTa2 = 0;
            int SoQuanDich2 = 0;

            for (int dem = 1; dem < 6 && dem + dong < Cons.ChessBoard_Width && cot + dem < Cons.ChessBoard_Height; dem++)
            {
                if (Matrix[cot + dem][dong + dem].BackgroundImage == Player[1].Mark)
                    SoQuanTa++;
                else
                {
                    if (Matrix[cot + dem][dong + dem].BackgroundImage == Player[0].Mark)
                    {

                        SoQuanDich++;
                        break;
                    }
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && cot + dem2 < Cons.ChessBoard_Height && dong + dem2 < Cons.ChessBoard_Width; dem2++)
                            if (Matrix[cot + dem2][dong + dem2].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                            }
                            else if (Matrix[cot + dem2][dong + dem2].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                                break;
                            }
                            else
                                break;
                        break;
                    }
                }
            }
            for (int dem = 1; dem < 6 && dong - dem >= 0 && cot - dem >= 0; dem++)
            {
                if (Matrix[cot - dem][dong - dem].BackgroundImage == Player[1].Mark)
                    SoQuanTa++;
                else
                {
                    if (Matrix[cot - dem][dong - dem].BackgroundImage == Player[0].Mark)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && cot - dem2 >= 0 && dong - dem2 >= 0; dem2++)
                            if (Matrix[cot - dem2][dong - dem2].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                            }
                            else if (Matrix[cot - dem2][dong - dem2].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                                break;
                            }
                            else
                                break;
                        break;
                    }
                }
            }

            if (SoQuanDich == 2)
                return 0;
            if (SoQuanDich == 0)
                DiemTong += AttackPoint[SoQuanTa] * 2;
            else
                DiemTong += AttackPoint[SoQuanTa];
            if (SoQuanDich2 == 0)
                DiemTong += AttackPoint[SoQuanTa2] * 2;
            else
                DiemTong += AttackPoint[SoQuanTa2];
            if (SoQuanTa >= SoQuanTa2)
                DiemTong -= 1;
            else
                DiemTong -= 2;

            if (SoQuanTa == 4)
                DiemTong *= 2;
            if (SoQuanTa == 0)
                DiemTong += DefensePoint[SoQuanDich] * 2;
            else
                DiemTong += DefensePoint[SoQuanDich];
            if (SoQuanTa2 == 0)
                DiemTong += DefensePoint[SoQuanDich2] * 2;
            else
                DiemTong += DefensePoint[SoQuanDich2];

            return DiemTong;
        }
        private long Attack_CheoPhu(int dong, int cot)
        {

            long DiemTong = 0;

            int SoQuanTa = 0;
            int SoQuanDich = 0;
            int SoQuanTa2 = 0;
            int SoQuanDich2 = 0;

            for (int dem = 1; dem < 6 && dem + dong < Cons.ChessBoard_Height && cot - dem >= 0; dem++)
            {
                if (Matrix[cot - dem][dong + dem].BackgroundImage == Player[1].Mark)
                    SoQuanTa++;
                else
                {
                    if (Matrix[cot - dem][dong + dem].BackgroundImage == Player[0].Mark)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && cot - dem2 >= 0 && dong + dem2 < Cons.ChessBoard_Height; dem2++)
                            if (Matrix[cot - dem2][dong + dem2].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                            }
                            else if (Matrix[cot - dem2][dong + dem2].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                                break;
                            }
                            else
                                break;
                        break;
                    }
                }
            }
            for (int dem = 1; dem < 6 && dong - dem >= 0 && cot + dem < Cons.ChessBoard_Height; dem++)
            {
                if (Matrix[cot + dem][dong - dem].BackgroundImage == Player[1].Mark)
                    SoQuanTa++;
                else
                {
                    if (Matrix[cot + dem][dong - dem].BackgroundImage == Player[0].Mark)
                    {
                        SoQuanDich++;
                        break;
                    }
                    else
                    {
                        for (int dem2 = 1; dem2 < 6 && cot + dem2 < Cons.ChessBoard_Width && dong - dem2 >= 0; dem2++)
                            if (Matrix[cot + dem2][dong - dem2].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                            }
                            else if (Matrix[cot + dem2][dong - dem2].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                                break;
                            }
                            else
                                break;
                        break;
                    }
                }
            }

            if (SoQuanDich == 2)
                return 0;
            if (SoQuanDich == 0)
                DiemTong += AttackPoint[SoQuanTa] * 2;
            else
                DiemTong += AttackPoint[SoQuanTa];
            if (SoQuanDich2 == 0)
                DiemTong += AttackPoint[SoQuanTa2] * 2;
            else
                DiemTong += AttackPoint[SoQuanTa2];
            if (SoQuanTa >= SoQuanTa2)
                DiemTong -= 1;
            else
                DiemTong -= 2;
            if (SoQuanTa == 4)
                DiemTong *= 2;
            if (SoQuanTa == 0)
                DiemTong += DefensePoint[SoQuanDich] * 2;
            else
                DiemTong += DefensePoint[SoQuanDich];
            if (SoQuanTa2 == 0)
                DiemTong += DefensePoint[SoQuanDich2] * 2;
            else
                DiemTong += DefensePoint[SoQuanDich2];

            return DiemTong;
        }
        private long Defense_Doc(int dong, int cot)
        {
            long DiemTong = 0;

            int SoQuanTa = 0;
            int SoQuanDich = 0;
            int SoQuanTa2 = 0;
            int SoQuanDich2 = 0;



            for (int dem = 1; dem < 6 && dem + dong < Cons.ChessBoard_Height; dem++)
            {
                if (Matrix[cot][dong + dem].BackgroundImage == Player[1].Mark)
                {
                    SoQuanTa++;
                    break;
                }

                else
                {
                    if (Matrix[cot][dong + dem].BackgroundImage == Player[0].Mark)
                        SoQuanDich++;
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && dong + dem2 < Cons.ChessBoard_Height; dem2++)
                            if (Matrix[cot][dong + dem2].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                                break;
                            }
                            else if (Matrix[cot][dong + dem2].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                            }
                            else
                                break;
                        break;
                    }
                }
            }
            for (int dem = 1; dem < 6 && dong - dem >= 0; dem++)
            {
                if (Matrix[cot][dong - dem].BackgroundImage == Player[1].Mark)
                {
                    SoQuanTa++;
                    break;
                }
                else
                {
                    if (Matrix[cot][dong - dem].BackgroundImage == Player[0].Mark)
                        SoQuanDich++;
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && dong - dem2 >= 0; dem2++)
                            if (Matrix[cot][dong - dem2].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                                break;
                            }
                            else if (Matrix[cot][dong - dem2].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                            }
                            else
                                break;
                        break;
                    }
                }
            }



            if (SoQuanTa == 2)
                return 0;
            if (SoQuanTa == 0)
                DiemTong += DefensePoint[SoQuanDich] * 2;
            else
                DiemTong += DefensePoint[SoQuanDich];
            if (SoQuanDich >= SoQuanDich2)
                DiemTong -= 1;
            else
                DiemTong -= 2;
            if (SoQuanDich == 4)
                DiemTong *= 2;

            return DiemTong;
        }
        private long Defense_Ngang(int dong, int cot)
        {
            long DiemTong = 0;

            int SoQuanTa = 0;
            int SoQuanDich = 0;
            int SoQuanTa2 = 0;
            int SoQuanDich2 = 0;

            for (int dem = 1; dem < 6 && dem + cot < Cons.ChessBoard_Height; dem++)
            {
                if (Matrix[cot + dem][dong].BackgroundImage == Player[1].Mark)
                {
                    SoQuanTa++;
                    break;
                }
                else
                {
                    if (Matrix[cot + dem][dong].BackgroundImage == Player[0].Mark)
                        SoQuanDich++;
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && cot + dem2 < Cons.ChessBoard_Height; dem2++)
                            if (Matrix[cot + dem2][dong].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                                break;
                            }
                            else if (Matrix[cot + dem2][dong].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                            }
                            else
                                break;
                        break;
                    }
                }
            }
            for (int dem = 1; dem < 6 && cot - dem >= 0; dem++)
            {
                if (Matrix[cot - dem][dong].BackgroundImage == Player[1].Mark)
                {
                    SoQuanTa++;
                    break;
                }
                else
                {
                    if (Matrix[cot - dem][dong].BackgroundImage == Player[0].Mark)
                        SoQuanDich++;
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && cot - dem2 >= 0; dem2++)
                            if (Matrix[cot - dem2][dong].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                                break;
                            }
                            else if (Matrix[cot - dem2][dong].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                            }
                            else break;
                        break;
                    }
                }
            }
            if (SoQuanTa == 2)
                return 0;
            if (SoQuanTa == 0)
                DiemTong += DefensePoint[SoQuanDich] * 2;
            else
                DiemTong += DefensePoint[SoQuanDich];
            if (SoQuanDich >= SoQuanDich2)
                DiemTong -= 1;
            else
                DiemTong -= 2;
            if (SoQuanDich == 4)
                DiemTong *= 2;

            return DiemTong;
        }
        private long Defense_CheoChinh(int dong, int cot)
        {
            long DiemTong = 0;

            int SoQuanTa = 0;
            int SoQuanDich = 0;
            int SoQuanTa2 = 0;
            int SoQuanDich2 = 0;

            for (int dem = 1; dem < 6 && dem + dong < Cons.ChessBoard_Width && cot + dem < Cons.ChessBoard_Height; dem++)
            {
                if (Matrix[cot + dem][dong + dem].BackgroundImage == Player[1].Mark)
                {
                    SoQuanTa++;
                    break;
                }
                else
                {
                    if (Matrix[cot + dem][dong + dem].BackgroundImage == Player[0].Mark)
                        SoQuanDich++;
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && dong + dem2 < Cons.ChessBoard_Width && cot + dem2 < Cons.ChessBoard_Height; dem2++)
                            if (Matrix[cot + dem2][dong + dem2].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                                break;
                            }
                            else if (Matrix[cot + dem2][dong + dem2].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                            }
                            else
                                break;
                        break;
                    }

                }
            }
            for (int dem = 1; dem < 6 && dong - dem >= 0 && cot - dem >= 0; dem++)
            {
                if (Matrix[cot - dem][dong - dem].BackgroundImage == Player[1].Mark)
                {
                    SoQuanTa++;
                    break;
                }
                else
                {
                    if (Matrix[cot - dem][dong - dem].BackgroundImage == Player[0].Mark)
                        SoQuanDich++;
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && cot - dem2 >= 0 && dong - dem2 >= 0; dem2++)
                            if (Matrix[cot - dem2][dong - dem2].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                                break;
                            }
                            else if (Matrix[cot - dem2][dong - dem2].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                            }
                            else
                                break;
                        break;
                    }

                }
            }

            if (SoQuanTa == 2)
                return 0;
            if (SoQuanTa == 0)
                DiemTong += DefensePoint[SoQuanDich] * 2;
            else
                DiemTong += DefensePoint[SoQuanDich];
            if (SoQuanDich >= SoQuanDich2)
                DiemTong -= 1;
            else
                DiemTong -= 2;
            if (SoQuanDich == 4)
                DiemTong *= 2;

            return DiemTong;
        }
        private long Defense_CheoPhu(int dong, int cot)
        {

            long DiemTong = 0;

            int SoQuanTa = 0;
            int SoQuanDich = 0;
            int SoQuanTa2 = 0;
            int SoQuanDich2 = 0;

            for (int dem = 1; dem < 6 && dem + dong < Cons.ChessBoard_Height && cot - dem >= 0; dem++)
            {
                if (Matrix[cot - dem][dong + dem].BackgroundImage == Player[1].Mark)
                {
                    SoQuanTa++;
                    break;
                }
                else
                {
                    if (Matrix[cot - dem][dong + dem].BackgroundImage == Player[0].Mark)
                        SoQuanDich++;
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && dong + dem2 < Cons.ChessBoard_Height && cot - dem2 >= 0; dem2++)
                            if (Matrix[cot - dem2][dong + dem2].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                                break;
                            }
                            else if (Matrix[cot + -dem2][dong + dem2].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                            }
                            else
                                break;
                        break;
                    }

                }
            }
            for (int dem = 1; dem < 6 && dong - dem >= 0 && cot + dem < Cons.ChessBoard_Height; dem++)
            {
                if (Matrix[cot + dem][dong - dem].BackgroundImage == Player[1].Mark)
                {
                    SoQuanTa++;
                    break;
                }
                else
                {
                    if (Matrix[cot + dem][dong - dem].BackgroundImage == Player[0].Mark)
                        SoQuanDich++;
                    else
                    {
                        for (int dem2 = 2; dem2 < 6 && dong - dem2 >= 0 && cot + dem2 < Cons.ChessBoard_Height; dem2++)
                            if (Matrix[cot + dem2][dong - dem2].BackgroundImage == Player[1].Mark)
                            {
                                SoQuanTa2++;
                                break;
                            }
                            else if (Matrix[cot + dem2][dong - dem2].BackgroundImage == Player[0].Mark)
                            {
                                SoQuanDich2++;
                            }
                            else
                                break;
                        break;
                    }
                }
            }

            if (SoQuanTa == 2)
                return 0;
            if (SoQuanTa == 0)
                DiemTong += DefensePoint[SoQuanDich] * 2;
            else
                DiemTong += DefensePoint[SoQuanDich];
            if (SoQuanDich >= SoQuanDich2)
                DiemTong -= 1;
            else
                DiemTong -= 2;
            if (SoQuanDich == 4)
                DiemTong *= 2;

            return DiemTong;
        }
        #endregion

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void quitGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuitGame();
        }

        private void tmCountDown_Tick(object sender, EventArgs e)
        {
            prcbCountDown.PerformStep();

            if (prcbCountDown.Value >= prcbCountDown.Maximum)
            {
                EndGame_PvsC();
            }
        }

        private void PvsC_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                QuitGame();
            }
            catch { }
            
        }

        private void rulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("-Hai bên lần lượt đánh vào từng\nô trên bàn cờ.\n-Bên nào đạt được 5 con trên 1" +
                "\nhàng ngang, dọc hoặc chéo\ntrước sẽ thắng." +
                "\n-Nếu đánh hết tất cả các ô trên" +
                "\nbàn cờ mà vẫn chưa có người" +
                "\nchiến thắng thì xem như hòa.", "Rules");
        }
    }
}
