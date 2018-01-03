using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI_First
{
    public class chessBasic
    {
        /// <summary>
        /// 諮詢棋子後可走之路徑格清單(X/Y-0~7)
        /// </summary>
        public List<int[]> LCanWalk;
        /// <summary>
        /// 諮詢棋子後可check格清單(X/Y-0~7)
        /// </summary>
        public List<int[]> LCanCheck;
        /// <summary>
        /// 諮詢棋子後該路徑格符合特殊規則清單(X-0~7/Y-0~7/chessSpecial)
        /// </summary>
        public List<int[]> LSpecial;
        /// <summary>
        /// (個體狀態)自身X座標(0~7)
        /// </summary>
        public int UX = -1;
        /// <summary>
        /// (個體狀態)自身Y座標(0~7)
        /// </summary>
        public int UY = -1;
        /// <summary>
        /// 自身類型
        /// </summary>
        public chessSquare.chessName cName = chessSquare.chessName.Null;
        public chessBasic()
        {
            LCanWalk = new List<int[]>();
            LCanCheck = new List<int[]>();
            LSpecial = new List<int[]>();
        }
        public chessBasic(int X, int Y, chessSquare.chessName NameIn)
        {
            UX = X;
            UY = Y;
            cName = NameIn;
            LCanWalk = new List<int[]>();
            LCanCheck = new List<int[]>();
            LSpecial = new List<int[]>();
        }
        public chessBasic(chessBasic cb)
        {
            UX = cb.UX;
            UY = cb.UY;
            cName = cb.cName;
            LCanWalk = new List<int[]>();
            LCanCheck = new List<int[]>();
            LSpecial = new List<int[]>();
            //LCanWalk = new List<int[]>(cb.LCanWalk);
            //LCanCheck = new List<int[]>(cb.LCanCheck);
            //LSpecial = new List<int[]>(cb.LSpecial);
        }
        public void canMoveRouteSUpdate(chessBasic cb)
        {
            if (cb == null) throw new NullReferenceException();

            //直接使用諮詢時新增之暫時物件
            LCanWalk = cb.LCanWalk;
            LCanCheck = cb.LCanCheck;
            LSpecial = cb.LSpecial;

        }
    }
}
