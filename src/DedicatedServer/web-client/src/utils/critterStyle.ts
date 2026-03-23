/**
 * critterStyle — pure mapping from a critter prefabId (entity.name) to
 * visual style properties: color, size multiplier, display label, marker letter.
 *
 * The server sends `name = prefabId` (e.g. "HatchHard", "PuftBaby", "DreckoBaby").
 * There are no separate isBaby / species / age / scale fields in the API.
 * Baby detection: prefabId contains "Baby".
 * Species detection: prefabId starts with a known species prefix.
 */

export interface CritterStyle {
  /** CSS color for the body fill */
  color: string;
  /** Size multiplier applied to the base pill radius.  Baby = 0.7, adult = 1.0. */
  sizeMultiplier: number;
  /** Short display label shown as a canvas annotation: e.g. "Hatch", "Puft (baby)" */
  label: string;
  /** Single-character abbreviation shown inside the marker at small zoom levels */
  letter: string;
}

interface SpeciesEntry {
  prefix: string;
  color: string;
  display: string;
  /** Upper-case letter for adults; getCritterStyle lowercases it for babies */
  letter: string;
}

// Ordered so that longer prefixes that share a root with shorter ones are listed
// first (e.g. "Drecklet" before "Drecko" would be needed if they shared a prefix
// — they don't, but the ordering is documented here for safety).
const SPECIES: SpeciesEntry[] = [
  { prefix: 'HatchMetal',   color: '#B0B0B0', display: 'Stone Hatch',  letter: 'H' },
  { prefix: 'HatchHard',    color: '#A0522D', display: 'Sage Hatch',   letter: 'H' },
  { prefix: 'HatchVeggie',  color: '#6B8E23', display: 'Hmm Hatch',    letter: 'H' },
  { prefix: 'Hatch',        color: '#8B6914', display: 'Hatch',        letter: 'H' },
  { prefix: 'PuftAlpine',   color: '#B39DDB', display: 'Plume Puft',   letter: 'P' },
  { prefix: 'PuftOxylite',  color: '#CE93D8', display: 'Ox. Puft',     letter: 'P' },
  { prefix: 'Puflet',       color: '#E1BEE7', display: 'Puflet',       letter: 'p' },
  { prefix: 'Puft',         color: '#9B59B6', display: 'Puft',         letter: 'P' },
  { prefix: 'DreckoBaby',   color: '#52BE80', display: 'Drecko',       letter: 'k' },
  { prefix: 'Drecklet',     color: '#52BE80', display: 'Drecklet',     letter: 'k' },
  { prefix: 'Drecko',       color: '#27AE60', display: 'Drecko',       letter: 'K' },
  { prefix: 'Slickster',    color: '#16A085', display: 'Slickster',    letter: 'S' },
  { prefix: 'PacuTropical', color: '#1E88E5', display: 'Tropical Pacu',letter: 'F' },
  { prefix: 'PacuCleaner',  color: '#29B6F6', display: 'Gulp Fish',    letter: 'F' },
  { prefix: 'Pacu',         color: '#2980B9', display: 'Pacu',         letter: 'F' },
  { prefix: 'MoleDelicacy', color: '#A1887F', display: 'Sweetle',      letter: 'M' },
  { prefix: 'Mole',         color: '#795548', display: 'Mole',         letter: 'M' },
  { prefix: 'Morb',         color: '#E91E63', display: 'Morb',         letter: 'O' },
  { prefix: 'OilFloater',   color: '#78909C', display: 'Oilfloater',   letter: 'L' },
  { prefix: 'Oilfloater',   color: '#78909C', display: 'Oilfloater',   letter: 'L' },
  { prefix: 'Squirrel',     color: '#D2691E', display: 'Pip',          letter: 'Q' },
  { prefix: 'Crab',         color: '#C0392B', display: 'Pokeshell',    letter: 'R' },
  { prefix: 'LightBug',     color: '#FFF176', display: 'Shine Bug',    letter: 'B' },
  { prefix: 'Shine',        color: '#F39C12', display: 'Shine Bug',    letter: 'B' },
  { prefix: 'Spider',       color: '#546E7A', display: 'Shove Vole',   letter: 'V' },
  { prefix: 'Divergent',    color: '#FFD700', display: 'Divergent',    letter: 'D' },
  { prefix: 'Dreep',        color: '#00BCD4', display: 'Dreep',        letter: 'E' },
  { prefix: 'Glop',         color: '#76C442', display: 'Glop',         letter: 'G' },
  { prefix: 'Flipped',      color: '#607D8B', display: 'Flipped',      letter: 'I' },
];

export const DEFAULT_CRITTER_COLOR = '#4cff91';

/**
 * Returns true when the prefabId belongs to a baby critter.
 * ONI encodes this by embedding "Baby" in the prefab name
 * (e.g. "HatchBaby", "PuftBabyAlpine") or via juvenile-only prefabs
 * like "Drecklet" and "Puflet" which are always juvenile forms.
 */
export function isCritterBaby(prefabId: string): boolean {
  return prefabId.includes('Baby')
    || prefabId.startsWith('Drecklet')
    || prefabId.startsWith('Puflet');
}

/**
 * Returns the human-readable species display name for the prefabId,
 * or null when the species is not in the known list.
 */
export function getCritterSpeciesName(prefabId: string): string | null {
  for (const s of SPECIES) {
    if (prefabId.startsWith(s.prefix)) return s.display;
  }
  return null;
}

/**
 * Returns the full visual style for a critter entity given its prefabId
 * (the value of `entity.name` when `entity.type === 'critter'`).
 *
 * Pure function — no DOM, no side effects.
 */
export function getCritterStyle(prefabId: string): CritterStyle {
  const baby = isCritterBaby(prefabId);

  for (const s of SPECIES) {
    if (prefabId.startsWith(s.prefix)) {
      // Drecklet and Puflet rows already encode the juvenile form; only add
      // "(baby)" suffix when the prefabId *additionally* contains "Baby".
      const isJuvenilePrefix = s.prefix === 'Drecklet' || s.prefix === 'Puflet';
      const labelSuffix = baby && !isJuvenilePrefix ? ' (baby)' : '';
      return {
        color:          s.color,
        sizeMultiplier: baby ? 0.7 : 1.0,
        label:          `${s.display}${labelSuffix}`,
        letter:         baby ? s.letter.toLowerCase() : s.letter,
      };
    }
  }

  // Unknown species — fall back to generic style derived from the prefabId itself.
  const cleanName = prefabId.replace('Baby', '').trim() || prefabId;
  return {
    color:          DEFAULT_CRITTER_COLOR,
    sizeMultiplier: baby ? 0.7 : 1.0,
    label:          baby ? `${cleanName} (baby)` : cleanName,
    letter:         'C',
  };
}
