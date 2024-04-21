/*
 * File: MathLibrary.cs
 * Description: 数学库，大部分杂项数学操作均位于此处。
 * Author: tianlan
 * Last update at 24/1/31 15:41
 * 
 * Update Records:
 * tianlan  24/1/30 添加Clamp函数
 * tianlan  24/1/31 添加注释
 */

using System;

namespace GameFramework.Core
{
    public class MathLibrary
    {
        /// <summary>
        /// 使当前值处在[最小值,最大值]的范围并返回处理后的值
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="value">当前值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns>处理后的值，这个值的范围为[min, max]</returns>
        /// <exception cref="ArgumentException">如果min > max, 抛出此异常</exception>
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            // if min > max, throw exception
            if (min.CompareTo(max) == 1)
            {
                Logger.LogError("Clamp: min = " + min + ", max = " + max + " min > max. Throw ArgumentException.");
                throw new ArgumentException("The argument min should not be greater than max.");
            }

            if (value.CompareTo(min) < 0)
            {
                return min;
            }
            else if (value.CompareTo(max) > 0)
            {
                return max;
            }
            else
            {
                return value;
            }
        }
    }

}

