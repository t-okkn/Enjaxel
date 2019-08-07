using System;
using System.Collections.Generic;
using System.Linq;

namespace Enjaxel.Linq
{
    /// <summary>
    /// IEnumerableの要素について組み合わせを求めるクラス
    /// </summary>
    public static class EnumCombination
    {
        /// <summary>
        /// 入力された要素から組み合わせを導出します
        /// </summary>
        /// <typeparam name="T"> 組み合わせを求める型 </typeparam>
        /// <param name="items"> 抽出元の要素 </param>
        /// <param name="pickUp"> 組み合わせ要素数 </param>
        /// <param name="withRepetition"> 重複を許可するかどうかのフラグ </param>
        /// <returns></returns>
        public static IEnumerable<T[]> Combination<T>
            (this IEnumerable<T> items, int pickUp, bool withRepetition)
            where T : IEquatable<T>
        {
            if (pickUp == 1)
            {
                foreach (T item in items)
                {
                    yield return new T[] { item };
                }
            }

            foreach (T item in items)
            {
                var left = new T[] { item };
                IEnumerable<T> unused = null;

                // itemよりも前のものを除く（順列と組み合わせの違い)
                // 重複を許さない場合、unusedから item そのものも取り除く
                if (withRepetition)
                {
                    unused = items;
                }
                else
                {
                    unused = items.SkipWhile(x => !x.Equals(item)).Skip(1);
                }

                foreach (var right in Combination(unused, pickUp - 1, withRepetition))
                {
                    yield return left.Concat(right).ToArray();
                }
            }
        }
    }
}
