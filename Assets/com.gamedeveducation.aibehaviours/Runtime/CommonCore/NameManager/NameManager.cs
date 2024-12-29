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

        System.UInt32 NameID;

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
        internal static uint CreateOrRetrieveID(string inName)
        {
            throw new NotImplementedException();
        }

        internal static string RetrieveNameFromID(uint nameID)
        {
            throw new NotImplementedException();
        }
    }
}
