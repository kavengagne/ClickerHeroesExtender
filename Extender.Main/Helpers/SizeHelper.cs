using System.Drawing;


namespace Extender.Main.Helpers
{
    public static class SizeHelper
    {
        public static Rectangle GetGameAreaRectangle(Rectangle clientSize)
        {
            const double normalGameWindowRatio = 16 / 9d;

            double windowRatio = clientSize.Width / (double)clientSize.Height;
            if (windowRatio > normalGameWindowRatio)
            {
                var width = (int)(clientSize.Height * normalGameWindowRatio);
                clientSize = new Rectangle((clientSize.Width - width) / 2, 0, width, clientSize.Height);
            }
            else
            {
                var height = (int)(clientSize.Width / normalGameWindowRatio);
                clientSize = new Rectangle(0, (clientSize.Height - height) / 2, clientSize.Width, height);
            }
            return clientSize;
        }
    }
}
