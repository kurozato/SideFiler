using BlackSugar.Model;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace BlackSugar.WinApi
{
    public class NetShare
    {
        internal static class NativeMethods
        {
            const int NERR_Success = 0;
            const uint MAX_PREFERRED_LENGTH = 0xFFFFFFFF;

            enum DataInformationLevel
            {
                /// <summary>
                /// 共有の名前を取得します。関数から制御が返ると、bufptr パラメータが指すバッファに、複数の 構造体からなる 1 つの配列が格納されます。
                /// </summary>
                Level0 = 0,
                /// <summary>
                /// リソースの名前、タイプ、リソースに関連付けられているコメントなど、共有リソースに関する情報を取得します。
                /// 関数から制御が返ると、bufptr パラメータが指すバッファに、複数の 構造体からなる 1 つの配列が格納されます。
                /// </summary>
                Level1 = 1,
                /// <summary>
                /// リソースの名前、タイプ、アクセス許可、パスワード、接続の数など、共有リソースに関する情報を取得します。関数から制御が返ると、bufptr パラメータが指すバッファに、複数の 構造体からなる 1 つの配列が格納されます。
                /// </summary>
                Level2 = 2,
                /// <summary>
                /// リソースの名前、タイプ、アクセス許可、接続の数、他の固有情報など、共有リソースに関する情報を取得します。関数から制御が返ると、bufptr パラメータが指すバッファに、複数の 構造体からなる 1 つの配列が格納されます。
                /// </summary>
                Level502 = 502,
            }

            internal enum SHARE_TYPE : uint
            {
                STYPE_DISKTREE = 0,
                STYPE_PRINTQ = 1,
                STYPE_DEVICE = 2,
                STYPE_IPC = 3,
                STYPE_CLUSTER_FS = 0x02000000,
                STYPE_CLUSTER_SOFS = 0x04000000,
                STYPE_CLUSTER_DFS = 0x08000000,
                STYPE_TEMPORARY = 0x40000000,
                STYPE_SPECIAL = 0x80000000,
            }

            /// <summary>
            /// 特定のサーバー上の各共有資源に関する情報を取得します。Windows 95/98 では使用できません。
            /// </summary>
            /// <param name="ServerName">この関数を実行するリモートサーバーの名前を表す、Unicode 文字列へのポインタを指定します。この文字列の先頭は "\\" でなければなりません。このパラメータで NULL を指定すると、ローカルコンピュータが使われます。</param>
            /// <param name="level">データの情報レベルを指定します。次の値のいずれかを指定します。</param>
            /// <param name="bufPtr">1 個のバッファへのポインタを指定します。関数から制御が返ると、このバッファに、指定したデータが格納されます。このデータの形式は、level パラメータの値によって異なります。このバッファはシステムによって割り当てられたものであり、NetApiBufferFree 関数を使って解放しなければなりません。この関数が失敗して ERROR_MORE_DATA が返った場合でも、このバッファを解放しなければならないことに注意してください。</param>
            /// <param name="prefmaxlen">取得するべきデータの最大の長さ（上限）をバイト単位で指定します。このパラメータが MAX_PREFERRED_LENGTH の場合、この関数はデータが必要とする量のメモリを割り当てます。このパラメータで他の値を指定すると、その値は、この関数が返すバイト数に制限を加えることがあります。バッファサイズが不足して一部のエントリを格納できない場合は、ERROR_MORE_DATA が返ります。詳細については、MSDN ライブラリの「Network Management Function Buffers」と「Network Management Function Buffer Lengths」を参照してください。</param>
            /// <param name="entriesread">1 つの DWORD 値へのポインタを指定します。関数から制御が返ると、この値に、実際に列挙された要素の数が格納されます。</param>
            /// <param name="totalentries">1 つの DWORD 値へのポインタを指定します。関数から制御が返ると、この値に、現在のレジューム位置以降で列挙できるはずのエントリの総数が格納されます。</param>
            /// <param name="resume_handle">引き続き既存の共有を検索するために使われるレジュームハンドルを保持している、1 つの DWORD 値へのポインタを指定します。このハンドルは最初の呼び出しのときに 0 であるべきで、それ以降の呼び出しでも変更しないでください。resume_handle パラメータで NULL を指定すると、レジュームハンドルは格納されません。</param>
            /// <returns>関数が成功すると、NERR_Success が返ります。関数が失敗すると、Win32 API のエラーコードが返ります。エラーコードのリストについては、MSDN ライブラリの「System Error Codes」を参照してください。</returns>
            /// <remarks>特定の共有が、DFS ルート内の DFS リンクであるかどうかを示す値を取得するには、情報レベル 1005 を指定して NetShareGetInfo 関数を呼び出してください。</remarks>
            [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
            static extern int NetShareEnum(
              StringBuilder serverName,
              DataInformationLevel level,
              ref IntPtr bufPtr,
              uint prefmaxlen,
              out int entriesread,
              out int totalentries,
              ref int resume_handle
            );

            /// <summary>
            /// NetApiBufferAllocate が割り当てたメモリを解放します。Windows NT/2000 上で他のネットワーク管理関数が返したメモリを解放するには、この関数を使ってください。
            /// </summary>
            /// <param name="Buffer">既に他のネットワーク管理関数が返したバッファへのポインタを指定します。</param>
            /// <returns>関数が成功すると、NERR_Success が返ります。</returns>
            [DllImport("Netapi32.dll", SetLastError = true)]
            static extern int NetApiBufferFree(IntPtr Buffer);

            /// <summary>
            /// The SHARE_INFO_1 structure contains information about the shared resource, including the name and type of the resource, and a comment associated with the resource.
            /// </summary>
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct SHARE_INFO_1
            {
                /// <summary>
                /// Pointer to a Unicode string specifying the share name of a resource. Calls to the NetShareSetInfo function ignore this member.
                /// </summary>
                public string shi1_netname;
                /// <summary>
                /// A bitmask of flags that specify the type of the shared resource. Calls to the NetShareSetInfo function ignore this member.
                /// </summary>
                public uint shi1_type;
                /// <summary>
                /// Pointer to a Unicode string specifying an optional comment about the shared resource.
                /// </summary>
                public string shi1_remark;

                public SHARE_INFO_1(string sharename, uint sharetype, string remark)
                {
                    shi1_netname = sharename;
                    shi1_type = sharetype;
                    shi1_remark = remark;
                }

                public override string ToString()
                {
                    return shi1_netname;
                }
            }

            /// <summary>
            /// サーバ上の共有ファイル一覧を取得。
            /// </summary>
            /// <param name="Server">対象サーバ。</param>
            /// <returns></returns>
            public static List<SHARE_INFO_1> GetNetShares(string server)
            {
                int entriesread;
                IntPtr bufPtr = GetBufPtr(server, out entriesread);
                try
                {
                    return GetShareInfos(bufPtr, entriesread);
                }
                finally
                {
                    NetApiBufferFree(bufPtr);
                }
            }

            private static List<SHARE_INFO_1> GetShareInfos(IntPtr bufPtr, int entriesread)
            {
                IntPtr currentPtr = bufPtr;
                var shareInfos = new List<SHARE_INFO_1>();
                int nStructSize = Marshal.SizeOf(typeof(SHARE_INFO_1));

                for (int i = 0; i < entriesread; i++)
                {
                    //SHARE_INFO_1 shi1 = (SHARE_INFO_1)Marshal.PtrToStructure(currentPtr, typeof(SHARE_INFO_1));
                    SHARE_INFO_1 shi1 = Marshal.PtrToStructure<SHARE_INFO_1>(currentPtr);

                    if (shi1.shi1_type == (uint)SHARE_TYPE.STYPE_DISKTREE)
                        shareInfos.Add(shi1);

                    currentPtr = new IntPtr(currentPtr.ToInt64() + nStructSize);
                }
                return shareInfos;
            }

            private static IntPtr GetBufPtr(string server, out int entriesread)
            {
                int totalentries;
                int resume_handle = 0;
                IntPtr bufPtr = IntPtr.Zero;

                int result = NetShareEnum(
                  new StringBuilder(server), DataInformationLevel.Level1, ref bufPtr
                  , MAX_PREFERRED_LENGTH, out entriesread, out totalentries, ref resume_handle
                );

                if (result != NERR_Success)
                {
                    throw new Win32Exception(result);
                }
                return bufPtr;
            }
        }

        private interface ISelector<T>
        {
            T Create(ref string fullName, NativeMethods.SHARE_INFO_1 shareInfo, string server);
        }

        private class FullNameSelector : ISelector<string>
        {
            public string Create(ref string fullName, NativeMethods.SHARE_INFO_1 shareInfo, string server)
                => fullName;
        }

        private class FileDataSelector : ISelector<FileData>
        {
            public FileData Create(ref string fullName, NativeMethods.SHARE_INFO_1 shareInfo, string server)
                => new FileData(ref fullName, shareInfo, server);
        }

        private static IEnumerable<T> EnumerateCore<T>(string server, ISelector<T> selector)
        {
            var sharelist = NativeMethods.GetNetShares(server);
            foreach (NativeMethods.SHARE_INFO_1 shareInfo in sharelist)
            {
                var path = server + @"\" + shareInfo.shi1_netname;
                yield return selector.Create(ref path, shareInfo, server);
            }
        }

        public static IEnumerable<FileData> GetShareData(string path)
        {
            return EnumerateCore(path, new FileDataSelector());
        }

        public static IEnumerable<string> GetShare(string path)
        {
            return EnumerateCore(path, new FullNameSelector());
        }
    }
}
