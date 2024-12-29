using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonCore
{
    public sealed class FastName : IEquatable<FastName>, IComparable<FastName>
    {
        // prevent others from initiating without providing additional information
        public static FastName None = new();

        UInt32 NameID;

        // invalid ID: 0
        private FastName()
        {
            NameID = 0;
        }   

        public FastName(string InName)
        {
            NameID = NameManager.CreateOrRetrieveID(InName);
        }

        public override string ToString()
        {
            if (this == None)
                return "## None ##";

            return NameManager.RetrieveNameFromID(NameID);
        }

        public override bool Equals(object InOther)
        {
            return Equals(InOther as FastName);
        }

        public bool Equals(FastName InOther)
        {
            return InOther is not null &&
                   NameID == InOther.NameID;
        }

        public override int GetHashCode()
        {
            return NameID.GetHashCode();
        }

        // compare based on name ID
        public int CompareTo(FastName InOther)
        {
            return NameID.CompareTo(InOther.NameID);
        }

        public static bool operator ==(FastName InLHS, FastName InRHS)
        {
            return EqualityComparer<FastName>.Default.Equals(InLHS, InRHS);
        }

        public static bool operator !=(FastName InLHS, FastName InRHS)
        {
            return !(InLHS == InRHS);
        }

        public static implicit operator bool(FastName InOther)
        {
            return InOther != None;
        }
    }

    public class NameManager : Singleton<NameManager>
    {
        UInt32 NextNameID = 1; // start at 1
        Dictionary<UInt32, string> NameIDs = new(); 
        static object _NameIDsLock = new object();

        internal static uint CreateOrRetrieveID(string InName)
        {
            if (Instance == null)
                return 0;

            return Instance.CreateOrRetrieveIDInternal(InName);
        }

        internal static string RetrieveNameFromID(UInt32 InNameID)
        {
            if (Instance == null)
                return "## No NameManager ##";

            return Instance.RetrieveNameFromIDInternal(InNameID);
        }

        internal UInt32 CreateOrRetrieveIDInternal(string InName)
        {
            lock (_NameIDsLock)
            {
                // does this name already exists?
                UInt32 FoundNameID = 0;
                foreach (var KVP in NameIDs)
                {
                    if (KVP.Value == InName)
                    {
                        FoundNameID = KVP.Key;
                        break;
                    }
                }

                // name ID not found - create
                if (FoundNameID == 0)
                {
                    FoundNameID = NextNameID;
                    ++NextNameID;

                    NameIDs.Add(FoundNameID, InName);
                }

                return FoundNameID;
            }
        }

        internal string RetrieveNameFromIDInternal(UInt32 InNameID)
        {
            lock ( _NameIDsLock)
            {
                string FoundName = null;

                if (NameIDs.TryGetValue(InNameID, out FoundName))
                    return FoundName;

                return "## Missing ID ##";
            }
        }
    }
}
