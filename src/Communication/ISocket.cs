using System;

namespace Enjaxel.Communication
{
    /// <summary>
    /// Socket通信関連インターフェース
    /// </summary>
    /// <remarks> IDisposableの実装を強制 </remarks>
    public interface ISocket : IDisposable
    {
        /// <summary>
        /// ポート番号
        /// </summary>
        int Port { get; }

        /// <summary>
        /// タイムアウト値（秒）
        /// </summary>
        int TimeoutSeconds { get; set; }
    }
}
