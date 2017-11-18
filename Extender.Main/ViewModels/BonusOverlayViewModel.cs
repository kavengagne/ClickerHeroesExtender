using Extender.Main.Models;
using GalaSoft.MvvmLight;


namespace Extender.Main.ViewModels
{
    public class BonusOverlayViewModel : ViewModelBase
    {
        private readonly ExtenderSettings _settings;


        public BonusOverlayViewModel(ExtenderSettings settings)
        {
            _settings = settings;
        }


        public BonusItem AttackLocation => _settings.AttackLocation;
        
        public BonusItemsObservableCollection BonusItems => _settings.BonusItemsObservableCollection;
    }
}
