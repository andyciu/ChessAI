using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI_First
{
    /// <summary>
    /// 節點儲存紀錄物件
    /// </summary>
    public class ChessPointStore
    {
        /// <summary>
        /// 該節點所屬之路線
        /// </summary>
        public WalkRoute WalkRouteThis = null;
        /// <summary>
        /// 該節點之所有下一步路線
        /// </summary>
        public List<WalkRoute> WalkRouteList = new List<WalkRoute>();
        /// <summary>
        /// 當前路線之地圖(移動棋子後)
        /// </summary>
        public chessSquare csq;
        /// <summary>
        /// 該節點最終選擇之路線
        /// </summary>
        public WalkRoute FinalSelectWR = null;
        private int readNum = 0;

        public ChessPointStore(WalkRoute WRIn)
        {
            WalkRouteThis = WRIn;
        }
        /// <summary>
        /// 加入下一步之路線
        /// </summary>
        /// <param name="csqIn">該路線地圖</param>
        /// <param name="cwpIn">該路線路徑</param>
        /// <param name="scoreIn">該路線分數</param>
        /// <param name="csideIn">該路線陣營方</param>
        public void AddRoute(WalkRoute WRIn)
        {
            WalkRouteList.Add(WRIn);
        }

        /// <summary>
        /// 開始讀取內部路線宣告
        /// </summary>
        public void ReadStart()
        {
            readNum = 0;
        }

        /// <summary>
        /// 讀取下一個之路線儲存紀錄
        /// </summary>
        /// <returns>路線儲存紀錄物件</returns>
        public WalkRoute WalkRouteReadNext()
        {
            if (readNum + 1 > WalkRouteList.Count)
            {
                return null;
            }
            else
            {
                return WalkRouteList[readNum++];
            }
        }
    }
}
