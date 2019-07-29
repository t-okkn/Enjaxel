using System;

namespace Enjaxel.TextParser
{
    /// <summary>
    /// ヘッダーを紐付けるための属性クラス
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class HeaderAttribute : Attribute
    {
        /// <summary> ヘッダー名 </summary>
        public string Name { get; set; }

        /// <summary>
        /// ヘッダーを紐付けるための属性
        /// </summary>
        public HeaderAttribute(string Name)
        {
            this.Name = Name;
        }
    }
}
