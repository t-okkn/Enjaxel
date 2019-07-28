using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enjaxel.Constant;
using Enjaxel.TextParser.Config;

namespace Enjaxel.TextParser
{
    /// <summary>
    /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供するクラス
    /// </summary>
    public class CsvReader : TextParser
    {
        /// <summary>
        /// マッピング時に使用するクラス
        /// </summary>
        internal class MappingObject
        {
            /// <summary> Headerと紐づく番号 </summary>
            internal int Number { get; set; }

            /// <summary> プロパティ名称 </summary>
            internal string Name { get; set; }

            /// <summary> プロパティの型 </summary>
            internal Type ObjectType { get; set; }

            /// <summary> Nullable判定 </summary>
            internal bool IsNullable { get; set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            internal MappingObject()
            {
                Number = -1;
                Name = string.Empty;
                ObjectType = null;
                IsNullable = false;
            }
        }

        /// <summary> ヘッダーの一覧 </summary>
        private List<string> Headers { get; set; }

        /// <summary> コンテンツ </summary>
        private List<IReadOnlyDictionary<string, string>> Contents { get; set; }

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
        public CsvReader(char Delimiter) : base(Delimiter, true)
        {
            Init();
        }

        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
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
        public CsvReader(char Delimiter, bool HasHeader)
            : base(Delimiter, HasHeader)
        {
            Init();
        }

        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        public CsvReader(char Delimiter, Encoding CodePage, bool HasHeader)
            : base(Delimiter, CodePage, HasHeader)
        {
            Init();
        }

        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <param name="ThrowError"> エラーをThrowするか </param>
        public CsvReader(char Delimiter, Encoding CodePage, bool HasHeader,
                         bool ThrowError)
            : base(Delimiter, CodePage, HasHeader, ThrowError)
        {
            Init();
        }

        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <param name="ThrowError"> エラーをThrowするか </param>
        /// <param name="IgnoreBlankLine"> 空行を無視するか </param>
        public CsvReader(char Delimiter, Encoding CodePage, bool HasHeader,
                         bool ThrowError, bool IgnoreBlankLine)
            : base(Delimiter, CodePage, HasHeader, ThrowError, IgnoreBlankLine)
        {
            Init();
        }

        /// <summary>
        /// 区切り文字によって区切られている形式化されたテキストファイルの読み込み機能を提供します
        /// </summary>
        /// <param name="Delimiter"> 区切り文字 </param>
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <param name="ThrowError"> エラーをThrowするか </param>
        /// <param name="IgnoreBlankLine"> 空行を無視するか </param>
        /// <param name="AllowComment"> コメント行を許可するか </param>
        public CsvReader(char Delimiter, Encoding CodePage, bool HasHeader,
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
        /// <param name="CodePage"> 文字コード </param>
        /// <param name="HasHeader"> ヘッダーが存在するか </param>
        /// <param name="ThrowError"> エラーをThrowするか </param>
        /// <param name="IgnoreBlankLine"> 空行を無視するか </param>
        /// <param name="AllowComment"> コメント行を許可するか </param>
        /// <param name="TrimWhiteSpace"> フィールド前後の空白をトリムするか（RFC4180非推奨） </param>
        public CsvReader(char Delimiter, Encoding CodePage, bool HasHeader,
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
        private void Init()
        {
            Headers = new List<string>();
            Contents = new List<IReadOnlyDictionary<string, string>>();
        }
        #endregion

        /// <summary>
        /// CSVデータを読み込みます
        /// </summary>
        /// <param name="sequence"> 文字列のリスト（Iterator） </param>
        /// <returns> エラー発生回数 </returns>
        public int Read(IEnumerable<string> sequence)
        {
            int response = 0;
            bool first_flag = true;

            try
            {
                foreach (string[] seq in ParseCsv(sequence))
                {
                    // 受け取ったデータが存在しなければ不正なデータとして処理
                    if (seq.Length == 0)
                    {
                        if (ThrowError)
                        {
                            throw new TextParseException("不正なデータです。");
                        }
                        else
                        {
                            response += 1;
                        }

                        continue;
                    }

                    var data = new Dictionary<string, string>(Headers.Count);

                    if (first_flag)
                    {
                        // 初回読み込み時にヘッダーの定義を行う
                        if (HasHeader)
                        {
                            // ヘッダーの存在フラグがあれば、データをそのままヘッダーとして扱う
                            Headers = seq.ToList();
                        }
                        else
                        {
                            // ヘッダーの存在フラグがなければ、ヘッダー情報を作成し、
                            // 1行目のデータは通常のデータとして扱う
                            for (int i = 0; i < seq.Length; i++)
                            {
                                Headers.Add("Data" + (i + 1).ToString());
                                data.Add(Headers[i], seq[i]);
                            }

                            Contents.Add(new ReadOnlyDictionary<string, string>(data));
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

                            data.Add(Headers[i], insert_str);
                        }

                        Contents.Add(new ReadOnlyDictionary<string, string>(data));
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
        public int Read(FileInfo file)
        {
            if (file.Exists)
            {
                return Read(CreateSequence(file));
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// CSVデータを読み込みます
        /// </summary>
        /// <param name="data"> CSV文字列 </param>
        /// <returns> エラー発生回数 </returns>
        public int Read(string data)
        {
            return Read(CreateSequence(data));
        }

        /// <summary>
        /// 読み込まれたCSVデータの内容を取得します
        /// </summary>
        /// <returns> Contentsオブジェクト </returns>
        public CsvContents GetContents()
        {
            return new CsvContents(Headers, Contents);
        }

        //public List<T> GetContents<T>() where T : new()
        //{
        //    var res = new List<T>();
        //    var maps = new List<MappingObject>();

        //    try
        //    {
        //        if (Headers.Count == 0)
        //        {
        //            return new List<T>(0);
        //        }

        //        PropertyInfo[] properties = typeof(T).GetProperties();

        //        if (properties.Length != Headers.Count)
        //        {
        //            throw new NotMatchNumberOfFielsException
        //                ("マッピングしようとしているPropertyの数とヘッダーの数が一致しません。");
        //        }

        //        foreach (PropertyInfo prop in properties)
        //        {
        //            var map = new MappingObject();

        //            // Propertyに紐付いた属性値を取得
        //            HeaderAttribute header_atrb =
        //                prop.GetCustomAttribute<HeaderAttribute>();

        //            if (prop.PropertyType.Name.Contains("Nullable"))
        //            {
        //                // Nullableの型の時
        //                // GetUnderlyingTypeから型を取得する
        //                map.ObjectType = Nullable.GetUnderlyingType(prop.PropertyType);

        //                // Nullableフラグを立てる
        //                map.IsNullable = true;
        //            }
        //            else
        //            {
        //                // Nullableの型でないときは普通にPropertyTypeから取得
        //                map.ObjectType = prop.PropertyType;
        //            }

        //            // 属性値が設定されていて、属性値に対応するヘッダーが存在している場合
        //            // ヘッダーに対応するプロパティ名を取得
        //            if (header_atrb != null && Headers.Contains(header_atrb.Name))
        //            {
        //                map.Name = prop.Name;
        //                map.Number = Headers.IndexOf(header_atrb.Name);
        //            }
        //            else if (Headers.Contains(prop.Name))
        //            {
        //                map.Name = prop.Name;
        //                map.Number = Headers.IndexOf(prop.Name);
        //            }
        //            else
        //            {
        //                throw new ArgumentException
        //                    ("ヘッダー名称がマッピングしようとしているオブジェクト内に" +
        //                     "存在しません。");
        //            }

        //            maps.Add(map);
        //        }

        //        foreach (var c in Contents)
        //        {
        //            var Tobj = Activator.CreateInstance<T>();

        //            foreach (var m in maps)
        //            {
        //                string str = c[Headers[m.Number]];
        //                TypeCode code = Type.GetTypeCode(m.ObjectType);
        //                object value = str.

        //                typeof(T).GetRuntimeProperty(m.Name).SetValue(Tobj, );
        //            }
        //        }

        //    }
        //    catch
        //    {

        //    }
        //}

        /// <summary>
        /// 文字列を一行ずつ読み込み、Iteratorとして返します
        /// </summary>
        /// <param name="input"> 文字列 </param>
        /// <returns></returns>
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
    }
}
