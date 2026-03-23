import { describe, it, expect } from 'vitest';
import {
  isCritterBaby,
  getCritterSpeciesName,
  getCritterStyle,
  DEFAULT_CRITTER_COLOR,
} from './critterStyle';

// ── isCritterBaby ─────────────────────────────────────────────────────────────

describe('isCritterBaby', () => {
  it('returns true for "HatchBaby"', () => {
    expect(isCritterBaby('HatchBaby')).toBe(true);
  });

  it('returns true for "PuftBaby"', () => {
    expect(isCritterBaby('PuftBaby')).toBe(true);
  });

  it('returns true for "DreckoBaby"', () => {
    expect(isCritterBaby('DreckoBaby')).toBe(true);
  });

  it('returns true for "PuftBabyAlpine" (Baby mid-name)', () => {
    expect(isCritterBaby('PuftBabyAlpine')).toBe(true);
  });

  it('returns true for "Drecklet" (juvenile-only prefab)', () => {
    expect(isCritterBaby('Drecklet')).toBe(true);
  });

  it('returns true for "Puflet" (juvenile-only prefab)', () => {
    expect(isCritterBaby('Puflet')).toBe(true);
  });

  it('returns false for adult "HatchHard"', () => {
    expect(isCritterBaby('HatchHard')).toBe(false);
  });

  it('returns false for adult "Hatch"', () => {
    expect(isCritterBaby('Hatch')).toBe(false);
  });

  it('returns false for adult "PuftAlpine"', () => {
    expect(isCritterBaby('PuftAlpine')).toBe(false);
  });

  it('returns false for adult "Drecko"', () => {
    expect(isCritterBaby('Drecko')).toBe(false);
  });

  it('returns false for empty string', () => {
    expect(isCritterBaby('')).toBe(false);
  });
});

// ── getCritterSpeciesName ─────────────────────────────────────────────────────

describe('getCritterSpeciesName', () => {
  it('returns "Hatch" for "Hatch"', () => {
    expect(getCritterSpeciesName('Hatch')).toBe('Hatch');
  });

  it('returns "Hatch" for "HatchHard"', () => {
    expect(getCritterSpeciesName('HatchHard')).toBe('Sage Hatch');
  });

  it('returns "Puft" for "Puft"', () => {
    expect(getCritterSpeciesName('Puft')).toBe('Puft');
  });

  it('returns species for "PuftBaby"', () => {
    expect(getCritterSpeciesName('PuftBaby')).toBe('Puft');
  });

  it('returns "Pacu" for "Pacu"', () => {
    expect(getCritterSpeciesName('Pacu')).toBe('Pacu');
  });

  it('returns "Shine Bug" for "LightBug"', () => {
    expect(getCritterSpeciesName('LightBug')).toBe('Shine Bug');
  });

  it('returns "Drecklet" for "Drecklet" (juvenile, distinct from Drecko)', () => {
    expect(getCritterSpeciesName('Drecklet')).toBe('Drecklet');
  });

  it('returns "Drecko" for "Drecko"', () => {
    expect(getCritterSpeciesName('Drecko')).toBe('Drecko');
  });

  it('returns "Puflet" for "Puflet"', () => {
    expect(getCritterSpeciesName('Puflet')).toBe('Puflet');
  });

  it('returns null for unknown species', () => {
    expect(getCritterSpeciesName('UnknownBeast')).toBeNull();
  });
});

// ── getCritterStyle — colors ───────────────────────────────────────────────────

describe('getCritterStyle — color per species', () => {
  it('Hatch → brown family', () => {
    const { color } = getCritterStyle('Hatch');
    expect(color).toBe('#8B6914');
  });

  it('HatchHard (Sage Hatch) → different shade', () => {
    const { color } = getCritterStyle('HatchHard');
    expect(color).toBe('#A0522D');
  });

  it('Puft → purple', () => {
    const { color } = getCritterStyle('Puft');
    expect(color).toBe('#9B59B6');
  });

  it('PuftAlpine → lighter purple', () => {
    const { color } = getCritterStyle('PuftAlpine');
    expect(color).toBe('#B39DDB');
  });

  it('Pacu → blue', () => {
    const { color } = getCritterStyle('Pacu');
    expect(color).toBe('#2980B9');
  });

  it('LightBug → yellow', () => {
    const { color } = getCritterStyle('LightBug');
    expect(color).toBe('#FFF176');
  });

  it('Morb → pink-red', () => {
    const { color } = getCritterStyle('Morb');
    expect(color).toBe('#E91E63');
  });

  it('Drecko → green', () => {
    const { color } = getCritterStyle('Drecko');
    expect(color).toBe('#27AE60');
  });

  it('Drecklet → lighter green (different from Drecko)', () => {
    const dreckoColor  = getCritterStyle('Drecko').color;
    const dreckletColor = getCritterStyle('Drecklet').color;
    expect(dreckletColor).not.toBe(dreckoColor);
  });

  it('unknown species → DEFAULT_CRITTER_COLOR', () => {
    expect(getCritterStyle('MysteryCreature').color).toBe(DEFAULT_CRITTER_COLOR);
  });
});

// ── getCritterStyle — size multiplier ─────────────────────────────────────────

describe('getCritterStyle — sizeMultiplier', () => {
  it('adult Hatch → 1.0', () => {
    expect(getCritterStyle('HatchHard').sizeMultiplier).toBe(1.0);
  });

  it('adult Puft → 1.0', () => {
    expect(getCritterStyle('Puft').sizeMultiplier).toBe(1.0);
  });

  it('HatchBaby → 0.7', () => {
    expect(getCritterStyle('HatchBaby').sizeMultiplier).toBe(0.7);
  });

  it('PuftBaby → 0.7', () => {
    expect(getCritterStyle('PuftBaby').sizeMultiplier).toBe(0.7);
  });

  it('Drecklet (juvenile) → 0.7', () => {
    expect(getCritterStyle('Drecklet').sizeMultiplier).toBe(0.7);
  });

  it('Puflet (juvenile) → 0.7', () => {
    expect(getCritterStyle('Puflet').sizeMultiplier).toBe(0.7);
  });

  it('unknown species adult → 1.0', () => {
    expect(getCritterStyle('UnknownBeast').sizeMultiplier).toBe(1.0);
  });

  it('unknown species baby → 0.7', () => {
    expect(getCritterStyle('UnknownBeastBaby').sizeMultiplier).toBe(0.7);
  });
});

// ── getCritterStyle — label ────────────────────────────────────────────────────

describe('getCritterStyle — label', () => {
  it('adult Hatch → "Hatch"', () => {
    expect(getCritterStyle('Hatch').label).toBe('Hatch');
  });

  it('HatchBaby → "Hatch (baby)"', () => {
    expect(getCritterStyle('HatchBaby').label).toBe('Hatch (baby)');
  });

  it('PuftBaby → "Puft (baby)"', () => {
    expect(getCritterStyle('PuftBaby').label).toBe('Puft (baby)');
  });

  it('Drecklet label does not double-add "(baby)"', () => {
    // Drecklet is already a juvenile prefix, so no extra "(baby)" suffix
    expect(getCritterStyle('Drecklet').label).toBe('Drecklet');
  });

  it('Puflet label does not double-add "(baby)"', () => {
    expect(getCritterStyle('Puflet').label).toBe('Puflet');
  });

  it('Drecko (adult) → "Drecko"', () => {
    expect(getCritterStyle('Drecko').label).toBe('Drecko');
  });

  it('DreckoBaby → "Drecko (baby)"', () => {
    expect(getCritterStyle('DreckoBaby').label).toBe('Drecko (baby)');
  });

  it('unknown adult → uses cleaned prefabId as label', () => {
    expect(getCritterStyle('MyCreature').label).toBe('MyCreature');
  });

  it('unknown baby → label ends with "(baby)"', () => {
    expect(getCritterStyle('MyCreatureBaby').label).toContain('(baby)');
  });
});

// ── getCritterStyle — letter ───────────────────────────────────────────────────

describe('getCritterStyle — letter', () => {
  it('adult Hatch → "H"', () => {
    expect(getCritterStyle('Hatch').letter).toBe('H');
  });

  it('HatchBaby → lowercase "h"', () => {
    expect(getCritterStyle('HatchBaby').letter).toBe('h');
  });

  it('adult Puft → "P"', () => {
    expect(getCritterStyle('Puft').letter).toBe('P');
  });

  it('unknown species → "C"', () => {
    expect(getCritterStyle('UnknownBeast').letter).toBe('C');
  });
});

// ── prefix collision safety ────────────────────────────────────────────────────

describe('prefix collision safety', () => {
  it('"Drecko" and "Drecklet" resolve to different entries', () => {
    const dreckoLabel   = getCritterStyle('Drecko').label;
    const dreckletLabel = getCritterStyle('Drecklet').label;
    expect(dreckoLabel).not.toBe(dreckletLabel);
  });

  it('"Puft" and "Puflet" resolve to different entries', () => {
    const puftLabel   = getCritterStyle('Puft').label;
    const pufletLabel = getCritterStyle('Puflet').label;
    expect(puftLabel).not.toBe(pufletLabel);
  });

  it('"PuftAlpine" resolves to PuftAlpine entry, not plain Puft', () => {
    expect(getCritterStyle('PuftAlpine').label).toBe('Plume Puft');
  });
});
