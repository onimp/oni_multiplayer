# ONI Dedicated Server — Web Visualizer

Web-based visualizer for the ONI Dedicated Server. Shows the game world grid with elements, temperature, and mass overlays.

## Tech Stack
- React 19 + TypeScript
- Vite (build tool)
- HTML Canvas (rendering)

## Development

```bash
npm install
npm run dev      # dev server with HMR at http://localhost:5173
npm run build    # production build → ../wwwroot/
```

## Build Output
The `npm run build` command outputs to `../wwwroot/` which is served by the C# `WebServer` (HttpListener) at runtime. The built assets are embedded as resources in the DedicatedServer DLL.
