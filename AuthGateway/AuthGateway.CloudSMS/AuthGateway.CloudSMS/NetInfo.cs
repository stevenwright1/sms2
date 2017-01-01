
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace SMSService
{
    public sealed class NetworkInformation
    {

        public enum JoinStatus
        {
            Unknown = 0,
            UnJoined = 1,
            Workgroup = 2,
            Domain = 3
        }

        [DllImport("netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int NetGetJoinInformation(string computerName, ref IntPtr buffer, ref JoinStatus status);

        [DllImport("netapi32.dll", SetLastError = true)]
        public static extern int NetApiBufferFree(IntPtr buffer);

        private static NetworkInformation _local = new NetworkInformation();
        private string _computerName;
        private string _domainName;

        private JoinStatus _status = JoinStatus.Unknown;
        public NetworkInformation(string computerName)
        {
            if (computerName == null || 0 == computerName.Length)
            {
                throw new ArgumentNullException("computerName");
            }

            _computerName = computerName;
            LoadInformation();
        }

        private NetworkInformation()
        {
            LoadInformation();
        }

        public static NetworkInformation LocalComputer
        {
            get { return _local; }
        }

        public string ComputerName
        {
            get
            {
                if (_computerName == null)
                    return "(local)";
                return _computerName;
            }
        }

        public string DomainName
        {
            get { return _domainName; }
        }

        public JoinStatus Status
        {
            get { return _status; }
        }

        private void LoadInformation()
        {
            IntPtr pBuffer = IntPtr.Zero;
            JoinStatus status = default(JoinStatus);

            try
            {
                int result = NetGetJoinInformation(_computerName, ref pBuffer, ref status);
                if (0 != result)
                    throw new Win32Exception();

                _status = status;
                _domainName = Marshal.PtrToStringUni(pBuffer);

            }
            finally
            {
                if (!IntPtr.Zero.Equals(pBuffer))
                {
                    NetApiBufferFree(pBuffer);
                }
            }
        }

        public override string ToString()
        {
            switch (_status)
            {
                case JoinStatus.Domain:
                    return ComputerName + " is a member of the domain " + DomainName;
                case JoinStatus.Workgroup:
                    return ComputerName + " is a member of the workgroup " + DomainName;
                case JoinStatus.UnJoined:
                    return ComputerName + " is a standalone computer";
                default:
                    return "Unable to determine the network status of " + ComputerName;
            }
        }
    }
}
