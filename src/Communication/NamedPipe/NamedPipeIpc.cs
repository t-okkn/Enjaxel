using System;

namespace Enjaxel.Communication.NamedPipe
{
    /// <summary>
    /// 名前付きパイプ通信用イベントハンドラ
    /// </summary>
    /// <param name="e"></param>
    public delegate void NamedPipeIpcEventHandler(NamedPipeIpcEventArgs e);

    /// <summary>
    /// 名前付きパイプIPC通信のイベントクラス
    /// </summary>
    public sealed class NamedPipeIpc
    {
        // <summary> 受信時に発生するイベント </summary>
        //public event NamedPipeIpcEventHandler Receive;

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public NamedPipeIpc()
        {
        }
    }
}
