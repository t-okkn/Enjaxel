using System.Threading.Tasks;

namespace Enjaxel.Communication
{
    /// <summary>
    /// ストリームサーバ関連インターフェース
    /// </summary>
    public interface IStreamServer
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
