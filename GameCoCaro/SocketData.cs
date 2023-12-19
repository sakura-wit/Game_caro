using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCoCaro
{
    [Serializable]
    public class SocketData
    {
        private int command;
        public int Command { get => command; set => command = value; }

        private Point point;
        public Point Point { get => point; set => point = value; }


        private String messege;
        public string Messege { get => messege; set => messege = value; }

    
        public SocketData(int command, Point point, String messege)
        {
            this.Command = command;
            this.Point = point;
            this.Messege = messege;
        }
    }

    public enum SocketCommand
    {
        SEND_POINT,
        NEW_GAME,
        RQ_NEW_GAME,
        QUIT,
        START
    }

}
