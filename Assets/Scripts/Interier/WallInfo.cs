using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WallInfo
{
    [SerializeField] private List<Cutout> cutouts;

    public WallInfo(List<Cutout> cutouts)
    {
        this.cutouts = cutouts;
    }

    public List<Cutout> Cutouts => cutouts;
}