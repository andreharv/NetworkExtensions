using ColossalFramework;
using ColossalFramework.Importers;
using ColossalFramework.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

namespace NetworkExtensions2.Framework.Import
{
    public class ImportAllTransitAssets
    {
        public static int FileCount { get; set; }

        protected static TaskDistributor sTaskDistributor = new TaskDistributor("SubTaskDistributor");
        public static void Import(string path)
        {
            if (Directory.Exists(path))
            {
                string[] filenames = Directory.GetFiles(path, "*.fbx");
                if (filenames.Length > 0)
                {
                    FileCount = filenames.Length;
                    var shader = Shader.Find("Custom/Net/Road");
                    //Task<bool>[] tasks = new Task<bool>[filenames.Length];
                    for(int i = 0; i < filenames.Length; i++)
                    {
                        ImportTransitAsset importTransitAsset = new ImportTransitAsset();
                        importTransitAsset.ImportAsset(shader, path, filenames[i]);
                    }
                    //tasks.WaitAll();
                    //for (int i = 0; i < tasks.Length; i++)
                    //{
                    //    if (!tasks[i].result)
                    //    {
                    //        Debug.Log($"***File {filenames[i]} timed out while loading asset!");
                    //    }
                    //}
                }
            }
            else
            {
                Debug.LogError("The specified path: " + path + " does not exist");
            }
        }
    }
}
