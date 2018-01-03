using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace ChessAI_First
{
    class ChessAI_MS
    {
        /// <summary>
        /// 最終選擇結果
        /// </summary>
        public chessWalkPoint Rcwp;
        /// <summary>
        /// 遞迴開創之主節點
        /// </summary>
        private ChessPointStore CPSZero = null;
        /// <summary>
        /// 總共欲推算之雙方步數和
        /// </summary>
        private int stepNumTot;
        /// <summary>
        /// 開創之主節點之多執行緒
        /// </summary>
        Thread[] TND;
        /// <summary>
        /// AI第一層多執行緒是否已結束標記
        /// </summary>
        private bool[] AIFirstFlag;
        /// <summary>
        /// 多執行緒鎖
        /// </summary>
        private object OBL = new object();
        /// <summary>
        /// 開創之主節點目前最高分門檻
        /// </summary>
        private int ZeroMaxScore = int.MinValue;
        /// <summary>
        /// AI是否宣告棄手標記
        /// </summary>
        public bool AIGiveUpAction = false;
        /// <summary>
        /// AI目前陣營方
        /// </summary>
        chessSquare.chessSides AIside;
        Random rd = new Random(Guid.NewGuid().GetHashCode());
        System.IO.StreamWriter sw;
        public ChessAI_MS() { }
        public ChessAI_MS(bool debugvalue)
        {
            if (debugvalue == true)
            {
                DateTime dt = DateTime.Now;
                string str1 = dt.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
                int tnum = rd.Next();
                str1 += "_AI_" + tnum + ".txt";
                sw = new System.IO.StreamWriter(str1); // open the file for streamwriter
            }
        }
        /// <summary>
        /// AI啟動計算函式
        /// </summary>
        /// <param name="csq">當前地圖</param>
        /// <param name="cside">當前陣營方</param>
        /// <param name="stepNum">預計算之步數</param>
        public void AIStart(chessSquare csq, chessSquare.chessSides cside, int stepNum)
        {
            stepNumTot = stepNum;
            chessSquare.chessSides Ncside = cside;
            AIside = cside;
            if ((int)cside == 1) Ncside = chessSquare.chessSides.Black; else Ncside = chessSquare.chessSides.White;
            Console.WriteLine("============");
            sw.WriteLine("==============");
            sw.WriteLine(csq.CoverToForsythEdwardsNotation());

            //清除資料
            CPSZero = null;
            ZeroMaxScore = int.MinValue;

            //計算另外一方之aSquare
            //csq.chessPointQnsALL(Ncside);
            csq.ListChessUpdate(Ncside, true);
            csq.aSquareUpdate(Ncside);

            //=================計算我方之路線
            AImain(new WalkRoute(csq, null, 0, cside, 0, stepNum, int.MinValue, int.MaxValue));
            //}
            //如果最終選擇之路線被宣告為無效者，且分數為負者，則棄手。
            if (CPSZero.FinalSelectWR.RouteIsInValid == true  && CPSZero.FinalSelectWR.score < 0)
            {
                Rcwp = null;
                AIGiveUpAction = true;
            }
            else
            {
                Rcwp = CPSZero.FinalSelectWR.cwp;
            }
            Console.WriteLine("============");
            sw.WriteLine("==============");
        }

        private void AImain(object OBIn)
        {
            WalkRoute WRMain = (WalkRoute)OBIn;
            int score = 0;  //MinMax用
            int scoreS = 0; //Select用
            int mNum = WRMain.mNum;
            chessSquare.chessSides cside = WRMain.cside;
            chessSquare.chessSides Ncside = cside;
            chessSquare.chessSides AINside = cside;
            chessSquare csq = WRMain.csq;
            chessWalkPoint cwp = WRMain.cwp;
            ChessPointStore cpsMain = new ChessPointStore(WRMain);
            bool GameOverFlag = false;  //遊戲是否已結束標記(國王被吃)            

            if (mNum < stepNumTot)  //非開創之主節點(Select)
            {
                //推算對方陣營
                if ((int)cside == 1) Ncside = chessSquare.chessSides.Black; else Ncside = chessSquare.chessSides.White;
                if ((int)AIside == 1) AINside = chessSquare.chessSides.Black; else AINside = chessSquare.chessSides.White;

                if (cwp.SpecialNum == chessSquare.chessSpecial.Null)
                {
                    //當前路線分數判斷部分
                    if (csq.PSquare[cwp.nNumX - 1, cwp.nNumY - 1] == Ncside)
                    {
                        //若是check的話則加分
                        //scoreS += (int)csq.mSquare[cwp.nNumX - 1, cwp.nNumY - 1] * 5;
                        switch(csq.mSquare[cwp.nNumX - 1, cwp.nNumY - 1])
                        {
                            case chessSquare.chessName.King:
                                scoreS += 999;
                                break;
                            case chessSquare.chessName.Queen:
                                scoreS += 60;
                                break;
                            case chessSquare.chessName.Pawn:
                                scoreS += 5;
                                break;
                            default:
                                scoreS += 20;
                                break;
                        }
                    }
                    
                }
                else if (cwp.SpecialNum == chessSquare.chessSpecial.S_No1)
                {
                    scoreS += 5;
                }
                else if (cwp.SpecialNum == chessSquare.chessSpecial.S_No3)
                {
                    scoreS += 100;
                }

                //如果該路線被吃之棋子為對手之國王時，視同結束本節點
                if (csq.mSquare[cwp.nNumX - 1, cwp.nNumY - 1] == chessSquare.chessName.King &&
                    csq.PSquare[cwp.nNumX - 1, cwp.nNumY - 1] == Ncside)
                {
                    scoreS = 9999;
                    GameOverFlag = true;
                    WRMain.RouteIsOver = true;
                }

                //移動棋子
                csq.movechess(cwp);
                //csq.aSquareUpdate(cside);
                if (cwp.SpecialNum == chessSquare.chessSpecial.Null)
                {
                    int[] chessnum = csq.chessCheckFucFull(csq, cside, cwp.nNumX - 1, cwp.nNumY - 1);
                    if (chessnum[1] > 0)
                    {
                        scoreS += chessnum[0];
                        scoreS -= chessnum[1];
                    }
                }
            }

            cpsMain.csq = csq;

            //如果國王已被吃掉時，直接結束本節點
            if (csq.ChessSquareXY[(int)AIside - 1][(int)chessSquare.chessName.King - 1][0][0] == -1 &&
               csq.ChessSquareXY[(int)AIside - 1][(int)chessSquare.chessName.King - 1][0][1] == -1)
            {
                WRMain.RouteIsOver = true;
                GameOverFlag = true;
                score = -9999;
            }
            else if (csq.ChessSquareXY[(int)AINside - 1][(int)chessSquare.chessName.King - 1][0][0] == -1 &&
               csq.ChessSquareXY[(int)AINside - 1][(int)chessSquare.chessName.King - 1][0][1] == -1)
            {
                WRMain.RouteIsOver = true;
                GameOverFlag = true;
                score = 9999;
            }
            else if (mNum == 0)  //末節點
            {
                int[] Tscore = new int[2];  //雙方評分分數
                bool[] IsKingAlive = new bool[2];

                for (int numSide = 0; numSide < 2; ++numSide)
                {
                    //對雙方棋子進行存在評分
                    foreach (int[][] Nchess in csq.ChessSquareXY[numSide])
                    {
                        foreach (int[] NAchess in Nchess)
                        {
                            if (NAchess[0] == -1 && NAchess[1] == -1) continue;  //該棋子已消失
                            else
                            {
                                switch (csq.mSquare[NAchess[0], NAchess[1]])
                                {
                                    case chessSquare.chessName.King:
                                        Tscore[numSide] += 999;
                                        IsKingAlive[numSide] = true;
                                        break;
                                    case chessSquare.chessName.Queen:
                                        Tscore[numSide] += 60;
                                        break;
                                    case chessSquare.chessName.Pawn:
                                        Tscore[numSide] += 5;
                                        break;
                                    default:
                                        Tscore[numSide] += 20;
                                        break;
                                }
                            }
                        }
                    }
                }

                if (AIside == chessSquare.chessSides.White)
                {
                    score = Tscore[0] - Tscore[1];
                }
                else if (AIside == chessSquare.chessSides.Black)
                {
                    score = Tscore[1] - Tscore[0];
                }
                if (IsKingAlive[(int)AIside - 1] == false)
                {
                    WRMain.RouteIsOver = true;
                    GameOverFlag = true;
                }
            }

            //============
            if (mNum > 0 || AIside == cside)  //非末節點或者該節點為AI方陣營者
            {
                if (csq.chessCheck_Check(cside) == true)
                {
                    if (mNum < stepNumTot)
                    {
                        //於進行本步下卻讓國王仍然或被置身於check狀態，視為違法棋步
                        WRMain.RouteIsInValid = true;
                        GameOverFlag = true;
                        if (cside == AIside)
                        {
                            score = -99999;
                        }
                        else
                        {
                            score = 99999;
                        }
                        scoreS = -99999;
                    }
                    else csq.ChessSquareCheckK[(int)cside - 1] = true;
                }
                else csq.ChessSquareCheckK[(int)cside - 1] = false;
            }
            //============
            if (mNum > 0)  //非末節點
            {
                if (csq.chessCheck_Check(Ncside) == true)
                {
                    //scoreS += 2;
                    csq.ChessSquareCheckK[(int)Ncside - 1] = true;
                }
                else csq.ChessSquareCheckK[(int)Ncside - 1] = false;
            }
            //============

            //非最後一階段函式計算路線並遞迴部分(下一步之敵方判斷)
            if (mNum > 0 && GameOverFlag == false)
            {
                List<chessWalkPoint> NcwpL = new List<chessWalkPoint>();  //每一條路線路徑
                //==========對我方做一階諮詢
                csq.ListChessUpdate(Ncside, false);
                csq.aSquareUpdate(Ncside);
                foreach (int[][] Nchess in csq.ChessSquareXY[(int)Ncside - 1])
                {
                    foreach (int[] NAchess in Nchess)
                    {
                        if (NAchess[0] == -1 && NAchess[1] == -1) continue;  //該棋子已消失
                        //======================
                        foreach (int[] tNum in csq.LChessBasic[NAchess[3]].LCanWalk.ToArray())
                        {
                            chessWalkPoint Ncwp = new chessWalkPoint(NAchess[0] + 1, NAchess[1] + 1, tNum[0] + 1, tNum[1] + 1);
                            NcwpL.Add(Ncwp);
                        }
                        foreach (int[] tNum in csq.LChessBasic[NAchess[3]].LCanCheck.ToArray())
                        {
                            chessWalkPoint Ncwp = new chessWalkPoint(NAchess[0] + 1, NAchess[1] + 1, tNum[0] + 1, tNum[1] + 1);
                            NcwpL.Add(Ncwp);
                        }
                        foreach (int[] tNum in csq.LChessBasic[NAchess[3]].LSpecial.ToArray())
                        {
                            //如果該路線為特殊2號規則，且於整場棋局內已使用過的話則視同無效
                            if ((chessSquare.chessSpecial)tNum[2] == chessSquare.chessSpecial.S_No2 &&
                                csq.IsChessSpecialNo2Used[(int)Ncside - 1] == true) continue;

                            chessWalkPoint Ncwp = new chessWalkPoint(NAchess[0] + 1, NAchess[1] + 1, tNum[0] + 1, tNum[1] + 1, (chessSquare.chessSpecial)tNum[2]);
                            NcwpL.Add(Ncwp);
                        }
                    }
                }
                //=============================================
                if (mNum == stepNumTot)
                {
                    TND = new Thread[NcwpL.Count];
                    AIFirstFlag = new bool[NcwpL.Count];
                }
                for (int i = NcwpL.Count - 1; i >= 0; --i) 
                {
                    int teamNum = WRMain.TeamNum;
                    if (mNum == stepNumTot)
                    {
                        teamNum = i;
                    }
                    WalkRoute WRNext = new WalkRoute(new chessSquare(csq, true), NcwpL[i], 0, Ncside, teamNum, mNum - 1, WRMain.PMaxScore, WRMain.PMinScore);
                    WRNext.scoreMS = scoreS;
                    //節點下路線新增
                    if (cpsMain != null)
                    {
                        cpsMain.AddRoute(WRNext);
                    }
                    if (mNum == stepNumTot)
                    {
                        if (Ncside == chessSquare.chessSides.Black && WRNext.cwp.nNumY < WRNext.cwp.pNumY)
                        {
                            WRNext.RouteIsForwardVerticalMove = true;
                        }
                        else if (Ncside == chessSquare.chessSides.White && WRNext.cwp.nNumY > WRNext.cwp.pNumY)
                        {
                            WRNext.RouteIsForwardVerticalMove = true;
                        }
                        else WRNext.RouteIsForwardVerticalMove = false;
                        AIFirstFlag[i] = false;
                        TND[i] = new Thread(AImain);
                        TND[i].Start(WRNext);
                    }
                    else
                    {
                        AImain(WRNext);
                        if (mNum % 2 == stepNumTot % 2) //我方陣營回合
                        {
                            WRMain.PMaxScore = Math.Max(WRMain.PMaxScore, WRNext.score);
                        }
                        else //敵方陣營回合
                        {
                            WRMain.PMinScore = Math.Min(WRMain.PMinScore, WRNext.score);
                        }
                        //本節點為第一層節點時更新開創之主節點之目前最高分數
                        if (mNum == stepNumTot - 1)
                        {
                            WRMain.PMaxScore = ZeroMaxScore;
                        }
                        /*當本節點目前選擇之最高路線分數
                        ，仍無法相等或超過與上一節點最高分門檻時*/
                        if (WRMain.PMinScore < WRMain.PMaxScore)
                        {
                            break;
                        }
                    }
                }

                if (mNum == stepNumTot)
                {
                    while (true)
                    {
                        bool BF = true;
                        for (int i = 0; i < AIFirstFlag.Length; ++i)
                        {
                            if (AIFirstFlag[i] == false)
                            {
                                BF = false;
                                break;
                            }
                        }
                        if (BF == true) break;
                        else Thread.Sleep(0);
                    }
                }
                if (cpsMain.WalkRouteList.Count > 1 && cpsMain.FinalSelectWR == null)
                {
                    //取所有下一步中最高分數
                    if (mNum % 2 == stepNumTot % 2) //我方陣營回合
                    {
                        //取所有下一步中最高分數
                        var result = cpsMain.WalkRouteList.Where(n =>
                              n.score == cpsMain.WalkRouteList.Max(x => x.score));
                        var result2 = result.Where(n =>
                              n.scoreMS == result.Max(x => x.scoreMS));
                        if (result2.Count() > 0)
                        {
                            WalkRoute wk = result2.ElementAt(rd.Next(result2.Count()));
                            score = wk.score;
                            scoreS += -wk.scoreMS;
                            cpsMain.FinalSelectWR = wk;
                        }
                        else
                        {
                            WalkRoute wk2 = result.ElementAt(rd.Next(result.Count()));
                            score = wk2.score;
                            scoreS += -wk2.scoreMS;
                            cpsMain.FinalSelectWR = wk2;
                        }
                    }
                    else //敵方陣營回合
                    {
                        //取所有下一步中最低分數
                        var result = cpsMain.WalkRouteList.Where(n =>
                              n.score == cpsMain.WalkRouteList.Min(x => x.score));
                        var result2 = result.Where(n =>
                              n.scoreMS == result.Max(x => x.scoreMS));
                        if (result2.Count() > 0)
                        {
                            WalkRoute wk = result2.ElementAt(rd.Next(result2.Count()));
                            score = wk.score;
                            scoreS += -wk.scoreMS;
                            cpsMain.FinalSelectWR = wk;
                        }
                        else
                        {
                            WalkRoute wk2 = result.ElementAt(rd.Next(result.Count()));
                            score = wk2.score;
                            scoreS += -wk2.scoreMS;
                            cpsMain.FinalSelectWR = wk2;
                        }
                    }

                }
                else if (cpsMain.WalkRouteList.Count == 1 && cpsMain.FinalSelectWR == null)
                {
                    score = cpsMain.WalkRouteList[0].score;
                    scoreS += -cpsMain.WalkRouteList[0].scoreMS;
                    cpsMain.FinalSelectWR = cpsMain.WalkRouteList[0];
                }
                //如果最終選擇之子路線已被宣告為無效者
                if (cpsMain.FinalSelectWR.RouteIsInValid == true)
                {
                    //如果本棋步未被check，但最終選擇之子路線已被宣告為無效者，視同逼和
                    if (csq.ChessSquareCheckK[(int)Ncside - 1] == false)
                    {
                        score = 0;
                        scoreS = 0;
                    }
                    //本路線視同為結束宣告
                    WRMain.RouteIsOver = true;
                }
                //如果最終選擇之子路線已被宣告為結束者，本路線視同為結束宣告
                if (cpsMain.FinalSelectWR.RouteIsOver == true)
                {
                    //本路線視同為結束宣告
                    WRMain.RouteIsOver = true;
                }
            }

            //節點紀錄物件儲存
            if (cpsMain != null)
            {
                cpsMain.WalkRouteThis.score = score;
                cpsMain.WalkRouteThis.scoreMS = scoreS;

                if (mNum == stepNumTot)  //遞迴開創之主
                {
                    CPSZero = cpsMain;
                }
            }

            if (mNum == stepNumTot - 1)
            {
                Console.WriteLine("[{0}]{1}{2}=>{3}{4} score={5} scoreMS={6} {7}{8}",
                 mNum, (char)(cwp.pNumX + 96), cwp.pNumY, (char)(cwp.nNumX + 96), cwp.nNumY, score, scoreS, WRMain.RouteIsInValid ? "X" : "", WRMain.RouteIsOver ? "G" : "");
                AIFirstFlag[WRMain.TeamNum] = true;
                //====================
                //本節點為第一層節點時更新開創之主節點之目前分數
                lock (OBL)
                {
                    if (score > ZeroMaxScore)
                    {
                        ZeroMaxScore = score;
                    }
                }
            }

            ////Debug用
            //lock (OBL)
            //{
            //    if (mNum <= stepNumTot - 1)
            //    {
            //        sw.WriteLine("[{0}] {1}{2}=>{3}{4} score={5} scoreMS={6} {7}{8} ({9})",
            //            mNum, (char)(cwp.pNumX + 96), cwp.pNumY, (char)(cwp.nNumX + 96), cwp.nNumY, score, scoreS, WRMain.RouteIsInValid ? "X" : "", WRMain.RouteIsOver ? "G" : "", WRMain.TeamNum);
            //    }
            //    if (mNum == stepNumTot - 1)
            //    {
            //        Console.WriteLine("[{0}]{1}{2}=>{3}{4} score={5} scoreMS={6} {7}{8}",
            //            mNum, (char)(cwp.pNumX + 96), cwp.pNumY, (char)(cwp.nNumX + 96), cwp.nNumY, score, scoreS, WRMain.RouteIsInValid ? "X" : "", WRMain.RouteIsOver ? "G" : "");
            //        AIFirstFlag[WRMain.TeamNum] = true;
            //        //====================
            //        //本節點為第一層節點時更新開創之主節點之目前分數
            //        if (score > ZeroMaxScore)
            //        {
            //            ZeroMaxScore = score;
            //        }
            //    }
            //}
        }
    }
}

