import { Dialog, DialogContent } from "@/components/ui/dialog";
import { useDownloadState } from "./store/use-download-state";
import { Button } from "@/components/ui/button";
import { CircleCheckIcon, XIcon } from "lucide-react";
import { DialogDescription, DialogTitle } from "@radix-ui/react-dialog";
import { download } from "@/utils/download";

const DownloadProgressModal = () => {
  const { progress, displayProgressModal, output, actions } = useDownloadState();
  const isCompleted = progress === 100;

  return (
    <Dialog
      open={displayProgressModal}
      onOpenChange={actions.setDisplayProgressModal}
    >
      <DialogContent className="flex h-[627px] flex-col gap-0 bg-background p-0 sm:max-w-[844px]">
        <DialogTitle className="hidden" />
        <DialogDescription className="hidden" />
        <div className="flex h-16 items-center border-b px-4 font-medium">
          Export to Azure
        </div>
        {isCompleted ? (
          <div className="flex flex-1 flex-col items-center justify-center gap-2 space-y-4">
            <div className="flex flex-col items-center space-y-1 text-center">
              <div className="font-semibold">
                <CircleCheckIcon />
              </div>
              <div className="font-bold py-2">Exported to Azure</div>
                          <div className="text-muted-foreground">
                              Access the video in the&nbsp;
                              <a
                                  href="/Admin/EditedVideoLibrary"
                                  target="_blank"
                                  rel="noopener noreferrer"
                                  className="text-blue-400 underline hover:text-blue-300 transition-colors duration-200"
                              >
                                  'Edited Videos'
                              </a>&nbsp;tab of the Admin dashboard.
                          </div>
            </div>
          </div>
        ) : (
        <div className="flex flex-1 flex-col items-center justify-center gap-6">
            <div className="text-5xl font-semibold text-white">
                {progress}%
            </div>
            <div className="font-bold text-xl text-white">Video Rendering</div>
            <div className="text-center text-zinc-300">
                <div className="mt-5 w-full max-w-lg">
                    <div className="flex justify-between">
                        <div className={`w-3 h-3 rounded-full transition-colors duration-300 ${progress >= 0 ? 'bg-green-400' : 'bg-white'}`}></div>
                        <div className={`w-3 h-3 rounded-full transition-colors duration-300 ${progress >= 20 ? 'bg-green-400' : 'bg-white'}`}></div>
                        <div className={`w-3 h-3 rounded-full transition-colors duration-300 ${progress >= 50 ? 'bg-green-400' : 'bg-white'}`}></div>
                        <div className={`w-3 h-3 rounded-full transition-colors duration-300 ${progress >= 80 ? 'bg-green-400' : 'bg-white'}`}></div>
                        <div className={`w-3 h-3 rounded-full transition-colors duration-300 ${progress >= 90 ? 'bg-green-400' : 'bg-white'}`}></div>
                        <div className={`w-3 h-3 rounded-full transition-colors duration-300 ${progress === 100 ? 'bg-green-400' : 'bg-white'}`}></div>
                    </div>
                    <div className="my-5 text-lg text-white">
                        {progress === 0 && <div>Initializing...</div>}
                        {progress >= 20 && progress < 50 && <div>Starting Render Service...</div>}
                        {progress >= 50 && progress < 80 && <div>Fetching Video Sources...</div>}
                        {progress >= 80 && progress < 90 && <div>Video Queued...</div>}
                        {progress >= 90 && progress < 100 && <div>Rendering...</div>}
                        {progress === 100 && <div>Completed!</div>}
                    </div>
                </div>
                <div>Closing the browser will not cancel the export.</div>
                <div>The video will be saved to Azure.</div>
                {progress === 90 && (
                    <div className="mt-5 text-yellow-500 font-semibold">
                        It will be here a while, but don't worry, it's processing.
                    </div>
                )}
            </div>
        </div>

        )}
      </DialogContent>
    </Dialog>
  );
};

export default DownloadProgressModal;
