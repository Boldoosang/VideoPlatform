import path from "path";
import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

export default defineConfig({
    plugins: [react()],
    resolve: {
        alias: {
            "@": path.resolve(__dirname, "./src"),
        },
    },
    build: {
        // Set the output directory to the wwwroot folder of your .NET project
        outDir: path.resolve(__dirname, "../VideoPlatform.Web/wwwroot"), // Adjust path as needed
    },
});
