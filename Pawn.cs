using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI_First
{
    class Pawn
    {
        private chessSquare cq1;
        private chessWalkPoint cwp;
        private chessBasic cb;
        /// <summary>
        /// 是否為諮詢模式標記
        /// </summary> 
        public bool aSquareUpdateCheck = false;
        /// <summary>
        /// 取得遊戲紀錄之上一步路線物件-予委派函式
        /// </summary>
        public static D_GetWalkRoute DGetWalkRouteFuc;
        public chessBasic canMoveRoute(int x, int y, chessSquare cq)
        {
            cwp = DGetWalkRouteFuc();
            if (cwp == null) cwp = new chessWalkPoint(-1, -1, -1, -1);

            cq1 = cq;
            int[,] dir;
            cb = new chessBasic();
            if (cq1.PSquare[x, y] == chessSquare.chessSides.Black)
            {
                if (y == 6)
                {
                    dir = new int[,] { { -1, -1 }, { 0, -1 }, { 1, -1 }, { 0, -2 } };//左,中1,右,中2
                    checkRoute(x, y, dir);
                }
                else
                {
                    dir = new int[,] { { -1, -1 }, { 0, -1 }, { 1, -1 } };//左,中1,右
                    checkRoute(x, y, dir);
                }
            }
            else
            {
                if (y == 1)
                {
                    dir = new int[,] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { 0, 2 } };//左,中1,右,中2
                    checkRoute(x, y, dir);
                }
                else
                {
                    dir = new int[,] { { -1, 1 }, { 0, 1 }, { 1, 1 } };//左,中1,右
                    checkRoute(x, y, dir);
                }
            }
            dir = new int[,] { { -1, 0 }, { 1, 0 } };
            specialPawnCheckRoute(x, y, dir);//吃過路兵
            return cb;
        }
        private void checkRoute(int x, int y, int[,] dir)
        {
            bool flag = false;
            for (int i = 0; i < dir.Length / 2; i++)//士兵的?個走法
            {
                int X = x + dir[i, 0];
                int Y = y + dir[i, 1];
                if (X >= 0 && X < 8 && Y >= 0 && Y < 8)//棋盤內
                {
                    if ((cq1.PSquare[x, y] == chessSquare.chessSides.Black && cq1.PSquare[X, Y] == chessSquare.chessSides.Black) ||
                        (cq1.PSquare[x, y] == chessSquare.chessSides.White && cq1.PSquare[X, Y] == chessSquare.chessSides.White))
                    {
                        if (i == 1) flag = true;
                        continue;//不能黑棋吃黑棋、白棋吃白棋
                    }

                    if ((cq1.mSquare[X, Y] != chessSquare.chessName.Null) ||
                        (aSquareUpdateCheck == true && (i == 0 || i == 2)))
                    {
                        if (i == 0 || i == 2)
                        {
                            if (Y == 0 || Y == 7) cb.LSpecial.Add(new int[] { X, Y, (int)chessSquare.chessSpecial.S_No3 });
                            else cb.LCanCheck.Add(new int[] { X, Y });
                        }
                        else if (i == 1)
                        {
                            flag = true;
                        }
                    }
                    else
                    {
                        if (i == 1)
                        {
                            if (Y == 0 || Y == 7) cb.LSpecial.Add(new int[] { X, Y, (int)chessSquare.chessSpecial.S_No3 });
                            else cb.LCanWalk.Add(new int[] { X, Y });
                        }
                        else if (i == 3 && flag == false)
                        {
                            if (Y == 0 || Y == 7) cb.LSpecial.Add(new int[] { X, Y, (int)chessSquare.chessSpecial.S_No3 });
                            else cb.LCanWalk.Add(new int[] { X, Y });
                        }
                    }
                }
            }
        }
        private void specialPawnCheckRoute(int x, int y, int[,] dir)
        {
            if (cq1.PSquare[x, y] == chessSquare.chessSides.White && y == 4)
            {
                for (int i = 0; i < dir.Length / 2; i++)
                {
                    int X = x + dir[i, 0];
                    int Y = y + dir[i, 1];
                    if (X >= 0 && X < 8 && Y >= 0 && Y < 8)
                    {
                        if (cq1.PSquare[X, Y] != chessSquare.chessSides.Black ||
                         cq1.mSquare[X, Y] != chessSquare.chessName.Pawn ||
                         cq1.mSquare[X, Y + 1] != chessSquare.chessName.Null ||
                         cq1.ChessSquareXY[(int)chessSquare.chessSides.Black - 1][(int)chessSquare.chessName.Pawn - 1][X][2] > 1 ||
                         !(cwp.nNumX - 1 == X && cwp.nNumY - 1 == Y))
                        {
                            continue;//白兵吃黑兵，我方兵才可以吃過路兵，斜前為空，敵方只走一步，敵方走完上一步
                        }
                        cb.LSpecial.Add(new int[] { X, Y, (int)chessSquare.chessSpecial.S_No1 });
                    }
                }

            }
            else if (cq1.PSquare[x, y] == chessSquare.chessSides.Black && y == 3)
            {
                for (int i = 0; i < dir.Length / 2; i++)
                {
                    int X = x + dir[i, 0];
                    int Y = y + dir[i, 1];
                    if (X >= 0 && X < 8 && Y >= 0 && Y < 8)
                    {
                        if (cq1.PSquare[X, Y] != chessSquare.chessSides.White ||
                         cq1.mSquare[X, Y] != chessSquare.chessName.Pawn ||
                         cq1.mSquare[X, Y - 1] != chessSquare.chessName.Null ||
                         cq1.ChessSquareXY[(int)chessSquare.chessSides.White - 1][(int)chessSquare.chessName.Pawn - 1][X][2] > 1 ||
                         !(cwp.nNumX - 1 == X && cwp.nNumY - 1 == Y))
                        {
                            continue;//黑兵吃白兵，兵才可以吃過路兵，斜後為空，敵方只走一步，敵方走完上一步
                        }
                        cb.LSpecial.Add(new int[] { X, Y, (int)chessSquare.chessSpecial.S_No1 });
                    }
                }
            }
        }
    }
}
