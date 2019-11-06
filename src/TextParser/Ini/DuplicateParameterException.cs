namespace Enjaxel.TextParser.Ini
{
    /// <summary>
    /// パラメータ名に重複がある場合に発生する例外
    /// </summary>
    public sealed class DuplicateParameterException : TextParseException
    {
        /// <summary>
        /// パラメータ名に重複がある場合に発生します
        /// </summary>
        /// <param name="Message"> 例外内容 </param>
        public DuplicateParameterException(string Message) : base(Message)
        {
        }
    }
}
