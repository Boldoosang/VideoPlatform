import { create } from "zustand";

export interface UploadedAudio {
  id: string;
  file?: File;
  src: string; // Now uses external URL from temp.sh
  name: string;
}

interface UploadedAudiosStore {
  audios: UploadedAudio[];
  addAudio: (file: File) => Promise<void>;
  addAudioByUrl: (url: string, name?: string) => void;
  removeAudio: (id: string) => void;
  clearAudios: () => void;
}

export const useUploadedAudiosStore = create<UploadedAudiosStore>((set) => ({
  audios: [],

  addAudio: async (file) => {
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

      const newAudio: UploadedAudio = { id, file, src, name };
      set((state) => ({
        audios: [...state.audios, newAudio]
      }));
    } catch (err) {
      console.error("Audio upload error:", err);
    }
  },

  addAudioByUrl: (url, name = "external-audio.mp3") => {
    const id = crypto.randomUUID();
    const newAudio: UploadedAudio = { id, src: url, name };

    set((state) => ({
      audios: [...state.audios, newAudio]
    }));
  },

  removeAudio: (id) =>
    set((state) => {
      const audioToRemove = state.audios.find((audio) => audio.id === id);
      if (audioToRemove?.file) {
        URL.revokeObjectURL(audioToRemove.src);
      }

      return {
        audios: state.audios.filter((audio) => audio.id !== id)
      };
    }),

  clearAudios: () =>
    set((state) => {
      state.audios.forEach((audio) => {
        if (audio.file) {
          URL.revokeObjectURL(audio.src);
        }
      });
      return { audios: [] };
    })
}));
