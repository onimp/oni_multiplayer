// Decompiled with JetBrains decompiler
// Type: Database.OrbitalTypeCategories
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Database
{
  public class OrbitalTypeCategories : ResourceSet<OrbitalData>
  {
    public OrbitalData backgroundEarth;
    public OrbitalData frozenOre;
    public OrbitalData heliumCloud;
    public OrbitalData iceCloud;
    public OrbitalData iceRock;
    public OrbitalData purpleGas;
    public OrbitalData radioactiveGas;
    public OrbitalData rocky;
    public OrbitalData gravitas;
    public OrbitalData orbit;
    public OrbitalData landed;

    public OrbitalTypeCategories(ResourceSet parent)
      : base(nameof (OrbitalTypeCategories), parent)
    {
      this.backgroundEarth = new OrbitalData(nameof (backgroundEarth), (ResourceSet) this, orbitalType: OrbitalData.OrbitalType.world, yGridPercent: 0.95f, minAngle: 10f, maxAngle: 10f);
      this.frozenOre = new OrbitalData(nameof (frozenOre), (ResourceSet) this, "starmap_frozen_ore_kanim", radiusScale: 1f);
      this.heliumCloud = new OrbitalData(nameof (heliumCloud), (ResourceSet) this, "starmap_helium_cloud_kanim");
      this.iceCloud = new OrbitalData(nameof (iceCloud), (ResourceSet) this, "starmap_ice_cloud_kanim");
      this.iceRock = new OrbitalData(nameof (iceRock), (ResourceSet) this, "starmap_ice_kanim");
      this.purpleGas = new OrbitalData(nameof (purpleGas), (ResourceSet) this, "starmap_purple_gas_kanim");
      this.radioactiveGas = new OrbitalData(nameof (radioactiveGas), (ResourceSet) this, "starmap_radioactive_gas_kanim");
      this.rocky = new OrbitalData(nameof (rocky), (ResourceSet) this, "starmap_rocky_kanim");
      this.gravitas = new OrbitalData(nameof (gravitas), (ResourceSet) this, "starmap_space_junk_kanim");
      this.orbit = new OrbitalData(nameof (orbit), (ResourceSet) this, "starmap_orbit_kanim", orbitalType: OrbitalData.OrbitalType.inOrbit, xGridPercent: 0.25f, rotatesBehind: false, distance: 4f);
      this.landed = new OrbitalData(nameof (landed), (ResourceSet) this, "starmap_landed_surface_kanim", orbitalType: OrbitalData.OrbitalType.landed, periodInCycles: 0.0f, yGridPercent: 0.35f, rotatesBehind: false, distance: 4f);
    }
  }
}
