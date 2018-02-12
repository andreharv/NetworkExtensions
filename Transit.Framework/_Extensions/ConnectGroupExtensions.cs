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
            None = 0,
            OnePlusOne = 1,
            OnePlusTwoS = 2,
            OnePlusThreeS = 4,
            TwoPlusTwo = 8,
            TwoPlusThree = 16,
            TwoPlusFour = 32,
            ThreePlusThree = 64,
            ThreePlusFour = 128,
            ThreePlusFive = 256,
            FourPlusFour = 512,
            OneMidL = 1024,
            TwoMidL = 2048,
            ThreeMidL = 16384
        }
        public static string GetConnextGroup(this NetInfo.ConnectGroup cGroup)
        {
            return ((ConnextGroup)(int)cGroup).ToString();
        }
        public static NetInfo.ConnectGroup GetConnectGroup(this ConnextGroup cGroup)
        {
            return (NetInfo.ConnectGroup)(int)cGroup;
        }
    }
}