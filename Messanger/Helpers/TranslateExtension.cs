using System.ComponentModel;
using Messanger.Services;

namespace Messanger.Helpers
{
    [ContentProperty(nameof(Key))]
    public class TranslateExtension : IMarkupExtension<BindingBase>
    {
        public string Key { get; set; }

        public BindingBase ProvideValue(IServiceProvider serviceProvider)
        {
            return new Binding
            {
                Mode = BindingMode.OneWay,
                Path = $"[{Key}]",
                Source = new TranslateSource()
            };
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }

    public class TranslateSource : INotifyPropertyChanged
    {
        public string this[string key] => LocalizationService.Get(key);

        public TranslateSource()
        {
            LocalizationService.LanguageChanged += () =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
