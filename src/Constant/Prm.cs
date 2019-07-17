using Enjaxel.Config;
using Enjaxel.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Enjaxel.Constant
{
    /// <summary>
    /// Globalで使用する定数パラメータ
    /// </summary>
    public class Prm
    {
        /// <summary> コンフィグファイルのPath </summary>
        public static string ConfigPath { get; set; }

        /// <summary> コンフィグのContents </summary>
        public static IConfig Config { get; set; }

        /// <summary> 動作しているアプリケーションのPath </summary>
        public static string AppPath { get; }

        /// <summary> 動作しているアプリケーションのディレクトリ </summary>
        public static string AppDir { get; }

        /// <summary> 動作しているアプリケーションの名前 </summary>
        public static string MyName { get; }

        /// <summary> 自端末が所持しているIPアドレス </summary>
        public static IPAddress[] MyIPAddress { get; }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static Prm()
        {
            var assembly = Assembly.GetExecutingAssembly();

            ConfigPath = string.Empty;
            AppPath = assembly.Location;
            AppDir = Directory.GetParent(assembly.Location).FullName;
            MyName = assembly.GetName().Name;
            MyIPAddress = NetworkTools.GetIpv4Address();
        }


    }
}
