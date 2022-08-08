﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetworkExtensions2.Framework.Import
{
    public class ResourceUnit
    {
        public Mesh Mesh { get; set; }
        public Mesh LodMesh { get; set; }
        public Material Material { get; set; }
        public Material LodMaterial { get; set; }

        public ResourceUnit(Mesh argMesh, Material argMaterial, Mesh argLodMesh, Material argLodMaterial)
        {
            Mesh = argMesh;
            Material = argMaterial;
            LodMesh = argLodMesh;
            LodMaterial = argLodMaterial;
        }
    }
}
