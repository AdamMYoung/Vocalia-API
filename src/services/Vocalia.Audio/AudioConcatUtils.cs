using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Vocalia.Audio
{
    public class AudioConcatUtils
    {
        public static MemoryStream ConcatAudioStreams(IEnumerable<MemoryStream> streams)
        {
            var mixedStream = new MemoryStream();
            var sampleProviders = new List<ISampleProvider>();

            foreach (var stream in streams) {
                stream.Position = 0;
                var reader = new WaveFileReader(stream);
                sampleProviders.Add(reader.ToSampleProvider());
            }

            var mixer = new MixingSampleProvider(sampleProviders);
            var waveProvider = mixer.ToWaveProvider();
            WaveFileWriter.WriteWavFileToStream(mixedStream, waveProvider);
            return mixedStream;
        }
    }
}
