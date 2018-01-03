using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI_First
{
    /// <summary>
    /// 路線儲存物件
    /// </summary>
    public class WalkRoute
    {
        /// <summary>
        /// 路線影響前之地圖
        /// </summary>
        public chessSquare csq;
        /// <summary>
        /// 該路線之路徑物件
        /// </summary>
        public chessWalkPoint cwp;
        /// <summary>
        /// 當前節點諮詢該路線所傳回之分數值
        /// </summary>
        public int score;
        /// <summary>
        /// 當前節點諮詢該路線所傳回之分數值(MS-Select用)
        /// </summary>
        public int scoreMS = 0;
        /// <summary>
        /// 該路線之陣營方
        /// </summary>
        public chessSquare.chessSides cside;
        /// <summary>
        /// 該路線之分組編號
        /// </summary>
        public int TeamNum = 0;
        /// <summary>
        /// 該路線之遞迴階層減數
        /// </summary>
        public int mNum = 0;
        /// <summary>
        /// (上一節點[select]/我方節點[minmax])目前最高分數
        /// </summary>
        public int PMaxScore = 0;
        /// <summary>
        /// 對方節點[minmax]目前最低分數
        /// </summary>
        public int PMinScore = 0;
        /// <summary>
        /// 路線是否被宣告無效標記
        /// </summary>
        public bool RouteIsInValid = false;
        /// <summary>
        /// 路線是否被宣告結束標記
        /// </summary>
        public bool RouteIsOver = false;
        /// <summary>
        /// 該路線是否為向前移動標記
        /// </summary>
        public bool RouteIsForwardVerticalMove = false;

        public WalkRoute(chessSquare csqIn, chessWalkPoint cwpIn, int scoreIn, chessSquare.chessSides csideIn, int TeamNumIn, int mNumIn, int pMaxScoreIn, int pMixScoreIn)
        {
            csq = csqIn;
            cwp = cwpIn;
            score = scoreIn;
            cside = csideIn;
            TeamNum = TeamNumIn;
            mNum = mNumIn;
            PMaxScore = pMaxScoreIn;
            PMinScore = pMixScoreIn;
        }
    }
}
