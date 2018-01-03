using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessAI_First
{
    /// <summary>
    /// 路線之路徑座標儲存物件
    /// </summary>
    public class chessWalkPoint
    {
        /// <summary>
        /// 目前X座標(1~8)
        /// </summary>
        public int pNumX;
        /// <summary>
        /// 目前Y座標(1~8)
        /// </summary>
        public int pNumY;
        /// <summary>
        /// 下一個移動之X座標(1~8)
        /// </summary>
        public int nNumX;
        /// <summary>
        /// 下一個移動之Y座標(1~8)
        /// </summary>
        public int nNumY;
        /// <summary>
        /// 特殊規則編號
        /// </summary>
        public chessSquare.chessSpecial SpecialNum;
        /// <summary>
        /// 建構式(無座標)
        /// </summary>
        public chessWalkPoint()
        {
            pNumX = 0; pNumY = 0;
            nNumX = 0; nNumY = 0;
            SpecialNum = chessSquare.chessSpecial.Null;
        }
        /// <summary>
        /// 建構式(指定座標)
        /// </summary>
        /// <param name="pointX">目前X座標(1~8)</param>
        /// <param name="pointY">目前Y座標(1~8)</param>
        /// <param name="NextpointX">下一個移動之X座標(1~8)</param>
        /// <param name="NextpointY">下一個移動之Y座標(1~8)</param>
        public chessWalkPoint(int pointX,int pointY,int NextpointX,int NextpointY)
        {
            pNumX = pointX; pNumY = pointY;
            nNumX = NextpointX; nNumY = NextpointY;
            SpecialNum = chessSquare.chessSpecial.Null;
        }
        /// <summary>
        /// 建構式(座標+特殊規則編號)
        /// </summary>
        /// <param name="pointX">目前X座標(1~8)</param>
        /// <param name="pointY">目前Y座標(1~8)</param>
        /// <param name="NextpointX">下一個移動之X座標(1~8)</param>
        /// <param name="NextpointY">下一個移動之Y座標(1~8)</param>
        /// <param name="SpecialIn">特殊規則編號</param>
        public chessWalkPoint(int pointX, int pointY, int NextpointX, int NextpointY, chessSquare.chessSpecial SpecialIn)
        {
            pNumX = pointX; pNumY = pointY;
            nNumX = NextpointX; nNumY = NextpointY;
            SpecialNum = SpecialIn;
        }
        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            chessWalkPoint cwp = obj as chessWalkPoint;
            // TODO: write your implementation of Equals() here
            return base.Equals(obj) ||
                ((pNumX == cwp.pNumX) && (pNumY == cwp.pNumY) && (nNumX == cwp.nNumX) && (nNumY == cwp.nNumY) && (SpecialNum==cwp.SpecialNum));
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            //throw new NotImplementedException();
            return pNumX ^ pNumY * nNumX ^ nNumY;
        }
    }
}
