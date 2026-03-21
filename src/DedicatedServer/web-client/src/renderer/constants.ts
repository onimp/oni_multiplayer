import type { ElementInfo } from '../api/types';

// Dynamic element data — populated from /api/elements
let elementColors: Record<number, string> = {};
let elementNames: Record<number, string> = {};
let elementsLoaded = false;

// Known ONI element colors (common elements)
const KNOWN_ELEMENT_COLORS: Record<string, string> = {
  'Vacuum':        '#0a0a0a',
  'Void':          '#0a0a0a',
  'Oxygen':        '#7ec8e3',
  'CarbonDioxide': '#8a8a8a',
  'Hydrogen':      '#f5e6ca',
  'Water':         '#2e86c1',
  'DirtyWater':    '#6b4e35',
  'SaltWater':     '#3498db',
  'Brine':         '#2980b9',
  'Granite':       '#7f8c8d',
  'SandStone':     '#c4a35a',
  'Algae':         '#27ae60',
  'Cuprite':       '#e67e22',
  'Ice':           '#aed6f1',
  'IgneousRock':   '#5d6d7e',
  'SedimentaryRock': '#a0522d',
  'Obsidian':      '#2c3e50',
  'Iron':          '#839192',
  'IronOre':       '#b7410e',
  'Gold':          '#ffd700',
  'GoldAmalgam':   '#daa520',
  'Copper':        '#b87333',
  'Lead':          '#6c757d',
  'Aluminum':      '#c0c0c0',
  'AluminumOre':   '#b0b0b0',
  'Wolframite':    '#4a4a4a',
  'Tungsten':      '#808080',
  'Diamond':       '#b9f2ff',
  'Coal':          '#2d2d2d',
  'Carbon':        '#333333',
  'Fertilizer':    '#8B4513',
  'Dirt':          '#8B7355',
  'Clay':          '#CD853F',
  'Sand':          '#EDC9AF',
  'Regolith':      '#bcaaa4',
  'Lime':          '#f5f5dc',
  'Rust':          '#b7410e',
  'Salt':          '#f0ead6',
  'BleachStone':   '#e0e0d1',
  'SlimeMold':     '#6b8e23',
  'Sulfur':        '#ffff00',
  'Phosphorite':   '#90ee90',
  'Fossil':        '#d2b48c',
  'Magma':         '#ff4500',
  'MoltenIron':    '#ff6347',
  'MoltenGold':    '#ffa500',
  'MoltenCopper':  '#ff8c00',
  'MoltenGlass':   '#ff7f50',
  'Petroleum':     '#2c2c2c',
  'CrudeOil':      '#1a1a2e',
  'NaphthGas':     '#3d3d3d',
  'Methane':       '#a8d8ea',
  'ChlorineGas':   '#98fb98',
  'Chlorine':      '#98fb98',
  'Steam':         '#dcdcdc',
  'ContaminatedOxygen': '#9acd32',
  'Katairite':     '#4682b4',
  'Unobtanium':    '#ff1493',
  'Abyssalite':    '#4682b4',
  'Neutronium':    '#ff1493',
  'Snow':          '#fffafa',
  'Glass':         '#e0f7fa',
  'Steel':         '#71797e',
  'Plastic':       '#f0e68c',
  'Polypropylene': '#fffdd0',
  'Isoresin':      '#daa06d',
  'Ceramic':       '#faebd7',
  'RefinedCarbon': '#1a1a1a',
  'Concrete':      '#b0b0a8',
  'TempConductorSolid': '#c0c0ff',
  'SuperInsulator': '#4a4a8a',
  'ViscoGel':      '#9b59b6',
  'SuperCoolant':  '#00ffff',
  'Niobium':       '#7b68ee',
  'PhosphorusGas': '#adff2f',
  'Phosphorus':    '#7cfc00',
  'Ethanol':       '#ffe4b5',
  'LiquidHydrogen': '#e0ffff',
  'LiquidOxygen':  '#87ceeb',
  'LiquidCarbonDioxide': '#778899',
  'LiquidSulfur':  '#ffee58',
  'LiquidPhosphorus': '#7cfc00',
  'LiquidMethane': '#b0e0e6',
  'Neon':          '#ff69b4',
  'Helium':        '#ffb6c1',
};

// Generate a color based on element state for unknown elements
function generateColorForState(state: string, index: number): string {
  // Use a hash-based hue for variety
  const hue = (index * 137.508) % 360; // golden angle for even distribution
  if (state.includes('Solid')) return `hsl(${hue}, 40%, 45%)`;
  if (state.includes('Liquid')) return `hsl(${hue}, 60%, 50%)`;
  if (state.includes('Gas')) return `hsl(${hue}, 50%, 65%)`;
  if (state.includes('Vacuum')) return '#0a0a0a';
  return `hsl(${hue}, 30%, 50%)`;
}

export function loadElements(elements: ElementInfo[]) {
  elementColors = {};
  elementNames = {};
  for (const elem of elements) {
    elementNames[elem.id] = elem.name;
    elementColors[elem.id] = KNOWN_ELEMENT_COLORS[elem.name] ?? generateColorForState(elem.state, elem.id);
  }
  elementsLoaded = true;
}

export function getElementColor(id: number): string {
  return elementColors[id] ?? '#ff00ff';
}

export function getElementName(id: number): string {
  return elementNames[id] ?? `Unknown (${id})`;
}

export function areElementsLoaded(): boolean {
  return elementsLoaded;
}

export const ENTITY_COLORS: Record<string, string> = {
  duplicant:  '#ffe033',  // bright yellow — highly visible
  critter:    '#4cff91',  // bright green
  building:   '#f5a623',
  entity:     '#5b8fff',  // blue-ish for generic entities
  pickupable: '#9b59b6',
  ore:        '#e67e22',
};
