using System;

namespace Enjaxel.Communication
{
    /// <summary>
    /// ネットワークの接続等に関して問題が発生した場合に発生する例外
    /// </summary>
    public class NetworkException : Exception
    {
        /// <summary>
        /// ネットワークの接続等に関して問題が発生した場合に発生します
        /// </summary>
        /// <param name="Message"></param>
        public NetworkException(string Message) : base(Message) { }
    }
}
