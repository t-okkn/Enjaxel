namespace Enjaxel.TextParser
{
    /// <summary>
    /// フィールドの項目名に重複がある場合に発生する例外
    /// </summary>
    public sealed class DuplicateFieldException : TextParseException
    {
        /// <summary>
        /// フィールドの項目名に重複がある場合に発生します
        /// </summary>
        /// <param name="Message"> 例外内容 </param>
        public DuplicateFieldException(string Message)
            : base(Message) { }
    }
}
