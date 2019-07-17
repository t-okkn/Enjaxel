using System;

namespace Enjaxel.Communication.NamedPipe
{
    /// <summary>
    /// 名前付きパイプIPC通信イベント用パラメータクラス
    /// </summary>
    public sealed class NamedPipeIpcEventArgs : EventArgs
    {
        /// <summary> 意味のあるID </summary>
        public string Id { get; set; }

        /// <summary> 通信内容 </summary>
        public string Message { get; set; }

        /// <summary> 送信日時 </summary>
        public DateTime SendDate { get; set; }
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="Message"> 通信内容 </param>
        /// <param name="Id"> ID </param>
        public NamedPipeIpcEventArgs(string Message, string Id)
        {
            this.Message = Message;
            this.Id = Id;
            this.SendDate = DateTime.Now;
        }
    }
}
