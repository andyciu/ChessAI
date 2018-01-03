using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI_First
{
    public delegate chessSquare.chessName D_QNSSpecialNo3();
    class Program
    {    
        static void Main(string[] args)
        {
            bool GameTest = false;  //是否開啟循環測試模式
            do
            {
                GameRecord GRM = new GameRecord();
                //================
                ChessAI_MS ai = new ChessAI_MS();
                ChessAI_MS ai2 = new ChessAI_MS();
                //================
                GUIVS gvs = new GUIVS();
                Pawn.DGetWalkRouteFuc = new D_GetWalkRoute(GRM.GetWalkRoute);
                chessSquare.dQnsSpecailNO3Fnc = new D_QNSSpecialNo3(gvs.QnsSpecialNO3CMD);
                chessWalkPoint cwpUser = null;
                chessSquare.chessSides gamecsideUser;
                chessSquare.chessSides gamecsideAI;
                chessSquare csq = new chessSquare();
                chessSquare csqBefore;
                int step = 0;
                //=============================================
                gamecsideUser = gvs.WBQnsUI();
                if ((int)gamecsideUser == 1) gamecsideAI = chessSquare.chessSides.Black; else gamecsideAI = chessSquare.chessSides.White;
                //=============================================
                csq.DebugPrint();
                Console.WriteLine();
                //================
                while (true)
                {
                    csqBefore = new chessSquare(csq, true);
                    ++step;
                    //====================
                    if (gamecsideUser == chessSquare.chessSides.White)
                    {
                        gvs.GameNowTurn = 1;
                        cwpUser = gvs.UserTurnModeUI();
                        csq.movechess(cwpUser);
                        GRM.AddWalkRoute(cwpUser, csq.SquareInformationString(), GRM.CoverToAlgebraicNotation(cwpUser, csqBefore, csq));
                        Console.WriteLine(step + "." + GRM.GetWalkSAN());
                    }
                    else if (gamecsideAI == chessSquare.chessSides.White)
                    {
                        gvs.GameNowTurn = 2;
                        ai2.AIStart(new chessSquare(csq, true), gamecsideAI, 4);
                        if (ai2.Rcwp == null && ai2.AIGiveUpAction == true)
                        {
                            Console.WriteLine("{0} Give Up.", gamecsideAI);
                            GRM.reportSentMessage = string.Format("{0} Give Up.", gamecsideAI);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\n{0}{1}=>{2}{3} Type={4}\n",
                                (char)(ai2.Rcwp.pNumX + 96), ai2.Rcwp.pNumY, (char)(ai2.Rcwp.nNumX + 96), ai2.Rcwp.nNumY, ai2.Rcwp.SpecialNum);
                            csq.movechess(ai2.Rcwp);
                            GRM.AddWalkRoute(ai2.Rcwp, csq.SquareInformationString(), GRM.CoverToAlgebraicNotation(ai2.Rcwp, csqBefore, csq));
                            csq.DebugPrint();
                            Console.WriteLine(csq.CoverToForsythEdwardsNotation());
                            Console.WriteLine(step + "." + GRM.GetWalkSAN());
                        }
                    }
                    //====================
                    if (GRM.CheckchessDraw(csq) == true)
                    {
                        if (GRM.DrawNum == 1)
                        {
                            Console.WriteLine("Draw：3步重複規則。");
                            GRM.reportSentMessage = "Draw：3步重複規則。";
                        }
                        else if (GRM.DrawNum == 2)
                        {
                            Console.WriteLine("Draw：50步規則。");
                            GRM.reportSentMessage = "Draw：50步規則。";
                        }
                        break;
                    }
                    if (gvs.CheckChessWin(csq, chessSquare.chessSides.White) == true)
                    {
                        Console.WriteLine("{0} Win!", chessSquare.chessSides.White);
                        GRM.reportSentMessage = string.Format("{0} Win!", chessSquare.chessSides.White);
                        break;
                    }
                    //====================================================
                    csqBefore = new chessSquare(csq, true);
                    //====================================================
                    if (gamecsideAI == chessSquare.chessSides.Black)
                    {
                        gvs.GameNowTurn = 2;
                        ai.AIStart(new chessSquare(csq, true), gamecsideAI, 4);
                        if (ai.Rcwp == null && ai.AIGiveUpAction == true)
                        {
                            Console.WriteLine("{0} Give Up.", gamecsideAI);
                            GRM.reportSentMessage = string.Format("{0} Give Up.", gamecsideAI);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\n{0}{1}=>{2}{3} Type={4}\n",
                                (char)(ai.Rcwp.pNumX + 96), ai.Rcwp.pNumY, (char)(ai.Rcwp.nNumX + 96), ai.Rcwp.nNumY, ai.Rcwp.SpecialNum);
                            csq.movechess(ai.Rcwp);
                            GRM.AddWalkRoute(ai.Rcwp, csq.SquareInformationString(), GRM.CoverToAlgebraicNotation(ai.Rcwp, csqBefore, csq));
                            csq.DebugPrint();
                            Console.WriteLine(step + "." + GRM.GetWalkSAN());
                        }
                    }
                    else if (gamecsideUser == chessSquare.chessSides.Black)
                    {
                        gvs.GameNowTurn = 1;
                        cwpUser = gvs.UserTurnModeUI();
                        csq.movechess(cwpUser);
                        GRM.AddWalkRoute(cwpUser, csq.SquareInformationString(), GRM.CoverToAlgebraicNotation(cwpUser, csqBefore, csq));
                        Console.WriteLine(step + "." + GRM.GetWalkSAN());
                    }
                    //=================
                    if (GRM.CheckchessDraw(csq) == true)
                    {
                        if (GRM.DrawNum == 1)
                        {
                            Console.WriteLine("Draw：3步重複規則。");
                            GRM.reportSentMessage = "Draw：3步重複規則。";
                        }
                        else if (GRM.DrawNum == 2)
                        {
                            Console.WriteLine("Draw：50步規則。");
                            GRM.reportSentMessage = "Draw：50步規則。";
                        }
                        break;
                    }
                    if (gvs.CheckChessWin(csq, chessSquare.chessSides.Black) == true)
                    {
                        Console.WriteLine("{0} Win!", chessSquare.chessSides.Black);
                        GRM.reportSentMessage = string.Format("{0} Win!", chessSquare.chessSides.Black);
                        break;
                    }
                }
                if (GRM.GameTotNum > 0)
                {
                    GRM.recordExport();
                }
            } while (GameTest == true);
            if (GameTest == false)
            {
                Console.ReadLine();
            }
        }

        class GUIVS
        {
            public int GameNowTurn = 0;  //現在由哪一方下棋(1-使用者/2-電腦)
            public chessSquare.chessName QnsSpecialNO3CMD()
            {
                if (GameNowTurn == 1)
                {
                    Console.WriteLine("\n 士兵升變:請輸入欲升格之類型([Q]Queen/[R]Rook/[K]Knight/[B]Bishop):");
                    while (true)
                    {
                        string strT = Console.ReadLine();
                        switch (strT[0])
                        {
                            case 'Q':
                            case 'q':
                                return chessSquare.chessName.Queen;
                            case 'R':
                            case 'r':
                                return chessSquare.chessName.Rook;
                            case 'K':
                            case 'k':
                                return chessSquare.chessName.Knight;
                            case 'B':
                            case 'b':
                                return chessSquare.chessName.Bishop;
                            default:
                                Console.WriteLine("Wrong Input.");
                                break;
                        }
                    }
                }
                else if (GameNowTurn == 2)
                {
                    return chessSquare.chessName.Queen;
                }
                else return chessSquare.chessName.Null;
            }
            public bool CheckChessWin(chessSquare csq, chessSquare.chessSides cside)
            {
                chessSquare.chessSides Ncside;

                if ((int)cside == 1) Ncside = chessSquare.chessSides.Black; else Ncside = chessSquare.chessSides.White;

                int TX = csq.ChessSquareXY[(int)Ncside - 1][(int)chessSquare.chessName.King - 1][0][0];
                int TY = csq.ChessSquareXY[(int)Ncside - 1][(int)chessSquare.chessName.King - 1][0][1];
                if (TX == -1 && TY == -1)
                {
                    return true;
                }
                return false;
            }
            public chessWalkPoint UserTurnModeUI()
            {
                while (true)
                {
                    string strT = Console.ReadLine();
                    string[] strTN = strT.Split(' ');
                    //移動棋子
                    if (strTN.Count() == 1)  //一般行動(欲操作棋子XY座標/欲走之棋格座標)
                    {
                        if (strT.Length == 4)
                        {
                            return new chessWalkPoint(strT[0] - 96, strT[1] - 48, strT[2] - 96, strT[3] - 48);
                        }
                    }
                    else if (strTN.Count() == 2)
                    {
                        if (strTN[1][0] == 's' && strTN[0].Length == 4)
                        {
                            switch (strTN[1][1])
                            {
                                case '1':  //路過棋子(欲操作棋子XY座標/欲吃之棋子座標)
                                    return new chessWalkPoint(strT[0] - 96, strT[1] - 48, strT[2] - 96, strT[3] - 48, chessSquare.chessSpecial.S_No1);
                                case '2':  //國王-城堡易位(國王XY座標/國王欲走之XY座標)
                                    return new chessWalkPoint(strT[0] - 96, strT[1] - 48, strT[2] - 96, strT[3] - 48, chessSquare.chessSpecial.S_No2);
                                case '3':   //士兵升變(欲操作士兵棋子之XY座標/欲走之棋格座標)
                                    return new chessWalkPoint(strT[0] - 96, strT[1] - 48, strT[2] - 96, strT[3] - 48, chessSquare.chessSpecial.S_No3);
                            }
                        }
                    }
                    Console.WriteLine("Wrong Input.");
                    continue;
                }
            }
            public chessSquare.chessSides WBQnsUI()
            {
                while (true)
                {
                    Console.WriteLine("請輸入玩家為哪方陣營:");
                    Console.WriteLine("([W]執白棋/[B]執黑棋");
                    string strT = Console.ReadLine();
                    if (strT.Length == 1)
                    {
                        switch (strT[0])
                        {
                            case 'W':
                            case 'w':
                                return chessSquare.chessSides.White;
                            case 'B':
                            case 'b':
                                return chessSquare.chessSides.Black;
                        }
                    }
                    Console.WriteLine("Wrong Input.");
                    continue;
                }
            }
        }
        
    }
}
