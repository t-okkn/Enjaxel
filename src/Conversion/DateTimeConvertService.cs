using System;
using System.Globalization;

namespace Enjaxel.Conversion
{
    /// <summary>
    /// DateTime型とString型を相互変換するクラス
    /// </summary>
    public static class DateTimeConvertService
    {
        /// <summary> 日時形式文字列に期待するフォーマット </summary>
        private static string[] ExpectedFormats { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        static DateTimeConvertService()
        {
            ExpectedFormats = new string[] {
                "yyyyMMddHHmmss",
                "yyyyMMddHHmmssf",
                "yyyyMMddHHmmssff",
                "yyyyMMddHHmmssfff",
                "yyyy/M/d H:m:s",
                "yyyy/MM/dd HH:mm:ss",
                "yyyy/MM/dd HH:mm:ss.f",
                "yyyy/MM/dd HH:mm:ss.ff",
                "yyyy/MM/dd HH:mm:ss.fff",
                "yyyy/MM/dd_HH:mm:ss",
                "yyyy/MM/dd_HH:mm:ss.f",
                "yyyy/MM/dd_HH:mm:ss.ff",
                "yyyy/MM/dd_HH:mm:ss.fff",
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd HH:mm:ss.f",
                "yyyy-MM-dd HH:mm:ss.ff",
                "yyyy-MM-dd HH:mm:ss.fff",
                "yyyy-MM-dd_HH:mm:ss",
                "yyyy-MM-dd_HH:mm:ss.f",
                "yyyy-MM-dd_HH:mm:ss.ff",
                "yyyy-MM-dd_HH:mm:ss.fff",
                "yyyy-MM-ddTHH:mm:sszzz"
            };
        }

        /// <summary>
        /// DateTime型からString型の"yyyyMMddHHmmssfff"形式に変換するメソッド
        /// </summary>
        /// <param name="inDatetime"></param>
        /// <returns></returns>
        public static string ToMiliSecString(this DateTime inDatetime)
        {
            return inDatetime.ToString("yyyyMMddHHmmssfff");
        }

        /// <summary>
        /// DateTime型からString型の"yyyy-MM-ddTHH:mm:ss+00:00"形式に変換するメソッド
        /// </summary>
        /// <param name="inDatetime"></param>
        /// <returns></returns>
        public static string ToISOString(this DateTime inDatetime)
        {
            return inDatetime.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }

        /// <summary>
        /// 文字列形式の日時表記をDateTime型に変換するメソッド
        /// </summary>
        /// <param name="inString"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string inString)
        {
            DateTime resDatetime = DateTime.MinValue;

            try
            {
                resDatetime = DateTime.ParseExact(inString,
                                                  ExpectedFormats,
                                                  DateTimeFormatInfo.InvariantInfo,
                                                  DateTimeStyles.None);
            }
            catch (Exception)
            {
                throw;
            }

            return resDatetime;
        }
    }
}
