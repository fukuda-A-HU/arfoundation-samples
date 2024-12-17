using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class SwatchesData
{
    [Serializable]
    public class SwatchData
    {
        public int SwatchNumber;
        public Color SwatchColor;
    }

    public List<SwatchData> Swatches;

    public SwatchesData()
    {
        Swatches = new List<SwatchData>();
    }
}