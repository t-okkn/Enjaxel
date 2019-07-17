using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Enjaxel.Communication.SocketCom
{
    /// <summary>
    /// ソケット通信用Baseクラス
    /// </summary>
    public abstract class TCPSocketServerBase : ISocket, IStreamServer
    {

        #region 定数
        /// <summary>
        /// Taskの同時実行上限
        /// </summary>
        private const int TASK_UPPER_LIMIT = 10;
        #endregion

        #region フィールドとプロパティ
        /// <summary>
        /// ポート番号
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// タイムアウト値（秒）
        /// </summary>
        /// <remarks> 既定値：3秒 </remarks>
        public int TimeoutSeconds { get; set; }

        /// <summary>
        /// サーバ稼働フラグ
        /// </summary>
        public bool IsOperation { get; set; }

        /// <summary>
        /// 解放済みフラグ
        /// </summary>
        protected bool IsDisposed { get; set; }

        /// <summary>
        /// Taskの実行上限管理
        /// </summary>
        private SemaphoreSlim TaskSemaphore;
        #endregion

        #region コンストラクタ・デストラクタ
        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        protected TCPSocketServerBase(int port) {
            Port = port;
            TimeoutSeconds = 3;
            TaskSemaphore = new SemaphoreSlim(TASK_UPPER_LIMIT, TASK_UPPER_LIMIT);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="port"> ポート番号 </param>
        /// <param name="timeoutSeconds"> タイムアウト時間（秒） </param>
        protected TCPSocketServerBase(int port, int timeoutSeconds)
        {
            Port = port;
            TimeoutSeconds = timeoutSeconds;
            TaskSemaphore = new SemaphoreSlim(TASK_UPPER_LIMIT, TASK_UPPER_LIMIT);
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~TCPSocketServerBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// 内部用Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // TODO: マネージド状態を破棄します (マネージド オブジェクト)。
                }

                // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
                // TODO: 大きなフィールドを null に設定します。

                IsDisposed = true;
            }
        }

        /// <summary>
        /// 公開用Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region サーバメソッド
        /// <summary>
        /// TCPサーバの待受を開始します
        /// </summary>
        public async Task Start()
        {
            await Task.Run(() => Console.WriteLine(""));
        }

        /// <summary>
        /// TCPサーバの待受を終了します
        /// </summary>
        public void Stop()
        {
        }
        #endregion
    }
}
