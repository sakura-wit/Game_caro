using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GameCoCaro.ChessBoardManager;

namespace GameCoCaro
{
    public partial class Start : Form
    {
        
        public Start()
        {
            InitializeComponent();
        }

        private void btn_Play_Click(object sender, EventArgs e)
        {
            if(tbName.Text =="")
            {
                errorNull.SetError(tbName, "Vui lòng nhập tên!!");
            }
            else
            {
                this.Hide();
                GameMode gameMode = new GameMode();
                gameMode.MessageGM = tbName.Text;
                gameMode.ShowDialog();
                gameMode = null;
                //gameMode.Show();
            }
        }
    }
}
