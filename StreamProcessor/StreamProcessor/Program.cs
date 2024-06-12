// See https://aka.ms/new-console-template for more information
using Emgu.CV;

var rtspUrl = "rtsp://localhost:8554/gopro";
var outputPath = "/Users/sjoerdnijhof/Pictures/Screenshots/poc/frame.png";
var ffmpegExecutablePath = "/opt/homebrew/bin/ffmpeg";

// Create a VideoCapture object
string opencvLibPath = "/opt/homebrew/opt/opencv/lib";
if (string.IsNullOrEmpty(opencvLibPath) || !Directory.Exists(opencvLibPath))
{
    Console.WriteLine("Error: OpenCV library path not found. Please check your DYLD_LIBRARY_PATH environment variable.");
    return;
}

// Create a VideoCapture object
using (VideoCapture capture = new VideoCapture(rtspUrl, VideoCapture.API.Ffmpeg))
{
    if (!capture.IsOpened)
    {
        Console.WriteLine("Error: Could not open RTSP stream.");
        return;
    }

    // Create a directory to save the frames
    string outputDir = "saved_frames";
    if (!Directory.Exists(outputDir))
    {
        Directory.CreateDirectory(outputDir);
    }

    int frameCount = 0;
    int savedFrameCount = 0;

    using (Mat frame = new Mat())
    {
        while (true)
        {
            capture.Read(frame);
            if (frame.IsEmpty)
            {
                Console.WriteLine("Failed to grab frame");
                break;
            }

            // Display the frame
            CvInvoke.Imshow("RTSP Stream", frame);

            // Save every 30th frame to disk
            if (frameCount % 30 == 0)
            {
                string frameFilename = Path.Combine(outputDir, $"frame_{savedFrameCount}.jpg");
                frame.Save(frameFilename);
                Console.WriteLine($"Saved {frameFilename}");
                savedFrameCount += 1;
            }

            frameCount++;

            // Press 'q' to exit the loop
            if (CvInvoke.WaitKey(1) == 'q')
            {
                break;
            }
        }
    }

    // Release the VideoCapture object
    capture.Release();
    CvInvoke.DestroyAllWindows();
}