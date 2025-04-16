const express = require("express");
const cors = require("cors");
const path = require("path");
const fs = require("fs");
const { bundle } = require("@remotion/bundler");
const { getCompositions, renderMedia, renderStill } = require("@remotion/renderer");
const { BlobServiceClient } = require("@azure/storage-blob");
const dotenv = require("dotenv");
const sharp = require("sharp")

dotenv.config();

const app = express();
const port = process.env.PORT || 8080;

app.use(cors());
app.use(express.json());

const AZURE_STORAGE_CONNECTION_STRING = process.env.AZURE_STORAGE_CONNECTION_STRING;
const CONTAINER_NAME = process.env.AZURE_STORAGE_CONTAINER_NAME;

app.get("/", (req, res) => {
  res.send("Video Platform video render server is running");
});

const renderStatusMap = new Map(); // key: videoId, value: { status, progress, url }

app.post("/api/render", async (req, res) => {
    try {
        const { design } = req.body;
        const videoName = req.body.design.title.trim().replace(/\s+/g, "_");
        const videoId = `${Date.now()}_${videoName}`; // unique ID
        const videoOutputFileName = `${videoId}.mp4`;
        const thumbnailOutputFileName = `${videoId}.png`;
        const videoOutputPath = path.join(process.cwd(), "out", videoOutputFileName);
        const thumbnailOutputPath = path.join(process.cwd(), "out", thumbnailOutputFileName);

        // Set initial status
        renderStatusMap.set(videoId, {
            status: "PENDING",
            progress: 0,
            url: null,
        });

        res.json({ success: true, videoId });

        const entry = path.join(process.cwd(), "src", "index.ts");

        const bundled = await bundle(entry);

        const compositions = await getCompositions(bundled, {
            inputProps: { design },
        });

        const composition = compositions.find((c) => c.id === "RenderVideo");

        if (!composition) {
            renderStatusMap.set(videoId, { status: "ERROR", progress: 0, url: null });
            return;
        }

        // Simulate progress tracking (Remotion doesn't have built-in progress)
        let progress = 0;
        const simulateProgress = setInterval(() => {
            if (progress < 90) {
                progress += 10;
                renderStatusMap.set(videoId, {
                    ...renderStatusMap.get(videoId),
                    progress,
                });
            }
        }, 1000);

        await renderMedia({
            composition,
            serveUrl: bundled,
            codec: "h264",
            outputLocation: videoOutputPath,
            inputProps: { design },
            scale: 0.6667,
            crf: 24
        });

        await renderStill({
            composition,
            serveUrl: bundled,
            output: thumbnailOutputPath,
            frame: 1,
            inputProps: { design },
        });

        const thumbnailOutputPathTemp = path.join(
            path.dirname(thumbnailOutputPath),
            "temp_" + path.basename(thumbnailOutputPath)
        );
        await sharp(thumbnailOutputPath)
            .resize({ width: 480 })
            .png({ quality: 40 })
            .toFile(thumbnailOutputPathTemp);

        fs.renameSync(thumbnailOutputPathTemp, thumbnailOutputPath);

        clearInterval(simulateProgress);

        const blobServiceClient = BlobServiceClient.fromConnectionString(AZURE_STORAGE_CONNECTION_STRING);
        const containerClient = blobServiceClient.getContainerClient(CONTAINER_NAME);
        const videoBlockBlobClient = containerClient.getBlockBlobClient(videoOutputFileName);
        const thumbnailBlockBlobClient = containerClient.getBlockBlobClient(thumbnailOutputFileName);

        const uploadVideoStream = fs.createReadStream(videoOutputPath);
        await videoBlockBlobClient.uploadStream(uploadVideoStream);

        const uploadThumbnailStream = fs.createReadStream(thumbnailOutputPath);
        await thumbnailBlockBlobClient.uploadStream(uploadThumbnailStream);

        const blobUrl = videoBlockBlobClient.url;

        renderStatusMap.set(videoId, {
            status: "COMPLETED",
            progress: 100,
            url: blobUrl,
        });

        fs.rm(path.join(process.cwd(), "out"), { recursive: true, force: true }, () => { });
    } catch (err) {
        console.error(err);
        // Update status to error
        renderStatusMap.set(videoId ?? 0, {
            status: "ERROR",
            progress: 0,
            url: null,
        });
    }
});

app.get("/api/render", (req, res) => {
    const { id, type } = req.query;

    if (!id || type !== "VIDEO_RENDERING") {
        return res.status(400).json({ error: "Invalid parameters" });
    }

    const statusInfo = renderStatusMap.get(id);

    if (!statusInfo) {
        return res.status(404).json({ error: "Video ID not found" });
    }

    res.json({ video: statusInfo });
});


app.listen(port, "0.0.0.0", () => {
  console.log(`Remotion render server running at http://localhost:${port}`);
});
