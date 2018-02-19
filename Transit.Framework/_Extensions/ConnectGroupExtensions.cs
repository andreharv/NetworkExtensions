using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Transit.Framework
{
    public static partial class NetInfoExtensions
    {
        private static long cFlag(int off)
        {
            var maxCgroup = Enum.GetNames(typeof(NetInfo.ConnectGroup)).Length;
            return (1L << (maxCgroup + off));
        }
        [Flags]
        public enum ConnextGroup
        {
            None = 0x0,
            OnePlusOne = 0x1,
            OnePlusTwoS = 0x2,
            OnePlusThreeS = 0x4,
            TwoPlusTwo = 0x8,
            TwoPlusThree = 0x10,
            TwoPlusFour = 0x20,
            ThreePlusThree = 0x40,
            ThreePlusFour = 0x80,
            ThreePlusFive = 0x100,
            FourPlusFour = 0x200,
            OneMidL = 0x400,
            TwoMidL = 0x800,
            ThreeMidL = 0x4000
        }
        public static string GetConnextGroup(this NetInfo.ConnectGroup cGroup)
        {
            return ((ConnextGroup)(int)cGroup).ToString();
        }
        public static NetInfo.ConnectGroup GetConnectGroup(this ConnextGroup cGroup)
        {
            int cg = (int)cGroup;
            if (cg >= 0x4000)
            {
                cg /= 0x4000;
            }
            return (NetInfo.ConnectGroup)cg;
        }
    }
}