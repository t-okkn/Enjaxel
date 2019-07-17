using System;

namespace Enjaxel.Reflection
{
    /// <summary>
    /// ToStringメソッドを作成する際に使用する付加属性クラス
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                    AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class ToStringAttribute : Attribute
    {
        /// <summary> 項目名 </summary>
        public string Name { get; }

        /// <summary> 項目名の前に修飾する文字（デフォルト => ""） </summary>
        public string Prefix { get; set; }

        /// <summary> デリミタ（デフォルト => " : "） </summary>
        public string Delimiter { get; set; }

        /// <summary> 項目名の後に修飾する文字（デフォルト => " | "） </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        /// <param name="Name"> 項目名 </param>
        public ToStringAttribute(string Name)
        {
            this.Name = Name;
            this.Prefix = string.Empty;
            this.Delimiter = " : ";
            this.Suffix = " | ";
        }
    }
}
