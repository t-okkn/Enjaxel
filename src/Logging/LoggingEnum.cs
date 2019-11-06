namespace Enjaxel.Logging
{
    /// <summary>
    /// 出力するLogレベルを定義する列挙型
    /// </summary>
    public enum LogLv
    {
        /// <summary> ログを出力しません </summary>
        OFF = 0,

        /// <summary> FATAL以上のログを出力します </summary>
        FATAL = 1,

        /// <summary> ERROR以上のログを出力します </summary>
        ERROR = 2,

        /// <summary> WARN以上のログを出力します </summary>
        WARN = 3,

        /// <summary> INFO以上のログを出力します </summary>
        INFO = 4,

        /// <summary> DEBUG以上のログを出力します </summary>
        DEBUG = 5,

        /// <summary> 全てのログを出力します </summary>
        TRACE = 6
    }

    /// <summary>
    /// Logタイトルを定義する列挙型
    /// </summary>
    public enum LogTitle
    {
        /// <summary> タイトルなし </summary>
        [LogTitle("")]
        NONE = 0,

        /// <summary> 致命的 </summary>
        [LogTitle("致命的")]
        FATAL = 1,

        /// <summary> エラー </summary>
        [LogTitle("重大")]
        ERROR = 2,

        /// <summary> 警告 </summary>
        [LogTitle("警告")]
        WARN = 3,

        /// <summary> 情報 </summary>
        [LogTitle("情報")]
        INFO = 4,

        /// <summary> デバッグ </summary>
        [LogTitle("デバッグ")]
        DEBUG = 5,

        /// <summary> トレース </summary>
        [LogTitle("トレース")]
        TRACE = 6,

        /// <summary> 開始 </summary>
        [LogTitle("開始")]
        BEGIN = 7,

        /// <summary> 終了 </summary>
        [LogTitle("終了")]
        END = 8,

        /// <summary> アプリケーション開始 </summary>
        [LogTitle("アプリケーション開始")]
        APP_START = 9,

        /// <summary> アプリケーション終了 </summary>
        [LogTitle("アプリケーション終了")]
        APP_TERMINATE = 10,

        /// <summary> 確認 </summary>
        [LogTitle("確認")]
        CHECK = 11,

        /// <summary> バグ追跡中 </summary>
        [LogTitle("バグ追跡中")]
        CHASE = 12
    }
}
