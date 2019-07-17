using System;

namespace Enjaxel
{
    /// <summary>
    /// 初期化されていないときに発生するエラー
    /// </summary>
    public sealed class NotInitializedException : Exception
    {
        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public NotInitializedException(string Message) : base(Message) { }
    }
}
