using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI_First
{
    /// <summary>
    /// 取得遊戲紀錄之上一步路線物件函式委派
    /// </summary>
    /// <returns>路線紀錄物件</returns>
    public delegate chessWalkPoint D_GetWalkRoute();

    /// <summary>
    /// 遊戲紀錄類別
    /// </summary>
    public class GameRecord
    {
        /// <summary>
        /// 棋局路徑紀錄物件List
        /// </summary>
        private List<chessWalkPoint> WRListTot = new List<chessWalkPoint>();
        /// <summary>
        /// 棋局通用唯一識別碼紀錄物件List
        /// </summary>
        private List<string> WRListInfTot = new List<string>();
        /// <summary>
        /// 棋局紀錄標準代數記號(SAN)物件List
        /// </summary>
        private List<string> WRListSANTot = new List<string>();
        /// <summary>
        /// 和局規則編號
        /// </summary>
        public int DrawNum = 0;
        /// <summary>
        /// 紀錄檔案結尾訊息
        /// </summary>
        public string reportSentMessage = "";
        public int GameTotNum { get; private set; }

        /// <summary>
        /// 讀取上一步數之路線紀錄
        /// </summary>
        /// <param name="WRnum">棋局第n步數</param>
        /// <returns>路線紀錄物件</returns>
        public chessWalkPoint GetWalkRoute()
        {
            if (GameTotNum > 0) return WRListTot[GameTotNum - 1];
            else return null;
        }

        /// <summary>
        /// 讀取上一步數之標準代數記號(SAN)
        /// </summary>
        /// <returns></returns>
        public string GetWalkSAN()
        {
            if (GameTotNum > 0) return WRListSANTot.Last();
            else return null;
        }

        public GameRecord()
        {
            GameTotNum = 0;
        }

        public void AddWalkRoute(chessWalkPoint wrIn, string csqAfterstr, string SANstr)
        {
            WRListTot.Add(wrIn);
            WRListInfTot.Add(csqAfterstr);
            WRListSANTot.Add(SANstr);
            GameTotNum++;
        }
        public void recordExport()
        {
            DateTime dt = DateTime.Now;
            int sannum = 1, sanround = 0;
            string str1 = dt.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            str1 += ".txt";
            System.IO.StreamWriter sw = new System.IO.StreamWriter(str1); // open the file for streamwriter
            foreach (var tNum in WRListTot)
            {
                sw.WriteLine("{0}{1}=>{2}{3} Type={4}",
                    (char)(tNum.pNumX + 96), tNum.pNumY, (char)(tNum.nNumX + 96), tNum.nNumY, tNum.SpecialNum);
            }
            sw.WriteLine("=======SAN Start========");
            foreach (var tNum in WRListSANTot)
            {
                if (sanround++ == 0)
                {
                    sw.Write("{0}.{1} ", sannum,tNum);
                }
                else
                {
                    sw.WriteLine(tNum);
                    sanround = 0;
                    ++sannum;
                }
            }
            sw.WriteLine("=======SAN End========");
            sw.WriteLine(reportSentMessage);
            sw.Close(); // close the file
        }
        public bool CheckchessDraw(chessSquare csq)
        {
            //=============3次重複原則
            var data = (from a in WRListInfTot
                        group a by a into g
                        where g.Count() >= 3
                        select g.Key);
            if (data.Count() > 0)
            {
                DrawNum = 1;
                return true;
            }
            //==============50步規則
            if(csq.ChessSquareDrawWalk[0] >= 50 && csq.ChessSquareDrawWalk[1] >= 50)
            {
                DrawNum = 2;
                return true;
            }
            DrawNum = 0;
            return false;
        }

        /// <summary>
        /// 轉換目前指令動作字串為標準代數記號(SAN)
        /// </summary>
        /// <param name="wrIn">行動路徑</param>
        /// <param name="csq">棋盤(移動前)</param>
        /// <param name="csqAfter">棋盤(移動後)</param>
        /// <returns></returns>
        public string CoverToAlgebraicNotation(chessWalkPoint wrIn, chessSquare csq, chessSquare csqAfter)
        {
            char[] row = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };
            char[] type = { 'P', 'N', 'B', 'R', 'Q', 'K' }; //照chessSquare.chessName排序
            string output = "";
            chessSquare.chessSides cside = csq.PSquare[wrIn.pNumX - 1, wrIn.pNumY - 1];
            chessSquare.chessSides Ncside;
            int tY=0;
            switch (wrIn.SpecialNum)
            {
                case chessSquare.chessSpecial.Null:
                    if (csq.mSquare[wrIn.pNumX - 1, wrIn.pNumY - 1] == chessSquare.chessName.Pawn)
                    {
                        if(csq.mSquare[wrIn.nNumX - 1, wrIn.nNumY - 1] != chessSquare.chessName.Null)
                        {
                            //士兵-一般吃子
                            output = String.Format("{0}x{1}{2}", row[wrIn.pNumX - 1], row[wrIn.nNumX - 1], wrIn.nNumY);
                        }
                        else
                        {
                            output = String.Format("{0}{1}", row[wrIn.nNumX - 1], wrIn.nNumY);
                        }
                    }
                    else
                    {
                        output = String.Format("{0}{1}{2}{3}",
                                type[(int)csq.mSquare[wrIn.pNumX - 1, wrIn.pNumY - 1] - 1],
                                csq.mSquare[wrIn.nNumX - 1, wrIn.nNumY - 1] != chessSquare.chessName.Null ? "x" : "",
                                row[wrIn.nNumX - 1],
                                wrIn.nNumY);
                    }
                    break;
                case chessSquare.chessSpecial.S_No1:
                    if (wrIn.nNumY <= 4) //黑方吃白方
                    {
                        tY = wrIn.nNumY - 1; //被吃方旗子之後面
                    }
                    else if (wrIn.nNumY > 4) //白方吃黑方
                    {
                        tY = wrIn.nNumY + 1; //被吃方旗子之後面
                    }
                    output = String.Format("{0}x{1}{2} e.p.", row[wrIn.pNumX - 1], row[wrIn.nNumX - 1], tY);
                    break;
                case chessSquare.chessSpecial.S_No2:
                    if (wrIn.nNumY == 8)  //黑方移動
                    {
                        if (wrIn.nNumX == 7)  //短易位
                        {
                            output = "0-0";
                        }
                        else if (wrIn.nNumX == 3)  //長易位
                        {
                            output = "0-0-0";
                        }
                    }
                    else if (wrIn.nNumY == 1)  //白方移動
                    {
                        if (wrIn.nNumX == 7)  //短易位
                        {
                            output = "0-0";
                        }
                        else if (wrIn.nNumX == 3)  //長易位
                        {
                            output = "0-0-0";
                        }
                    }
                    break;
                case chessSquare.chessSpecial.S_No3:
                    output = String.Format("{0}{1}={2}", row[wrIn.nNumX - 1], wrIn.nNumY, type[(int)csqAfter.mSquare[wrIn.nNumX - 1, wrIn.nNumY - 1] - 1]);
                    break;
                default:
                    return null;
            }
            if ((int)cside == 1) Ncside = chessSquare.chessSides.Black; else Ncside = chessSquare.chessSides.White;
            if (csqAfter.chessCheck_Check(Ncside) == true)
            {
                output = output + "+";
            }
            return output;
        }
    }
}
