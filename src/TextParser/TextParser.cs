using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Enjaxel.TextParser
{
    /// <summary>
    /// 形式化されたテキストファイルの構文解析機能を提供するクラス
    /// </summary>
    public abstract class TextParser : ITextParserConfiguration
    {
        /// <summary> 区切り文字 </summary>
        public char Delimiter { get; set; }

        /// <summary> 文字コード </summary>
        public Encoding CodePage { get; set; }

        /// <summary> ヘッダーが存在するかのフラグ </summary>
        public bool HasHeader { get; set; }

        /// <summary> エラーをThrowするかどうかのフラグ </summary>
        public bool ThrowError { get; set; }

        /// <summary> 空行を無視するかどうかのフラグ </summary>
        public bool IgnoreBlankLine { get; set; }

        /// <summary> コメント行を許可するかどうかのフラグ </summary>
        public bool AllowComment { get; set; }

        /// <summary> フィールド前後の空白をトリムするかどうかのフラグ </summary>
        public bool TrimWhiteSpace { get; set; }

        /// <summary> 環境依存の改行コード </summary>
        internal string NL { get; }

        #region コンストラクタ
        /// <summary>
        /// 形式化されたテキストファイルの構文解析機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        internal TextParser(char Delimiter)
        {
            this.Delimiter = Delimiter;
            CodePage = Encoding.GetEncoding("Shift_JIS");
            HasHeader = false;
            ThrowError = true;
            IgnoreBlankLine = false;
            AllowComment = true;
            TrimWhiteSpace = false;
            NL = Environment.NewLine;
        }

        /// <summary>
        /// 形式化されたテキストファイルの構文解析機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
        internal TextParser(char Delimiter, Encoding CodePage)
        {
            this.Delimiter = Delimiter;
            this.CodePage = CodePage;
            HasHeader = false;
            ThrowError = true;
            IgnoreBlankLine = false;
            AllowComment = true;
            TrimWhiteSpace = false;
            NL = Environment.NewLine;
        }

        /// <summary>
        /// 形式化されたテキストファイルの構文解析機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        internal TextParser(char Delimiter, bool HasHeader)
        {
            this.Delimiter = Delimiter;
            this.HasHeader = HasHeader;
            CodePage = Encoding.GetEncoding("Shift_JIS");
            ThrowError = true;
            IgnoreBlankLine = false;
            AllowComment = true;
            TrimWhiteSpace = false;
            NL = Environment.NewLine;
        }

        /// <summary>
        /// 形式化されたテキストファイルの構文解析機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        internal TextParser(char Delimiter, Encoding CodePage, bool HasHeader)
        {
            this.Delimiter = Delimiter;
            this.CodePage = CodePage;
            this.HasHeader = HasHeader;
            ThrowError = true;
            IgnoreBlankLine = false;
            AllowComment = true;
            TrimWhiteSpace = false;
            NL = Environment.NewLine;
        }

        /// <summary>
        /// 形式化されたテキストファイルの構文解析機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <param name="ThrowError"> エラーをThrowするか </param>
        internal TextParser(char Delimiter, Encoding CodePage, bool HasHeader,
                            bool ThrowError)
        {
            this.Delimiter = Delimiter;
            this.CodePage = CodePage;
            this.HasHeader = HasHeader;
            this.ThrowError = ThrowError;
            IgnoreBlankLine = false;
            AllowComment = true;
            TrimWhiteSpace = false;
            NL = Environment.NewLine;
        }

        /// <summary>
        /// 形式化されたテキストファイルの構文解析機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <param name="ThrowError"> エラーをThrowするか </param>
        /// <param name="IgnoreBlankLine"> 空行を無視するか </param>
        internal TextParser(char Delimiter, Encoding CodePage, bool HasHeader,
                            bool ThrowError, bool IgnoreBlankLine)
        {
            this.Delimiter = Delimiter;
            this.CodePage = CodePage;
            this.HasHeader = HasHeader;
            this.ThrowError = ThrowError;
            this.IgnoreBlankLine = IgnoreBlankLine;
            AllowComment = true;
            TrimWhiteSpace = false;
            NL = Environment.NewLine;
        }

        /// <summary>
        /// 形式化されたテキストファイルの構文解析機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <param name="ThrowError"> エラーをThrowするか </param>
        /// <param name="IgnoreBlankLine"> 空行を無視するか </param>
        /// <param name="AllowComment"> コメント行を許可するか </param>
        internal TextParser(char Delimiter, Encoding CodePage, bool HasHeader,
                            bool ThrowError, bool IgnoreBlankLine, bool AllowComment)
        {
            this.Delimiter = Delimiter;
            this.CodePage = CodePage;
            this.HasHeader = HasHeader;
            this.ThrowError = ThrowError;
            this.IgnoreBlankLine = IgnoreBlankLine;
            this.AllowComment = AllowComment;
            TrimWhiteSpace = false;
            NL = Environment.NewLine;
        }

        /// <summary>
        /// 形式化されたテキストファイルの構文解析機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <param name="ThrowError"> エラーをThrowするか </param>
        /// <param name="IgnoreBlankLine"> 空行を無視するか </param>
        /// <param name="AllowComment"> コメント行を許可するか </param>
        /// <param name="TrimWhiteSpace"> フィールド前後の空白をトリムするか </param>
        internal TextParser(char Delimiter, Encoding CodePage, bool HasHeader,
                            bool ThrowError, bool IgnoreBlankLine, bool AllowComment,
                            bool TrimWhiteSpace)
        {
            this.Delimiter = Delimiter;
            this.CodePage = CodePage;
            this.HasHeader = HasHeader;
            this.ThrowError = ThrowError;
            this.IgnoreBlankLine = IgnoreBlankLine;
            this.AllowComment = AllowComment;
            this.TrimWhiteSpace = TrimWhiteSpace;
            NL = Environment.NewLine;
        }
        #endregion

        #region CSV関連
        /// <summary>
        /// CSVを一行ずつ解析して、CSVのフィールドデータを返します
        /// </summary>
        /// <param name="contents"> 元データ </param>
        /// <returns> CSVのフィールドデータ群 </returns>
        internal IEnumerable<string[]> ParseCsv(IEnumerable<string> contents)
        {
            string stack = string.Empty;

            foreach (string str in contents)
            {
                // コメント行は設定次第で無視する
                if (AllowComment && Regex.IsMatch(str, "^#"))
                {
                    continue;
                }

                int dq_num = CountDoubleQuotes(str);
                bool is_stack_empty = string.IsNullOrEmpty(stack);

                if ((dq_num % 2 == 1 && is_stack_empty) ||
                    (dq_num % 2 == 0 && (!is_stack_empty)))
                {
                    // ダブルクォーテーションの数が奇数でスタックが空文字か
                    // ダブルクォーテーションの数が偶数でスタックが空文字でない場合
                    // stackへ積み込んで次へ
                    stack += str + NL;
                    continue;
                }
                else
                {
                    // ダブルクォーテーションの数が偶数でスタックが空文字か
                    // ダブルクォーテーションの数が奇数でスタックが空文字でない場合
                    // 対象とする文字列を生成
                    string target = is_stack_empty ? str : stack + str;
                    string[] split = target.Split(Delimiter);

                    if (is_stack_empty && IgnoreBlankLine &&
                        split.Length == 1 && string.IsNullOrEmpty(split[0]))
                    {
                        // stackが空文字・空行を無視・空行の確認ができた場合、次へ
                        continue;
                    }

                    if (CountDoubleQuotes(target) % 2 == 0)
                    {
                        // 対象とする文字列がダブルクォーテーションに問題がない場合
                        // フィールドデータを取得
                        yield return GetCsvFieldData(ref split);
                    }

                    stack = string.Empty;
                }
            }

            if (!string.IsNullOrEmpty(stack))
            {
                throw new TextParseException
                    ("データ内に無効なダブルクォーテーションが存在します。");
            }
        }

        /// <summary>
        /// CSVのフィールドデータを取得します
        /// </summary>
        /// <param name="input"> 区切り文字で区切られた各データ </param>
        /// <returns> CSVのフィールドデータ </returns>
        private string[] GetCsvFieldData(ref string[] input)
        {
            // ローカル変数を宣言
            var res_arr = new string[0];
            string stack = string.Empty;
            var dq_reg = new Regex(@"^""(.*)""$", RegexOptions.Singleline);

            // 区切り文字で区切られた各データをそれぞれ読み込みます
            foreach (string str in input)
            {
                int dq_num = CountDoubleQuotes(str);
                bool is_stack_empty = string.IsNullOrEmpty(stack);

                if (dq_num > 0)
                {
                    // ダブルクォーテーションが文字列内に含まれている場合
                    if ((dq_num % 2 == 1 && is_stack_empty) ||
                        (dq_num % 2 == 0 && (!is_stack_empty)))
                    {
                        // ダブルクォーテーションの数が奇数でスタックが空文字か
                        // ダブルクォーテーションの数が偶数でスタックが空文字でない場合
                        // stackへ積み込んで次へ
                        stack += str + Delimiter.ToString();
                        continue;
                    }
                    else
                    {
                        // ダブルクォーテーションの数が偶数でスタックが空文字か
                        // ダブルクォーテーションの数が奇数でスタックが空文字でない場合
                        // 対象とする文字列を生成
                        string target = is_stack_empty ? str : stack + str;

                        if (CountDoubleQuotes(target) % 2 == 0)
                        {
                            Match m = dq_reg.Match(target.Trim());
                            string last_str = string.Empty;

                            // 前後のダブルクォーテーションを一つずつ取り除いて
                            // ダブルクォーテーションを一つにする
                            if (m.Success)
                            {
                                last_str = m.Groups[1].Value.Replace(@"""""", @"""");
                            }

                            // CSVのフィールドデータとして確定
                            Array.Resize(ref res_arr, res_arr.Length + 1);
                            res_arr[res_arr.Length - 1] = last_str;
                        }

                        stack = string.Empty;
                    }
                }
                else
                {
                    // ダブルクォーテーションが文字列内に含まれていない場合
                    if (!is_stack_empty)
                    {
                        // stackが空文字でない場合、stackへ積み込んで次へ
                        stack += str + Delimiter.ToString();
                        continue;
                    }
                    else
                    {
                        // stackが空文字の場合、CSVのフィールドデータとして確定
                        Array.Resize(ref res_arr, res_arr.Length + 1);
                        res_arr[res_arr.Length - 1] = TrimWhiteSpace ? str.Trim() : str;
                    }
                }
            }

            return res_arr;
        }
        #endregion

        /// <summary>
        /// 文字列内に含まれているダブルクォーテーションの数を数えます
        /// </summary>
        /// <param name="input"> 入力文字列 </param>
        /// <returns> 文字列内に含まれているダブルクォーテーションの数 </returns>
        private int CountDoubleQuotes(string input)
        {
            return input.Where((x) => x == '"').Count();
        }
    }
}
