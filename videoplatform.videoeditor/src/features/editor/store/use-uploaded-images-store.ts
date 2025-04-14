import { create } from "zustand";

export interface UploadedImage {
  id: string;
  file?: File;
  src: string; // Now uses external URL from temp.sh
  name: string;
}

interface UploadedImagesStore {
  images: UploadedImage[];
  addImage: (file: File) => Promise<void>;
  addImageByUrl: (url: string, name?: string) => void;
  removeImage: (id: string) => void;
  clearImages: () => void;
}

export const useUploadedImagesStore = create<UploadedImagesStore>((set) => ({
  images: [],

  addImage: async (file) => {
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

      const newImage: UploadedImage = { id, file, src, name };
      set((state) => ({
        images: [...state.images, newImage]
      }));
    } catch (err) {
      console.error("Image upload error:", err);
    }
  },

  addImageByUrl: (url, name = "external-image.png") => {
    const id = crypto.randomUUID();
    const newImage: UploadedImage = { id, src: url, name };

    set((state) => ({
      images: [...state.images, newImage]
    }));
  },

  removeImage: (id) =>
    set((state) => {
      const imageToRemove = state.images.find((img) => img.id === id);
      if (imageToRemove?.file) {
        URL.revokeObjectURL(imageToRemove.src);
      }

      return {
        images: state.images.filter((img) => img.id !== id)
      };
    }),

  clearImages: () =>
    set((state) => {
      state.images.forEach((img) => {
        if (img.file) {
          URL.revokeObjectURL(img.src);
        }
      });
      return { images: [] };
    })
}));
