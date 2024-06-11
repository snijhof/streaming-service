# streaming-service

TODO

## Installing prerequisites

### FFMpeg

You can install ffmpeg via the following command on MacOS:

``` bash
brew install ffmpeg
```

You can check which inputs you have via the following command:

``` bash
ffmpeg -f avfoundation -list_devices true -i ""
```

To run your stream you can run the following command:



### Mediamtx

Download Mediamtx on the page with all the [releases](https://github.com/bluenviron/mediamtx/releases).

Unzip the files (this can differ based on your OS). Add the following path to the yaml settings:

``` yml
paths:
  gopro:
    source: publisher
```

## Setup video stream

Start mediamtx:

``` bash
./mediamtx
```

After mediamtx is running you can run the following command to run your stream via ffmpeg, be sure you set the correct rtsp url and input (-i):

``` bash
ffmpeg -f avfoundation -framerate 30 -i "0:0" -buffer_size 512k -vsync 1 -r 30 -f rtsp rtsp://localhost:8554/gopro
```

Te check if the stream is running correctly run the following command, be sure to set the correct url.

``` bash
ffplay rtsp://localhost:8554/gopro
```

## Run application to process stream
 
### Python

To start of we can create a virtual environment and install opencv for python.

``` bash
python3 -m venv venv
source venv/bin/activate
pip install opencv-python
```

You can now copy this application and run it to see if the stream is working:

> Check the rtsp_url and set the output_dir path!

``` python
import cv2
import os

# Define the RTSP stream URL
rtsp_url = "rtsp://localhost:8554/gopro"

# Open a connection to the RTSP stream
cap = cv2.VideoCapture(rtsp_url)

if not cap.isOpened():
    print("Error: Could not open RTSP stream.")
    exit()

# Create a directory to save the frames
output_dir = "saved_frames"
if not os.path.exists(output_dir):
    os.makedirs(output_dir)

frame_count = 0
saved_frame_count = 0

while True:
    ret, frame = cap.read()
    if not ret:
        print("Failed to grab frame")
        break

    # Display the frame
    cv2.imshow('RTSP Stream', frame)

    # Save every 30th frame to disk
    if frame_count % 30 == 0:
        frame_filename = os.path.join(output_dir, f"frame_{saved_frame_count}.jpg")
        cv2.imwrite(frame_filename, frame)
        print(f"Saved {frame_filename}")
        saved_frame_count += 1

    frame_count += 1

    # Press 'q' to exit the loop
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Release the capture and close any OpenCV windows
cap.release()
cv2.destroyAllWindows()
```

### .NET