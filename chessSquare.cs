using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI_First
{
    /// <summary>
    /// 棋盤地圖物件
    /// </summary>
    public class chessSquare
    {
        /// <summary>
        /// 棋類型標記(a~h,1~8)>enum列舉(chessName)
        /// </summary>
        public chessName[,] mSquare = new chessName[8, 8];
        /// <summary>
        /// 棋勢力範圍數(a~h,1~8)
        /// </summary>
        public int[,] ISquare = new int[8, 8];
        /// <summary>
        /// 棋雙方標記(a~h,1~8)>enum列舉(chessSides)
        /// </summary>
        public chessSides[,] PSquare = new chessSides[8, 8];
        /// <summary>
        /// 棋格子行動標記>enum列舉(squareAction)
        /// </summary>
        public squareAction[,] aSquare = new squareAction[8, 8];
        /// <summary>
        /// 諮詢棋子後可走之路徑格清單(X/Y-0~7)
        /// </summary>
        //public List<int[]> LCanWalk = new List<int[]>();
        /// <summary>
        /// 諮詢棋子後可check格清單(X/Y-0~7)
        /// </summary>
        //public List<int[]> LCanCheck = new List<int[]>();
        /// <summary>
        /// 諮詢棋子後該路徑格符合特殊規則清單(X-0~7/Y-0~7/chessSpecial)
        /// </summary>
        //public List<int[]> LSpecial = new List<int[]>();
        /// <summary>
        /// 棋子基本類別List
        /// </summary>
        public List<chessBasic> LChessBasic;
        /// <summary>
        /// 棋子座標紀錄
        /// (chessSides-1,chessName-1,第n個,X-0~7/Y-0~7/該棋子總共走之步數/LChessBasic編號)
        /// </summary>
        public int[][][][] ChessSquareXY = new int[2][][][];
        /// <summary>
        /// 雙方總共行動步數紀錄(chessSides-1)
        /// </summary>
        public int[] ChessSquareTotWalk = new int[2];
        /// <summary>
        /// 雙方國王是否被Check旗標(chessSides-1)
        /// </summary>
        public bool[] ChessSquareCheckK = new bool[] { false, false };
        /// <summary>
        /// 棋局雙方是否使用過特殊2號規則（國王/城堡易位）
        /// </summary>
        public bool[] IsChessSpecialNo2Used = new bool[] { false, false };
        /// <summary>
        /// (Visual Studio限定)詢問GUI特別3號規則士兵升變之類型-予委派
        /// </summary>
        public static D_QNSSpecialNo3 dQnsSpecailNO3Fnc = null;
        /// <summary>
        /// 雙方於50步和局規則中總共記數紀錄(chessSides-1)
        /// </summary>
        public int[] ChessSquareDrawWalk = new int[2];
        /// <summary>
        /// 棋子類型名稱列舉
        /// </summary>
        public enum chessName : int
        {
            Null,
            Pawn,
            Knight,
            Bishop,
            Rook,
            Queen,
            King
        }
        /// <summary>
        /// 棋子陣營方列舉
        /// </summary>
        public enum chessSides : int
        {
            Null,
            White,
            Black
        }
        /// <summary>
        /// 棋格可動作型態列舉
        /// </summary>
        public enum squareAction : int
        {
            Null,
            /// <summary>
            /// 該格於可行動範圍中
            /// </summary>
            CanWalk,
            /// <summary>
            /// 該格上之棋子處於可吃之狀態
            /// </summary>
            CanCheck,
            /// <summary>
            /// 該格屬特殊規則走法
            /// </summary>
            Special
        }
        /// <summary>
        /// 特殊規則列舉
        /// </summary>
        public enum chessSpecial
        {
            Null,
            /// <summary>
            /// 特殊1號規則(路過棋子)
            /// </summary>
            S_No1,
            /// <summary>
            /// 特殊2號規則(國王/城堡易位)
            /// </summary>
            S_No2,
            /// <summary>
            /// 特殊3號規則(士兵升變)
            /// </summary>
            S_No3
        }
        //諮詢用物件
        private Bishop bishop = new Bishop();
        private King king = new King();
        private Knight knight = new Knight();
        private Pawn pawn = new Pawn();
        private Queen queen = new Queen();
        private Rook rook = new Rook();
        /// <summary>
        /// 建構式（初始）
        /// </summary>
        public chessSquare()
        {
            //chessSquare csqP;
            LChessBasic = new List<chessBasic>();
            int NumLchessbasic = 0;
            for (int i = 0; i < 2; ++i)
            {
                ChessSquareXY[i]= new int[6][][];
                for (int j = 0; j < 6; ++j)
                {
                    if (j == 0) ChessSquareXY[i][j] = new int[8][];  //Pawn
                    else if(j>=4) ChessSquareXY[i][j] = new int[1][];  //Queen,King
                    else ChessSquareXY[i][j] = new int[2][];
                    for(int k = 0; k < ChessSquareXY[i][j].Length; ++k)
                    {
                        ChessSquareXY[i][j][k] = new int[4];
                    }
                }
            }
            for (int j = 0; j < 8; ++j)
            {
                mSquare[j, 1] = chessName.Pawn;
                PSquare[j, 1] = chessSides.White;
                ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Pawn - 1][j][0] = j;
                ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Pawn - 1][j][1] = 1;
                ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Pawn - 1][j][3] = NumLchessbasic++;
                LChessBasic.Add(new chessBasic(j, 1, chessName.Pawn));
                mSquare[j, 6] = chessName.Pawn;
                PSquare[j, 6] = chessSides.Black;
                ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Pawn - 1][j][0] = j;
                ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Pawn - 1][j][1] = 6;
                ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Pawn - 1][j][3] = NumLchessbasic++;
                LChessBasic.Add(new chessBasic(j, 6, chessName.Pawn));
            }
            mSquare[0, 0] = chessName.Rook;
            PSquare[0, 0] = chessSides.White;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Rook - 1][0][0] = 0;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Rook - 1][0][1] = 0;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Rook - 1][0][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(0, 0, chessName.Rook));
            mSquare[7, 0] = chessName.Rook;
            PSquare[7, 0] = chessSides.White;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Rook - 1][1][0] = 7;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Rook - 1][1][1] = 0;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Rook - 1][1][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(7, 0, chessName.Rook));
            //======================
            mSquare[0, 7] = chessName.Rook;
            PSquare[0, 7] = chessSides.Black;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Rook - 1][0][0] = 0;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Rook - 1][0][1] = 7;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Rook - 1][0][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(0, 7, chessName.Rook));
            mSquare[7, 7] = chessName.Rook;
            PSquare[7, 7] = chessSides.Black;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Rook - 1][1][0] = 7;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Rook - 1][1][1] = 7;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Rook - 1][1][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(7, 7, chessName.Rook));
            //======================
            mSquare[1, 0] = chessName.Knight;
            PSquare[1, 0] = chessSides.White;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Knight - 1][0][0] = 1;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Knight - 1][0][1] = 0;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Knight - 1][0][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(1, 0, chessName.Knight));
            mSquare[6, 0] = chessName.Knight;
            PSquare[6, 0] = chessSides.White;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Knight - 1][1][0] = 6;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Knight - 1][1][1] = 0;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Knight - 1][1][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(6, 0, chessName.Knight));
            //======================
            mSquare[1, 7] = chessName.Knight;
            PSquare[1, 7] = chessSides.Black;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Knight - 1][0][0] = 1;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Knight - 1][0][1] = 7;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Knight - 1][0][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(1, 7, chessName.Knight));
            mSquare[6, 7] = chessName.Knight;
            PSquare[6, 7] = chessSides.Black;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Knight - 1][1][0] = 6;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Knight - 1][1][1] = 7;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Knight - 1][1][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(6, 7, chessName.Knight));
            //======================
            mSquare[2, 0] = chessName.Bishop;
            PSquare[2, 0] = chessSides.White;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Bishop - 1][0][0] = 2;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Bishop - 1][0][1] = 0;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Bishop - 1][0][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(2, 0, chessName.Bishop));
            mSquare[5, 0] = chessName.Bishop;
            PSquare[5, 0] = chessSides.White;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Bishop - 1][1][0] = 5;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Bishop - 1][1][1] = 0;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Bishop - 1][1][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(5, 0, chessName.Bishop));
            //======================
            mSquare[2, 7] = chessName.Bishop;
            PSquare[2, 7] = chessSides.Black;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Bishop - 1][0][0] = 2;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Bishop - 1][0][1] = 7;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Bishop - 1][0][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(2, 7, chessName.Bishop));
            mSquare[5, 7] = chessName.Bishop;
            PSquare[5, 7] = chessSides.Black;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Bishop - 1][1][0] = 5;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Bishop - 1][1][1] = 7;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Bishop - 1][1][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(5, 7, chessName.Bishop));
            //======================
            mSquare[3, 0] = chessName.Queen;
            PSquare[3, 0] = chessSides.White;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Queen - 1][0][0] = 3;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Queen - 1][0][1] = 0;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.Queen - 1][0][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(3, 0, chessName.Queen));
            mSquare[3, 7] = chessName.Queen;
            PSquare[3, 7] = chessSides.Black;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Queen - 1][0][0] = 3;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Queen - 1][0][1] = 7;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.Queen - 1][0][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(3, 7, chessName.Queen));
            //======================
            mSquare[4, 0] = chessName.King;
            PSquare[4, 0] = chessSides.White;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.King - 1][0][0] = 4;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.King - 1][0][1] = 0;
            ChessSquareXY[(int)chessSides.White - 1][(int)chessName.King - 1][0][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(4, 0, chessName.King));
            mSquare[4, 7] = chessName.King;
            PSquare[4, 7] = chessSides.Black;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.King - 1][0][0] = 4;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.King - 1][0][1] = 7;
            ChessSquareXY[(int)chessSides.Black - 1][(int)chessName.King - 1][0][3] = NumLchessbasic++;
            LChessBasic.Add(new chessBasic(4, 7, chessName.King));
        }
        /// <summary>
        /// 建構式(繼承)
        /// </summary>
        /// <param name="sq1"></param>
        public chessSquare(chessSquare sq1)
        {
            StartFuc(sq1);
        }
        /// <summary>
        /// 建構式(繼承含LChessBasic)
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="bK"></param>
        public chessSquare(chessSquare sq1, bool bK)
        {
            StartFuc(sq1);
            LChessBasic = new List<chessBasic>();
            int count = sq1.LChessBasic.Count();
            for (int i = 0; i < count; ++i)
            {
                LChessBasic.Add(new chessBasic(sq1.LChessBasic[i]));
            }
        }
        /// <summary>
        /// 建構式繼承主體方法
        /// </summary>
        /// <param name="sq1"></param>
        private void StartFuc(chessSquare sq1)
        {
            Array.Copy(sq1.mSquare, mSquare, sq1.mSquare.Length);
            //Array.Copy(ISquare, sq1.ISquare, ISquare.Length);
            Array.Copy(sq1.aSquare, aSquare, sq1.aSquare.Length);
            Array.Copy(sq1.PSquare, PSquare, sq1.PSquare.Length);
            for (int i = 0; i < 2; ++i)
            {
                ChessSquareXY[i] = new int[6][][];
                for (int j = 0; j < 6; ++j)
                {
                    ChessSquareXY[i][j] = new int[sq1.ChessSquareXY[i][j].Length][];
                    for (int k = 0; k < sq1.ChessSquareXY[i][j].Length; ++k)
                    {
                        ChessSquareXY[i][j][k] = new int[sq1.ChessSquareXY[i][j][k].Length];
                        Buffer.BlockCopy(sq1.ChessSquareXY[i][j][k], 0, ChessSquareXY[i][j][k], 0, sq1.ChessSquareXY[i][j][k].Length*4);
                    }
                }
                ChessSquareTotWalk[i] = sq1.ChessSquareTotWalk[i];
                ChessSquareCheckK[i] = sq1.ChessSquareCheckK[i];
                IsChessSpecialNo2Used[i] = sq1.IsChessSpecialNo2Used[i];
                ChessSquareDrawWalk[i] = sq1.ChessSquareDrawWalk[i];
            }
        }
        public void DebugPrint()
        {
            for (int j = 7; j >= 0; --j)
            {
                for (int i = 0; i < 8; ++i)
                {
                    switch (mSquare[i, j])
                    {
                        case chessName.Pawn:
                            if (PSquare[i, j] == chessSides.White)
                            {
                                Console.Write("P");
                            }
                            else if (PSquare[i, j] == chessSides.Black)
                            {
                                Console.Write("p");
                            }
                            else Console.Write("@");
                            break;
                        case chessName.Bishop:
                            if (PSquare[i, j] == chessSides.White)
                            {
                                Console.Write("B");
                            }
                            else if (PSquare[i, j] == chessSides.Black)
                            {
                                Console.Write("b");
                            }
                            else Console.Write("@");
                            break;
                        case chessName.King:
                            if (PSquare[i, j] == chessSides.White)
                            {
                                Console.Write("K");
                            }
                            else if (PSquare[i, j] == chessSides.Black)
                            {
                                Console.Write("k");
                            }
                            else Console.Write("@");
                            break;
                        case chessName.Knight:
                            if (PSquare[i, j] == chessSides.White)
                            {
                                Console.Write("N");
                            }
                            else if (PSquare[i, j] == chessSides.Black)
                            {
                                Console.Write("n");
                            }
                            else Console.Write("@");
                            break;
                        case chessName.Queen:
                            if (PSquare[i, j] == chessSides.White)
                            {
                                Console.Write("Q");
                            }
                            else if (PSquare[i, j] == chessSides.Black)
                            {
                                Console.Write("q");
                            }
                            else Console.Write("@");
                            break;
                        case chessName.Rook:
                            if (PSquare[i, j] == chessSides.White)
                            {
                                Console.Write("R");
                            }
                            else if (PSquare[i, j] == chessSides.Black)
                            {
                                Console.Write("r");
                            }
                            else Console.Write("@");
                            break;
                        default:
                            Console.Write("*");
                            break;
                    }
                }
                Console.WriteLine();
            }
        }
        /// <summary>
        /// 地圖移動棋子方法
        /// </summary>
        /// <param name="cwp">指定座標與路徑</param>
        public void movechess(chessWalkPoint cwpIn)
        {
            int NumLCB = -1; //LChessBasic編號紀錄暫時變數
            chessWalkPoint cwp = cwpIn;
            bool IsDrawNumReset = false; //50步和局規則-是否重設計數標記

            switch (cwp.SpecialNum)
            {
                case chessSpecial.Null:
                    if (PSquare[cwp.nNumX - 1, cwp.nNumY - 1] != chessSides.Null && mSquare[cwp.nNumX - 1, cwp.nNumY - 1] != chessName.Null)
                    {
                        foreach (int[] tNum in ChessSquareXY[(int)PSquare[cwp.nNumX - 1, cwp.nNumY - 1] - 1][(int)mSquare[cwp.nNumX - 1, cwp.nNumY - 1] - 1])
                        {
                            if (tNum[0] == cwp.nNumX - 1 && tNum[1] == cwp.nNumY - 1)
                            {
                                tNum[0] = -1;
                                tNum[1] = -1;
                                LChessBasic[tNum[3]].UX = -1;
                                LChessBasic[tNum[3]].UY = -1;
                                //======================50步和局規則-吃子重設計數
                                IsDrawNumReset = true;
                                //======================
                                break;
                            }
                        }
                    }
                    foreach (int[] tNum in ChessSquareXY[(int)PSquare[cwp.pNumX - 1, cwp.pNumY - 1] - 1][(int)mSquare[cwp.pNumX - 1, cwp.pNumY - 1] - 1])
                    {
                        if (tNum[0] == cwp.pNumX - 1 && tNum[1] == cwp.pNumY - 1)
                        {
                            tNum[0] = cwp.nNumX - 1;
                            tNum[1] = cwp.nNumY - 1;
                            tNum[2]++;
                            NumLCB = tNum[3];
                            ChessSquareTotWalk[(int)PSquare[cwp.pNumX - 1, cwp.pNumY - 1] - 1]++;
                            break;
                        }
                    }
                    mSquare[cwp.nNumX - 1, cwp.nNumY - 1] = mSquare[cwp.pNumX - 1, cwp.pNumY - 1];
                    mSquare[cwp.pNumX - 1, cwp.pNumY - 1] = chessName.Null;
                    PSquare[cwp.nNumX - 1, cwp.nNumY - 1] = PSquare[cwp.pNumX - 1, cwp.pNumY - 1];
                    PSquare[cwp.pNumX - 1, cwp.pNumY - 1] = chessSides.Null;
                    //=======================
                    LChessBasic[NumLCB].UX = cwp.nNumX - 1;
                    LChessBasic[NumLCB].UY = cwp.nNumY - 1;
                    //======================50步和局規則-移動士兵重設計數
                    if (mSquare[cwp.nNumX - 1, cwp.nNumY - 1] == chessName.Pawn)
                    {
                        IsDrawNumReset = true;
                    }
                    break;
                case chessSpecial.S_No1:
                    int tX = -1, tY = -1;

                    if (PSquare[cwp.nNumX - 1, cwp.nNumY - 1] != chessSides.Null && mSquare[cwp.nNumX - 1, cwp.nNumY - 1] != chessName.Null)
                    {
                        foreach (int[] tNum in ChessSquareXY[(int)PSquare[cwp.nNumX - 1, cwp.nNumY - 1] - 1][(int)mSquare[cwp.nNumX - 1, cwp.nNumY - 1] - 1])
                        {
                            if (tNum[0] == cwp.nNumX - 1 && tNum[1] == cwp.nNumY - 1)
                            {
                                tNum[0] = -1;
                                tNum[1] = -1;
                                LChessBasic[tNum[3]].UX = -1;
                                LChessBasic[tNum[3]].UY = -1;
                                break;
                            }
                        }
                    }
                    foreach (int[] tNum in ChessSquareXY[(int)PSquare[cwp.pNumX - 1, cwp.pNumY - 1] - 1][(int)mSquare[cwp.pNumX - 1, cwp.pNumY - 1] - 1])
                    {
                        if (tNum[0] == cwp.pNumX - 1 && tNum[1] == cwp.pNumY - 1)
                        {
                            if (cwp.nNumY <= 4) //黑方吃白方
                            {
                                tX = cwp.nNumX - 1;
                                tY = cwp.nNumY - 2; //被吃方旗子之後面
                            }
                            else if (cwp.nNumY > 4) //白方吃黑方
                            {
                                tX = cwp.nNumX - 1;
                                tY = cwp.nNumY; //被吃方旗子之後面
                            }
                            tNum[0] = tX;
                            tNum[1] = tY;
                            tNum[2]++;
                            NumLCB = tNum[3];
                            ChessSquareTotWalk[(int)PSquare[cwp.pNumX - 1, cwp.pNumY - 1] - 1]++;
                            break;
                        }
                    }
                    if (cwp.nNumY <= 4) //黑方吃白方
                    {
                        mSquare[cwp.nNumX - 1, cwp.nNumY - 2] = mSquare[cwp.pNumX - 1, cwp.pNumY - 1];
                        mSquare[cwp.nNumX - 1, cwp.nNumY - 1] = chessName.Null;
                        mSquare[cwp.pNumX - 1, cwp.pNumY - 1] = chessName.Null;
                        PSquare[cwp.nNumX - 1, cwp.nNumY - 2] = PSquare[cwp.pNumX - 1, cwp.pNumY - 1];
                        PSquare[cwp.nNumX - 1, cwp.nNumY - 1] = chessSides.Null;
                        PSquare[cwp.pNumX - 1, cwp.pNumY - 1] = chessSides.Null;
                    }
                    else if (cwp.nNumY > 4) //白方吃黑方
                    {
                        mSquare[cwp.nNumX - 1, cwp.nNumY] = mSquare[cwp.pNumX - 1, cwp.pNumY - 1];
                        mSquare[cwp.nNumX - 1, cwp.nNumY - 1] = chessName.Null;
                        mSquare[cwp.pNumX - 1, cwp.pNumY - 1] = chessName.Null;
                        PSquare[cwp.nNumX - 1, cwp.nNumY] = PSquare[cwp.pNumX - 1, cwp.pNumY - 1];
                        PSquare[cwp.nNumX - 1, cwp.nNumY - 1] = chessSides.Null;
                        PSquare[cwp.pNumX - 1, cwp.pNumY - 1] = chessSides.Null;
                    }
                    //=======================
                    LChessBasic[NumLCB].UX = tX;
                    LChessBasic[NumLCB].UY = tY;
                    //======================50步和局規則-移動士兵重設計數
                    IsDrawNumReset = true;
                    break;
                case chessSpecial.S_No2:
                    if(cwp.nNumY == 8)  //黑方移動
                    {
                        if(cwp.nNumX == 7)  //短易位
                        {
                            movechess(new chessWalkPoint(5, 8, 7, 8));
                            movechess(new chessWalkPoint(8, 8, 6, 8));
                        }
                        else if (cwp.nNumX == 3)  //長易位
                        {
                            movechess(new chessWalkPoint(5, 8, 3, 8));
                            movechess(new chessWalkPoint(1, 8, 4, 8));
                        }
                        IsChessSpecialNo2Used[(int)chessSides.Black - 1] = true;
                        ChessSquareTotWalk[(int)chessSides.Black - 1]--;  //規則上一步之調整
                    }
                    else if (cwp.nNumY == 1)  //白方移動
                    {
                        if (cwp.nNumX == 7)  //短易位
                        {
                            movechess(new chessWalkPoint(5, 1, 7, 1));
                            movechess(new chessWalkPoint(8, 1, 6, 1));
                        }
                        else if (cwp.nNumX == 3)  //長易位
                        {
                            movechess(new chessWalkPoint(5, 1, 3, 1));
                            movechess(new chessWalkPoint(1, 1, 4, 1));
                        }
                        IsChessSpecialNo2Used[(int)chessSides.White - 1] = true;
                        ChessSquareTotWalk[(int)chessSides.White - 1]--;  //規則上一步之調整
                    }
                    break;
                case chessSpecial.S_No3:
                    chessName NameT = dQnsSpecailNO3Fnc();
                    if(NameT!=chessName.Null && NameT!=chessName.Pawn && NameT != chessName.King)
                    {
                        if (PSquare[cwp.nNumX - 1, cwp.nNumY - 1] != chessSides.Null && mSquare[cwp.nNumX - 1, cwp.nNumY - 1] != chessName.Null)
                        {
                            foreach (int[] tNum in ChessSquareXY[(int)PSquare[cwp.nNumX - 1, cwp.nNumY - 1] - 1][(int)mSquare[cwp.nNumX - 1, cwp.nNumY - 1] - 1])
                            {
                                if (tNum[0] == cwp.nNumX - 1 && tNum[1] == cwp.nNumY - 1)
                                {
                                    tNum[0] = -1;
                                    tNum[1] = -1;
                                    LChessBasic[tNum[3]].UX = -1;
                                    LChessBasic[tNum[3]].UY = -1;
                                    break;
                                }
                            }
                        }
                        foreach (int[] tNum in ChessSquareXY[(int)PSquare[cwp.pNumX - 1, cwp.pNumY - 1] - 1][(int)mSquare[cwp.pNumX - 1, cwp.pNumY - 1] - 1])
                        {
                            if (tNum[0] == cwp.pNumX - 1 && tNum[1] == cwp.pNumY - 1)
                            {
                                int tLength = ChessSquareXY[(int)PSquare[cwp.pNumX - 1, cwp.pNumY - 1] - 1][(int)NameT - 1].Length;
                                int[][] tNumNew = new int[tLength + 1][];
                                int tempNum = 0;
                                foreach (int[] tOldData in ChessSquareXY[(int)PSquare[cwp.pNumX - 1, cwp.pNumY - 1] - 1][(int)NameT - 1])
                                {
                                    tNumNew[tempNum] = new int[tOldData.Length];
                                    for (int i = 0; i < tOldData.Length; ++i)
                                    {
                                        tNumNew[tempNum][i] = tOldData[i];
                                    }
                                    tempNum++;
                                }
                                tNumNew[tempNum] = new int[tNumNew[0].Length];
                                tNumNew[tempNum][0] = cwp.nNumX - 1;
                                tNumNew[tempNum][1] = cwp.nNumY - 1;
                                tNumNew[tempNum][2] = ++tNum[2];
                                LChessBasic.Add(new chessBasic(cwp.nNumX - 1, cwp.nNumY - 1, NameT));
                                tNumNew[tempNum][3] = LChessBasic.Count() - 1;
                                NumLCB = LChessBasic.Count() - 1;
                                ChessSquareXY[(int)PSquare[cwp.pNumX - 1, cwp.pNumY - 1] - 1][(int)NameT - 1] = tNumNew;
                                tNum[0] = -1;
                                tNum[1] = -1;
                                ChessSquareTotWalk[(int)PSquare[cwp.pNumX - 1, cwp.pNumY - 1] - 1]++;
                                break;
                            }
                        }
                        mSquare[cwp.nNumX - 1, cwp.nNumY - 1] = NameT;
                        mSquare[cwp.pNumX - 1, cwp.pNumY - 1] = chessName.Null;
                        PSquare[cwp.nNumX - 1, cwp.nNumY - 1] = PSquare[cwp.pNumX - 1, cwp.pNumY - 1];
                        PSquare[cwp.pNumX - 1, cwp.pNumY - 1] = chessSides.Null;
                        //=======================
                        LChessBasic[NumLCB].UX = cwp.nNumX - 1;
                        LChessBasic[NumLCB].UY = cwp.nNumY - 1;
                        //======================50步和局規則-移動士兵重設計數
                        IsDrawNumReset = true;
                        break;
                    }
                    break;
            }
            if (IsDrawNumReset == true)
            {
                ChessSquareDrawWalk = new int[2];
            }
            else
            {
                ChessSquareDrawWalk[(int)PSquare[cwp.nNumX - 1, cwp.nNumY - 1] - 1]++;
            }
        }
        /// <summary>
        /// 對指定之棋子進行諮詢方法-靜態
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="csqT"></param>
        /// <returns></returns>
        public chessBasic chessPointQns(chessName cName, int X, int Y, chessSquare csqT)
        {
            switch (cName)
            {
                case chessName.Bishop:
                    return bishop.canMoveRoute(X, Y, csqT);
                case chessName.King:
                    return king.canMoveRoute(X, Y, csqT);
                case chessName.Knight:
                    return knight.canMoveRoute(X, Y, csqT);
                case chessName.Pawn:
                    pawn.aSquareUpdateCheck = false;
                    return pawn.canMoveRoute(X, Y, csqT);
                case chessName.Queen:
                    return queen.canMoveRoute(X, Y, csqT);
                case chessName.Rook:
                    return rook.canMoveRoute(X, Y, csqT);
                default:
                    return null;
            }
        }
        /// <summary>
        /// 對指定之棋子進行諮詢方法-靜態(強制諮詢模式)
        /// </summary>
        /// <param name="cName"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="csqT"></param>
        /// <param name="aSquareCheck"></param>
        /// <returns></returns>
        public chessBasic chessPointQns(chessName cName, int X, int Y, chessSquare csqT,bool aSquareCheck)
        {
            //強制諮詢模式：無視對手陣營方棋子之目前位置，直接諮詢目前cancheck之行動格子結果
            if (cName == chessName.Pawn)
            {
                pawn.aSquareUpdateCheck = true;
                return pawn.canMoveRoute(X, Y, csqT);
            }
            else
            {
                return chessPointQns(cName, X, Y, csqT);
            }
        }
        /// <summary>
        /// 對指定陣營方國王是否被check之檢查(自身地圖)
        /// </summary>
        /// <param name="cside">該路徑下之陣營方</param>
        /// <returns>是否被check</returns>
        public bool chessCheck_Check(chessSides cside)
        {
            return chessCheck_Check(this, cside);
        }
        /// <summary>
        /// 對指定陣營方國王是否被check之檢查(指定地圖)
        /// </summary>
        /// <param name="csq">地圖</param>
        /// <param name="cside">該路徑下之陣營方</param>
        /// <returns>是否被check</returns>
        public bool chessCheck_Check(chessSquare csq, chessSides cside)
        {
            return chessCheckFuc(csq, cside,
                csq.ChessSquareXY[(int)cside - 1][(int)chessName.King - 1][0][0],
                csq.ChessSquareXY[(int)cside - 1][(int)chessName.King - 1][0][1]);
        }
        /// <summary>
        /// 對指定陣營方指定棋子是否被check之檢查(指定地圖)
        /// </summary>
        /// <param name="csq">地圖</param>
        /// <param name="cside">該路徑下之陣營方</param>
        /// <param name="TX">X座標</param>
        /// <param name="TY">Y座標</param>
        /// <returns></returns>
        public bool chessCheckFuc(chessSquare csq, chessSides cside, int TX, int TY)
        {
            bool[] safeflag = new bool[8];
            chessSides Ncside;
            if ((int)cside == 1) Ncside = chessSides.Black; else Ncside = chessSides.White;

            if (TX >= 0 && TY >= 0 && TX < 8 && TY < 8)
            {
                for (int i = 1; i < 8; ++i)
                {
                    //直+橫線
                    if (TX - i >= 0 && TY >= 0 && safeflag[0] == false)
                    {
                        if (csq.PSquare[TX - i, TY] == Ncside)
                        {
                            if (csq.mSquare[TX - i, TY] == chessName.Rook || csq.mSquare[TX - i, TY] == chessName.Queen)
                            {
                                return true;
                            }
                            else safeflag[0] = true;
                        }
                        else if (csq.PSquare[TX - i, TY] == cside) safeflag[0] = true;
                    }
                    if (TX + i < 8 && TY < 8 && safeflag[1] == false)
                    {
                        if (csq.PSquare[TX + i, TY] == Ncside)
                        {
                            if (csq.mSquare[TX + i, TY] == chessName.Rook || csq.mSquare[TX + i, TY] == chessName.Queen)
                            {
                                return true;
                            }
                            else safeflag[1] = true;
                        }
                        else if (csq.PSquare[TX + i, TY] == cside)  safeflag[1] = true;
                    }
                    if (TX >= 0 && TY - i >= 0 && safeflag[2] == false)
                    {
                        if (csq.PSquare[TX, TY - i] == Ncside)
                        {
                            if (csq.mSquare[TX, TY - i] == chessName.Rook || csq.mSquare[TX, TY - i] == chessName.Queen)
                            {
                                return true;
                            }
                            else safeflag[2] = true;
                        }
                        else if (csq.PSquare[TX, TY - i] == cside) safeflag[2] = true;
                    }
                    if (TX < 8 && TY + i < 8 && safeflag[3] == false)
                    {
                        if (csq.PSquare[TX, TY + i] == Ncside)
                        {
                            if (csq.mSquare[TX, TY + i] == chessName.Rook || csq.mSquare[TX, TY + i] == chessName.Queen)
                            {
                                return true;
                            }
                            else safeflag[3] = true;
                        }
                        else if (csq.PSquare[TX, TY + i] == cside) safeflag[3] = true;
                    }
                    //斜線
                    if(TX - i >= 0 && TY - i >= 0 && safeflag[4] == false)
                    {
                        if (csq.PSquare[TX - i, TY - i] == Ncside)
                        {
                            if (csq.mSquare[TX - i, TY - i] == chessName.Bishop || csq.mSquare[TX - i, TY - i] == chessName.Queen)
                            {
                                return true;
                            }
                            else safeflag[4] = true;
                        }
                        else if (csq.PSquare[TX - i, TY - i] == cside) safeflag[4] = true;
                    }
                    if (TX + i < 8 && TY + i < 8 && safeflag[5] == false)
                    {
                        if (csq.PSquare[TX + i, TY + i] == Ncside)
                        {
                            if (csq.mSquare[TX + i, TY + i] == chessName.Bishop || csq.mSquare[TX + i, TY + i] == chessName.Queen)
                            {
                                return true;
                            }
                            else safeflag[5] = true;
                        }
                        else if (csq.PSquare[TX + i, TY + i] == cside) safeflag[5] = true;
                    }
                    if (TX - i >= 0 && TY + i < 8 && safeflag[6] == false)
                    {
                        if (csq.PSquare[TX - i, TY + i] == Ncside)
                        {
                            if (csq.mSquare[TX - i, TY + i] == chessName.Bishop || csq.mSquare[TX - i, TY + i] == chessName.Queen)
                            {
                                return true;
                            }
                            else safeflag[6] = true;
                        }
                        else if (csq.PSquare[TX - i, TY + i] == cside) safeflag[6] = true;
                    }
                    if (TX + i < 8 && TY - i >= 0 && safeflag[7] == false)
                    {
                        if (csq.PSquare[TX + i, TY - i] == Ncside)
                        {
                            if (csq.mSquare[TX + i, TY - i] == chessName.Bishop || csq.mSquare[TX + i, TY - i] == chessName.Queen)
                            {
                                return true;
                            }
                            else safeflag[7] = true;
                        }
                        else if (csq.PSquare[TX + i, TY - i] == cside) safeflag[7] = true;
                    }
                    //士兵
                    if (i == 1)
                    {
                        if(TX - i >= 0 && TY - i >= 0 && cside == chessSides.Black)
                        {
                            if (csq.PSquare[TX - i, TY - i] == Ncside && csq.mSquare[TX - i, TY - i] == chessName.Pawn)
                            {
                                return true;
                            }
                        }
                        if (TX + i < 8 && TY - i >= 0 && cside == chessSides.Black)
                        {
                            if (csq.PSquare[TX + i, TY - i] == Ncside && csq.mSquare[TX + i, TY - i] == chessName.Pawn)
                            {
                                return true;
                            }
                        }
                        if (TX - i >= 0 && TY + i < 8 && cside == chessSides.White)
                        {
                            if (csq.PSquare[TX - i, TY + i] == Ncside && csq.mSquare[TX - i, TY + i] == chessName.Pawn)
                            {
                                return true;
                            }
                        }
                        if (TX + i < 8 && TY + i < 8 && cside == chessSides.White)
                        {
                            if (csq.PSquare[TX + i, TY + i] == Ncside && csq.mSquare[TX + i, TY + i] == chessName.Pawn)
                            {
                                return true;
                            }
                        }
                    }
                    //國王
                    if (i == 1)
                    {
                        if (TX - i >= 0 && TY - i >= 0)
                        {
                            if (csq.PSquare[TX - i, TY - i] == Ncside && csq.mSquare[TX - i, TY - i] == chessName.King)
                            {
                                return true;
                            }
                        }
                        if (TX + i < 8 && TY - i >= 0)
                        {
                            if (csq.PSquare[TX + i, TY - i] == Ncside && csq.mSquare[TX + i, TY - i] == chessName.King)
                            {
                                return true;
                            }
                        }
                        if (TX - i >= 0 && TY + i < 8)
                        {
                            if (csq.PSquare[TX - i, TY + i] == Ncside && csq.mSquare[TX - i, TY + i] == chessName.King)
                            {
                                return true;
                            }
                        }
                        if (TX + i < 8 && TY + i < 8)
                        {
                            if (csq.PSquare[TX + i, TY + i] == Ncside && csq.mSquare[TX + i, TY + i] == chessName.King)
                            {
                                return true;
                            }
                        }
                        if (TX - i >= 0 && TY >= 0)
                        {
                            if (csq.PSquare[TX - i, TY] == Ncside && csq.mSquare[TX - i, TY] == chessName.King)
                            {
                                return true;
                            }
                        }
                        if (TX + i < 8 && TY < 8)
                        {
                            if (csq.PSquare[TX + i, TY] == Ncside && csq.mSquare[TX + i, TY] == chessName.King)
                            {
                                return true;
                            }
                        }
                        if (TX >= 0 && TY - i >= 0)
                        {
                            if (csq.PSquare[TX, TY - i] == Ncside && csq.mSquare[TX, TY - i] == chessName.King)
                            {
                                return true;
                            }
                        }
                        if (TX < 8 && TY + i < 8)
                        {
                            if (csq.PSquare[TX, TY + i] == Ncside && csq.mSquare[TX, TY + i] == chessName.King)
                            {
                                return true;
                            }
                        }
                    }
                    //騎士
                    if (i == 1)
                    {
                        if(TX - i >= 0 && TY + 2 < 8)
                        {
                            if (csq.PSquare[TX - i, TY + 2] == Ncside && csq.mSquare[TX - i, TY + 2] == chessName.Knight)
                            {
                                return true;
                            }
                        }
                        if (TX - i >= 0 && TY - 2 >= 0)
                        {
                            if (csq.PSquare[TX - i, TY - 2] == Ncside && csq.mSquare[TX - i, TY - 2] == chessName.Knight)
                            {
                                return true;
                            }
                        }
                        if (TX + i < 8 && TY + 2 < 8)
                        {
                            if (csq.PSquare[TX + i, TY + 2] == Ncside && csq.mSquare[TX + i, TY + 2] == chessName.Knight)
                            {
                                return true;
                            }
                        }
                        if (TX + i < 8 && TY - 2 >= 0)
                        {
                            if (csq.PSquare[TX + i, TY - 2] == Ncside && csq.mSquare[TX + i, TY - 2] == chessName.Knight)
                            {
                                return true;
                            }
                        }
                    }
                    if (i == 2)
                    {
                        if (TX - i >= 0 && TY + 1 < 8)
                        {
                            if (csq.PSquare[TX - i, TY + 1] == Ncside && csq.mSquare[TX - i, TY + 1] == chessName.Knight)
                            {
                                return true;
                            }
                        }
                        if (TX - i >= 0 && TY - 1 >= 0)
                        {
                            if (csq.PSquare[TX - i, TY - 1] == Ncside && csq.mSquare[TX - i, TY - 1] == chessName.Knight)
                            {
                                return true;
                            }
                        }
                        if (TX + i < 8 && TY + 1 < 8)
                        {
                            if (csq.PSquare[TX + i, TY + 1] == Ncside && csq.mSquare[TX + i, TY + 1] == chessName.Knight)
                            {
                                return true;
                            }
                        }
                        if (TX + i < 8 && TY - 1 >= 0)
                        {
                            if (csq.PSquare[TX + i, TY - 1] == Ncside && csq.mSquare[TX + i, TY - 1] == chessName.Knight)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            //沒有找到
            return false;
        }
        /// <summary>
        /// 對指定陣營方指定棋子是否被check之檢查(指定地圖-完整)
        /// </summary>
        /// <param name="csq">地圖</param>
        /// <param name="cside">該路徑下之陣營方</param>
        /// <param name="TX">X座標</param>
        /// <param name="TY">Y座標</param>
        /// <returns>我方、敵方</returns>
        public int[] chessCheckFucFull(chessSquare csq, chessSides cside, int TX, int TY)
        {
            bool[] safeflag = new bool[8];
            int[] tNum = { 0, 0 };
            chessSides Ncside;
            if ((int)cside == 1) Ncside = chessSides.Black; else Ncside = chessSides.White;

            if (TX >= 0 && TY >= 0 && TX < 8 && TY < 8)
            {
                for (int i = 1; i < 8; ++i)
                {
                    //直+橫線
                    if (TX - i >= 0 && TY >= 0 && safeflag[0] == false)
                    {
                        if (csq.PSquare[TX - i, TY] == Ncside)
                        {
                            if (csq.mSquare[TX - i, TY] == chessName.Rook || csq.mSquare[TX - i, TY] == chessName.Queen)
                            {
                                ++tNum[1];
                            }
                            else safeflag[0] = true;
                        }
                        else if (csq.PSquare[TX - i, TY] == cside)
                        {
                            if (csq.mSquare[TX - i, TY] == chessName.Rook || csq.mSquare[TX - i, TY] == chessName.Queen)
                            {
                                ++tNum[0];
                            }
                            else safeflag[0] = true;
                        }
                    }
                    if (TX + i < 8 && TY < 8 && safeflag[1] == false)
                    {
                        if (csq.PSquare[TX + i, TY] == Ncside)
                        {
                            if (csq.mSquare[TX + i, TY] == chessName.Rook || csq.mSquare[TX + i, TY] == chessName.Queen)
                            {
                                ++tNum[1];
                            }
                            else safeflag[1] = true;
                        }
                        else if (csq.PSquare[TX + i, TY] == cside)
                        {
                            if (csq.mSquare[TX + i, TY] == chessName.Rook || csq.mSquare[TX + i, TY] == chessName.Queen)
                            {
                                ++tNum[0];
                            }
                            else safeflag[1] = true;
                        }
                    }
                    if (TX >= 0 && TY - i >= 0 && safeflag[2] == false)
                    {
                        if (csq.PSquare[TX, TY - i] == Ncside)
                        {
                            if (csq.mSquare[TX, TY - i] == chessName.Rook || csq.mSquare[TX, TY - i] == chessName.Queen)
                            {
                                ++tNum[1];
                            }
                            else safeflag[2] = true;
                        }
                        else if (csq.PSquare[TX, TY - i] == cside)
                        {
                            if (csq.mSquare[TX, TY - i] == chessName.Rook || csq.mSquare[TX, TY - i] == chessName.Queen)
                            {
                                ++tNum[0];
                            }
                            else safeflag[2] = true;
                        }
                    }
                    if (TX < 8 && TY + i < 8 && safeflag[3] == false)
                    {
                        if (csq.PSquare[TX, TY + i] == Ncside)
                        {
                            if (csq.mSquare[TX, TY + i] == chessName.Rook || csq.mSquare[TX, TY + i] == chessName.Queen)
                            {
                                ++tNum[1];
                            }
                            else safeflag[3] = true;
                        }
                        else if (csq.PSquare[TX, TY + i] == cside)
                        {
                            if (csq.mSquare[TX, TY + i] == chessName.Rook || csq.mSquare[TX, TY + i] == chessName.Queen)
                            {
                                ++tNum[0];
                            }
                            else safeflag[3] = true;
                        }
                    }
                    //斜線
                    if (TX - i >= 0 && TY - i >= 0 && safeflag[4] == false)
                    {
                        if (csq.PSquare[TX - i, TY - i] == Ncside)
                        {
                            if (csq.mSquare[TX - i, TY - i] == chessName.Bishop || csq.mSquare[TX - i, TY - i] == chessName.Queen)
                            {
                                ++tNum[1];
                            }
                            else safeflag[4] = true;
                        }
                        else if (csq.PSquare[TX - i, TY - i] == cside)
                        {
                            if (csq.mSquare[TX - i, TY - i] == chessName.Bishop || csq.mSquare[TX - i, TY - i] == chessName.Queen)
                            {
                                ++tNum[0];
                            }
                            else safeflag[4] = true;
                        }
                    }
                    if (TX + i < 8 && TY + i < 8 && safeflag[5] == false)
                    {
                        if (csq.PSquare[TX + i, TY + i] == Ncside)
                        {
                            if (csq.mSquare[TX + i, TY + i] == chessName.Bishop || csq.mSquare[TX + i, TY + i] == chessName.Queen)
                            {
                                ++tNum[1];
                            }
                            else safeflag[5] = true;
                        }
                        else if (csq.PSquare[TX + i, TY + i] == cside)
                        {
                            if (csq.mSquare[TX + i, TY + i] == chessName.Bishop || csq.mSquare[TX + i, TY + i] == chessName.Queen)
                            {
                                ++tNum[0];
                            }
                            else safeflag[5] = true;
                        }
                    }
                    if (TX - i >= 0 && TY + i < 8 && safeflag[6] == false)
                    {
                        if (csq.PSquare[TX - i, TY + i] == Ncside)
                        {
                            if (csq.mSquare[TX - i, TY + i] == chessName.Bishop || csq.mSquare[TX - i, TY + i] == chessName.Queen)
                            {
                                ++tNum[1];
                            }
                            else safeflag[6] = true;
                        }
                        else if (csq.PSquare[TX - i, TY + i] == cside)
                        {
                            if (csq.mSquare[TX - i, TY + i] == chessName.Bishop || csq.mSquare[TX - i, TY + i] == chessName.Queen)
                            {
                                ++tNum[0];
                            }
                            else safeflag[6] = true;
                        }
                    }
                    if (TX + i < 8 && TY - i >= 0 && safeflag[7] == false)
                    {
                        if (csq.PSquare[TX + i, TY - i] == Ncside)
                        {
                            if (csq.mSquare[TX + i, TY - i] == chessName.Bishop || csq.mSquare[TX + i, TY - i] == chessName.Queen)
                            {
                                ++tNum[1];
                            }
                            else safeflag[7] = true;
                        }
                        else if (csq.PSquare[TX + i, TY - i] == cside)
                        {
                            if (csq.mSquare[TX + i, TY - i] == chessName.Bishop || csq.mSquare[TX + i, TY - i] == chessName.Queen)
                            {
                                ++tNum[0];
                            }
                            else safeflag[7] = true;
                        }
                    }
                    //士兵
                    if (i == 1)
                    {
                        if (TX - i >= 0 && TY - i >= 0)
                        {
                            if (csq.PSquare[TX - i, TY - i] == Ncside && csq.mSquare[TX - i, TY - i] == chessName.Pawn && cside == chessSides.Black)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX - i, TY - i] == cside && csq.mSquare[TX - i, TY - i] == chessName.Pawn && cside == chessSides.White)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX + i < 8 && TY - i >= 0)
                        {
                            if (csq.PSquare[TX + i, TY - i] == Ncside && csq.mSquare[TX + i, TY - i] == chessName.Pawn && cside == chessSides.Black)
                            {
                                ++tNum[1];
                            }
                            else if(csq.PSquare[TX + i, TY - i] == cside && csq.mSquare[TX + i, TY - i] == chessName.Pawn && cside == chessSides.White)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX - i >= 0 && TY + i < 8)
                        {
                            if (csq.PSquare[TX - i, TY + i] == Ncside && csq.mSquare[TX - i, TY + i] == chessName.Pawn && cside == chessSides.White)
                            {
                                ++tNum[1];
                            }
                            else if(csq.PSquare[TX - i, TY + i] == cside && csq.mSquare[TX - i, TY + i] == chessName.Pawn && cside == chessSides.Black)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX + i < 8 && TY + i < 8)
                        {
                            if (csq.PSquare[TX + i, TY + i] == Ncside && csq.mSquare[TX + i, TY + i] == chessName.Pawn && cside == chessSides.White)
                            {
                                ++tNum[1];
                            }
                            else if(csq.PSquare[TX + i, TY + i] == cside && csq.mSquare[TX + i, TY + i] == chessName.Pawn && cside == chessSides.Black)
                            {
                                ++tNum[0];
                            }
                        }
                    }
                    //國王
                    if (i == 1)
                    {
                        if (TX - i >= 0 && TY - i >= 0)
                        {
                            if (csq.PSquare[TX - i, TY - i] == Ncside && csq.mSquare[TX - i, TY - i] == chessName.King)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX - i, TY - i] == cside && csq.mSquare[TX - i, TY - i] == chessName.King)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX + i < 8 && TY - i >= 0)
                        {
                            if (csq.PSquare[TX + i, TY - i] == Ncside && csq.mSquare[TX + i, TY - i] == chessName.King)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX + i, TY - i] == cside && csq.mSquare[TX + i, TY - i] == chessName.King)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX - i >= 0 && TY + i < 8)
                        {
                            if (csq.PSquare[TX - i, TY + i] == Ncside && csq.mSquare[TX - i, TY + i] == chessName.King)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX - i, TY + i] == cside && csq.mSquare[TX - i, TY + i] == chessName.King)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX + i < 8 && TY + i < 8)
                        {
                            if (csq.PSquare[TX + i, TY + i] == Ncside && csq.mSquare[TX + i, TY + i] == chessName.King)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX + i, TY + i] == cside && csq.mSquare[TX + i, TY + i] == chessName.King)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX - i >= 0 && TY >= 0)
                        {
                            if (csq.PSquare[TX - i, TY] == Ncside && csq.mSquare[TX - i, TY] == chessName.King)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX - i, TY] == cside && csq.mSquare[TX - i, TY] == chessName.King)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX + i < 8 && TY < 8)
                        {
                            if (csq.PSquare[TX + i, TY] == Ncside && csq.mSquare[TX + i, TY] == chessName.King)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX + i, TY] == cside && csq.mSquare[TX + i, TY] == chessName.King)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX >= 0 && TY - i >= 0)
                        {
                            if (csq.PSquare[TX, TY - i] == Ncside && csq.mSquare[TX, TY - i] == chessName.King)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX, TY - i] == cside && csq.mSquare[TX, TY - i] == chessName.King)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX < 8 && TY + i < 8)
                        {
                            if (csq.PSquare[TX, TY + i] == Ncside && csq.mSquare[TX, TY + i] == chessName.King)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX, TY + i] == cside && csq.mSquare[TX, TY + i] == chessName.King)
                            {
                                ++tNum[0];
                            }
                        }
                    }
                    //騎士
                    if (i == 1)
                    {
                        if (TX - i >= 0 && TY + 2 < 8)
                        {
                            if (csq.PSquare[TX - i, TY + 2] == Ncside && csq.mSquare[TX - i, TY + 2] == chessName.Knight)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX - i, TY + 2] == cside && csq.mSquare[TX - i, TY + 2] == chessName.Knight)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX - i >= 0 && TY - 2 >= 0)
                        {
                            if (csq.PSquare[TX - i, TY - 2] == Ncside && csq.mSquare[TX - i, TY - 2] == chessName.Knight)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX - i, TY - 2] == cside && csq.mSquare[TX - i, TY - 2] == chessName.Knight)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX + i < 8 && TY + 2 < 8)
                        {
                            if (csq.PSquare[TX + i, TY + 2] == Ncside && csq.mSquare[TX + i, TY + 2] == chessName.Knight)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX + i, TY + 2] == cside && csq.mSquare[TX + i, TY + 2] == chessName.Knight)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX + i < 8 && TY - 2 >= 0)
                        {
                            if (csq.PSquare[TX + i, TY - 2] == Ncside && csq.mSquare[TX + i, TY - 2] == chessName.Knight)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX + i, TY - 2] == cside && csq.mSquare[TX + i, TY - 2] == chessName.Knight)
                            {
                                ++tNum[0];
                            }
                        }
                    }
                    if (i == 2)
                    {
                        if (TX - i >= 0 && TY + 1 < 8)
                        {
                            if (csq.PSquare[TX - i, TY + 1] == Ncside && csq.mSquare[TX - i, TY + 1] == chessName.Knight)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX - i, TY + 1] == cside && csq.mSquare[TX - i, TY + 1] == chessName.Knight)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX - i >= 0 && TY - 1 >= 0)
                        {
                            if (csq.PSquare[TX - i, TY - 1] == Ncside && csq.mSquare[TX - i, TY - 1] == chessName.Knight)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX - i, TY - 1] == cside && csq.mSquare[TX - i, TY - 1] == chessName.Knight)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX + i < 8 && TY + 1 < 8)
                        {
                            if (csq.PSquare[TX + i, TY + 1] == Ncside && csq.mSquare[TX + i, TY + 1] == chessName.Knight)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX + i, TY + 1] == cside && csq.mSquare[TX + i, TY + 1] == chessName.Knight)
                            {
                                ++tNum[0];
                            }
                        }
                        if (TX + i < 8 && TY - 1 >= 0)
                        {
                            if (csq.PSquare[TX + i, TY - 1] == Ncside && csq.mSquare[TX + i, TY - 1] == chessName.Knight)
                            {
                                ++tNum[1];
                            }
                            else if (csq.PSquare[TX + i, TY - 1] == cside && csq.mSquare[TX + i, TY - 1] == chessName.Knight)
                            {
                                ++tNum[0];
                            }
                        }
                    }
                }
            }
            return tNum;
        }
        /// <summary>
        /// 地圖更新指定陣營方所有棋子之行動標記方法
        /// </summary>
        /// <param name="cside">指定陣營方</param>
        public void aSquareUpdate(chessSides cside)
        {
            aSquare = new squareAction[8, 8];
            foreach (int[][] Nchess in ChessSquareXY[(int)cside - 1])
            {
                foreach (int[] NAchess in Nchess)
                {
                    if (NAchess[0] == -1 && NAchess[1] == -1) continue;  //該棋子已消失
                    //======================
                    foreach (int[] tNum in LChessBasic[NAchess[3]].LCanWalk)
                    {
                        aSquare[tNum[0], tNum[1]] = squareAction.CanWalk;
                    }
                    foreach (int[] tNum in LChessBasic[NAchess[3]].LCanCheck)
                    {
                        aSquare[tNum[0], tNum[1]] = squareAction.CanCheck;
                    }
                    foreach (int[] tNum in LChessBasic[NAchess[3]].LSpecial)
                    {
                        aSquare[tNum[0], tNum[1]] = squareAction.Special;
                    }
                }
            }
        }
        /// <summary>
        /// 地圖諮詢指定陣營方所有棋子之行動清單方法
        /// </summary>
        /// <param name="cside">指定陣營方</param>
        /// <param name="aSquareCheck">是否使用強制諮詢模式</param>
        public void ListChessUpdate(chessSides cside, bool aSquareCheck)
        {
            chessBasic csb;
            if (aSquareCheck == true) aSquare = new squareAction[8, 8];

            foreach (int[][] Nchess in ChessSquareXY[(int)cside - 1])
            {
                foreach (int[] NAchess in Nchess)
                {
                    if (NAchess[0] == -1 && NAchess[1] == -1) continue;  //該棋子已消失
                    //======================
                    if (aSquareCheck == true)
                    {
                        csb = chessPointQns(mSquare[NAchess[0], NAchess[1]], NAchess[0], NAchess[1], this, true);
                    }
                    else
                    {
                        csb = chessPointQns(mSquare[NAchess[0], NAchess[1]], NAchess[0], NAchess[1], this);
                    }
                    LChessBasic[NAchess[3]].canMoveRouteSUpdate(csb);
                }
            }
        }
        /// <summary>
        /// 棋局通用唯一識別碼匯出方法
        /// </summary>
        /// <returns></returns>
        public string SquareInformationString()
        {
            string strM = "";
            string Scide = "";
            string Sname = "";

            for(int i = 0; i < 8; ++i)
            {
                for(int j = 0; j < 8; ++j)
                {
                    switch (PSquare[i,j])
                    {
                        case chessSides.White:
                            Scide = "W";
                            break;
                        case chessSides.Black:
                            Scide = "B";
                            break;
                        default:
                            Scide = "N";
                            break;
                    }
                    switch (mSquare[i, j])
                    {
                        case chessName.King:
                            Sname = "K";
                            break;
                        case chessName.Queen:
                            Sname = "Q";
                            break;
                        case chessName.Knight:
                            Sname = "T";
                            break;
                        case chessName.Rook:
                            Sname = "R";
                            break;
                        case chessName.Bishop:
                            Sname = "B";
                            break;
                        case chessName.Pawn:
                            Sname = "P";
                            break;
                        default:
                            Sname = "N";
                            break;
                    }
                    strM += Scide + Sname;
                }
            }
            return strM;
        }
        /// <summary>
        /// 轉換目前棋局為字串(FEN記號)
        /// </summary>
        /// <returns></returns>
        public string CoverToForsythEdwardsNotation()
        {
            string strM = "";
            int nowNullnum = 0;

            for (int i = 7; i >= 0; --i) 
            {
                for (int j = 0; j < 8; ++j)
                {
                    switch (PSquare[j, i])
                    {
                        case chessSides.White:
                            if (nowNullnum > 0)
                            {
                                strM += nowNullnum;
                                nowNullnum = 0;
                            }
                            switch (mSquare[j, i])
                            {
                                case chessName.King:
                                    strM += "K";
                                    break;
                                case chessName.Queen:
                                    strM += "Q";
                                    break;
                                case chessName.Knight:
                                    strM += "N";
                                    break;
                                case chessName.Rook:
                                    strM += "R";
                                    break;
                                case chessName.Bishop:
                                    strM += "B";
                                    break;
                                case chessName.Pawn:
                                    strM += "P";
                                    break;
                            }
                            break;
                        case chessSides.Black:
                            if (nowNullnum > 0)
                            {
                                strM += nowNullnum;
                                nowNullnum = 0;
                            }
                            switch (mSquare[j, i])
                            {
                                case chessName.King:
                                    strM += "k";
                                    break;
                                case chessName.Queen:
                                    strM += "q";
                                    break;
                                case chessName.Knight:
                                    strM += "n";
                                    break;
                                case chessName.Rook:
                                    strM += "r";
                                    break;
                                case chessName.Bishop:
                                    strM += "b";
                                    break;
                                case chessName.Pawn:
                                    strM += "p";
                                    break;
                            }
                            break;
                        default:
                            ++nowNullnum;
                            break;
                    }
                }
                if (nowNullnum > 0)
                {
                    strM += nowNullnum;
                    nowNullnum = 0;
                }
                if (i > 0) strM += '/';
            }
            return strM;
        }
    }
}
