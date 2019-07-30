using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Enjaxel.Conversion;
using Enjaxel.TextParser.Config;

namespace Enjaxel.TextParser
{
    /// <summary>
    /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供するクラス
    /// </summary>
    public class CsvReader : TextParser
    {
        /// <summary> ヘッダーの一覧 </summary>
        private List<string> Headers { get; set; }

        /// <summary> コンテンツ </summary>
        private List<IReadOnlyList<string>> Contents { get; set; }

        /// <summary> フィールド前後の空白をトリムするかどうかのフラグ </summary>
        public new bool TrimWhiteSpace { get; }

        #region コンストラクタ
        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        public CsvReader() : base(',', true)
        {
            Init();
        }

        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvReader(char Delimiter) : base(Delimiter, true)
        {
            Init();
        }

        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvReader(bool HasHeader) : base(',', HasHeader)
        {
            Init();
        }

        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvReader(char Delimiter, Encoding CodePage)
            : base(Delimiter, CodePage, true)
        {
            Init();
        }

        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvReader(char Delimiter, bool HasHeader)
            : base(Delimiter, HasHeader)
        {
            Init();
        }

        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvReader(char Delimiter, bool HasHeader, Encoding CodePage)
            : base(Delimiter, CodePage, HasHeader)
        {
            Init();
        }

        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="ThrowError"> エラーをThrowするか </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvReader(char Delimiter, bool HasHeader, Encoding CodePage,
                         bool ThrowError)
            : base(Delimiter, CodePage, HasHeader, ThrowError)
        {
            Init();
        }

        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="ThrowError"> エラーをThrowするか </param>
        /// <param name="IgnoreBlankLine"> 空行を無視するか </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvReader(char Delimiter, bool HasHeader, Encoding CodePage,
                         bool ThrowError, bool IgnoreBlankLine)
            : base(Delimiter, CodePage, HasHeader, ThrowError, IgnoreBlankLine)
        {
            Init();
        }

        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="ThrowError"> エラーをThrowするか </param>
        /// <param name="IgnoreBlankLine"> 空行を無視するか </param>
        /// <param name="AllowComment"> コメント行を許可するか </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvReader(char Delimiter, bool HasHeader, Encoding CodePage,
                         bool ThrowError, bool IgnoreBlankLine, bool AllowComment)
            : base(Delimiter, CodePage, HasHeader, ThrowError, IgnoreBlankLine,
                   AllowComment)
        {
            Init();
        }

        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="ThrowError"> エラーをThrowするか </param>
        /// <param name="IgnoreBlankLine"> 空行を無視するか </param>
        /// <param name="AllowComment"> コメント行を許可するか </param>
        /// <param name="TrimWhiteSpace"> フィールド前後の空白をトリムするか（RFC4180非推奨） </param>
        /// <exception cref="ArgumentException"></exception>
        public CsvReader(char Delimiter, bool HasHeader, Encoding CodePage,
                         bool ThrowError, bool IgnoreBlankLine, bool AllowComment,
                         bool TrimWhiteSpace)
            : base(Delimiter, CodePage, HasHeader, ThrowError, IgnoreBlankLine,
                   AllowComment, TrimWhiteSpace)
        {
            Init();
        }

        /// <summary>
        /// コンストラクタ用初期化メソッド
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private void Init()
        {
            Headers = new List<string>();
            Contents = new List<IReadOnlyList<string>>();

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
                Delimiter = ',';
            }
        }
        #endregion

        #region Readメソッド
        /// <summary>
        /// CSVデータを読み込みます
        /// </summary>
        /// <param name="sequence"> 文字列のリスト（Iterator） </param>
        /// <returns> エラー発生回数 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="TextParseException"></exception>
        /// <exception cref="DuplicateHeaderException"></exception>
        /// <exception cref="NotMatchNumberOfFielsException"></exception>
        public int Read(IEnumerable<string> sequence)
        {
            int response = 0;
            bool first_flag = true;

            // コンテンツに何かデータがあれば削除
            if (Contents.Count > 0) { Clear(); }

            try
            {
                foreach (string[] seq in ParseCsv(sequence))
                {
                    var data = new List<string>(Headers.Count);

                    if (first_flag)
                    {
                        // 初回読み込み時にヘッダーの定義を行う
                        if (HasHeader)
                        {
                            if (seq.Length == 1 && string.IsNullOrEmpty(seq[0]))
                            {
                                throw new TextParseException("ヘッダーが空行です。");
                            }

                            // ヘッダーの存在フラグがあれば、データをそのままヘッダーとして扱う
                            Headers = seq.ToList();

                            // ヘッダーの空要素・要素重複は許可しない
                            for (int i = 0; i < Headers.Count; i++)
                            {
                                if (string.IsNullOrWhiteSpace(Headers[i]))
                                {
                                    throw new TextParseException
                                        ("ヘッダーに空文字列の要素もしくは空白文字のみで" +
                                         "構成された要素が存在しています。");
                                }

                                if (i != Headers.LastIndexOf(Headers[i]))
                                {
                                    throw new DuplicateHeaderException
                                        ("ヘッダーに重複した要素が存在しています。");
                                }
                            }
                        }
                        else
                        {
                            if (seq.Length == 1 && string.IsNullOrEmpty(seq[0]))
                            {
                                throw new TextParseException("1行目が空行です。");
                            }

                            // ヘッダーの存在フラグがなければ、ヘッダー情報を作成し、
                            // 1行目のデータは通常のデータとして扱う
                            for (int i = 0; i < seq.Length; i++)
                            {
                                Headers.Add("Data" + (i + 1).ToString());
                                data.Add(seq[i]);
                            }

                            Contents.Add(new ReadOnlyCollection<string>(data));
                        }

                        first_flag = false;
                    }
                    else
                    {
                        // ヘッダーの数を基準として読み込み
                        for (int i = 0; i < Headers.Count; i++)
                        {
                            string insert_str = string.Empty;

                            if (Headers.Count == seq.Length)
                            {
                                insert_str = seq[i];
                            }
                            else if ((!IgnoreBlankLine) &&
                                     seq.Length == 1 && string.IsNullOrEmpty(seq[0]))
                            {
                                // 空行を無視しない設定となっている場合、空行は
                                // 空データ扱いとして処理する
                            }
                            else
                            {
                                if (ThrowError)
                                {
                                    if (seq.Length == 1 && string.IsNullOrEmpty(seq[0]))
                                    {
                                        throw new TextParseException("不正なデータです。");
                                    }

                                    throw new NotMatchNumberOfFielsException
                                        ("フィールド数とヘッダーの数（1行目のフィールド数）" +
                                         "が一致していません。");
                                }
                                else
                                {
                                    response += 1;
                                }

                                // エラーをThrowしない場合、できる限り処理を行う
                                if (i < seq.Length && (!string.IsNullOrEmpty(seq[i])))
                                {
                                    insert_str = seq[i];
                                }
                            }

                            data.Add(insert_str);
                        }

                        Contents.Add(new ReadOnlyCollection<string>(data));
                    }
                }
            }
            catch (Exception)
            {
                if (ThrowError) { throw; }
                else { response += 1; }
            }

            return response;
        }

        /// <summary>
        /// CSVデータを読み込みます
        /// </summary>
        /// <param name="file"> CSVファイル </param>
        /// <returns> エラー発生回数 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="OutOfMemoryException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        /// <exception cref="NotMatchNumberOfFielsException"></exception>
        /// <exception cref="DuplicateHeaderException"></exception>
        /// <exception cref="TextParseException"></exception>
        public int Read(FileInfo file)
        {
            if (file.Exists)
            {
                return Read(CreateSequence(file));
            }
            else
            {
                if (ThrowError)
                {
                    throw new FileNotFoundException
                        ($"指定したファイル：[{file.FullName}] が存在しません。");
                }
                else
                {
                    return 1;
                }
            }
        }

        /// <summary>
        /// CSVデータを読み込みます
        /// </summary>
        /// <param name="data"> CSV文字列 </param>
        /// <returns> エラー発生回数 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="OutOfMemoryException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="RegexMatchTimeoutException"></exception>
        /// <exception cref="DuplicateHeaderException"></exception>
        /// <exception cref="NotMatchNumberOfFielsException"></exception>
        /// <exception cref="TextParseException"></exception>
        public int Read(string data)
        {
            return Read(CreateSequence(data));
        }
        #endregion

        #region Contents取得
        /// <summary>
        /// 読み込まれたCSVデータの内容を標準のオブジェクトで取得します
        /// </summary>
        /// <returns> Contentsオブジェクト </returns>
        /// <exception cref="ArgumentNullException"></exception>
        public CsvContents GetContents()
        {
            return new CsvContents(Headers, Contents);
        }

        /// <summary>
        /// 読み込まれたCSVデータの内容をカスタムオブジェクトで取得します
        /// </summary>
        /// <typeparam name="T"> マッピングするオブジェクトの型 </typeparam>
        /// <returns> マッピングされたオブジェクトのList </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        /// <exception cref="MethodAccessException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <exception cref="TargetException"></exception>
        /// <exception cref="TargetInvocationException"></exception>
        /// <exception cref="AmbiguousMatchException"></exception>
        /// <exception cref="FailedMappingException"></exception>
        public List<T> GetContents<T>() where T : new()
        {
            var res = new List<T>();
            var maps = new List<MappingObject>();

            try
            {
                if (Headers.Count == 0)
                {
                    return res;
                }

                PropertyInfo[] properties = typeof(T).GetProperties();

                if (properties.Length != Headers.Count)
                {
                    throw new FailedMappingException
                        ("マッピングしようとしているPropertyの数とヘッダーの数が一致しません。");
                }

                foreach (PropertyInfo prop in properties)
                {
                    var map = new MappingObject();

                    // Propertyに紐付いた属性値を取得
                    HeaderAttribute header_atrb =
                        prop.GetCustomAttribute<HeaderAttribute>();

                    if (prop.PropertyType.Name.Contains("Nullable"))
                    {
                        // Nullableの型の時
                        // GetUnderlyingTypeから型を取得する
                        map.ObjectType = Nullable.GetUnderlyingType(prop.PropertyType);

                        // Nullableフラグを立てる
                        map.IsNullable = true;
                    }
                    else
                    {
                        // Nullableの型でないときは普通にPropertyTypeから取得
                        map.ObjectType = prop.PropertyType;
                    }

                    if (header_atrb != null && Headers.Contains(header_atrb.Name))
                    {
                        // 属性値が設定されていて、属性値に対応するヘッダーが存在している場合
                        // ヘッダーに対応するプロパティ名を取得
                        map.Name = prop.Name;
                        map.Number = Headers.IndexOf(header_atrb.Name);
                    }
                    else if (Headers.Contains(prop.Name))
                    {
                        // 属性値が設定されていなければヘッダーに対応するプロパティ名を
                        // 検索して取得
                        map.Name = prop.Name;
                        map.Number = Headers.IndexOf(prop.Name);
                    }
                    else
                    {
                        throw new FailedMappingException
                            ("ヘッダー名称がマッピングしようとしているオブジェクト内に" +
                             "存在しません。");
                    }

                    maps.Add(map);
                }

                // プロパティに値を設定するためのローカル関数
                void setValue(ref T obj, string propName, object value) =>
                    typeof(T).GetRuntimeProperty(propName).SetValue(obj, value);

                foreach (var ctn in Contents)
                {
                    // ジェネリクスのインスタンス作成
                    var Tobj = Activator.CreateInstance<T>();

                    foreach (var map in maps)
                    {
                        // コンテンツから値を取得
                        string str = ctn[map.Number];

                        if (map.ObjectType.Equals(typeof(string)))
                        {
                            // マッピング先の型がstringならそのまま値を入れる
                            setValue(ref Tobj, map.Name, str);
                        }
                        else
                        {
                            // マッピング先の型がstring以外なら値を変換
                            TypeCode code = Type.GetTypeCode(map.ObjectType);
                            object value = string.IsNullOrWhiteSpace(str) ?
                                               null : str.ConvertValue(code);

                            // valueがnullで、マッピング先の型がNullableでない場合を除き
                            // 値を設定する
                            if (!(value == null && (!map.IsNullable)))
                            {
                                setValue(ref Tobj, map.Name, value);
                            }

                            // valueがnullで、マッピング先の型がNullableでない場合は
                            // マッピング先クラスのコンストラクタの初期値に依存させる
                        }
                    }

                    res.Add(Tobj);
                }
            }
            catch
            {
                throw;
            }

            return res;
        }

        /// <summary>
        /// 読み込まれたCSVデータの内容をDataTableとして取得します
        /// </summary>
        /// <param name="tableName"> DataTableのテーブル名 </param>
        /// <returns> DataTable </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DuplicateNameException"></exception>
        /// <exception cref="InvalidExpressionException"></exception>
        /// <exception cref="ConstraintException"></exception>
        /// <exception cref="NoNullAllowedException"></exception>
        public DataTable GetContents(string tableName)
        {
            var dt = new DataTable(tableName);

            try
            {
                for (int i = 0; i < Headers.Count; i++)
                {
                    dt.Columns.Add(new DataColumn(Headers[i], typeof(string)));
                }

                foreach (var ctn in Contents)
                {
                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < Headers.Count; i++)
                    {
                        dr[i] = ctn[i];
                    }

                    dt.Rows.Add(dr);
                }
            }
            catch
            {
                throw;
            }

            return dt;
        }

        /// <summary>
        /// CSVから読み込んだ一時データを削除します
        /// </summary>
        public void Clear()
        {
            Headers.Clear();
            Contents.Clear();
        }
        #endregion

        #region privateメソッド
        /// <summary>
        /// 文字列を一行ずつ読み込み、Iteratorとして返します
        /// </summary>
        /// <param name="input"> 文字列 </param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="OutOfMemoryException"></exception>
        private IEnumerable<string> CreateSequence(string input)
        {
            using (var sr = new StringReader(input))
            {
                while (sr.Peek() > -1)
                {
                    yield return sr.ReadLine();
                }
            }
        }

        /// <summary>
        /// ファイルの内容を一行ずつ読み込み、Iteratorとして返します
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="OutOfMemoryException"></exception>
        /// <exception cref="IOException"></exception>
        private IEnumerable<string> CreateSequence(FileInfo file)
        {
            using (var sr = new StreamReader(file.FullName, CodePage))
            {
                while (sr.Peek() > -1)
                {
                    yield return sr.ReadLine();
                }
            }
        }
        #endregion
    }
}
