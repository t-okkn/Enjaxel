using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Enjaxel.Conversion
{
    /// <summary>
    /// XmlSerializerのWrapperクラス
    /// </summary>
    public static class XmlSerializeService
    {
        /// <summary>
        /// XmlのStream → T型Entityに変換します
        /// </summary>
        /// <typeparam name="T"> 変換する型 </typeparam>
        /// <param name="stream"> 変換するStream </param>
        /// <returns> T型Entity </returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        public static T XmlDeserialize<T>(Stream stream)
            where T : new()
        {
            var result = default(T);

            try
            {
                // T型インスタンスを作成
                result = Activator.CreateInstance<T>();

                // デシリアライズの準備
                var serializer = new XmlSerializer(typeof(T));
                var xr_settings = new XmlReaderSettings()
                {
                    // XMLとして不正な文字をチェックしない
                    CheckCharacters = false
                };

                // Xml文字列をメモリ上に展開
                using (var xr = XmlReader.Create(stream, xr_settings))
                {
                    // メモリ上のXmlからT型インスタンスに値を詰め込む
                    result = (T)serializer.Deserialize(xr);
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// Xml文字列 → T型Entityに変換します
        /// </summary>
        /// <typeparam name="T"> 変換する型 </typeparam>
        /// <param name="xml"> Xml文字列 </param>
        /// <param name="encode"> 文字コード </param>
        /// <returns> T型Entity </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        /// <exception cref="EncoderFallbackException"></exception>
        public static T XmlDeserialize<T>(string xml, Encoding encode)
            where T : new()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(xml))
                {
                    throw new ArgumentException("文字列はnullか空文字列です。");
                }

                // Xml文字列をメモリ上に展開
                using (var stream = new MemoryStream(encode.GetBytes(xml)))
                {
                    return XmlDeserialize<T>(stream);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Xml文字列 → T型Entityに変換します（UTF-8固定）
        /// </summary>
        /// <typeparam name="T"> 変換する型 </typeparam>
        /// <param name="xml"> Xml文字列 </param>
        /// <returns> T型Entity </returns>
        /// <remarks> 文字コードはUTF-8 </remarks>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        /// <exception cref="EncoderFallbackException"></exception>
        public static T XmlDeserialize<T>(string xml)
            where T : new()
        {
            return XmlDeserialize<T>(xml, Encoding.UTF8);
        }

        /// <summary>
        /// Xmlテキストファイル → T型Entityに変換します
        /// </summary>
        /// <typeparam name="T"> 変換する型 </typeparam>
        /// <param name="fileInfo"> XmlテキストファイルのFileInfo </param>
        /// <param name="encode"> 文字コード </param>
        /// <returns> T型Entity </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        public static T XmlDeserialize<T>(FileInfo fileInfo, Encoding encode)
            where T : new()
        {
            try
            {
                if (!fileInfo.Exists)
                {
                    throw new FileNotFoundException("対象のファイルが存在しません。");
                }

                // Xml文字列をメモリ上に展開
                using (var sr = new StreamReader(fileInfo.FullName, encode))
                {
                    return XmlDeserialize<T>(sr.BaseStream);
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Xmlテキストファイル → T型Entityに変換します（UTF-8固定）
        /// </summary>
        /// <typeparam name="T"> 変換する型 </typeparam>
        /// <param name="fileInfo"> XmlテキストファイルのFileInfo </param>
        /// <returns> T型Entity </returns>
        /// <remarks> 文字コードはUTF-8 </remarks>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        public static T XmlDeserialize<T>(FileInfo fileInfo)
            where T : new()
        {
            return XmlDeserialize<T>(fileInfo, Encoding.UTF8);
        }

        /// <summary>
        /// T型Entity → Xml文字列に変換するメソッド
        /// </summary>
        /// <param name="target"> T型Entity </param>
        /// <param name="encode"> 文字コード </param>
        /// <returns> Xml文字列 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="DecoderFallbackException"></exception>
        public static string XmlSerialize(object target, Encoding encode)
        {
            string result = string.Empty;

            try
            {
                using (var stream = new MemoryStream())
                {
                    // シリアライズの準備
                    var serializer = new XmlSerializer(target.GetType());

                    // T型オブジェクトをメモリストリームへ展開
                    serializer.Serialize(stream, target);

                    // 結果を取得
                    result = encode.GetString(stream.ToArray());
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// T型Entity → Xml文字列に変換するメソッド（UTF-8固定）
        /// </summary>
        /// <param name="target"> T型Entity </param>
        /// <returns> Xml文字列 </returns>
        /// <remarks> エラーの場合は空文字 </remarks>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="DecoderFallbackException"></exception>
        public static string XmlSerialize(object target)
        {
            return XmlSerialize(target, Encoding.UTF8);
        }
    }
}
