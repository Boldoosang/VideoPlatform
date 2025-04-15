import { useEffect, useState } from "react";
import Editor from "./features/editor";
import useDataState from "./features/editor/store/use-data-state";
import { getCompactFontData } from "./features/editor/utils/fonts";
import { FONTS } from "./features/editor/data/fonts";
import { useLocation } from "react-router-dom";
import { ADD_VIDEO } from "@designcombo/state";
import { dispatch } from "@designcombo/events";
import { generateId } from "@designcombo/timeline";
import logo from "@/assets/logo-dark.png";
import { AlertTriangle, Loader2 } from "lucide-react";
import { useUploadedVideosStore } from "./features/editor/store/use-uploaded-videos-store";

function useQuery() {
    return new URLSearchParams(useLocation().search);
}

export default function App() {
    const query = useQuery();
    const videoUrl = query.get("videoUrl");
    const { setCompactFonts, setFonts } = useDataState();
    const addVideoByUrl = useUploadedVideosStore((state) => state.addVideoByUrl);

    const [isLoading, setIsLoading] = useState(true);
    const [hasError, setHasError] = useState(false);

    // Make handleAddVideo async
    const handleAddVideo = async (src: string) => {
        return new Promise<void>((resolve, reject) => {
            try {
                dispatch(ADD_VIDEO, {
                    payload: {
                        id: generateId(),
                        details: { src },
                        metadata: { resourceId: src },
                    },
                    options: {
                        resourceId: "main",
                    },
                });
                resolve();
            } catch (error) {
                setHasError(true); 
                reject(error);
            }
        });
    };

    useEffect(() => {
        setCompactFonts(getCompactFontData(FONTS));
        setFonts(FONTS);
    }, []);

    useEffect(() => {
        const loadVideo = async () => {
            if (videoUrl) {
                const isValidVideoUrl = videoUrl.toLowerCase().endsWith(".mp4");  // Check for .mp4 extension

                if (!isValidVideoUrl) {
                    setHasError(true);  // Set error state for invalid file type
                    return;  // Early return if the file is not a valid mp4
                }
                try {
                    // Try to add the video
                    await addVideoByUrl(videoUrl, "teams-recording.mp4");
                    await handleAddVideo(videoUrl);
                    setIsLoading(false);
                } catch (error) {
                    console.error("Error adding video:", error);
                    setHasError(true);  // Set error state to show the error message
                }
            } else {
                setHasError(true);  // If no videoUrl, set error state
            }
        };

        loadVideo();
    }, [videoUrl, addVideoByUrl]);

    if (hasError) {
        return (
            <div className="flex flex-col items-center justify-center min-h-screen bg-gradient-to-br from-[#1e1e1e] via-[#2d2d2d] to-[#1a1a1a] text-white font-mono px-6 py-12">
                <img
                    src={logo}
                    alt="App Logo"
                    className="w-60 h-auto mb-8 opacity-90 drop-shadow-lg"
                />
                <div className="w-full max-w-2xl bg-[#2a2a2a] border border-[#3c3c3c] rounded-xl shadow-lg p-8 animate-fade-in">
                    <div className="flex items-center mb-6">
                        <AlertTriangle className="w-6 h-6 text-yellow-400 mr-3 animate-pulse" />
                        <h1 className="text-2xl font-semibold tracking-wide text-white">
                            Error: Video Loading Failed
                        </h1>
                    </div>

                    <div className="bg-[#1e1e1e] border border-[#333] rounded-md p-6 text-sm text-gray-300 leading-relaxed">
                        <p className="mb-3">
                            We encountered an issue while trying to load the video for editing.
                        </p>
                        <p className="text-yellow-300">
                            Please ensure the video URL is valid and points to an accessible video file.
                        </p>
                    </div>

                    <div className="text-right mt-6">
                        <a
                            href="/Admin"
                            className="inline-block bg-yellow-500 hover:bg-yellow-600 text-black font-medium text-sm py-2 px-5 rounded transition duration-300"
                        >
                            Return to Admin Dashboard
                        </a>
                    </div>
                </div>
            </div>
        );
    }



    if (!videoUrl) {
        return (
            <div className="flex flex-col items-center justify-center min-h-screen bg-gradient-to-br from-[#1e1e1e] via-[#2d2d2d] to-[#1a1a1a] text-white font-mono px-6 py-12">
                <img
                    src={logo}
                    alt="App Logo"
                    className="w-60 h-auto mb-8 opacity-90 drop-shadow-lg"
                />
                <div className="w-full max-w-2xl bg-[#2a2a2a] border border-[#3c3c3c] rounded-xl shadow-lg p-8 animate-fade-in">
                    <div className="flex items-center mb-6">
                        <AlertTriangle className="w-6 h-6 text-yellow-400 mr-3 animate-pulse" />
                        <h1 className="text-2xl font-semibold tracking-wide text-white">
                            Missing Video URL
                        </h1>
                    </div>

                    <div className="bg-[#1e1e1e] border border-[#333] rounded-md p-6 text-sm text-gray-300 leading-relaxed">
                        <p className="mb-3">
                            We couldn't find the requested video resource for editing.
                        </p>
                        <p className="text-yellow-300">
                            Please ensure the <code>videoUrl</code> query parameter points to a video file.
                        </p>
                    </div>

                    <div className="text-right mt-6">
                        <a
                            href="/Admin"
                            className="inline-block bg-yellow-500 hover:bg-yellow-600 text-black font-medium text-sm py-2 px-5 rounded transition duration-300"
                        >
                            Return to Admin Dashboard
                        </a>
                    </div>
                </div>
            </div>
        );
    }

    if (isLoading) {
        return (
            <div className="flex flex-col items-center justify-center min-h-screen bg-gradient-to-br from-[#141414] via-[#1e1e1e] to-[#141414] text-white font-mono px-6 py-12 animate-fade-in">
                <img
                    src={logo}
                    alt="Loading Logo"
                    className="w-56 h-auto mb-10 opacity-95 drop-shadow-xl"
                />
                <div className="flex items-center space-x-4">
                    <Loader2 className="w-6 h-6 animate-spin text-gray-300" />
                    <p className="text-lg tracking-wide text-gray-300">Loading video editor...</p>
                </div>
            </div>
        );
    }

    return <Editor />;
}
