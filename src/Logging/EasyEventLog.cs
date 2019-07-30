using System;
using System.Diagnostics;
using System.Security;
using System.ComponentModel;

namespace Enjaxel.Logging
{
    /// <summary>
    /// WindowsEventLogのラッパークラス
    /// </summary>
    public class EasyEventLog
    {
        #region フィールド
        /// <summary> イベントログ接続用 </summary>
        private static EventLog ELog;

        /// <summary> ログイベント通知用 </summary>
        private static LoggingEvent Logging;

        /// <summary> パラメータ初期化確認用フラグ </summary>
        private static bool IsInitialized;
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static EasyEventLog()
        {
            ELog = new EventLog();
            Logging = new LoggingEvent(2);
            IsInitialized = false;
        }

        #region パラメータ設定
        /// <summary>
        /// WindowsEventLogの書き込み先に関するパラメータの初期化を行います
        /// </summary>
        /// <param name="sourceName"> ソース名 </param>
        /// <param name="logName"> ログの名前 </param>
        /// <param name="machineName"> WindowsEventLogを書きこむコンピューター名 </param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void InitializeEventLog(string sourceName, string logName, string machineName)
        {
            if (!EventLog.SourceExists(sourceName, machineName))
            {
                var escd = new EventSourceCreationData(sourceName, logName)
                {
                    MachineName = machineName
                };

                EventLog.CreateEventSource(escd);
            }

            ELog.Source = sourceName;
            ELog.MachineName = machineName;
            ELog.Log = logName;
            IsInitialized = true;
        }

        /// <summary>
        /// WindowsEventLogの書き込み先に関するパラメータの初期化を行います
        /// </summary>
        /// <param name="sourceName"> ソース名 </param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void InitializeEventLog(string sourceName)
        {
            // machineName = "." -> ローカルマシンを表す
            InitializeEventLog(sourceName, "Enjaxel", ".");
        }

        /// <summary>
        /// WindowsEventLogの書き込み先に関するパラメータの初期化を行います
        /// </summary>
        /// <param name="sourceName"> ソース名 </param>
        /// <param name="logName"> ログの名前 </param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="SecurityException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static void InitializeEventLog(string sourceName, string logName)
        {
            // machineName = "." -> ローカルマシンを表す
            InitializeEventLog(sourceName, logName, ".");
        }
        #endregion

        #region ログ書き込みメソッド
        #region Base - WriteLog
        /// <summary>
        /// WindowsEventLogへログ情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="evType"> イベントの種類 </param>
        /// <param name="eventId"> イベントID </param>
        /// <param name="categoryId"> カテゴリID </param>
        /// <param name="rawData"> バイナリデータ </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void WriteLog(string message, EventLogEntryType evType,
                                    int eventId, short categoryId, byte[] rawData)
        {
            if (!IsInitialized)
            {
                throw new NotInitializedException
                    ("EventLogの初期化に必要なパラメータが設定されていません。");
            }

            if (rawData != null) { rawData = new byte[] { }; }
            ELog.WriteEntry(message, evType, eventId, categoryId, rawData);
        }

        /// <summary>
        /// WindowsEventLogへログ情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="evType"> イベントの種類 </param>
        /// <param name="eventId"> イベントID </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void WriteLog(string message, EventLogEntryType evType, int eventId)
        {
            short cat_id = 0;
            byte[] raw_data = new byte[] { };

            WriteLog(message, evType, eventId, cat_id, raw_data);
        }

        /// <summary>
        /// WindowsEventLogへログ情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="evType"> イベントの種類 </param>
        /// <param name="eventId"> イベントID </param>
        /// <param name="categoryId"> カテゴリID </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void WriteLog(string message, EventLogEntryType evType,
                                    int eventId, short categoryId)
        {
            byte[] raw_data = new byte[] { };

            WriteLog(message, evType, eventId, categoryId, raw_data);
        }
        #endregion

        #region Error
        /// <summary>
        /// WindowsEventLogへエラー関連情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Error(string message)
        {
            EventLogEntryType ev_type = EventLogEntryType.Error;
            int ev_id = 1;

            WriteLog(message, ev_type, ev_id);
        }

        /// <summary>
        /// WindowsEventLogへエラー関連情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="eventId"> イベントID </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Error(string message, int eventId)
        {
            EventLogEntryType ev_type = EventLogEntryType.Error;

            WriteLog(message, ev_type, eventId);
        }

        /// <summary>
        /// WindowsEventLogへエラー関連情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="categoryId"> カテゴリID </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Error(string message, short categoryId)
        {
            EventLogEntryType ev_type = EventLogEntryType.Error;
            int ev_id = 1;

            WriteLog(message, ev_type, ev_id, categoryId);
        }

        /// <summary>
        /// WindowsEventLogへエラー関連情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="eventId"> イベントID </param>
        /// <param name="categoryId"> カテゴリID </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Error(string message, int eventId, short categoryId)
        {
            EventLogEntryType ev_type = EventLogEntryType.Error;

            WriteLog(message, ev_type, eventId, categoryId);
        }

        /// <summary>
        /// WindowsEventLogへエラー関連情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="eventId"> イベントID </param>
        /// <param name="categoryId"> カテゴリID </param>
        /// <param name="rawData"> バイナリデータ </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Error(string message, int eventId, short categoryId, byte[] rawData)
        {
            EventLogEntryType ev_type = EventLogEntryType.Error;

            WriteLog(message, ev_type, eventId, categoryId, rawData);
        }
        #endregion

        #region Warn
        /// <summary>
        /// WindowsEventLogへ警告関連情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Warn(string message)
        {
            EventLogEntryType ev_type = EventLogEntryType.Warning;
            int ev_id = 2;

            WriteLog(message, ev_type, ev_id);
        }

        /// <summary>
        /// WindowsEventLogへ警告関連情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="eventId"> イベントID </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Warn(string message, int eventId)
        {
            EventLogEntryType ev_type = EventLogEntryType.Warning;

            WriteLog(message, ev_type, eventId);
        }

        /// <summary>
        /// WindowsEventLogへ警告関連情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="categoryId"> カテゴリID </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Warn(string message, short categoryId)
        {
            EventLogEntryType ev_type = EventLogEntryType.Warning;
            int ev_id = 2;

            WriteLog(message, ev_type, ev_id, categoryId);
        }

        /// <summary>
        /// WindowsEventLogへ警告関連情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="eventId"> イベントID </param>
        /// <param name="categoryId"> カテゴリID </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Warn(string message, int eventId, short categoryId)
        {
            EventLogEntryType ev_type = EventLogEntryType.Warning;

            WriteLog(message, ev_type, eventId, categoryId);
        }

        /// <summary>
        /// WindowsEventLogへ警告関連情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="eventId"> イベントID </param>
        /// <param name="categoryId"> カテゴリID </param>
        /// <param name="rawData"> バイナリデータ </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Warn(string message, int eventId, short categoryId, byte[] rawData)
        {
            EventLogEntryType ev_type = EventLogEntryType.Warning;

            WriteLog(message, ev_type, eventId, categoryId, rawData);
        }
        #endregion

        #region Information
        /// <summary>
        /// WindowsEventLogへ情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Info(string message)
        {
            EventLogEntryType ev_type = EventLogEntryType.Information;
            int ev_id = 4;

            WriteLog(message, ev_type, ev_id);
        }

        /// <summary>
        /// WindowsEventLogへ情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="eventId"> イベントID </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Info(string message, int eventId)
        {
            EventLogEntryType ev_type = EventLogEntryType.Information;

            WriteLog(message, ev_type, eventId);
        }

        /// <summary>
        /// WindowsEventLogへ情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="categoryId"> カテゴリID </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Info(string message, short categoryId)
        {
            EventLogEntryType ev_type = EventLogEntryType.Information;
            int ev_id = 4;

            WriteLog(message, ev_type, ev_id, categoryId);
        }

        /// <summary>
        /// WindowsEventLogへ情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="eventId"> イベントID </param>
        /// <param name="categoryId"> カテゴリID </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Info(string message, int eventId, short categoryId)
        {
            EventLogEntryType ev_type = EventLogEntryType.Information;

            WriteLog(message, ev_type, eventId, categoryId);
        }

        /// <summary>
        /// WindowsEventLogへ情報を書き込みます
        /// </summary>
        /// <param name="message"> ログ内容 </param>
        /// <param name="eventId"> イベントID </param>
        /// <param name="categoryId"> カテゴリID </param>
        /// <param name="rawData"> バイナリデータ </param>
        /// <exception cref="NotInitializedException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        /// <exception cref="Win32Exception"></exception>
        public static void Info(string message, int eventId, short categoryId, byte[] rawData)
        {
            EventLogEntryType ev_type = EventLogEntryType.Information;

            WriteLog(message, ev_type, eventId, categoryId, rawData);
        }
        #endregion
        #endregion

        #region Close
        /// <summary>
        /// WindowsEventLogの接続を閉じ、リソースの解放を行います
        /// </summary>
        public static void Close()
        {
            ELog.Close();

            if (ELog != null)
            {
                ELog.Dispose();
                ELog = null;
            }
        }
        #endregion
    }
}
