import { useEffect } from "react";
import Editor from "./features/editor";
import useDataState from "./features/editor/store/use-data-state";
import { getCompactFontData } from "./features/editor/utils/fonts";
import { FONTS } from "./features/editor/data/fonts";
import { useLocation } from "react-router-dom";
import { ADD_VIDEO } from "@designcombo/state";
import { dispatch } from "@designcombo/events";
import { generateId } from "@designcombo/timeline";
import { useUploadedVideosStore } from "./features/editor/store/use-uploaded-videos-store";

function useQuery() {
  return new URLSearchParams(useLocation().search);
}

export default function App() {
const query = useQuery();
  const videoUrl = query.get("videoUrl");
  const { setCompactFonts, setFonts } = useDataState();

  const addVideoByUrl = useUploadedVideosStore((state) => state.addVideoByUrl);

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

  useEffect(() => {
    setCompactFonts(getCompactFontData(FONTS));
    setFonts(FONTS);
  }, []);

    useEffect(() => {
    if (videoUrl) {
        addVideoByUrl(videoUrl, "teams-recording.mp4");
        handleAddVideo(videoUrl);
    }
    }, [videoUrl, addVideoByUrl]);


      if (!videoUrl) {
    return (
      <div>
        <h1>Error</h1>
        <p>No video URL provided.</p>
      </div>
    );
  }
  return <Editor />;
}
