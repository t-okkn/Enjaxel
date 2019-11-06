namespace Enjaxel.TextParser
{
    /// <summary>
    /// ヘッダーの情報に重複がある場合に発生する例外
    /// </summary>
    public sealed class DuplicateHeaderException : TextParseException
    {
        /// <summary>
        /// ヘッダーの情報に重複がある場合に発生します
        /// </summary>
        /// <param name="Message"> 例外内容 </param>
        public DuplicateHeaderException(string Message) : base(Message)
        {
        }
    }
}
