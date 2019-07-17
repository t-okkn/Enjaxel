namespace Enjaxel.Constant
{
    interface ISQLConstant
    {
        /// <summary>
        /// SQLサーバのホスト接続先を示す文字列
        /// </summary>
        /// <remarks> IPアドレス or ホスト名 </remarks>
        string SQLHost { get; set; }

        /// <summary>
        /// SQLサーバのPort番号
        /// </summary>
        int SQLPort { get; set; }

        /// <summary>
        /// SQLサーバ接続ユーザ
        /// </summary>
        string SQLUser { get; set; }

        /// <summary>
        /// SQLサーバ接続ユーザのパスワード
        /// </summary>
        string SQLPassword { get; set; }

        /// <summary>
        /// SQLサーバ接続先データベース名
        /// </summary>
        string SQLDatabaseName { get; set; }

        /// <summary>
        /// SQLサーバ接続先ConnectionString
        /// </summary>
        string SQLConnectionString { get; set; }

        /// <summary>
        /// SQLサーバへの接続においてSSL接続有効化フラグ
        /// </summary>
        bool IsSslConnect { get; set; }

        /// <summary>
        /// SQLサーバへの接続文字列・パスワードの暗号化フラグ
        /// </summary>
        bool IsEncrypted { get; set; }
    }
}
