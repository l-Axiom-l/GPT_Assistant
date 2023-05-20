using System.Net.Http.Headers;
using System.Speech;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using OpenAI;
using NAudio;
using NAudio.Wave;
using System.Windows.Forms;
using GPT_Assistant;
using Gma.System.MouseKeyHook;
using Gma.System.MouseKeyHook.Implementation;
using Message = GPT_Assistant.Message;

MainClass m = new MainClass();
await m.Main();

class MainClass
{
    string text = "";
    string key = "";
    List<Message> chat = new List<Message>();
    int count = 0;
    string outputFilePath = "recorded_audio.wav";
    WaveFileWriter waveWriter;

    private IKeyboardMouseEvents m_GlobalHook;

    bool recOn = false;
    public async Task Main()
    {
        await LogArchive();
        if (File.Exists("Key.txt"))
            key = File.ReadAllLines("Key.txt")[0];
        else File.WriteAllText("Key.txt", "Replace this with your API Key");
        Console.WriteLine(key);

        OpenAI_API api = new OpenAI_API(key);

        m_GlobalHook = Hook.GlobalEvents();
        m_GlobalHook.KeyUp += EndR ;
        m_GlobalHook.KeyDown += startR;

        while (true)
        {
            while (!recOn) await Task.Delay(100);
            await StartRecording(count);
            string tts = await api.TTS("recorded_audio.wav");
            chat.Add(new Message { role = "user", content = tts });
            WriteLog("user: " + tts);
            string temp = await api.Ask(tts, chat);
            chat.Add(new Message { role = "assistant", content = temp });
            WriteLog("assistant: " + temp);
            SpeechSynthesizer synth = new SpeechSynthesizer();
            synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.NotSet, 0, System.Globalization.CultureInfo.CurrentUICulture);
            Prompt p = synth.SpeakAsync(temp);

            while (!p.IsCompleted)
                await Task.Delay(100);
        }
    }

    async Task StartRecording(int count)
    {
        WaveInEvent waveIn = new WaveInEvent();
        waveIn.WaveFormat = new WaveFormat(44100, 16, 1); // 44.1kHz, 16-bit, mono
        waveWriter = new WaveFileWriter(outputFilePath, waveIn.WaveFormat);
        waveIn.DataAvailable += WaveIn_DataAvailable;
        waveIn.RecordingStopped += WaveIn_RecordingStopped;
        waveIn.StartRecording();
        Console.WriteLine("Recording started. Press any key to stop.");

        while(recOn) await Task.Delay(100);

        waveIn.StopRecording();
        waveIn.Dispose();
        waveWriter.Dispose();
        Console.WriteLine("Recording Stopped");
    }

    void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
    {
        waveWriter.Write(e.Buffer, 0, e.Buffer.Length);
    }

    void WaveIn_RecordingStopped(object sender, StoppedEventArgs e)
    {
        waveWriter.Flush();
    }

    void startR(object sender, KeyEventArgs e)
    {
        Console.WriteLine("Test");
        if (e.KeyCode != Keys.NumPad0)
            return;

        recOn = true;
    }

    void EndR(object sender, KeyEventArgs e)
    {
        Console.WriteLine("Test");
        if (e.KeyCode != Keys.NumPad0)
            return;

        recOn = false;
    }

    async Task LogArchive()
    {
        int count = 0;
        if (Directory.Exists("logs"))
            count = Directory.GetFiles("logs").Length;
        else Directory.CreateDirectory("logs");

        if (File.Exists("Log.txt"))
            File.Move("Log.txt", $@"logs\Log{count + 1}.txt");
        else File.Create("Log.txt");
    }

    void WriteLog(string text)
    {
        File.AppendAllText("Log.txt", text + Environment.NewLine);
    }
}



