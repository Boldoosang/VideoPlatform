const express = require("express");
const cors = require("cors");
const path = require("path");
const fs = require("fs");
const { bundle } = require("@remotion/bundler");
const { getCompositions, renderMedia, renderStill } = require("@remotion/renderer");
const { BlobServiceClient } = require("@azure/storage-blob");
const dotenv = require("dotenv");
const sharp = require("sharp");

dotenv.config();

const app = express();
const port = process.env.PORT || 8080;

app.use(cors());
app.use(express.json());

const AZURE_STORAGE_CONNECTION_STRING = process.env.AZURE_STORAGE_CONNECTION_STRING;
const CONTAINER_NAME = process.env.AZURE_STORAGE_CONTAINER_NAME;
const CHROMIUM_EXECUTABLE_PATH = process.env.CHROMIUM_EXECUTABLE_PATH || "/path/to/chromium";

app.get("/", (req, res) => {
    console.log("Video Platform video render server is running");
    res.send("Video Platform video render server is running");
});

const renderStatusMap = new Map(); // key: videoId, value: { status, progress, url }

const retryOperation = async (operation) => {
    try {
        await operation();
    } catch (err) {
        console.log("Operation failed, retrying...");
        try {
            await operation(); // Retry once
        } catch (err) {
            console.error("Operation failed after retrying:", err);
            throw err;
        }
    }
};

app.post("/api/render", async (req, res) => {
    let videoId = null;
    try {
        const { design } = req.body;
        const videoName = req.body.design.title.trim().replace(/\s+/g, "_");
        videoId = `${Date.now()}_${videoName}`;
        const videoOutputFileName = `${videoId}.mp4`;
        const thumbnailOutputFileName = `${videoId}.png`;
        const videoOutputPath = path.join(process.cwd(), "out", videoOutputFileName);
        const thumbnailOutputPath = path.join(process.cwd(), "out", thumbnailOutputFileName);

        renderStatusMap.set(videoId, { status: "PENDING", progress: 0, url: null, updatedAt: null });

        console.log(`Started rendering video with ID: ${videoId}`);
        res.json({ success: true, videoId });

        const entry = path.join(process.cwd(), "src", "index.ts");

        console.log("Bundling the Remotion project...");
        renderStatusMap.set(videoId, { status: "PENDING", progress: 10, url: null });
        const bundled = await bundle(entry);

        console.log("Getting compositions...");
        renderStatusMap.set(videoId, { status: "PENDING", progress: 20, url: null });
        const compositions = await getCompositions(bundled, {
            inputProps: { design },
        });

        const composition = compositions.find((c) => c.id === "RenderVideo");

        if (!composition) {
            console.error("Composition RenderVideo not found.");
            renderStatusMap.set(videoId, { status: "ERROR", progress: -1, url: null });
            return;
        }

        console.log("Rendering video...");
        renderStatusMap.set(videoId, { status: "PENDING", progress: 40, url: null });
        await retryOperation(async () => {
            await renderMedia({
                composition,
                serveUrl: bundled,
                codec: "h264",
                outputLocation: videoOutputPath,
                inputProps: { design },
                scale: 0.6667,
                crf: 24,
                chromiumExecutable: CHROMIUM_EXECUTABLE_PATH,
            });
        });

        console.log("Rendering thumbnail...");
        renderStatusMap.set(videoId, { status: "PENDING", progress: 80, url: null });
        await retryOperation(async () => {
            await renderStill({
                composition,
                serveUrl: bundled,
                output: thumbnailOutputPath,
                frame: 1,
                inputProps: { design },
            });
        });

        const thumbnailOutputPathTemp = path.join(
            path.dirname(thumbnailOutputPath),
            "temp_" + path.basename(thumbnailOutputPath)
        );
        console.log("Optimizing thumbnail...");
        await sharp(thumbnailOutputPath)
            .resize({ width: 480 })
            .png({ quality: 40 })
            .toFile(thumbnailOutputPathTemp);

        fs.renameSync(thumbnailOutputPathTemp, thumbnailOutputPath);

        renderStatusMap.set(videoId, { status: "PENDING", progress: 90, url: null });

        console.log("Uploading to Azure...");
        renderStatusMap.set(videoId, { status: "PENDING", progress: 95, url: null });
        const blobServiceClient = BlobServiceClient.fromConnectionString(AZURE_STORAGE_CONNECTION_STRING);
        const containerClient = blobServiceClient.getContainerClient(CONTAINER_NAME);
        const videoBlockBlobClient = containerClient.getBlockBlobClient(videoOutputFileName);
        const thumbnailBlockBlobClient = containerClient.getBlockBlobClient(thumbnailOutputFileName);

        await videoBlockBlobClient.uploadStream(fs.createReadStream(videoOutputPath));
        await thumbnailBlockBlobClient.uploadStream(fs.createReadStream(thumbnailOutputPath));

        const blobUrl = videoBlockBlobClient.url;

        renderStatusMap.set(videoId, {
            status: "COMPLETED",
            progress: 100,
            updatedAt: Date.now(),
            url: blobUrl,
        });

        console.log(`Video rendering completed. URL: ${blobUrl}`);

        fs.rm(path.join(process.cwd(), "out"), { recursive: true, force: true }, () => { });
    } catch (err) {
        console.error("Error occurred during rendering:", err);
        renderStatusMap.set(videoId ?? 0, {
            status: "ERROR",
            progress: -1,
            updatedAt: Date.now(),
            url: null,
        });
    }
});

app.get("/api/render", (req, res) => {
    const { id, type } = req.query;

    if (!id || type !== "VIDEO_RENDERING") {
        console.error("Invalid parameters received:", req.query);
        return res.status(400).json({ error: "Invalid parameters" });
    }

    const statusInfo = renderStatusMap.get(id);
    if (!statusInfo) {
        console.error(`Video ID ${id} not found`);
        return res.status(404).json({ error: "Video ID not found" });
    }

    console.log(`Fetching status for video ID: ${id}`);
    res.json({ video: statusInfo });
});

app.get("/jobs", (req, res) => {
    const jobs = [];
    const now = Date.now();

    const clearNonPendingInterval = 30 * 1000;

    renderStatusMap.forEach((value, key) => {
        const isPending = value.status === "PENDING";
        const isRecent = value.updatedAt && (now - value.updatedAt <= clearNonPendingInterval);

        if (isPending || isRecent) {
            jobs.push({
                videoId: key,
                progress: value.progress,
                status: value.status
            });
        }
    });

    res.json({ jobs });
});

app.listen(port, "0.0.0.0", () => {
    console.log(`Remotion render server running at http://localhost:${port}`);
});
