using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI_First
{
    class Knight
    {
        public chessBasic canMoveRoute(int x, int y, chessSquare cq)
        {
            chessBasic cb = new chessBasic();
            int[,] dir = { { -1, 2 }, { 1, -2 },
                       { -1, -2 }, { -2, 1 },
                       { 2, -1 }, { -2, -1 },
                       { 1, 2 }, { 2, 1 } };
            for (int i = 0; i < 8; i++)//騎士的8個走法
            {
                int X = x + dir[i, 0];
                int Y = y + dir[i, 1];
                if (X >= 0 && X < 8 && Y >= 0 && Y < 8)//棋盤內
                {
                    if ((cq.PSquare[x, y] == chessSquare.chessSides.Black && cq.PSquare[X, Y] == chessSquare.chessSides.Black) ||
                        (cq.PSquare[x, y] == chessSquare.chessSides.White && cq.PSquare[X, Y] == chessSquare.chessSides.White)) continue;//不能黑棋吃黑棋、白棋吃白棋
                    if (cq.mSquare[X, Y] != chessSquare.chessName.Null)
                    {
                        cb.LCanCheck.Add(new int[] { X, Y });
                    }
                    else {
                        cb.LCanWalk.Add(new int[] { X, Y });
                    }
                }
            }
            return cb;
        }
    }
}
