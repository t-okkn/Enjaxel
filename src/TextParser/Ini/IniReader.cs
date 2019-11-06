using System;
using System.Collections.Generic;
using System.Text;

// 実装途中

namespace Enjaxel.TextParser.Ini
{
    /// <summary>
    /// INIファイルの読み込み機能を提供します
    /// </summary>
    public class IniReader : TextParser
    {
        /// <summary> セクションの一覧 </summary>
        private List<string> Sections { get; set; }

        /// <summary> コンテンツ </summary>
        private List<IniSection> Contents { get; set; }

        /// <summary> パラメータの区切り文字 </summary>
        public new char Delimiter { get; }

        /// <summary> ヘッダーが存在するかのフラグ（ヘッダーは存在しない） </summary>
        private new bool HasHeader { get; set; }

        /// <summary> 空行を無視するかどうかのフラグ（空行は常に無視） </summary>
        private new bool IgnoreBlankLine { get; set; }

        /// <summary>
        /// INIファイルの読み込み機能を提供します
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public IniReader()
            : this(Encoding.GetEncoding("Shift_JIS"), true, true, '=', true)
        {
        }

        /// <summary>
        /// INIファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="CodePage"> 文字コード </param>
        /// <exception cref="ArgumentException"></exception>
        public IniReader(Encoding CodePage) : this(CodePage, true, true, '=', true)
        {
        }

        /// <summary>
        /// INIファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="ThrowError"> エラーをThrowするか（Readのみ） </param>
        /// <exception cref="ArgumentException"></exception>
        public IniReader(Encoding CodePage, bool ThrowError)
            : this(CodePage, ThrowError, true, '=', true)
        {
        }

        /// <summary>
        /// INIファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="ThrowError"> エラーをThrowするか（Readのみ） </param>
        /// <param name="TrimWhiteSpace"> パラメータ前後の空白をトリムするか </param>
        /// <exception cref="ArgumentException"></exception>
        public IniReader(Encoding CodePage, bool ThrowError, bool TrimWhiteSpace)
            : this(CodePage, ThrowError, TrimWhiteSpace, '=', true)
        {
        }

        /// <summary>
        /// INIファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="ThrowError"> エラーをThrowするか（Readのみ） </param>
        /// <param name="TrimWhiteSpace"> パラメータ前後の空白をトリムするか </param>
        /// <param name="Delimiter"> パラメータの区切り文字 </param>
        /// <exception cref="ArgumentException"></exception>
        public IniReader(Encoding CodePage, bool ThrowError, bool TrimWhiteSpace,
                         char Delimiter)
            : this(CodePage, ThrowError, TrimWhiteSpace, Delimiter, true)
        {
        }

        /// <summary>
        /// INIファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="ThrowError"> エラーをThrowするか（Readのみ） </param>
        /// <param name="TrimWhiteSpace"> パラメータ前後の空白をトリムするか </param>
        /// <param name="Delimiter"> パラメータの区切り文字 </param>
        /// <param name="AllowComment"> コメントを許可するか </param>
        /// <exception cref="ArgumentException"></exception>
        public IniReader(Encoding CodePage, bool ThrowError, bool TrimWhiteSpace,
                         char Delimiter, bool AllowComment)
            : base(Delimiter, CodePage, false, ThrowError, true,
                   AllowComment, TrimWhiteSpace)
        {
            Sections = new List<string>();
            Contents = new List<IniSection>();

            if (ThrowError)
            {
                if (Delimiter == '"')
                {
                    throw new ArgumentException
                        ("区切り文字にダブルクォーテーションは指定できません。");
                }
                else if (Delimiter == '\0')
                {
                    throw new ArgumentException
                        ("区切り文字にnull文字は指定できません。");
                }
                else if (Delimiter == '\r' || Delimiter == '\n')
                {
                    throw new ArgumentException
                        ("区切り文字に改行コードは指定できません。");
                }
            }
            else
            {
                Delimiter = '=';
            }
        }
    }
}
