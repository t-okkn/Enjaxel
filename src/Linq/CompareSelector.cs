using System;

namespace Enjaxel.Linq
{
    /// <summary>
    /// オブジェクトの等価比較を行います
    /// </summary>
    /// <typeparam name="T"> 入力型 </typeparam>
    /// <typeparam name="TKey"> 戻りの型 </typeparam>
    internal class CompareSelector<T, TKey> : EqualityComparer<T, TKey>
    {
        /// <summary>
        /// オブジェクトの等価比較を行います
        /// </summary>
        /// <param name="Selector"> 各要素に適用する等価評価処理 </param>
        internal CompareSelector(Func<T, TKey> Selector) : base(Selector, null)
        {
        }

        /// <summary>
        /// オブジェクトの等価比較を行います
        /// </summary>
        /// <param name="Selector"> 各要素に適用する等価評価処理 </param>
        internal CompareSelector(Func<object, TKey> Selector) : base(null, Selector)
        {
        }

        /// <summary>
        /// 指定したオブジェクトが、現在のオブジェクトと等しいかどうか判断します
        /// </summary>
        /// <param name="x"> object1 </param>
        /// <param name="y"> object2 </param>
        /// <returns></returns>
        public override bool Equals(T x, T y)
        {
            return Selector(x).Equals(Selector(y));
        }

        /// <summary>
        /// 指定した型のオブジェクトが、現在の同一型のオブジェクトと等しいかどうか判断します
        /// </summary>
        /// <param name="x"> object1 </param>
        /// <param name="y"> object2 </param>
        /// <returns></returns>
        public override bool Equals(object x, object y)
        {
            return ObjSelector(x).Equals(ObjSelector(y));
        }

        /// <summary>
        /// オブジェクトのハッシュ値を取得します
        /// </summary>
        /// <param name="obj"> object </param>
        /// <returns></returns>
        public override int GetHashCode(T obj)
        {
            return Selector(obj).GetHashCode();
        }

        /// <summary>
        /// オブジェクトのハッシュ値を取得します
        /// </summary>
        /// <param name="obj"> object </param>
        /// <returns></returns>
        public override int GetHashCode(object obj)
        {
            return ObjSelector(obj).GetHashCode();
        }
    }
}
