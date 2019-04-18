using Concentus.Oggfile;
using Concentus.Structs;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Vocalia.Audio
{
    public static class AudioEditUtils
    {
        /// <summary>
        /// Trims an incoming stream.
        /// </summary>
        /// <param name="stream">Stream to trim.</param>
        /// <param name="startSeconds">Seconds to trim off of start.</param>
        /// <param name="endSeconds">Seconds to trim off of end.</param>
        /// <returns></returns>
        public static async Task<MemoryStream> TrimFile(MemoryStream stream, int startSeconds, int endSeconds)
        {
            var editedStream = new MemoryStream();

            stream.Position = 0;
            using (var reader = new WaveFileReader(stream))
            using (var writer = new WaveFileWriter(editedStream, reader.WaveFormat))
            {
                int bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000;

                int startPos = (startSeconds * 100) * bytesPerMillisecond;
                startPos -= startPos % reader.WaveFormat.BlockAlign;

                int endBytes = (endSeconds * 100) * bytesPerMillisecond;
                endBytes -= endBytes % reader.WaveFormat.BlockAlign;
                int endPos = (int)reader.Length - endBytes;

                await TrimWavFile(reader, writer, startPos, endPos);
                stream.Dispose();
                return editedStream;
            }
        }

        /// <summary>
        /// Processes the reader, exporting the results to the writer.
        /// </summary>
        /// <param name="reader">WAV stream to read.</param>
        /// <param name="writer">WAV stream to write.</param>
        /// <param name="startPos">Start position.</param>
        /// <param name="endPos">End position.</param>
        /// <returns></returns>
        private static async Task TrimWavFile(WaveFileReader reader, WaveFileWriter writer, int startPos, int endPos)
        {
            reader.Position = startPos;
            byte[] buffer = new byte[1024];
            while (reader.Position < endPos)
            {
                int bytesRequired = (int)(endPos - reader.Position);
                if (bytesRequired > 0)
                {
                    int bytesToRead = Math.Min(bytesRequired, buffer.Length);
                    int bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        await writer.WriteAsync(buffer, 0, bytesRead);
                    }
                }
            }
        }

    }
}
