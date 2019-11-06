using System;
using System.Threading.Tasks;

namespace Enjaxel.Communication
{
    /// <summary>
    /// ストリームサーバ関連インターフェース
    /// </summary>
    /// <remarks> IDisposableの実装を強制 </remarks>
    public interface IStreamServer : IDisposable
    {
        /// <summary>
        /// サーバの開始
        /// </summary>
        Task Start();

        /// <summary>
        /// サーバの停止
        /// </summary>
        void Stop();
    }
}
