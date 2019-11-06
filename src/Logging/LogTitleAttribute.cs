using System;

namespace Enjaxel.Logging
{
    /// <summary>
    /// ログタイトルの紐付けを行う属性クラス
    /// </summary>
    public sealed class LogTitleAttribute : Attribute
    {
        /// <summary> タイトル </summary>
        public string Title { get; set; }

        /// <summary>
        /// ログタイトルの紐付けを行います
        /// </summary>
        /// <param name="Title"> タイトル </param>
        public LogTitleAttribute(string Title)
        {
            this.Title = Title;
        }
    }
}
