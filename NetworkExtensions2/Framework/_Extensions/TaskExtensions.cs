using ColossalFramework.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetworkExtensions2.Framework._Extensions
{
    public static class TaskExtensions
    {
        public static T WaitResult<T>(this Task<T> task) 
        {
            if (!task.hasEnded)
                task.Wait();
            return task.result;
        }
    }
}
