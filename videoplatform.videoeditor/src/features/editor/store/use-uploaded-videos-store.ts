import { create } from "zustand";

export interface UploadedVideo {
  id: string;
  file?: File;
  src: string; // Now a remote URL from temp.sh
  name: string;
}

interface UploadedVideosStore {
  videos: UploadedVideo[];
  addVideo: (file: File) => Promise<void>;
  addVideoByUrl: (url: string, name?: string) => void;
  removeVideo: (id: string) => void;
  clearVideos: () => void;
}

export const useUploadedVideosStore = create<UploadedVideosStore>((set) => ({
  videos: [],

  addVideo: async (file) => {
    const id = crypto.randomUUID();
    const name = file.name;

    try {
        const formData = new FormData();
        formData.append("file", file);

        const res = await fetch("https://tmpfiles.org/api/v1/upload", {
        method: "POST",
        body: formData,
        });

        if (!res.ok) {
        throw new Error("Upload to tmpfiles.org failed.");
        }

        const result = await res.json();

        if (!result.data || !result.data.url) {
        throw new Error("tmpfiles.org response did not contain a URL.");
        }

        const src = `https://tmpfiles.org/dl${result.data.url.split('tmpfiles.org')[1]}`;

      const newVideo: UploadedVideo = { id, file, src, name };
      set((state) => ({
        videos: [...state.videos, newVideo]
      }));
    } catch (error) {
      console.error("Error uploading file to temp.sh:", error);
    }
  },

  addVideoByUrl: (url, name = "external-video.mp4") => {
    const id = crypto.randomUUID();
    const newVideo: UploadedVideo = { id, src: url, name };

    set((state) => ({
      videos: [...state.videos, newVideo]
    }));
  },

  removeVideo: (id) =>
    set((state) => {
      const videoToRemove = state.videos.find((v) => v.id === id);
      if (videoToRemove?.file) {
        URL.revokeObjectURL(videoToRemove.src);
      }

      return {
        videos: state.videos.filter((v) => v.id !== id)
      };
    }),

  clearVideos: () =>
    set((state) => {
      state.videos.forEach((v) => {
        if (v.file) {
          URL.revokeObjectURL(v.src);
        }
      });
      return { videos: [] };
    })
}));
