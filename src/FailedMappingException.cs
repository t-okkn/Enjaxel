using System;

namespace Enjaxel
{
    /// <summary>
    /// オブジェクトとの紐付けに失敗した場合に発生する例外
    /// </summary>
    public sealed class FailedMappingException : Exception
    {
        /// <summary>
        /// オブジェクトとの紐付けに失敗した場合に発生します
        /// </summary>
        /// <param name="Message"> 例外内容 </param>
        public FailedMappingException(string Message) : base(Message)
        {
        }
    }
}
