namespace Enjaxel.Communication.Sockets
{
    /// <summary>
    /// TCPソケットのProtocolモード
    /// </summary>
    public enum TcpSocketMode
    {
        /// <summary> IPv4, IPv6のどちらにも対応 </summary>
        DualMode = 0,

        /// <summary> IPv4のみ対応 </summary>
        IPv4 = 1,

        /// <summary> IPv6のみ対応 </summary>
        IPv6 = 2
    }
}
