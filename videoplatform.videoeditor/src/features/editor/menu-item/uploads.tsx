import { Button } from "@/components/ui/button";
import { UploadIcon, CheckCircle2 } from "lucide-react";
import { useRef, useState } from "react";
import { useUploadedVideosStore } from "../store/use-uploaded-videos-store";
import { useUploadedImagesStore } from "../store/use-uploaded-images-store";
import { useUploadedAudiosStore } from "../store/use-uploaded-audio-store";
import { Progress } from "@/components/ui/progress";

export const Uploads = () => {
  const inputFileRef = useRef<HTMLInputElement>(null);

  const addVideo = useUploadedVideosStore((state) => state.addVideo);
  const addImage = useUploadedImagesStore((state) => state.addImage);
  const addAudio = useUploadedAudiosStore((state) => state.addAudio);

  const [progress, setProgress] = useState(0);
  const [uploading, setUploading] = useState(false);
  const [successMessage, setSuccessMessage] = useState("");

  const simulateUpload = async (file: File) => {
    setUploading(true);
    setProgress(0);
    setSuccessMessage("");

    // Simulate progress
    for (let i = 1; i <= 100; i += 10) {
      await new Promise((r) => setTimeout(r, 80));
      setProgress(i);
    }

    // Finish upload logic
    if (file.type.startsWith("video/")) {
      addVideo(file);
    } else if (file.type.startsWith("image/")) {
      addImage(file);
    } else if (file.type.startsWith("audio/")) {
      addAudio(file);
    }

    setUploading(false);
    setSuccessMessage(`${file.name} uploaded successfully!`);
  };

  const onInputFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      simulateUpload(file);
    } else {
      console.log("No file selected.");
    }
  };

  return (
    <div className="flex-1 flex flex-col">
      <div className="text-sm flex-none text-text-primary font-medium h-12 flex items-center px-4">
        Your media
      </div>

      <input
        onChange={onInputFileChange}
        ref={inputFileRef}
        type="file"
        className="hidden"
        accept="image/*,audio/*,video/*"
      />

      <div className="px-4 py-2">
      <Button
              onClick={() => {
                inputFileRef.current?.click();
              }}
              className="flex gap-2 w-full"
              variant="secondary"
              disabled={uploading}
            >
              <UploadIcon size={16} /> {uploading ? "Uploading..." : "Upload"}
            </Button>
            {uploading && (
              <div className="mt-4">
                <Progress value={progress} />
              </div>
            )}
            {successMessage && !uploading && (
              <div className="mt-4 flex items-center text-green-600 text-sm gap-2">
                <CheckCircle2 size={18} />
                <span>{successMessage}</span>
              </div>
            )}
      </div>
    </div>
  );
};
