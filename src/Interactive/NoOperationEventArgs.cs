using System;

namespace Enjaxel.Interactive
{
    /// <summary>
    /// 無操作イベントパラメータ
    /// </summary>
    public sealed class NoOperationEventArgs : EventArgs
    {
        /// <summary> 最後に操作された時刻 </summary>
        public DateTime LastOperationTime { get; }

        /// <summary> 実無操作時間 </summary>
        public TimeSpan NoOperationPeriod { get; }

        /// <summary>
        /// デフォルトコンスタラクタ
        /// </summary>
        public NoOperationEventArgs(DateTime LastOperationTime,
                                    TimeSpan NoOperationPeriod)
        {
            this.LastOperationTime = LastOperationTime;
            this.NoOperationPeriod = NoOperationPeriod;
        }
    }
}
