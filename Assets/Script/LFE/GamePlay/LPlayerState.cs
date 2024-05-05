/*
 * File: LPlayerState.cs
 * Description: 玩家属性，继承自GameFramework.GamePlay.MyPlayerState
 * Author: tianlan
 * Last update at 24/4/22   23:00
 *
 * Update Records:
 * tianlan  24/4/22
 */

using Script.LFE.Core;

namespace Script.LFE.GamePlay
{
    /// <summary>
    /// 拓展的玩家属性
    /// </summary>
    public class LPlayerState : GameFramework.GamePlay.MyPlayerState
    {
        // Now Element & Judge
        #region ElementJudge 

        /// <summary>
        /// 玩家当前属性
        /// </summary>
        public NativeElement nowElement;

        /// <summary>
        /// 判断五行元素from是否生dest
        /// </summary>
        /// <param name="from">出发的元素</param>
        /// <param name="dest">目标元素</param>
        /// <returns>from是否生dest,如果from是None则永远为false,如果dest是None则永远为true.</returns>
        public static bool IsMutuallyGenerated(NativeElement from, NativeElement dest)
        {
            if (from == NativeElement.None)
            {
                return false;
            }

            if (dest == NativeElement.None)
            {
                return true;
            }

            return (dest & GetGenerateElement(from)) != 0;
        }

        /// <summary>
        /// 获取当前元素生的元素
        /// </summary>
        /// <param name="source">当前元素</param>
        /// <returns>生的目标元素，例如水生木</returns>
        public static NativeElement GetGenerateElement(NativeElement source)
        {
            return source switch
            {
                NativeElement.Jin => NativeElement.Shui,
                NativeElement.Mu => NativeElement.Huo,
                NativeElement.Shui => NativeElement.Mu,
                NativeElement.Huo => NativeElement.Tu,
                NativeElement.Tu => NativeElement.Jin,
                _ => NativeElement.None
            };
        }

        /// <summary>
        /// 判断五行元素from是否克dest
        /// </summary>
        /// <param name="from">出发的元素</param>
        /// <param name="dest">目标元素</param>
        /// <returns>from是否克dest,如果from是None则永远为false,如果dest是None则永远为true.</returns>
        public static bool IsMutuallyExclusive(NativeElement from, NativeElement dest)
        {
            if (from == NativeElement.None)
            {
                return false;
            }

            if (dest == NativeElement.None)
            {
                return true;
            }

            return (dest & GetExclusiveElement(from)) != 0;
        }

        /// <summary>
        /// 获取当前元素克的元素
        /// </summary>
        /// <param name="source">当前元素</param>
        /// <returns>生的目标元素，例如水克火</returns>
        public static NativeElement GetExclusiveElement(NativeElement source)
        {
            return source switch
            {
                NativeElement.Jin => NativeElement.Mu,
                NativeElement.Mu => NativeElement.Tu,
                NativeElement.Shui => NativeElement.Huo,
                NativeElement.Huo => NativeElement.Jin,
                NativeElement.Tu => NativeElement.Shui,
                _ => NativeElement.None
            };
        }

        public void MoveToNextElement()
        {
            nowElement = GetGenerateElement(nowElement);
        }

        #endregion
        
    }
}