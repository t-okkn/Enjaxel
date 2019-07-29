namespace Enjaxel.TextParser
{
    /// <summary>
    /// フィールドの数がヘッダー部の数と一致しない場合に発生する例外
    /// </summary>
    public sealed class NotMatchNumberOfFielsException : TextParseException
    {
        /// <summary>
        /// フィールドの数がヘッダー部の数と一致しない場合に発生します
        /// </summary>
        /// <param name="Message"> 例外内容 </param>
        public NotMatchNumberOfFielsException(string Message)
            : base(Message) { }
    }
}
