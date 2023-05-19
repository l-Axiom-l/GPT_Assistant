using System.Net.Http.Headers;
using System.Speech;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using OpenAI;


using (SpeechRecognitionEngine rec = new SpeechRecognitionEngine(System.Globalization.CultureInfo.CurrentUICulture))
{
    Console.WriteLine("Active");
    rec.LoadGrammar(new DictationGrammar());
    rec.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(rec_SpeechRecognized);
    rec.SetInputToDefaultAudioDevice();
    rec.RecognizeAsync(RecognizeMode.Multiple);

    // Keep the console window open.  
    while (true)
    {
        Console.ReadLine();
    }
    Console.WriteLine("Inactive");
};

static void rec_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
{
    Console.WriteLine("Recognized text: " + e.Result.Text);
}

//OpenAI_API api = new OpenAI_API();
//string temp = await api.Ask(Console.ReadLine());


SpeechSynthesizer synth = new SpeechSynthesizer();
synth.SelectVoiceByHints(VoiceGender.Female, VoiceAge.NotSet, 0, System.Globalization.CultureInfo.CurrentUICulture);
Prompt p = synth.SpeakAsync("");

while(!p.IsCompleted)
    await Task.Delay(100);

