using System;
using System.Windows.Forms;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Enjaxel.Logging
{
    /// <summary>
    /// フォームに設置したテキストボックスにログを出力するクラス
    /// </summary>
    public sealed class FormLogger
    {
        #region フィールドとプロパティ
        /// <summary> 環境依存の改行コード </summary>
        private static readonly string NL;

        /// <summary> ログイベント通知用 </summary>
        private static LoggingEvent Logging;

        /// <summary> 出力先のテキストボックスを指定します </summary>
        public static TextBox OutputTextBox { private get; set; }

        /// <summary> テキストボックスに出力させる最大行数 </summary>
        public static ushort MaxLine { get; set; }
        #endregion

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static FormLogger()
        {
            NL = Environment.NewLine;
            Logging = new LoggingEvent(2);
            MaxLine = 10000;
            OutputTextBox = null;

            Logging.ReceiveLog += ShowTextBox;
        }

        #region publicメソッド
        /// <summary>
        /// テキストボックスへログを表示します
        /// </summary>
        /// <param name="msg"> ログ内容 </param>
        /// <param name="line"> 実行されたソースの行数（自動で挿入） </param>
        [Conditional("DEBUG")]
        public static void Log(string msg, [CallerLineNumber] int line = -1)
        {
            Logging.NotifyLog(msg, line);
        }

        /// <summary>
        /// テキストボックスへログを表示します
        /// </summary>
        /// <param name="msg"> ログ内容 </param>
        /// <param name="ex"> 例外 </param>
        /// <param name="line"> 実行されたソースの行数（自動で挿入） </param>
        [Conditional("DEBUG")]
        public static void Log(string msg, Exception ex,
                               [CallerLineNumber] int line = -1)
        {
            Logging.NotifyLog(msg, line, ex);
        }
        #endregion

        #region Event
        /// <summary>
        /// テキストボックスへログを表示します
        /// </summary>
        /// <param name="ex"> 例外 </param>
        /// <param name="e"> ログパラメータ </param>
        private static void ShowTextBox(Exception ex, LoggingEventArgs e)
        {
            // 出力先のテキストボックスがNullなら終了
            if (OutputTextBox == null)
            {
                return;
            }

            // 出力するテキストを成形
            string str_date = e.LoggedDate.ToString("yyyy/MM/dd HH:mm:ss.fff");
            string logged_pos = $"{e.ClassName} - {e.MethodName} LINE.{e.LineNumber}";
            string msg = $"{str_date}｜{e.Message} 〔 {logged_pos} 〕";

            // エラーもあればテキストに成形
            string err_msg = string.Empty;
            if (ex != null)
            {
                err_msg = $"{str_date}｜エラー：【{ex.Message}】〔 {logged_pos} 〕";
                err_msg += NL + ex.StackTrace;
            }

            // テキストボックスへ出力
            OutputTextBox.Invoke(new Action(() =>
            {
                int max = Convert.ToInt32(MaxLine);

                if (string.IsNullOrEmpty(OutputTextBox.Text))
                {
                    OutputTextBox.Text = msg;
                }
                else
                {
                    OutputTextBox.Text += NL + msg;
                }

                if (!string.IsNullOrEmpty(err_msg))
                {
                    OutputTextBox.Text += NL + err_msg;
                }

                int now_line = OutputTextBox.Text.Length;
                if (now_line > max)
                {
                    OutputTextBox.Text = OutputTextBox.Text.Remove(0, now_line - max);
                }
            }));
        }
        #endregion
    }
}
