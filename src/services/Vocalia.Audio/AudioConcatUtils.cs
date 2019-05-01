using NAudio.Utils;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Collections.Generic;
using System.IO;

namespace Vocalia.Audio
{
    public class AudioConcatUtils
    {
        /// <summary>
        /// Concatenates a set of audio sequences on top of each other, returning the compiled file as a stream.
        /// </summary>
        /// <param name="streams">Streams to concatenate.</param>
        /// <returns></returns>
        public static Stream ConcatAudioStreams(IEnumerable<Stream> streams)
        {
            var mixedStream = new MemoryStream();
            var sampleProviders = new List<ISampleProvider>();

            foreach (var stream in streams)
            {
                stream.Position = 0;
                var reader = new WaveFileReader(stream);
                sampleProviders.Add(reader.ToSampleProvider());
            }

            var mixer = new MixingSampleProvider(sampleProviders);
            var waveProvider = mixer.ToWaveProvider();
            WaveFileWriter.WriteWavFileToStream(new IgnoreDisposeStream(mixedStream), waveProvider);

            foreach (var stream in streams)
                stream.Dispose();

            return mixedStream;
        }

        /// <summary>
        /// Combines file sequences one after the other, and exports the combined file as a stream.
        /// </summary>
        /// <param name="streams">Streams to combine.</param>
        /// <returns></returns>
        public static Stream SequenceAudioStreams(IEnumerable<Stream> streams)
        {
            byte[] buffer = new byte[1024];
            var outputStream = new MemoryStream();
            WaveFileWriter waveFileWriter = null;

            foreach (var stream in streams)
            {
                stream.Position = 0;
                using (WaveFileReader reader = new WaveFileReader(stream))
                {
                    if (waveFileWriter == null)
                        waveFileWriter = new WaveFileWriter(new IgnoreDisposeStream(outputStream), reader.WaveFormat);

                    int read;
                    while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        waveFileWriter.Write(buffer, 0, read);
                    }
                }
                stream.Dispose();
            }

            waveFileWriter.Dispose();
            return outputStream;
        }
    }
}
