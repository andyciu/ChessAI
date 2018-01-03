using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI_First
{
    class Rook
    {
        public chessBasic canMoveRoute(int x, int y, chessSquare cq)
        {
            chessBasic cb = new chessBasic();
            int[,] dir = { { 0, 1 }, { 1, 0 },
                       { 0, -1 }, { -1, 0 }};
            for (int i = 0; i < 4; i++)//城堡上下左右4個方向
            {
                int X = x + dir[i, 0];
                int Y = y + dir[i, 1];
                int flag = 1;
                for (int j = 0; j < 8 && flag == 1; j++)
                {
                    if (X >= 0 && X < 8 && Y >= 0 && Y < 8)//棋盤內
                    {
                        if ((cq.PSquare[x, y] == chessSquare.chessSides.Black && cq.PSquare[X, Y] == chessSquare.chessSides.Black) ||
                            (cq.PSquare[x, y] == chessSquare.chessSides.White && cq.PSquare[X, Y] == chessSquare.chessSides.White)) continue;//不能黑棋吃黑棋、白棋吃白棋
                        if (cq.mSquare[X, Y] != chessSquare.chessName.Null)
                        {
                            cb.LCanCheck.Add(new int[] { X, Y });
                            flag = 0;
                        }
                        else {
                            cb.LCanWalk.Add(new int[] { X, Y });
                        }
                    }
                    X = X + dir[i, 0];
                    Y = Y + dir[i, 1];
                }
            }
            return cb;
        }
    }
}
