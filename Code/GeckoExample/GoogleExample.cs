#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gecko;

#endregion

#region Copyright

// Пример к уроку: 
// Автор: dredei

#endregion

namespace GeckoExample
{
    public class GoogleExample
    {
        private readonly GeckoWebBrowser _webBrowser; // ссылка на браузер
        private readonly Timer _loadingTimer; // таймер загрузки
        private bool _loading; // указывает на статус загрузки

        public GoogleExample( GeckoWebBrowser webBrowser )
        {
            this._webBrowser = webBrowser;
            this._webBrowser.CreateWindow2 += this._webBrowser_CreateWindow2;
            this._loadingTimer = new Timer { Interval = 2000 };
            this._loadingTimer.Tick += this._loadingTimer_Tick;
        }

        private async void _webBrowser_CreateWindow2( object sender, GeckoCreateWindow2EventArgs e )
        {
            e.Cancel = true;
            await this.Navigate( e.Uri );
        }

        private void _loadingTimer_Tick( object sender, EventArgs e )
        {
            this._loadingTimer.Stop();
            this._loading = false;
        }

        /// <summary>
        /// Ожидает загрузки страницы
        /// </summary>
        private async Task WaitForLoading()
        {
            while ( this._loading )
            {
                await TaskEx.Delay( 200 );
            }
        }

        /// <summary>
        /// Загружает указанную страницу и ждет завершения загрузки
        /// </summary>
        /// <param name="url">Url страницы</param>
        private async Task Navigate( string url )
        {
            this._loading = true;
            this._webBrowser.Navigate( url );
            this._loadingTimer.Start();
            await this.WaitForLoading();
        }

        /// <summary>
        /// Загружает страницу гугла и ждет загрузки
        /// </summary>
        public async Task OpenGoogle()
        {
            await this.Navigate( "http://www.google.ru/" );
            // если гугл выдает нам страницу не на русском и предлагает включить русский, то включаем
            var aElements = this._webBrowser.Document.GetElementsByTagName( "a" );
            this._loading = true;
            this._loadingTimer.Start();
            foreach ( var element in aElements )
            {
                if ( element.InnerHtml.IndexOf( "русском", StringComparison.Ordinal ) >= 0 )
                {
                    this._loading = true;
                    element.Click();
                    this._loadingTimer.Start();
                    await this.WaitForLoading();
                    break;
                }
            }
        }

        /// <summary>
        /// Пишет поисковый запрос
        /// </summary>
        /// <param name="query">Поисковый запрос</param>
        public async Task WriteQuery( string query )
        {
            // получаем все элементы с тегом input
            var inputsElements = this._webBrowser.Document.GetElementsByTagName( "input" );
            // получаем первый input, имя (атрибут "name") которого равно q
            var queryElement = inputsElements.First( i => i.GetAttribute( "name" ) == "q" );
            // "плавный" набор поискового запроса
            Random random = new Random();
            for ( int i = 0; i < query.Length; i++ )
            {
                queryElement.SetAttribute( "value", query.Substring( 0, i + 1 ) );
                await TaskEx.Delay( random.Next( 100, 350 ) );
            }
            // получаем все элементы с тегом button
            var buttonsElements = this._webBrowser.Document.GetElementsByTagName( "button" );
            // указываем, что началась загрузка, кликаем по первой кнопке с именем "btnG, запускаем таймер и ждем загрузки страницы
            this._loading = true;
            buttonsElements.First( i => i.GetAttribute( "name" ) == "btnG" ).Click();
            this._loadingTimer.Start();
            await this.WaitForLoading();
        }

        public async Task ClickRandomLink()
        {
            // получаем все элементы с тегом input
            var inputsElements = this._webBrowser.Document.GetElementsByTagName( "a" ).ToList();
            // получаем все ссылки, у которых значение атрибута "onmousedown" начинается с "return rwt("
            List<GeckoHtmlElement> needleLinks = new List<GeckoHtmlElement>();
            foreach ( var element in inputsElements )
            {
                try
                {
                    if ( element.GetAttribute( "onmousedown" ).StartsWith( "return rwt(" ) &&
                         element.GetAttribute( "href" )
                             .IndexOf( "webcache.googleusercontent.com", StringComparison.Ordinal ) < 0 )
                    {
                        needleLinks.Add( element );
                    }
                }
                catch
                {
                    continue;
                }
            }
            Random random = new Random();
            this._loading = true;
            // нажимаем по случайной ссылке
            needleLinks[ random.Next( needleLinks.Count ) ].Click();
            this._loadingTimer.Start();
            await this.WaitForLoading();
        }
    }
}