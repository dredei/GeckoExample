#region Using

using System;
using System.Windows.Forms;
using Gecko;

#endregion

#region Copyright

// Пример к статье: http://www.softez.pp.ua/2014/01/28/gecko-%d0%b8-csharp-geckofx/
// Автор: dredei

#endregion

namespace GeckoExample
{
    public partial class FrmMain : Form
    {
        private readonly GeckoWebBrowser _webBrowser;

        public FrmMain()
        {
            // инициализация Xulrunner
            Xpcom.Initialize( Application.StartupPath + "\\xulrunner\\" );
            this.InitializeComponent();
            this._webBrowser = new GeckoWebBrowser
            {
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom,
                Width = this.Width,
                Height = this.Height - this.pnl1.Height - 5,
                Top = this.pnl1.Bottom + 5
            };
            // после загрузки страницы будем менять значение tbUrl, в котором находится текущий урл
            this._webBrowser.DocumentCompleted += this._webBrowser_DocumentCompleted;
            // устанавливаем UserAgent браузера в Firefox 22
            GeckoPreferences.User[ "general.useragent.override" ] =
                "Mozilla/5.0 (Windows NT 6.1; rv:22.0) Gecko/20130405 Firefox/22.0";
            // добавляем контрол браузера на форму
            this.Controls.Add( this._webBrowser );
        }

        private void _webBrowser_DocumentCompleted( object sender, EventArgs e )
        {
            this.tbUrl.Text = this._webBrowser.Url.ToString();
        }

        private async void btnStart_Click( object sender, EventArgs e )
        {
            var testClass = new GoogleExample( this._webBrowser );
            await testClass.OpenGoogle();
            await testClass.WriteQuery( "создание приложений с поддержкой плагинов c#" );
            await testClass.ClickRandomLink();
        }
    }
}