using System;
using System.Diagnostics;
using System.IO;
using NAudio.Wave;

namespace Mp3Merge
{
    class Program
    {
        private const string Version = "1.0.0";

        #region SilenceFrame

        private static readonly byte[] silenceFrame =
        {
            255, 251, 176, 196, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 73, 110, 102, 111, 0, 0, 0, 15, 0, 0, 1, 128, 0, 3, 174, 218, 0, 2, 5, 8, 10, 13, 16, 18, 20,
            23, 26, 28, 31, 33, 36, 38, 41, 44, 46, 48, 51, 54, 56, 59, 62, 64, 66, 69, 72, 74, 77, 80, 82, 84,
            87, 90, 92, 95, 97, 100, 102, 105, 108, 110, 112, 115, 118, 120, 123, 126, 128, 130, 133, 136, 138,
            141, 144, 146, 148, 151, 154, 156, 159, 161, 164, 166, 169, 172, 174, 176, 179, 182, 184, 187, 190,
            192, 194, 197, 200, 202, 205, 208, 210, 212, 215, 218, 220, 223, 225, 228, 230, 233, 236, 238, 240,
            243, 246, 248, 251, 254, 0, 0, 0, 57, 76, 65, 77, 69, 51, 46, 57, 57, 114, 1, 205, 0, 0, 0, 0, 0, 0,
            0, 0, 52, 192, 36, 3, 24, 65, 0, 0, 192, 0, 3, 174, 218, 170, 98, 36, 64, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };

        #endregion

        static void Main(string[] args)
        {
            Console.WriteLine($"MP3 Merge version {Version}");

            string path = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();

            var soundFiles = GetSoundFilesPaths(path);
            if (soundFiles.Length < 1)
                ReportErrorAndExit("Couldn't find any mp3 files in given directory...");

            var outputStream = new MemoryStream();
            
            //TODO need some validations here
            Console.WriteLine("How many repeats of each sentence do you want? (numbers >0 only, in other case it will fail :-)");
            var numberOfRepeats = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("How many gaps do you need after each sentence to pronounce it by yourself? (numbers >0 only, in other case it will fail :-)");
            var numberOfGaps = Convert.ToInt32(Console.ReadLine());

            var outputFileName = string.Empty;
            while (string.IsNullOrEmpty(outputFileName) || File.Exists(Path.Combine(Directory.GetCurrentDirectory(), outputFileName)))
            {
                Console.WriteLine("Enter output file name (Without extension; .mp3 is default. File can't already exist!): ");
                outputFileName = Console.ReadLine();
            }


            var timeCounter = new Stopwatch();
            timeCounter.Start();
            CombineSounds(soundFiles, numberOfRepeats, numberOfGaps, outputStream);
            timeCounter.Stop();

            WriteToDisc(outputStream, $"{outputFileName}.mp3");

            ShowStatistics(soundFiles.Length, timeCounter.Elapsed.TotalSeconds);

            Console.ReadLine();
        }

        public static void ShowStatistics(int numberOfFiles, double elapsedSeconds)
        {
            Console.WriteLine("Some statistics:");
            Console.WriteLine("Files count: " + numberOfFiles);
            Console.WriteLine("Merging time: " + elapsedSeconds + "s");
        }

        public static void ReportErrorAndExit(string message)
        {
            Console.WriteLine($"I'm not sure what exactly was happend, but something went wrong: {message}");
            Console.ReadLine();
            Environment.Exit(1);
        }

        public static void WriteToDisc(MemoryStream stream, string fileName)
        {
            //need some checks to ensure if user has write priviledges and etcs...
            //also: check if it exist... if so -> ask again
            //also: check if it is not using by another process
            try
            {
                var pathToWrite = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                File.WriteAllBytes(pathToWrite, stream.ToArray());
            }
            catch (Exception ex)
            {
                ReportErrorAndExit(ex.Message);
            }
        }

        public static string[] GetSoundFilesPaths(string path)
        {
            string[] soundFiles = null;
            try
            {
                soundFiles = Directory.GetFiles(path, "*.mp3");

            }
            catch (Exception ex)
            {

                ReportErrorAndExit($"Error: {ex.Message}");
            }

            return soundFiles;
        }

        public static void CombineSounds(string[] inputFiles, int repeats, int gaps, Stream output)
        {
            foreach (string file in inputFiles)
            {   //add using statement to be able to dispose object when it is not in use any more

                Mp3FileReader reader = new Mp3FileReader(file);
                //var bytesPerMillisecond = 24;//reader.WaveFormat.AverageBytesPerSecond / 1000;
                var seconds = reader.TotalTime.TotalSeconds;

                if ((output.Position == 0) && (reader.Id3v2Tag != null))
                {
                    output.Write(reader.Id3v2Tag.RawData, 0, reader.Id3v2Tag.RawData.Length);
                }
                for (int i = 0; i < repeats; i++)
                {
                    Mp3Frame frame;
                    while ((frame = reader.ReadNextFrame()) != null)
                    {
                        output.Write(frame.RawData, 0, frame.RawData.Length);
                    }
                    reader.Position = 1;
                }

                //50? avoid magic numbers!
                for (int x = 0; x < gaps * seconds * 50 ; x++)
                {
                    output.Write(silenceFrame, 0, silenceFrame.Length);
                }

                //TODO clean up this mess
                //Mp3Frame silenceFrame;
                //reader = new Mp3FileReader("set.dat");
                //while ((silenceFrame = reader.ReadNextFrame()) != null)
                //{
                //    foreach (var b in silenceFrame.RawData)
                //    {
                //        Console.Write(b + ", ");
                //    }
                //
                //    output.Write(silenceFrame.RawData, 0, silenceFrame.RawData.Length);
                //
                //    for (int x = 0; x < 10; x++)
                //    {
                //        output.Write(silenceFrame.RawData, 0, silenceFrame.RawData.Length);
                //    }
                //}
                reader.Position = 1;

            }
        }
    }
}