using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HybridGOAP
{
    [System.Flags]
    public enum ECharacterResources 
    {
        None  = 0x00,
        Legs  = 0x01,
        Torso = 0x02,
        Head  = 0x04,

        All   = 0xFF,
    }

    public static class ResourcesHelper
    {
        public static int GetNumResourcesUsed(ECharacterResources InResourcesFlags)
        {
            int BitCount = 0;
            UInt32 Value = (UInt32) InResourcesFlags;
            while (Value != 0)
            {
                ++BitCount;
                Value &= Value - 1;
            }
            return BitCount;
        }
    }

    public static class GoalPriority
    {
        public const int Maximum    = 100;

        public const int Critical   = 90;
        public const int High       = 75;
        public const int Medium     = 50;
        public const int Low        = 25;
        public const int Ambient    = 10;

        public const int Minimum    = 0;

        public const int DoNotRun   = int.MinValue;
    }

    public static class Names
    {

    }
}
