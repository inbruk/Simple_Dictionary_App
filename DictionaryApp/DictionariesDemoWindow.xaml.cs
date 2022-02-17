using System;
using System.Threading;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using BL = DictionaryApp.BusinessLogicLayer;
using DTO = DictionaryApp.DataAccessLayer.DataTransferObjects;

namespace DictionaryApp.PresentationLayer
{
    /// <summary>
    /// Interaction logic for DictionariesDemoWindow.xaml
    /// </summary>
    public partial class DictionariesDemoWindow : Window
    {
        public delegate void ShowDemoValues();
        public delegate void HideDemoValues();

        public BL.DictionaryCatalog CurrentDictionaryCatalog { get; set; }

        private Thread _demoThread;

        public DictionariesDemoWindow()
        {
            InitializeComponent();

            CurrentDictionaryCatalog = null;
            _demoThread = null;

            WindowStartupLocation = WindowStartupLocation.Manual;
            Top = System.Windows.SystemParameters.PrimaryScreenHeight - Height - 100;
            Left = System.Windows.SystemParameters.PrimaryScreenWidth - Width - 50;
        }

        /// <summary>
        /// когда окно уже полностью отрисовано, запускаем задачу демонстрации словаря (если словери подключены)
        /// </summary>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (CurrentDictionaryCatalog != null)
            {
                _demoThread = new Thread(DemoThreadMethod);
                _demoThread.Start();
            }
        }

        /// <summary>
        /// перед закрытием окна, которое инициируется из основного окна, нужно обязательно остановить задачу демонстрации
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if( _demoThread!=null )
            {
                _demoThread.Abort();
            }

            base.OnClosing(e);
        }

        private void ShowThisWindowWithNewDictionariesValueHandler()
        {
            DTO.DictionaryItem currItem = CurrentDictionaryCatalog.GetRandomDictionaryItem();

            lblEngWord.Content = currItem.Eng;
            lblRusWord.Content = currItem.Rus;

            this.Show();
        }

        private void HideThisWindowHandler()
        {
            this.Hide();
        }

        /// <summary>
        /// это отдельная задача в которой происходит показ работающего отдельного окна, 
        /// </summary>
        private void DemoThreadMethod()
        {
            try
            {
                while(true)
                {
                    // нельзя просто вызвать ShowThisWindowWithNewDictionariesValueHandler(); 
                    // так как GUI не захочет работать не из своей задачи
                    // нужно его вызвать именно в задаче GUI, для этого используется this.Dispatcher.Invoke() и делегат

                    this.Dispatcher.Invoke(new ShowDemoValues( this.ShowThisWindowWithNewDictionariesValueHandler ) );
                    
                    Thread.Sleep(2000);

                    // нельзя просто вызвать HideThisWindowHandler(); 
                    // так как GUI не захочет работать не из своей задачи
                    // нужно его вызвать именно в задаче GUI, для этого используется this.Dispatcher.Invoke() и делегат

                    this.Dispatcher.Invoke(new HideDemoValues( this.HideThisWindowHandler ));

                    Thread.Sleep(2000);
                }
            }
            catch (ThreadAbortException abortException)
            {
                // prevent exceptions when Abort() invoked
            }
        }

    }
}
