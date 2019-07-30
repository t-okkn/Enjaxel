using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using Enjaxel.TextParser.Config;

namespace Enjaxel.TextParser
{
    /// <summary>
    /// データを形式化されたテキストファイルとして出力します
    /// </summary>
    public class CsvWriter : ITextParserConfiguration
    {
        /// <summary> 区切り文字 </summary>
        public char Delimiter { get; set; }

        /// <summary> 文字コード </summary>
        public Encoding CodePage { get; set; }

        /// <summary> ヘッダーを出力するかのフラグ </summary>
        public bool HasHeader { get; set; }

        /// <summary> フィールドをダブルクォーテーションで全て囲うかのフラグ </summary>
        public bool IsEncloseFieldInDoubleQuotes { get; set; }

        #region コンストラクタ
        /// <summary>
        /// データを形式化されたテキストファイルとして出力します
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public CsvWriter()
        {
            Delimiter = ',';
            CodePage = Encoding.GetEncoding("Shift_JIS");
            HasHeader = true;
            IsEncloseFieldInDoubleQuotes = false;

            CtorCommon();
        }

        /// <summary>
        /// データを形式化されたテキストファイルとして出力します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvWriter(char Delimiter)
        {
            this.Delimiter = Delimiter;
            CodePage = Encoding.GetEncoding("Shift_JIS");
            HasHeader = true;
            IsEncloseFieldInDoubleQuotes = false;

            CtorCommon();
        }

        /// <summary>
        /// データを形式化されたテキストファイルとして出力します
        /// </summary>
        /// <param name="HasHeader"> ヘッダーを出力するか </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvWriter(bool HasHeader)
        {
            this.HasHeader = HasHeader;
            Delimiter = ',';
            CodePage = Encoding.GetEncoding("Shift_JIS");
            IsEncloseFieldInDoubleQuotes = false;

            CtorCommon();
        }

        /// <summary>
        /// データを形式化されたテキストファイルとして出力します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="HasHeader"> ヘッダーを出力するか </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvWriter(char Delimiter, bool HasHeader)
        {
            this.Delimiter = Delimiter;
            this.HasHeader = HasHeader;
            CodePage = Encoding.GetEncoding("Shift_JIS");
            IsEncloseFieldInDoubleQuotes = false;

            CtorCommon();
        }

        /// <summary>
        /// データを形式化されたテキストファイルとして出力します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvWriter(char Delimiter, Encoding CodePage)
        {
            this.Delimiter = Delimiter;
            this.CodePage = CodePage;
            HasHeader = true;
            IsEncloseFieldInDoubleQuotes = false;

            CtorCommon();
        }

        /// <summary>
        /// データを形式化されたテキストファイルとして出力します
        /// </summary>
        /// <param name="HasHeader"> ヘッダーを出力するか </param>
        /// <param name="IsEncloseFieldInDoubleQuotes">
        /// フィールドをダブルクォーテーションで全て囲うか </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvWriter(bool HasHeader, bool IsEncloseFieldInDoubleQuotes)
        {
            this.HasHeader = HasHeader;
            this.IsEncloseFieldInDoubleQuotes = IsEncloseFieldInDoubleQuotes;
            Delimiter = ',';
            CodePage = Encoding.GetEncoding("Shift_JIS");

            CtorCommon();
        }

        /// <summary>
        /// データを形式化されたテキストファイルとして出力します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="HasHeader"> ヘッダーを出力するか </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvWriter(char Delimiter, Encoding CodePage, bool HasHeader)
        {
            this.Delimiter = Delimiter;
            this.CodePage = CodePage;
            this.HasHeader = HasHeader;
            IsEncloseFieldInDoubleQuotes = false;

            CtorCommon();
        }

        /// <summary>
        /// データを形式化されたテキストファイルとして出力します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="HasHeader"> ヘッダーを出力するか </param>
        /// <param name="IsEncloseFieldInDoubleQuotes">
        /// フィールドをダブルクォーテーションで全て囲うか </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvWriter(char Delimiter, bool HasHeader,
                         bool IsEncloseFieldInDoubleQuotes)
        {
            this.Delimiter = Delimiter;
            this.HasHeader = HasHeader;
            this.IsEncloseFieldInDoubleQuotes = IsEncloseFieldInDoubleQuotes;
            CodePage = Encoding.GetEncoding("Shift_JIS");

            CtorCommon();
        }

        /// <summary>
        /// データを形式化されたテキストファイルとして出力します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="HasHeader"> ヘッダーを出力するか </param>
        /// <param name="IsEncloseFieldInDoubleQuotes">
        /// フィールドをダブルクォーテーションで全て囲うか </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvWriter(char Delimiter, Encoding CodePage, bool HasHeader,
                         bool IsEncloseFieldInDoubleQuotes)
        {
            this.Delimiter = Delimiter;
            this.CodePage = CodePage;
            this.HasHeader = HasHeader;
            this.IsEncloseFieldInDoubleQuotes = IsEncloseFieldInDoubleQuotes;

            CtorCommon();
        }

        /// <summary>
        /// コンストラクタ用共通メソッド
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private void CtorCommon()
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
        #endregion

        #region Writeメソッド
        /// <summary>
        /// CsvContentsの情報をCSVとしてテキストファイルに出力します
        /// </summary>
        /// <param name="outputPath"> 出力先テキストのPath </param>
        /// <param name="contents"> CsvContents </param>
        /// <example></example>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        /// <exception cref="SecurityException"></exception>
        public void Write(string outputPath, CsvContents contents)
        {
            if (contents.Contents.Count == 0) { return; }

            string path = DirectoryOperation(outputPath);

            using (var sw = new StreamWriter(path, false, CodePage))
            {
                foreach (string str in GetCsvString(contents))
                {
                    sw.WriteLine(str);
                }
            }
        }

        /// <summary>
        /// T型Entityの情報をCSVとしてテキストファイルに出力します
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="outputPath"> 出力先テキストのPath </param>
        /// <param name="contents"> T型EntityのList </param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        /// <exception cref="SecurityException"></exception>
        public void Write<T>(string outputPath, List<T> contents)
            where T : new()
        {
            if (contents.Count == 0) { return; }

            string path = DirectoryOperation(outputPath);

            using (var sw = new StreamWriter(path, false, CodePage))
            {
                foreach (string str in GetCsvString(contents))
                {
                    sw.WriteLine(str);
                }
            }
        }

        /// <summary>
        /// DataTableの情報をCSVとしてテキストファイルに出力します
        /// </summary>
        /// <param name="outputPath"> 出力先テキストのPath </param>
        /// <param name="contents"> DataTable </param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DuplicateNameException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        /// <exception cref="SecurityException"></exception>
        public void Write(string outputPath, DataTable contents)
        {
            if (contents.Rows.Count == 0) { return; }

            string path = DirectoryOperation(outputPath);

            using (var sw = new StreamWriter(path, false, CodePage))
            {
                foreach (string str in GetCsvString(contents))
                {
                    sw.WriteLine(str);
                }
            }
        }
        #endregion

        #region GetCsvStringメソッド
        /// <summary>
        /// オブジェクトから文字列としてのCSVデータを取得します
        /// </summary>
        /// <param name="contents"> CsvContents </param>
        /// <returns> CSVデータ（Iterator） </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public IEnumerable<string> GetCsvString(CsvContents contents)
        {
            // ヘッダーを出力する場合
            if (HasHeader)
            {
                string header = string.Empty;

                for (int i = 0; i < contents.Headers.Count; i++)
                {
                    header += contents.Headers[i] + Delimiter.ToString();
                }

                yield return header.Remove(header.Length - 1, 1);
            }

            // Contentsから行ごとのデータを返す
            foreach (var ctn in contents.Contents)
            {
                yield return GetLineData(ctn);
            }
        }

        /// <summary>
        /// オブジェクトから文字列としてのCSVデータを取得します
        /// </summary>
        /// <typeparam name="T"> Entityの型 </typeparam>
        /// <param name="contents"> T型オブジェクトのList </param>
        /// <returns> CSVデータ（Iterator） </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public IEnumerable<string> GetCsvString<T>(List<T> contents)
            where T : new()
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            string[] propNames = properties.Select((x) => x.Name).ToArray();

            // T型オブジェクトの情報をstringのListに変換するローカル関数
            List<string> getValues(T obj)
            {
                var l = new List<string>(propNames.Length);

                foreach (string n in propNames)
                {
                    l.Add(typeof(T).GetRuntimeProperty(n).GetValue(obj).ToString());
                }

                return l;
            };

            // ヘッダーを出力する場合
            if (HasHeader)
            {
                string header = string.Empty;

                foreach (var prop in properties)
                {
                    // Propertyに紐付いた属性値を取得
                    HeaderAttribute header_atrb =
                        prop.GetCustomAttribute<HeaderAttribute>();

                    if (header_atrb != null &&
                        (!string.IsNullOrWhiteSpace(header_atrb.Name)))
                    {
                        // 属性値が設定されていて、属性値が空文字・空白でない場合
                        // ヘッダーとする
                        header += header_atrb.Name + Delimiter.ToString();
                    }
                    else
                    {
                        header += prop.Name + Delimiter.ToString();
                    }
                }

                yield return header.Remove(header.Length - 1, 1);
            }

            // T型オブジェクトから行ごとのデータを返す
            foreach (var ctn in contents)
            {
                yield return GetLineData(getValues(ctn));
            }
        }

        /// <summary>
        /// オブジェクトから文字列としてのCSVデータを取得します
        /// </summary>
        /// <param name="contents"> DataTable </param>
        /// <returns> CSVデータ（Iterator） </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="DuplicateNameException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        public IEnumerable<string> GetCsvString(DataTable contents)
        {
            // DataRowの情報をstringのListに変換するローカル関数
            List<string> getValues(DataRow dr)
            {
                var l = new List<string>(contents.Columns.Count);

                foreach (DataColumn c in contents.Columns)
                {
                    l.Add(dr[c].ToString());
                }

                return l;
            };

            // ヘッダーを出力する場合
            if (HasHeader)
            {
                string header = string.Empty;

                // DataColumnからヘッダー情報を生成
                foreach (DataColumn col in contents.Columns)
                {
                    header += col.ColumnName + Delimiter.ToString();
                }

                yield return header.Remove(header.Length - 1, 1);
            }

            // DataRowから行ごとのデータを返す
            foreach (DataRow dr in contents.Rows)
            {
                yield return GetLineData(getValues(dr));
            }
        }
        #endregion

        #region privateメソッド
        /// <summary>
        /// 書き込み先のファイルについて、ディレクトリ情報の前操作を行う
        /// </summary>
        /// <param name="path"> 書き込み先のファイルPath </param>
        /// <returns> 調査済の書き込み先のファイルPath </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="IOException"></exception>
        private string DirectoryOperation(string path)
        {
            if (File.Exists(path))
            {
                string basedir = Path.GetDirectoryName(path);
                string filename = Path.GetFileNameWithoutExtension(path);
                string ext = Path.GetExtension(path);

                // ファイルが既に存在する場合は現在日時を付与
                filename += $"_{DateTime.Now.ToString("yyyyMMdd-hhmmss")}" + ext;

                path = Path.Combine(basedir, filename);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            return path;
        }

        /// <summary>
        /// 1行分のフィールドデータから1行分の文字列データを生成します
        /// </summary>
        /// <param name="input"> 1行分のフィールドデータ </param>
        /// <returns> 1行分の文字列 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        private string GetLineData(IEnumerable<string> input)
        {
            string line = string.Empty;
            bool first_field = true;

            foreach (string value in input)
            {
                if (IsEncloseFieldInDoubleQuotes)
                {
                    // 全てのフィールドをダブルクォーテーションで囲う設定の場合はそれに従う
                    line += EncloseDoubleQuotes(value) + Delimiter.ToString();
                }
                else
                {
                    // 全てのフィールドをダブルクォーテーションで囲う設定でない場合は
                    // フィールドデータごとに判定を行う
                    if (IsNeedDoubleQuotes(value) ||
                        (first_field && Regex.IsMatch(value, "^#")))
                    {
                        line += EncloseDoubleQuotes(value) + Delimiter.ToString();
                    }
                    else
                    {
                        line += value + Delimiter.ToString();
                    }
                }

                first_field = false;
            }

            return line.Remove(line.Length - 1, 1);
        }

        /// <summary>
        /// 文字列をダブルクォーテーションで囲みます
        /// </summary>
        /// <param name="field"> 文字列 </param>
        /// <returns> ダブルクォーテーションで囲われた文字列 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        private string EncloseDoubleQuotes(string field)
        {
            // ダブルクォーテーションがあればエスケープする
            if (field.Contains('"'))
            {
                field = field.Replace(@"""", @"""""");
            }

            return $@"""{field}""";
        }

        /// <summary>
        /// 文字列をダブルクォーテーションで囲む必要があるか判定します
        /// </summary>
        /// <param name="field"> 文字列 </param>
        /// <returns> 要否 </returns>
        /// <exception cref="ArgumentNullException"></exception>
        private bool IsNeedDoubleQuotes(string field)
        {
            return field.Contains('"') || field.Contains(Delimiter) ||
                   field.Contains('\n') || field.Contains('\r');
        }
        #endregion
    }
}
