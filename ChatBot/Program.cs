using System;
using System.IO;
using NAudio.Wave;
using Vosk;

namespace VoiceRecording
{
    class Program
    {
        static void Main(string[] args)
        {
/*            Console.WriteLine("Périphériques audio disponibles :");
            for (int n = 0; n < WaveInEvent.DeviceCount; n++)
            {
                WaveInCapabilities deviceInfo = WaveInEvent.GetCapabilities(n);
                Console.WriteLine($"{n}: {deviceInfo.ProductName}");
            }

            Console.Write("Sélectionne le numéro du périphérique audio : ");
            int selectedDeviceIndex = int.Parse(Console.ReadLine()) - 1;

            if (selectedDeviceIndex < 0 || selectedDeviceIndex >= WaveInEvent.DeviceCount)
            {
                Console.WriteLine("Numéro de périphérique non valide.");
                return;
            }*/

            Console.WriteLine("Chargement de system de reconnaissance vocale");

            var model = new Model("F:/!!!!! ARTEMIS/ChatBot/audio model translator/vosk-model-fr-0.22/");
            var recognizer = new VoskRecognizer(model, 16000.0f);

            Console.Clear();
            Console.WriteLine("Appuie sur Entrée pour commencer l'enregistrement...");
            Console.ReadLine();

            string outputPath = "F:/!!!!! ARTEMIS/ChatBot/audio output/output.wav"; // Chemin de sortie du fichier audio enregistré


            using (model)
            using (recognizer)
            // Crée un enregistreur audio
            {
                var waveIn = new WaveInEvent();
                waveIn.DeviceNumber = 0;
                waveIn.WaveFormat = new WaveFormat(16000, 1); // Format audio pour Vosk

                var bufferStream = new MemoryStream();
                recognizer.SetMaxAlternatives(3); // Nombre maximal d'alternatives de transcription

                waveIn.DataAvailable += (sender, e) =>
                {
                    bufferStream.Write(e.Buffer, 0, e.BytesRecorded);
                  
                    if(recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
                    {
                        Console.WriteLine(recognizer.Result());
                    }
    
                };


                /* using (var waveIn = new WaveInEvent())
                 {
                     waveIn.WaveFormat = new WaveFormat(48000, 2); // Format audio Stéreo à 48 kHz
                     var writer = new WaveFileWriter(outputPath, waveIn.WaveFormat);
                     waveIn.DataAvailable += (sender, e) =>
                     {
                         writer.Write(e.Buffer, 0, e.BytesRecorded);
                         Console.Write(".");
                         writer.Flush();
                     };
     */
                // Commence l'enregistrement
                waveIn.StartRecording();
                Console.WriteLine("Enregistrement audio en cours... Appuie sur Entrée pour arrêter.");
                Console.ReadLine();
                Console.Clear();

                Console.WriteLine(recognizer.FinalResult());
                Console.ReadLine();

                // Arrête l'enregistrement
                waveIn.StopRecording();
                Console.WriteLine($"Enregistrement terminé. Fichier audio enregistré : {outputPath}");
            }
        }
    }
}
