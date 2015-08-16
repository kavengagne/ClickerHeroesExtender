using System.Windows.Input;
using Extender.Main.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using WinApiWrapper.Wrappers;


namespace Extender.Main.ViewModels
{
    public class BonusOverlayViewModel : ViewModelBase
    {
        private readonly ExtenderSettings _settings;
        private ICommand _addBonusItemCommand;


        public BonusOverlayViewModel(ExtenderSettings settings)
        {
            _settings = settings;
        }


        public BonusItemsObservableCollection BonusItems => _settings.BonusItemsObservableCollection;

        public ICommand AddBonusItemCommand
            => _addBonusItemCommand ?? (_addBonusItemCommand = new RelayCommand(AddBonusItem));

        private void AddBonusItem()
        {

        }
    }
}