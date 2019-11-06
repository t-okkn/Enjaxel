using System;
using System.Collections;
using System.Collections.Generic;

namespace Enjaxel.Linq
{
    /// <summary>
    /// オブジェクトの等価比較をサポートします
    /// </summary>
    /// <typeparam name="T"> 入力型 </typeparam>
    /// <typeparam name="TKey"> 戻りの型 </typeparam>
    public abstract class EqualityComparer<T, TKey> : IEqualityComparer, IEqualityComparer<T>
    {
        /// <summary> 各要素に適用する等価評価処理 </summary>
        protected Func<T, TKey> Selector { get; set; }

        /// <summary> 各要素に適用する等価評価処理 </summary>
        protected Func<object, TKey> ObjSelector { get; set; }

        /// <summary>
        /// オブジェクトの等価比較をサポートします
        /// </summary>
        /// <param name="Selector"> T型用処理 </param>
        /// <param name="ObjSelector"> object型用処理 </param>
        protected EqualityComparer(Func<T, TKey> Selector, Func<object, TKey> ObjSelector)
        {
            this.Selector = Selector;
            this.ObjSelector = ObjSelector;
        }

        /// <summary>
        /// 指定したオブジェクトが、現在のオブジェクトと等しいかどうか判断します
        /// </summary>
        /// <param name="x"> object1 </param>
        /// <param name="y"> object2 </param>
        /// <returns></returns>
        public abstract new bool Equals(object x, object y);

        /// <summary>
        /// 指定した型のオブジェクトが、現在の同一型のオブジェクトと等しいかどうか判断します
        /// </summary>
        /// <param name="x"> object1 </param>
        /// <param name="y"> object2 </param>
        /// <returns></returns>
        public abstract bool Equals(T x, T y);

        /// <summary>
        /// オブジェクトのハッシュ値を取得します
        /// </summary>
        /// <param name="obj"> object </param>
        /// <returns></returns>
        public abstract int GetHashCode(object obj);

        /// <summary>
        /// オブジェクトのハッシュ値を取得します
        /// </summary>
        /// <param name="obj"> object </param>
        /// <returns></returns>
        public abstract int GetHashCode(T obj);
    }
}
