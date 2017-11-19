using WinApiWrapper.Interfaces;

namespace Extender.Main.Messages
{
    public class GameWindowChangedMessage
    {
        public GameWindowChangedMessage(IWinApiWindow gameWindow)
        {
            GameWindow = gameWindow;
        }

        public IWinApiWindow GameWindow { get; set; }
    }
}
