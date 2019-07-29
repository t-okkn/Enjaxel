namespace Enjaxel.TextParser
{
    /// <summary>
    /// セクションの情報に重複がある場合に発生する例外
    /// </summary>
    public sealed class DuplicateSectionException : TextParseException
    {
        /// <summary>
        /// セクションの情報に重複がある場合に発生します
        /// </summary>
        /// <param name="Message"> 例外内容 </param>
        public DuplicateSectionException(string Message)
            : base(Message) { }
    }
}
