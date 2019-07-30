using System;
using System.Timers;

namespace Enjaxel.Interactive
{
    /// <summary>
    /// 無操作イベント用デリゲート
    /// </summary>
    /// <param name="e"> 無操作イベントパラメータ </param>
    public delegate void NoOperationEventHandler(NoOperationEventArgs e);

    /// <summary>
    /// 無操作監視クラス
    /// </summary>
    public sealed class NoOperationWatcher : IDisposable
    {
        /// <summary> 無操作時間超過通知 </summary>
        public event NoOperationEventHandler ExpiredNotice;

        /// <summary> 最終操作日時を示すプロパティ </summary>
        public DateTime LastOperation { get; private set; }

        /// <summary> 無操作監視用タイマー </summary>
        private Timer EventTimer;

        /// <summary> 無操作許容時間 </summary>
        private TimeSpan Period;

        /// <summary> Disposeフラグ </summary>
        private bool IsDisposed;

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        /// <param name="periodValue"> 無操作許容時間（秒） </param>
        public NoOperationWatcher(int periodValue)
        {
            Period = new TimeSpan(0, 0, periodValue);

            double interval = 0.0D;
            if (periodValue <= 3600)
            {
                interval = Math.Floor(periodValue * 1000.0D / 60.0D);
            }
            else
            {
                // 1分間隔
                interval = 60000.0D;
            }

            EventTimer = new Timer(interval);
            Initialize();
        }

        /// <summary>
        /// 確認間隔指定可能コンストラクタ
        /// </summary>
        /// <param name="periodValue"> 無操作許容時間 </param>
        /// <param name="interval"> 確認間隔（秒）</param>
        public NoOperationWatcher(TimeSpan periodValue, int interval)
        {
            Period = periodValue;
            EventTimer = new Timer(interval * 1000.0D);
            Initialize();
        }

        /// <summary>
        /// TimeSpan指定コンストラクタ
        /// </summary>
        /// <param name="periodValue"> 無操作許容時間 </param>
        /// <param name="interval"> 確認間隔 </param>
        public NoOperationWatcher(TimeSpan periodValue, TimeSpan interval)
        {
            Period = periodValue;
            EventTimer = new Timer(interval.TotalMilliseconds);
            Initialize();
        }

        /// <summary>
        /// コンストラクタ向け共通処理記載メソッド
        /// </summary>
        private void Initialize()
        {
            IsDisposed = false;
            EventTimer.Elapsed += ExpiredCheck;

            LastOperation = DateTime.Now;
            EventTimer.Enabled = false;
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~NoOperationWatcher()
        {
            Dispose(false);
        }

        /// <summary>
        /// リソースを開放します
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 内部Dispose
        /// </summary>
        /// <param name="isDisposing"></param>
        private void Dispose(bool isDisposing)
        {
            if (!IsDisposed)
            {
                if (isDisposing)
                {
                    if (EventTimer != null)
                    {
                        EventTimer.Stop();
                        EventTimer.Dispose();
                        EventTimer = null;
                    }
                }

                IsDisposed = true;
            }
        }

        /// <summary>
        /// 無操作時間を延長します
        /// </summary>
        public void Extension()
        {
            LastOperation = DateTime.Now;
        }

        /// <summary>
        /// 無操作監視を開始します
        /// </summary>
        public void WatchStart()
        {
            if (!EventTimer.Enabled)
            {
                LastOperation = DateTime.Now;

                EventTimer.Enabled = true;
                EventTimer.Start();
            }
        }

        /// <summary>
        /// 無操作監視を停止します
        /// </summary>
        public void WatchStop()
        {
            if (EventTimer.Enabled)
            {
                EventTimer.Stop();
                EventTimer.Enabled = false;
            }
        }

        /// <summary>
        /// 無操作時間が過ぎていないか確認します
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"> Timerイベントデータ </param>
        private void ExpiredCheck(object sender, ElapsedEventArgs e)
        {
            TimeSpan diff = e.SignalTime - LastOperation;

            if (diff.TotalSeconds > Period.TotalSeconds)
            {
                ExpiredNotice(new NoOperationEventArgs(LastOperation, diff));
            }
        }
    }

}
