using System;

namespace Enjaxel.TextParser
{
    /// <summary>
    /// テキストの構文解析中に発生する例外
    /// </summary>
    public class TextParseException : Exception
    {
        /// <summary>
        /// テキストの構文解析中に発生します
        /// </summary>
        /// <param name="Message"> 例外内容 </param>
        public TextParseException(string Message) : base(Message)
        {
        }
    }
}
