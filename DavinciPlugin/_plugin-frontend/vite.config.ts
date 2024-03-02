import { defineConfig, splitVendorChunkPlugin } from "vite";
import { resolve } from "path";
import react from "@vitejs/plugin-react";
import { fileURLToPath, URL } from "node:url";

export default defineConfig({
  define: {
    "process.env.NODE_ENV": JSON.stringify(
      process.env.NODE_ENV || "development"
    ),
    global: "window",
  },
  build: {
    minify: true,
    outDir: resolve(__dirname, "../wwwroot/dist"),
    emptyOutDir: true,
    lib: {
      entry: resolve(__dirname, "src/main.tsx"),
      name: "trinity-plugin-sample",
      fileName: "main",
      formats: ["umd"],
    },
    rollupOptions: {
      output: {
        entryFileNames: "[name].js",
        chunkFileNames: "[name]-[hash].js",
        globals: {
          react: "window.React",
          "react-dom": "window.ReactDOM",
          trinityApp: "window.trinityApp",
          "@inertiajs/react": "InertiaReact",
          primereact: "PrimeReact",
        },
      },
      external: ["react", "react-dom", "@inertiajs/react", "primereact"],
    },
  },
  plugins: [react(), splitVendorChunkPlugin()],
  resolve: {
    alias: {
      // @ts-ignore
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
});
