using System.Text;

namespace Enjaxel.TextParser
{
    /// <summary>
    /// 形式化されたテキストを読み込む際の設定値を定めるインターフェース
    /// </summary>
    internal interface ITextParserConfiguration
    {
        /// <summary> 文字コード </summary>
        Encoding CodePage { get; set; }

        /// <summary> 区切り文字 </summary>
        char Delimiter { get; set; }

        /// <summary> エラーをThrowするかどうかのフラグ </summary>
        bool ThrowError { get; set; }

        /// <summary> コメント行を許可するかどうかのフラグ </summary>
        bool AllowComment { get; set; }
    }
}
