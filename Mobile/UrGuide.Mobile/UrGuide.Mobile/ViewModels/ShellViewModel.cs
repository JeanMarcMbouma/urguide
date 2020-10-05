using FFImageLoading;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using UrGuide.Mobile.Services;

namespace UrGuide.Mobile.ViewModels
{
    class ShellViewModel : ReactiveObject
    {
        readonly ObservableAsPropertyHelper<bool> _isLoggedIn;
        readonly ObservableAsPropertyHelper<bool> _isGuide;
        public ShellViewModel(IPreferenceService preference)
        {
            Preference = preference ?? throw new ArgumentNullException(nameof(preference));

            _isLoggedIn = this.WhenAnyValue(x => x.Preference.UserId)
                .Select(x => !string.IsNullOrEmpty(x))
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.IsLoggedIn);

            _isGuide = this.WhenAnyValue(x => x.Preference.Role)
                .Select(x => "guide".Equals(x))
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.IsGuide);

            _isLoggedIn.ThrownExceptions.Subscribe(e =>
            {
            });

            _isGuide.ThrownExceptions.Subscribe(e =>
            {

            });
        }

        public IPreferenceService Preference { get; }
        public bool IsGuide => _isGuide.Value;
        public bool IsLoggedIn => _isLoggedIn.Value;
    }
}
