namespace Enjaxel.TextParser.Csv
{
    /// <summary>
    /// フィールドの数がヘッダー部の数と一致しない場合に発生する例外
    /// </summary>
    public sealed class MismatchFielsCountException : TextParseException
    {
        /// <summary>
        /// フィールドの数がヘッダー部の数と一致しない場合に発生します
        /// </summary>
        /// <param name="Message"> 例外内容 </param>
        public MismatchFielsCountException(string Message) : base(Message)
        {
        }
    }
}
