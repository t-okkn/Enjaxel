using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Enjaxel.Communication
{
    /// <summary>
    /// 通信に使用するネットワーク関連メソッドをまとめたクラス
    /// </summary>
    public class NetworkTools
    {
        /// <summary>
        /// IPv4アドレス（ループバック・APIPA以外）を取得します
        /// </summary>
        /// <returns> IPv4アドレス（ループバック・APIPA以外） </returns>
        public static IPAddress[] GetIpv4Address(IPAddress[] ipAddrs)
        {
            // IPv4アドレスを入れる配列
            var ipv4Arr = new IPAddress[0];

            // 入力されたIPアドレスからIPv4アドレス（ループバック・APIPA以外）のみを抜き出す
            if (ipAddrs.Length > 0)
            {
                foreach (var ipAddr in ipAddrs)
                {
                    if (ipAddr.AddressFamily == AddressFamily.InterNetwork &&
                        (!IPAddress.IsLoopback(ipAddr)) && (!IsAPIPA(ipAddr)))
                    {
                        Array.Resize(ref ipv4Arr, ipv4Arr.Length + 1);
                        ipv4Arr[ipv4Arr.Length - 1] = ipAddr;
                    }
                }
            }

            return ipv4Arr;
        }

        /// <summary>
        /// 自端末のIPv4アドレス（ループバック・APIPA以外）を取得します
        /// </summary>
        /// <returns> 自端末のIPv4アドレス（ループバック・APIPA以外） </returns>
        public static IPAddress[] GetIpv4Address()
        {
            // 自端末のIPアドレスを取得
            string hostname = Dns.GetHostName();
            IPAddress[] addrList = Dns.GetHostAddresses(hostname);

            IPAddress[] ipv4Arr = GetIpv4Address(addrList);

            // 抜き出したIPv4アドレスが存在するかチェック
            if (ipv4Arr.Length < 1)
            {
                throw new NetworkException
                    ("IPアドレスが端末に設定されていないか、LANケーブルが端末に接続されていません。");
            }

            return ipv4Arr;
        }

        /// <summary>
        /// 対象のIPアドレスに対して、Pingを行います
        /// </summary>
        /// <returns> 疎通性 </returns>
        public static bool Ping(IPAddress ip, int timeout, int count)
        {
            return true;
        }

        /// <summary>
        /// 与えられたIPv4アドレスがAPIPA(Automatic Private IP Addressing)かどうか判別します
        /// </summary>
        /// <param name="ip"> IPアドレス </param>
        /// <returns></returns>
        private static bool IsAPIPA(IPAddress ip)
        {
            // IPv6は非対応
            if (ip.AddressFamily == AddressFamily.InterNetworkV6) return false;

            byte[] byte_ip = ip.GetAddressBytes().Reverse().ToArray();

            byte[] byte_sub =
                IPAddress.Parse("255.255.0.0").GetAddressBytes().Reverse().ToArray();
            byte[] byte_apipa =
                IPAddress.Parse("169.254.0.0").GetAddressBytes().Reverse().ToArray();

            uint int_ip = BitConverter.ToUInt32(byte_ip, 0);
            uint int_sub = BitConverter.ToUInt32(byte_sub, 0);
            uint int_apipa = BitConverter.ToUInt32(byte_apipa, 0);

            uint int_nw = int_ip & int_sub;

            return (int_apipa ^ int_nw) == 0 ? true : false;
        }
    }
}
