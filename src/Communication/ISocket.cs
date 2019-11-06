namespace Enjaxel.Communication
{
    /// <summary>
    /// Socket通信関連インターフェース
    /// </summary>
    public interface ISocket
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
