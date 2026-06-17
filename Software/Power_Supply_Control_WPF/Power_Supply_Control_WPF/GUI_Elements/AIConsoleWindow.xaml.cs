using Microsoft.VisualBasic;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Whisper.net;

namespace Power_Supply_Control_WPF.GUI_Elements
{
    /// <summary>
    /// Interaction logic for AIConsoleWindow.xaml
    /// </summary>
    public partial class AIConsoleWindow : Window
    {
        public event EventHandler<string>? MessageSubmitted;
        private WaveInEvent? _waveIn;
        private List<byte>? _accumulatedAudioBytes;
        private readonly object _lockObject = new object();

        private CancellationTokenSource? _cts;
        private readonly string _modelPath = "ggml-base.bin";

        private DateTime _lastVoiceDetected = DateTime.Now;
        private const int SilenceThreshold = 8;
        private const int SilenceTimeoutMs = 3000;
        private System.Threading.Timer? _silenceTimer;

        public AIConsoleWindow()
        {
            InitializeComponent();
        }

        public void AppendSystemMessage(string message)
        {
            // Ensure thread safety if called from a background thread
            Dispatcher.Invoke(() =>
            {
                string logEntry = $"[{DateTime.Now:HH:mm:ss}] Agent : {message}";
                MessageHistory.Items.Add(logEntry);
                MessageHistory.ScrollIntoView(logEntry);
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InputTextBox.Focus();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SubmitMessage();
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SubmitMessage();
            }
        }

        private void SubmitMessage()
        {
            string text = InputTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(text))
            {
                string logEntry = $"[{DateTime.Now:HH:mm:ss}] User: {text}";
                MessageHistory.Items.Add(logEntry);
                MessageHistory.ScrollIntoView(logEntry);
                InputTextBox.Clear();
                MessageSubmitted?.Invoke(this, text);
            }
        }

        private async void MicButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (ToggleButton)sender;

            if (button.IsChecked == true)
            {
                if (WaveInEvent.DeviceCount == 0)
                {
                    MessageBox.Show(
                        "No microphone or audio input device could be detected on this computer. " +
                        "Please plug in a microphone and verify your Windows sound privacy settings.",
                        "Microphone Missing",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );

                    button.IsChecked = false;
                    return;
                }

                button.Content = "🛑";

                _accumulatedAudioBytes = new List<byte>();
                _cts = new CancellationTokenSource();
                _lastVoiceDetected = DateTime.Now;
                StartSilenceWatchdog();
                StartLiveRecording();

                // Fire off the background processing loop completely detached from UI thread
                _ = Task.Run(() => StartStreamingTranscriptionLoopAsync(_cts.Token));
            }
            else
            {
                await StopRecordingAndTranscribeAsync();
            }
        }

        private void StartLiveRecording()
        {
            try
            {
                // Whisper format constraint: 16000Hz, 16-bit depth, Mono (1 channel)
                var recordingFormat = new WaveFormat(16000, 16, 1);

                _waveIn = new WaveInEvent
                {
                    WaveFormat = recordingFormat,
                    BufferMilliseconds = 50
                };

                _waveIn.DataAvailable += (s, e) =>
                {
                    lock (_lockObject)
                    {
                        _accumulatedAudioBytes!.AddRange(e.Buffer.Take(e.BytesRecorded));
                    }

                    float maxSampleValue = 0;

                    for (int i = 0; i < e.BytesRecorded; i += 2)
                    {
                        short sample = (short)((e.Buffer[i + 1] << 8) | e.Buffer[i]);

                        // Get the absolute value to normalize the amplitude wave spikes
                        float absoluteSample = Math.Abs(sample / 32768f);

                        if (absoluteSample > maxSampleValue)
                        {
                            maxSampleValue = absoluteSample;
                        }
                    }

                    // Convert to a clean percentage base (0 to 100)
                    int volumePercentage = (int)(maxSampleValue * 100);

                    if (volumePercentage > SilenceThreshold)
                    {
                        _lastVoiceDetected = DateTime.Now;
                    }

                    // Safely push the volume level update
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        VolumeMeter.Value = volumePercentage;

                        // Dynamic visual coloring indicator (Turn red if peaking/clipping)
                        if (volumePercentage > 85)
                            VolumeMeter.Foreground = System.Windows.Media.Brushes.Red;
                        else if (volumePercentage > 50)
                            VolumeMeter.Foreground = System.Windows.Media.Brushes.Orange;
                        else
                            VolumeMeter.Foreground = System.Windows.Media.Brushes.Green;
                    }));
                };

                _waveIn.StartRecording();
            }
            catch (NAudio.MmException mmEx)
            {
                MessageBox.Show($"Could not open microphone stream. This usually happens if another app has exclusive control or Windows Privacy settings block microphone access. Details: {mmEx.Message}",
                                "Audio Hardware Error", MessageBoxButton.OK, MessageBoxImage.Error);

                MicButton.IsChecked = false;
                MicButton.Content = "🎙️ Start Streaming Speech";
                InputTextBox.Text = "Recording initialization failed.";
                _cts?.Cancel();
            }
        }

        private void StopLiveRecording()
        {
            _waveIn?.StopRecording();
            _waveIn?.Dispose();
            _waveIn = null;
            Dispatcher.Invoke(() => VolumeMeter.Value = 0);
        }

        private async Task StartStreamingTranscriptionLoopAsync(CancellationToken token)
        {
            // Set context properties once to avoid overhead in the quick repeating loop
            using var factory = WhisperFactory.FromPath(_modelPath);
            using var processor = factory.CreateBuilder().WithLanguage("en").Build();

            while (!token.IsCancellationRequested)
            {
                try
                {
                    // Run the transcription refresh slice every 1.2 seconds
                    await Task.Delay(1200, token);

                    byte[] currentAudioSnapshot;
                    lock (_lockObject)
                    {
                        currentAudioSnapshot = _accumulatedAudioBytes!.ToArray();
                    }

                    // Don't waste CPU resources processing silent or empty streams
                    if (currentAudioSnapshot.Length < 32000) continue;

                    string liveText = await ProcessAudioBytesWithWhisper(processor, currentAudioSnapshot);

                    // Safely dispatch the live updating text directly back to the WPF UI thread
                    Dispatcher.Invoke(() =>
                    {
                        if (!string.IsNullOrWhiteSpace(liveText))
                        {
                            InputTextBox.Text = liveText;
                            InputTextBox.ScrollToEnd();
                        }
                    });
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Streaming Loop Error: {ex.Message}");
                }
            }
        }

        private async Task FinalPassTranscriptionAsync()
        {
            if (_accumulatedAudioBytes == null || _accumulatedAudioBytes.Count == 0) return;

            byte[] finalSnapshot;
            lock (_lockObject)
            {
                finalSnapshot = _accumulatedAudioBytes.ToArray();
            }

            using var factory = WhisperFactory.FromPath(_modelPath);
            using var processor = factory.CreateBuilder().WithLanguage("en").Build();

            string finalText = await Task.Run(() => ProcessAudioBytesWithWhisper(processor, finalSnapshot));

            Dispatcher.Invoke(() =>
            {
                InputTextBox.Text = string.IsNullOrWhiteSpace(finalText) ? InputTextBox.Text : finalText;
            });

            _accumulatedAudioBytes.Clear();
        }

        private async Task<string> ProcessAudioBytesWithWhisper(WhisperProcessor processor, byte[] rawAudio)
        {
            using var memoryStream = new MemoryStream();

            using (var wavWriter = new WaveFileWriter(
                new IgnoreDisposeStream(memoryStream),
                new WaveFormat(16000, 16, 1)))
            {
                wavWriter.Write(rawAudio, 0, rawAudio.Length);
            }

            memoryStream.Position = 0;

            var textBuilder = new StringBuilder();

            await foreach (var segment in processor.ProcessAsync(memoryStream))
            {
                textBuilder.Append(segment.Text);
            }

            return textBuilder.ToString().Trim();
        }

        private async Task StopRecordingAndTranscribeAsync()
        {
            _silenceTimer?.Dispose();
            _silenceTimer = null;

            _cts?.Cancel();

            StopLiveRecording();

            await FinalPassTranscriptionAsync();

            await Dispatcher.InvokeAsync(() =>
            {
                MicButton.Content = "🎙️";
                MicButton.IsEnabled = true;
                MicButton.IsChecked = false;
                MessageSubmitted?.Invoke(this, InputTextBox.Text);
                InputTextBox.Clear();
            });
        }

        private void StartSilenceWatchdog()
        {
            _silenceTimer?.Dispose();

            _silenceTimer = new System.Threading.Timer(
                async _ =>
                {
                    try
                    {
                        if ((DateTime.Now - _lastVoiceDetected).TotalMilliseconds >
                            SilenceTimeoutMs)
                        {
                            await Dispatcher.InvokeAsync(() =>
                            {
                                MicButton.IsChecked = false;
                            });

                            await StopRecordingAndTranscribeAsync();
                        }
                    }
                    catch
                    {
                    }
                },
                null,
                1000,
                1000);
        }
    }
}
