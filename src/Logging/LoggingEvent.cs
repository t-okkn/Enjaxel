using System;
using System.Diagnostics;

namespace Enjaxel.Logging
{
    /// <summary>
    /// ログイベントを委譲します
    /// </summary>
    /// <param name="ex"> 例外 </param>
    /// <param name="e"> ログ通知時のパラメータ </param>
    public delegate void LoggingEventHandler(Exception ex, LoggingEventArgs e);

    /// <summary>
    /// ログイベントを発生させます
    /// </summary>
    public class LoggingEvent
    {
        /// <summary>
        /// 通知されたログを受け取るイベント
        /// </summary>
        public event LoggingEventHandler ReceiveLog;

        /// <summary>
        /// 呼び出されたクラス名、メソッド名を取得するために遡るフレーム量
        /// </summary>
        private int SkipFlames { get; set; }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public LoggingEvent()
        {
            SkipFlames = 1;
        }

        /// <summary>
        /// スキップフレーム指定子付きコンストラクタ
        /// </summary>
        public LoggingEvent(int SkipFlames)
        {
            this.SkipFlames = SkipFlames;
        }

        /// <summary>
        /// ログを通知します
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="line"> 呼び出されたソースの行番号 </param>
        /// <param name="ex"> 例外 </param>
        public void NotifyLog(string message, int line, Exception ex = null)
        {
            var sf = new StackFrame(SkipFlames, true);
            // 呼び出し元のメソッド名を取得する
            string method_name = sf.GetMethod().Name;
            // 呼び出し元のクラス名を取得する
            string class_name = sf.GetMethod().ReflectedType.FullName;

            // イベント発火
            var lea = new LoggingEventArgs
                              (method_name, class_name, line, DateTime.Now, message);
            ReceiveLog(ex, lea);
        }
    }
}
