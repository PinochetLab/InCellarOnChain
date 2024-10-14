using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WallInfo {
	[SerializeField] private Material material;
    [SerializeField] private List<Cutout> cutouts;

    public WallInfo(List<Cutout> cutouts)
    {
        this.cutouts = cutouts;
    }
    
    public WallInfo(Material material, List<Cutout> cutouts) {
	    this.material = material;
	    this.cutouts = cutouts;
    }
    
    public Material Material => material;

    public List<Cutout> Cutouts => cutouts;
}