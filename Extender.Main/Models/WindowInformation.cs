namespace Extender.Main.Models
{
    public class WindowInformation
    {
        public string ClassName { get; set; }
        public string Title { get; set; }

        public WindowInformation(string title, string className)
        {
            Title = title;
            ClassName = className;
        }

        public override bool Equals(object obj)
        {
            var info = obj as WindowInformation;
            if (info == null)
            {
                return false;
            }
            return string.Equals(Title, info.Title) &&
                   string.Equals(ClassName, info.ClassName);
        }

        public override int GetHashCode()
        {
            return Title.GetHashCode() + ClassName.GetHashCode();
        }
    }
}