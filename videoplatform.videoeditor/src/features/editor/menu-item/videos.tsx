import Draggable from "@/components/shared/draggable";
import { ScrollArea } from "@/components/ui/scroll-area";
import { dispatch } from "@designcombo/events";
import { ADD_VIDEO } from "@designcombo/state";
import { generateId } from "@designcombo/timeline";
import { IVideo } from "@designcombo/types";
import React from "react";
import { useIsDraggingOverTimeline } from "../hooks/is-dragging-over-timeline";
import { useUploadedVideosStore } from "../store/use-uploaded-videos-store";

export const Videos = () => {
  const uploadedVideos = useUploadedVideosStore((state) => state.videos);
  const isDraggingOverTimeline = useIsDraggingOverTimeline();

    const handleAddVideo = (src: string) => {
    dispatch(ADD_VIDEO, {
      payload: {
        id: generateId(),
        details: {
          src: src
        },
        metadata: {
          resourceId: src
        }
      },
      options: {
        resourceId: "main"
      }
    });
  };

  return (
    <div className="flex flex-1 flex-col">
      <div className="text-text-primary flex h-12 flex-none items-center px-4 text-sm font-medium">
        Videos
      </div>
          <ScrollArea className="h-64 overflow-y-scroll border m-2">
              <div className="px-4">
                  {uploadedVideos.length === 0 ? (
                      <p className="text-sm text-muted-foreground text-center py-4">
                          No videos uploaded yet.
                      </p>
                  ) : (
                      uploadedVideos.map((video, index) => {
                          console.log(video);
                          return (
                              <div
                                  onClick={() => handleAddVideo(video.src)}
                                  key={index}
                                  className="flex w-full bg-background pb-2 overflow-hidden cursor-pointer"
                              >
                                  <p>{video.src.split("/").pop()}</p>
                              </div>
                          );
                      })
                  )}
              </div>
          </ScrollArea>
    </div>
  );
};

const VideoItem = ({
  handleAddImage,
  video,
  shouldDisplayPreview,
}: {
  handleAddImage: (payload: Partial<IVideo>) => void;
  video: Partial<IVideo>;
  shouldDisplayPreview: boolean;
}) => {
  const style = React.useMemo(
    () => ({
      backgroundImage: `url(${video.preview})`,
      backgroundSize: "cover",
      width: "80px",
      height: "80px",
    }),
    [video.preview],
  );

  return (
    <Draggable
      data={{
        ...video,
        metadata: {
          previewUrl: video.preview,
        },
      }}
      renderCustomPreview={<div style={style} className="draggable" />}
      shouldDisplayPreview={shouldDisplayPreview}
    >
      <div
        onClick={() =>
          handleAddImage({
            id: generateId(),
            details: {
              src: video.details!.src,
            },
            metadata: {
              previewUrl: video.preview,
            },
          } as any)
        }
        className="flex w-full items-center justify-center overflow-hidden bg-background pb-2"
      >
        <img
          draggable={false}
          src={video.preview}
          className="h-full w-full rounded-md object-cover"
          alt="image"
        />
      </div>
    </Draggable>
  );
};
