using System.Net.Http.Headers;
using System.Speech;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using OpenAI;
using NAudio;
using NAudio.Wave;
using System.Runtime.InteropServices;

string text = "";
WaveInEvent waveIn = new WaveInEvent();
waveIn.WaveFormat = new WaveFormat(44100, 16, 1); // 44.1kHz, 16-bit, mono
string outputFilePath = "recorded_audio.wav";
WaveFileWriter waveWriter;

string key = "";
if (File.Exists("Key.txt"))
    key = File.ReadAllLines("Key.txt")[0];
else File.WriteAllText("Key.txt", "Replace this with your API Key");
Console.WriteLine(key);

while(true)
{
    await StartRecording();
    OpenAI_API api = new OpenAI_API(key);
    string tts = await api.TTS("recorded_audio.wav");
    string temp = await api.Ask(tts);

    SpeechSynthesizer synth = new SpeechSynthesizer();
    synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.NotSet, 0, System.Globalization.CultureInfo.CurrentUICulture);
    Prompt p = synth.SpeakAsync(temp);

    while(!p.IsCompleted)
        await Task.Delay(100);
}

async Task StartRecording()
{
    waveWriter = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
    waveIn.DataAvailable += WaveIn_DataAvailable;
    waveIn.RecordingStopped += WaveIn_RecordingStopped;
    waveIn.StartRecording();
    Console.WriteLine("Recording started. Press any key to stop.");
    Console.ReadKey();
    waveIn.StopRecording();
    waveIn.Dispose();
    waveWriter.Dispose();
    Console.WriteLine("Recording Stopped");
}

void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
{
    if (waveWriter == null)
        return;

    waveWriter.Write(e.Buffer, 0, e.Buffer.Length);
}

void WaveIn_RecordingStopped(object sender, StoppedEventArgs e)
{
    waveWriter.Flush();
}
