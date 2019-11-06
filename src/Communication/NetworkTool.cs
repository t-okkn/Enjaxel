using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace Enjaxel.Communication
{
    /// <summary>
    /// 通信に使用するネットワーク関連メソッドをまとめたクラス
    /// </summary>
    public static class NetworkTools
    {
        #region GetIPAddress
        /// <summary>
        /// IPv4アドレス（ループバック・APIPA以外）を取得します
        /// </summary>
        /// <returns> IPv4アドレス（ループバック・APIPA以外） </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
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
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="NetworkException"></exception>
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
        /// IPv6アドレス（ループバック以外）を取得します
        /// </summary>
        /// <returns> IPv6アドレス（ループバック以外） </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static IPAddress[] GetIpv6Address(IPAddress[] ipAddrs)
        {
            // IPv6アドレスを入れる配列
            var ipv6Arr = new IPAddress[0];

            // 入力されたIPアドレスからIPv6アドレス（ループバック以外）を抜き出す
            if (ipAddrs.Length > 0)
            {
                foreach (var ipAddr in ipAddrs)
                {
                    if (ipAddr.AddressFamily == AddressFamily.InterNetworkV6 &&
                        (!IPAddress.IsLoopback(ipAddr)))
                    {
                        Array.Resize(ref ipv6Arr, ipv6Arr.Length + 1);
                        ipv6Arr[ipv6Arr.Length - 1] = ipAddr;
                    }
                }
            }

            return ipv6Arr;
        }

        /// <summary>
        /// 自端末のIPv6アドレス（ループバック以外）を取得します
        /// </summary>
        /// <returns> 自端末のIPv6アドレス（ループバック以外） </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="SocketException"></exception>
        /// <exception cref="NetworkException"></exception>
        public static IPAddress[] GetIpv6Address()
        {
            // 自端末のIPアドレスを取得
            string hostname = Dns.GetHostName();
            IPAddress[] addrList = Dns.GetHostAddresses(hostname);

            IPAddress[] ipv6Arr = GetIpv6Address(addrList);

            // 抜き出したIPv6アドレスが存在するかチェック
            if (ipv6Arr.Length < 1)
            {
                throw new NetworkException
                    ("IPアドレスが端末に設定されていないか、LANケーブルが端末に接続されていません。");
            }

            return ipv6Arr;
        }
        #endregion

        #region PingWithResult
        /// <summary>
        /// 対象のIPアドレスに対して、Pingを行い結果を取得します
        /// </summary>
        /// <param name="ip"> IPアドレス </param>
        /// <param name="timeout"> タイムアウト値（ミリ秒） </param>
        /// <param name="count"> 試行回数 </param>
        /// <returns> 応答結果 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="PingException"></exception>
        public static IPStatus[] PingWithResult(IPAddress ip, int timeout, int count)
        {
            if (ip == null)
            {
                throw new ArgumentNullException("ip", "IPアドレスの値がNullです。");
            }

            if (timeout < 1)
            {
                throw new ArgumentException
                    ("タイムアウト値には1以上の値を指定してください。", "timeout");
            }

            if (count < 1)
            {
                throw new ArgumentException
                    ("試行回数には1以上の値を指定してください。", "count");
            }

            var res = new IPStatus[count];

            using (var p = new Ping())
            {
                for (int i = 0; i < count; i++)
                {
                    PingReply reply = p.Send(ip, timeout);
                    res[i] = reply.Status;
                }
            }

            return res;
        }

        /// <summary>
        /// 対象のホストに対して、Pingを行い結果を取得します
        /// </summary>
        /// <param name="hostName"> ホスト名 </param>
        /// <param name="timeout"> タイムアウト値（ミリ秒） </param>
        /// <param name="count"> 試行回数 </param>
        /// <returns> 応答結果 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="PingException"></exception>
        public static IPStatus[] PingWithResult(string hostName, int timeout, int count)
        {
            if (string.IsNullOrEmpty(hostName))
            {
                throw new ArgumentException("ホスト名が無効です。", "hostName");
            }

            if (timeout < 1)
            {
                throw new ArgumentException
                    ("タイムアウト値には1以上の値を指定してください。", "timeout");
            }

            if (count < 1)
            {
                throw new ArgumentException
                    ("試行回数には1以上の値を指定してください。", "count");
            }

            var res = new IPStatus[count];

            using (var p = new Ping())
            {
                for (int i = 0; i < count; i++)
                {
                    PingReply reply = p.Send(hostName, timeout);
                    res[i] = reply.Status;
                }
            }

            return res;
        }

        /// <summary>
        /// 対象のIPアドレスに対して、Pingを1度だけ行い結果を取得します
        /// </summary>
        /// <param name="ip"> IPアドレス </param>
        /// <param name="timeout"> タイムアウト値（ミリ秒） </param>
        /// <returns> 応答結果 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="PingException"></exception>
        public static IPStatus[] PingWithResult(IPAddress ip, int timeout)
        {
            return PingWithResult(ip, timeout, 1);
        }

        /// <summary>
        /// 対象のホストに対して、Pingを1度だけ行い結果を取得します
        /// </summary>
        /// <param name="hostName"> ホスト名 </param>
        /// <param name="timeout"> タイムアウト値（ミリ秒） </param>
        /// <returns> 応答結果 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="PingException"></exception>
        public static IPStatus[] PingWithResult(string hostName, int timeout)
        {
            return PingWithResult(hostName, timeout, 1);
        }

        /// <summary>
        /// 対象のIPアドレスに対して、Pingを1度だけ行い結果を取得します（タイムアウト値：500ミリ秒）
        /// </summary>
        /// <param name="ip"> IPアドレス </param>
        /// <returns> 応答結果 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="PingException"></exception>
        public static IPStatus[] PingWithResult(IPAddress ip)
        {
            return PingWithResult(ip, 500, 1);
        }

        /// <summary>
        /// 対象のホストに対して、Pingを1度だけ行い結果を取得します（タイムアウト値：500ミリ秒）
        /// </summary>
        /// <param name="hostName"> ホスト名 </param>
        /// <returns> 応答結果 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="PingException"></exception>
        public static IPStatus[] PingWithResult(string hostName)
        {
            return PingWithResult(hostName, 500, 1);
        }
        #endregion

        #region Ping
        /// <summary>
        /// 対象のIPアドレスに対して、Pingを行います
        /// </summary>
        /// <param name="ip"> IPアドレス </param>
        /// <param name="timeout"> タイムアウト値（ミリ秒） </param>
        /// <param name="count"> 試行回数 </param>
        /// <returns> 疎通性 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="PingException"></exception>
        public static bool Ping(IPAddress ip, int timeout, int count)
        {
            IPStatus[] res = PingWithResult(ip, timeout, count);
            int sum = res.Select(x => x == IPStatus.Success ? 1 : 0).Sum();

            return count == sum ? true : false;
        }

        /// <summary>
        /// 対象のホストに対して、Pingを行います
        /// </summary>
        /// <param name="hostName"> ホスト名 </param>
        /// <param name="timeout"> タイムアウト値（ミリ秒） </param>
        /// <param name="count"> 試行回数 </param>
        /// <returns> 疎通性 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="PingException"></exception>
        public static bool Ping(string hostName, int timeout, int count)
        {
            IPStatus[] res = PingWithResult(hostName, timeout, count);
            int sum = res.Select(x => x == IPStatus.Success ? 1 : 0).Sum();

            return count == sum ? true : false;
        }

        /// <summary>
        /// 対象のIPアドレスに対して、Pingを1度だけ行います
        /// </summary>
        /// <param name="ip"> IPアドレス </param>
        /// <param name="timeout"> タイムアウト値（ミリ秒） </param>
        /// <returns> 疎通性 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="PingException"></exception>
        public static bool Ping(IPAddress ip, int timeout)
        {
            return Ping(ip, timeout, 1);
        }

        /// <summary>
        /// 対象のホストに対して、Pingを1度だけ行います
        /// </summary>
        /// <param name="hostName"> ホスト名 </param>
        /// <param name="timeout"> タイムアウト値（ミリ秒） </param>
        /// <returns> 疎通性 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="PingException"></exception>
        public static bool Ping(string hostName, int timeout)
        {
            return Ping(hostName, timeout, 1);
        }

        /// <summary>
        /// 対象のIPアドレスに対して、Pingを1度だけ行います（タイムアウト値：500ミリ秒）
        /// </summary>
        /// <param name="ip"> IPアドレス </param>
        /// <returns> 疎通性 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="PingException"></exception>
        public static bool Ping(IPAddress ip)
        {
            return Ping(ip, 500, 1);
        }

        /// <summary>
        /// 対象のホストに対して、Pingを1度だけ行います（タイムアウト値：500ミリ秒）
        /// </summary>
        /// <param name="hostName"> ホスト名 </param>
        /// <returns> 疎通性 </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ObjectDisposedException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="PingException"></exception>
        public static bool Ping(string hostName)
        {
            return Ping(hostName, 500, 1);
        }
        #endregion

        #region Privateメソッド
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
        #endregion
    }
}
