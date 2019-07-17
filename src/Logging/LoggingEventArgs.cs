using System;

namespace Enjaxel.Logging
{
    /// <summary>
    /// ログ通知時のパラメータを定義します
    /// </summary>
    public sealed class LoggingEventArgs : EventArgs
    {

        /// <summary> メソッド名 </summary>
        public string MethodName { get; set; }

        /// <summary> クラス名 </summary>
        public string ClassName { get; set; }

        /// <summary> クラス名 </summary>
        public int LineNumber { get; set; }

        /// <summary> 記録日時 </summary>
        public DateTime LoggedDate { get; set; }

        /// <summary> ログ内容 </summary>
        public string Message { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="MethodName"> メソッド名 </param>
        /// <param name="ClassName"> クラス名 </param>
        /// <param name="LineNumber"> 行番号 </param>
        /// <param name="LoggedDate"> 記録日時 </param>
        /// <param name="Message"> ログ内容 </param>
        public LoggingEventArgs(string MethodName, string ClassName, int LineNumber,
                                DateTime LoggedDate, string Message)
        {
            this.MethodName = MethodName;
            this.ClassName = ClassName;
            this.LineNumber = LineNumber;
            this.LoggedDate = LoggedDate;
            this.Message = Message;
        }
    }
}

