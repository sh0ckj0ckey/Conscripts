using System;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Conscripts.ViewModels
{
    public partial class TitleBarViewModel : ObservableObject
    {
        private static readonly Random _random = new();

        private CancellationTokenSource? _cancellationTokenSource;

        private string _animatedTitleText = string.Empty;

        public string AnimatedTitleText
        {
            get => _animatedTitleText;
            private set => SetProperty(ref _animatedTitleText, value);
        }

        public void Start()
        {
            if (_cancellationTokenSource is not null)
            {
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _ = RunAnimationLoopAsync(_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private async Task RunAnimationLoopAsync(CancellationToken cancellationToken)
        {
            const string title = "Conscripts";
            const string cursor = "_";

            string[] leadingBlinkSequence = [cursor, string.Empty, cursor, string.Empty, cursor];
            string[] trailingBlinkSequence = [title, title + cursor, title, title + cursor, title, title + cursor, title, title + cursor, title, title + cursor, title, title + cursor];

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    foreach (string text in leadingBlinkSequence)
                    {
                        AnimatedTitleText = text;
                        await Task.Delay(800, cancellationToken);
                    }

                    for (int i = 1; i <= title.Length; i++)
                    {
                        AnimatedTitleText = title[..i] + cursor;
                        await Task.Delay(_random.Next(95, 141), cancellationToken);
                    }

                    foreach (string text in trailingBlinkSequence)
                    {
                        AnimatedTitleText = text;
                        await Task.Delay(800, cancellationToken);
                    }

                    for (int i = title.Length - 1; i >= 0; i--)
                    {
                        AnimatedTitleText = title[..i] + cursor;
                        await Task.Delay(_random.Next(65, 96), cancellationToken);
                    }
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception ex) { System.Diagnostics.Trace.WriteLine($"TitleBar animation failed: {ex}"); }
        }
    }
}
