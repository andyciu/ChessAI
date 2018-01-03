using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI_First
{
    public class King
    {
        private chessSquare cq1;
        public chessBasic canMoveRoute(int x, int y, chessSquare cq)
        {
            cq1 = cq;
            chessBasic cb = new chessBasic();
            int[,] dir = { { -1, 1 }, { 0, 1 },
                       { 1, 1 }, { -1, 0 },
                       { 1, 0 }, { -1, -1 },
                       { 0, -1 }, { 1, -1 } };
            for (int i = 0; i < 8; i++)//國王的8個走法
            {
                int X = x + dir[i, 0];
                int Y = y + dir[i, 1];
                if (X >= 0 && X < 8 && Y >= 0 && Y < 8)//棋盤內
                {
                    if ((cq1.PSquare[x, y] == chessSquare.chessSides.Black && cq1.PSquare[X, Y] == chessSquare.chessSides.Black) ||
                        (cq1.PSquare[x, y] == chessSquare.chessSides.White && cq1.PSquare[X, Y] == chessSquare.chessSides.White)) continue;//不能黑棋吃黑棋、白棋吃白棋
                    if (cq1.mSquare[X, Y] != chessSquare.chessName.Null)
                    {
                        cb.LCanCheck.Add(new int[] { X, Y });
                    }
                    else {
                        cb.LCanWalk.Add(new int[] { X, Y });
                    }
                }
            }

            if (x == 4 && y == 0 && cq1.PSquare[4, 0] == chessSquare.chessSides.White)
            {
                //白國王[4,0]白城堡[0,0] 
                if (cq1.mSquare[0, 0] == chessSquare.chessName.Rook && Rule1((int)chessSquare.chessSides.White, (int)chessSquare.chessName.King, (int)chessSquare.chessSides.White, (int)chessSquare.chessName.Rook, 0)
                    && Rule2(y, 4, 0) && Rule3(0, y, 4, 0))
                {
                    cb.LSpecial.Add(new int[] { 2, 0, (int)chessSquare.chessSpecial.S_No2 });
                }
                //白國王[4,0]白城堡[7,0] 
                if (cq1.mSquare[7, 0] == chessSquare.chessName.Rook && Rule1((int)chessSquare.chessSides.White, (int)chessSquare.chessName.King, (int)chessSquare.chessSides.White, (int)chessSquare.chessName.Rook, 1)
                    && Rule2(y, 4, 7) && Rule3(0, y, 4, 7))
                {
                    cb.LSpecial.Add(new int[] { 6, 0, (int)chessSquare.chessSpecial.S_No2 });
                }
            }
            else if (x == 4 && y == 7 && cq1.PSquare[4, 7] == chessSquare.chessSides.Black)
            {
                // 黑國王[4,7]黑城堡[0,7] 
                if (cq1.mSquare[0, 7] == chessSquare.chessName.Rook && Rule1((int)chessSquare.chessSides.Black, (int)chessSquare.chessName.King, (int)chessSquare.chessSides.Black, (int)chessSquare.chessName.Rook, 0)
                    && Rule2(y, 4, 0) && Rule3(1, y, 4, 0))
                {
                    cb.LSpecial.Add(new int[] { 2, 7, (int)chessSquare.chessSpecial.S_No2 });
                }
                //黑國王[4,7]黑城堡[7,7] 
                if (cq1.mSquare[7, 7] == chessSquare.chessName.Rook && Rule1((int)chessSquare.chessSides.Black, (int)chessSquare.chessName.King, (int)chessSquare.chessSides.Black, (int)chessSquare.chessName.Rook, 1)
                    && Rule2(y, 4, 7) && Rule3(1, y, 4, 7))
                {
                    cb.LSpecial.Add(new int[] { 6, 7, (int)chessSquare.chessSpecial.S_No2 });
                }
            }
            return cb;
        }
        //1.王與要進行易位的城堡都未經移動。
        private bool Rule1(int kingFirst, int kingSecond, int rookFirst, int rookSecond, int rookThird)
        {
            if (cq1.ChessSquareXY[kingFirst - 1][kingSecond - 1][0][2] == 0
              && cq1.ChessSquareXY[rookFirst - 1][rookSecond - 1][rookThird][2] == 0)
            {
                return true;
            }
            return false;
        }
        //2.王與要進行易位的城堡之間沒有其他棋子阻隔。
        private bool Rule2(int Y, int kingX, int rookX)
        {
            for (int i = Math.Min(kingX, rookX) + 1; i < Math.Max(kingX, rookX); i++)
            {
                if (cq1.mSquare[i, Y] != chessSquare.chessName.Null) return false;
            }
            return true;
        }
        //3.王所在、經過和到達的格子皆未受到攻擊
        private bool Rule3(int kingIdx, int Y, int kingX, int rookX)
        {
            if (cq1.ChessSquareCheckK[kingIdx]) return false;//王所在
            for (int i = Math.Min(kingX, rookX) + 1; i < Math.Max(kingX, rookX); i++)//經過、到達
            {
                if (cq1.aSquare[i, Y] == chessSquare.squareAction.CanCheck
                    || cq1.aSquare[i, Y] == chessSquare.squareAction.CanWalk
                    || cq1.aSquare[i, Y] == chessSquare.squareAction.Special) return false;
            }
            return true;
        }
    }
}