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
    public partial class GameMode : Form
    {


        
        

        public GameMode()
        {
            InitializeComponent();
        }

        //Lấy dữ liệu từ form khác cho GameMode
        private string messGM;
        public string MessageGM
        {
            get { return messGM; }
            set { messGM = value; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            this.Hide();
            PvsP form1 = new PvsP();
            form1.MessageF1 = messGM;
            form1.ShowDialog();
            form1 = null;
            this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            PvsC pvsc = new PvsC();
            //pvsc.MessPC = messGM;
            pvsc.ShowDialog();
            pvsc = null;
            this.Show();
        }

        private void GameMode_Load(object sender, EventArgs e)
        {
            
        }

        private void GameMode_Shown(object sender, EventArgs e)
        {
           
        }
    }
}
