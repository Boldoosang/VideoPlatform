import { IDesign } from "@designcombo/types";
import { create } from "zustand";

interface Output {
  url: string;
  type: string;
}

interface DownloadState {
  error: string;
  projectId: string;
  exporting: boolean;
  exportType: "json" | "mp4";
  progress: number;
  output?: Output;
  payload?: IDesign;
  displayProgressModal: boolean;
    actions: {
    setErrorState: (error: string) => void;
    setProjectId: (projectId: string) => void;
    setExporting: (exporting: boolean) => void;
    setExportType: (exportType: "json" | "mp4") => void;
    setProgress: (progress: number) => void;
    setState: (state: Partial<DownloadState>) => void;
    setOutput: (output: Output) => void;
    startExport: (title: string) => void;
    setDisplayProgressModal: (displayProgressModal: boolean) => void;
  };
}

export const useDownloadState = create<DownloadState>((set, get) => ({
  error: "",
  projectId: "",
  exporting: false,
  exportType: "mp4",
  progress: 0,
  displayProgressModal: false,
  actions: {
    setErrorState: (error) => {
          set({ error })
          set({ exporting: false, displayProgressModal: true });
          set({ output: { url: null, type: get().exportType } });
          set({ progress: -1 });
    },
    setProjectId: (projectId) => set({ projectId }),
    setExporting: (exporting) => set({ exporting }),
    setExportType: (exportType) => set({ exportType }),
    setProgress: (progress) => set({ progress }),
    setState: (state) => set({ ...state }),
    setOutput: (output) => set({ output }),
    setDisplayProgressModal: (displayProgressModal) => set({ displayProgressModal }),
    startExport: async (title: string) => {
        try {
            const { exporting } = get();
            if (exporting) {
                set({ displayProgressModal: true });
                return; 
            } 
            set({ exporting: true, displayProgressModal: true });
            const VIDEO_RENDERER_BACKEND = import.meta.env.VITE_VIDEO_RENDERER_BACKEND;

            const { payload } = get();
            if (!payload) {
                set({ error: "Failed to export video." })
                set({ exporting: false, displayProgressModal: true });
                set({ output: { url: null, type: get().exportType } });
                set({ progress: -1 });
                throw new Error("Failed to submit export request.");
            } 

            payload.title = title;

            const response = await fetch(`${VIDEO_RENDERER_BACKEND}/api/render`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({
                    design: payload,
                    options: {
                        fps: 30,
                        size: payload.size,
                        format: "mp4",
                    },
                }),
            });

            if (!response.ok) {
                set({ error: "Failed to export video." })
                set({ exporting: false, displayProgressModal: true });
                set({ output: { url: null, type: get().exportType } });
                set({ progress: -1 });
                throw new Error("Failed to submit export request.");
            } 

            const jobInfo = await response.json();
            const videoId = jobInfo.videoId; 

            if (!videoId) {
                set({ error: "Failed to export video." })
                set({ exporting: false, displayProgressModal: true });
                set({ output: { url: null, type: get().exportType } });
                set({ progress: -1 });
                throw new Error("No video ID returned");
            } 

            const checkStatus = async (videoId: string) => {
                const statusResponse = await fetch(
                    `${VIDEO_RENDERER_BACKEND}/api/render?id=${videoId}&type=VIDEO_RENDERING`
                );

                if (!statusResponse.ok) {
                    set({ error: "Failed to export video." })
                    set({ exporting: false, displayProgressModal: true });
                    set({ output: { url: null, type: get().exportType } });
                    set({ progress: -1 });
                    throw new Error("Failed to fetch export status.");
                }


                const statusInfo = await statusResponse.json();
                const { status, progress, url } = statusInfo.video;

                set({ progress });

                if (status === "COMPLETED") {
                    set({ exporting: false, output: { url, type: get().exportType } });
                } else if (status === "PENDING") {
                    setTimeout(() => checkStatus(videoId), 2500);
                } else if (status === "ERROR") {
                    set({ error: "Failed to export video." })
                    set({ exporting: false, displayProgressModal: true });
                    set({ output: { url: null, type: get().exportType } });
                    set({ progress: -1 });
                    throw new Error("Rendering failed."); 
                }
            };

            checkStatus(videoId); 
        } catch (error) {
            console.error(error);
            set({ error })
            set({ exporting: false, displayProgressModal: true });
            set({ output: { url: null, type: get().exportType } });
            set({ progress: -1 });
        }
    }
  }
}));
