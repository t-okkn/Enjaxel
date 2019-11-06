using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Enjaxel.TextParser.Config
{
    /// <summary>
    /// CSVの内容をListで保持するクラス
    /// </summary>
    public class CsvContents : IConfig
    {
        /// <summary> コンフィグの種別 </summary>
        public ConfigType Type { get; }

        /// <summary> ヘッダーの一覧 </summary>
        public IReadOnlyList<string> Headers { get; }

        /// <summary> コンテンツ </summary>
        public IReadOnlyList<IReadOnlyList<string>> Contents { get; }

        /// <summary>
        /// CSVの内容をListで保持します
        /// </summary>
        public CsvContents()
        {
            Type = ConfigType.CSV;
        }

        /// <summary>
        /// CSVの内容をListで保持します
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        internal CsvContents(IEnumerable<string> Headers,
                             IEnumerable<IReadOnlyList<string>> Contents)
        {
            // コピーを作成する
            var h = new List<string>(Headers);
            var c = new List<IReadOnlyList<string>>(Contents);

            Type = ConfigType.CSV;
            this.Headers = new ReadOnlyCollection<string>(h);
            this.Contents = new ReadOnlyCollection<IReadOnlyList<string>>(c);
        }

        /// <summary>
        /// コンフィグを読み込みます
        /// </summary>
        /// <param name="configPath"> コンフィグファイルのパス </param>
        /// <returns></returns>
        public IConfig ReadConfig(string configPath)
        {
            var csv = new CsvReader();
            var file = new FileInfo(configPath);

            try
            {
                int count = csv.Read(file);

                return count == 0 ? csv.GetContents() : null;
            }
            finally
            {
                csv = null;
                file = null;
            }
        }

        /// <summary>
        /// CsvContentsの情報を文字列として表します
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"ConfigType：{Type.ToString()}");
            sb.AppendLine($"Header：[{string.Join("], [", Headers)}]");

            foreach (var ctn in Contents)
            {
                string line = string.Empty;

                for (int i = 0; i < Headers.Count; i++)
                {
                    line += $"[{Headers[i]}] => [{ctn[i]}] | ";
                }

                sb.AppendLine(line.Remove(line.Length - 3));
            }

            return sb.ToString();
        }

        /// <summary>
        /// CsvContentsの任意の行の情報を文字列として表します
        /// </summary>
        /// <param name="row"> 行番号 </param>
        /// <returns></returns>
        public string ToString(int row)
        {
            string line = string.Empty;

            if (Contents.Count > row)
            {
                for (int i = 0; i < Headers.Count; i++)
                {
                    line += $"[{Headers[i]}] => [{Contents[row][i]}] | ";
                }

                line = line.Remove(line.Length - 3);
            }

            return line;
        }
    }
}
