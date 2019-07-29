using System;

namespace Enjaxel
{
    /// <summary>
    /// 初期化されていないときに発生する例外
    /// </summary>
    public sealed class NotInitializedException : Exception
    {
        /// <summary>
        /// 初期化されていないときに発生します
        /// </summary>
        /// <param name="Message"> 例外内容 </param>
        public NotInitializedException(string Message) : base(Message) { }
    }
}
