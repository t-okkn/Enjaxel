using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Enjaxel.Constant;

namespace Enjaxel.TextParser.Config
{
    /// <summary>
    /// CSVの内容を抽象的に保持するdllのデフォルトクラス
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
        /// CSVの内容を抽象的に保持します
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public CsvContents(IList<string> Headers,
                           IList<IReadOnlyList<string>> Contents)
        {
            Type = ConfigType.CSV;
            this.Headers = new ReadOnlyCollection<string>(Headers);
            this.Contents =
                new ReadOnlyCollection<IReadOnlyList<string>>(Contents);
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

            int count = csv.Read(file);

            return count == 0 ? csv.GetContents() : null;
        }

        /// <summary>
        /// CsvContentsの情報を文字列として表します
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"種別：{Type.ToString()}");
            sb.AppendLine($"ヘッダー：[{string.Join("], [", Headers)}]");
            sb.AppendLine();

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
        /// CsvContentsの任意の列情報を文字列として表します
        /// </summary>
        /// <param name="row"> 列番号 </param>
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
