using System.Web.UI;


namespace Gettext.Cs.Web
{
    [ParseChildren(false)]
    public abstract class AspTranslate : Control
    {
        string content;

        protected override void AddParsedSubObject(object obj)
        {
            if (obj is LiteralControl)
            {
                content = Translate(((LiteralControl)obj).Text);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write(content);
        }

        protected abstract string Translate(string text);
    }
}
