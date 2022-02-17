using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using WordQuizClasses;

using BL = DictionaryApp.BusinessLogicLayer;
using DTO = DictionaryApp.DataAccessLayer.DataTransferObjects;

namespace DictionaryApp.PresentationLayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BL.DictionaryCatalog _dicCat;
        private DictionariesDemoWindow _currDicDemoWindow;
        private DictionaryEditorWindow _currDicEditorWindow;
        public MainWindow()
        {
            InitializeComponent();

            _dicCat = new BL.DictionaryCatalog();

            RefreshMyDictionariesTabGrid();
            RefreshMyDictionariesTabButtons();
        }
        
        // ----------------------------------- My Dictionaries Tab--------------------------------\\
        private void RefreshMyDictionariesTabGrid()
        {
            List<BL.DictionaryCatalogItem> data2Bind = _dicCat.GetRawData();
            dgrDictionaries.ItemsSource = data2Bind;
        }
        private void RefreshMyDictionariesTabButtons()
        {
            btnStartDicDemo.IsEnabled = _dicCat.IsAnyDictionarySelected();
            btnStopDicDemo.IsEnabled = false;
        }
        
        private void OnDictionaryChecked(object sender, RoutedEventArgs e)
        {
            DataGridCell cell = (DataGridCell)sender;
            BL.DictionaryCatalogItem currItem = (BL.DictionaryCatalogItem)cell.DataContext;
            _dicCat.SelectDictionary(currItem);
            RefreshMyDictionariesTabButtons();
        }

        private void OnDictionaryUnchecked(object sender, RoutedEventArgs e)
        {
            DataGridCell cell = (DataGridCell)sender;
            BL.DictionaryCatalogItem currItem = (BL.DictionaryCatalogItem)cell.DataContext;
            _dicCat.UnselectDictionary(currItem);
            RefreshMyDictionariesTabButtons();
        }

        private void btnStartDicDemo_Click(object sender, RoutedEventArgs e)
        {
            dgrDictionaries.IsEnabled = false;
            btnStartDicDemo.IsEnabled = false;
            btnStopDicDemo.IsEnabled  = true;

            this.WindowState = WindowState.Minimized;

            _currDicDemoWindow = new DictionariesDemoWindow();
            _currDicDemoWindow.CurrentDictionaryCatalog = _dicCat;
            _currDicDemoWindow.Show();
        }

        private void CloseDicDemoWindow()
        {
            _currDicDemoWindow.Close();
            _currDicDemoWindow = null;
        }

        private void btnStopDicDemo_Click(object sender, RoutedEventArgs e)
        {
            CloseDicDemoWindow();

            dgrDictionaries.IsEnabled = true;
            RefreshMyDictionariesTabButtons();
        }

        /// <summary>
        /// нужно не забыть закрыть дополнительное окно, при закрытии основного !
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (_currDicDemoWindow!=null)
            {
                CloseDicDemoWindow();
            }

            base.OnClosing(e);
        }

        private void btnEditIndgrDictionaries_Click(object sender, RoutedEventArgs e)
        {
            if( _currDicEditorWindow==null )
            {      
                // добудем из привязанных к гриду данных словарь
                Button currButton = (Button) sender;
                BL.DictionaryCatalogItem currItem = (BL.DictionaryCatalogItem) currButton.DataContext;
                BL.Dictionary currDic = currItem.Dic;
                
                // создадим окно и правильно его проинициализируем
                _currDicEditorWindow = new DictionaryEditorWindow();
                _currDicEditorWindow.CurrentDictionary = currDic;

                // навесим обработчик для правильного уничтожения окна редактора 
                // обработчик сработает, когда дочернее окно закроет пользователь
                _currDicEditorWindow.Closed += new EventHandler(DicEditorWindowClosedHandler);

                // запрещаем запуск окна демонстрации
                btnStartDicDemo.IsEnabled = false;
            }
            if( _currDicEditorWindow.IsVisible ) // если окно редактора уже открыто, то новое не открываем
            {
                MessageBoxResult result = MessageBox.Show("Прежде чем начинать новое редактирование словаря нужно завершить предыдущее (закрыть окно редактора).");
            }
            else // если окно не открыто
            {                
                _currDicEditorWindow.Show();
            }
        }

        /// <summary>
        /// при закрытии окна редактирования пользователем необходимо удалить ссылку на его прокси объект в программе, чтобы не нарушать ее логику
        /// то есть закрытие окна приводит к удалению ссылки на него в родительском окне
        /// дело в том что узнать закрыто ли окно сложнее чем обработать в родительсом его закрытие
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DicEditorWindowClosedHandler(object sender, EventArgs e)
        {
            _currDicEditorWindow = null;
            RefreshMyDictionariesTabButtons(); // разрешаем запуск окна демонстрации
        }

        //------------------------------Обработчик меню-------------------------------\\

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            // Створюємо діалогове вікно та налагоджуємо його властивості:
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory; // поточна тека
            dlg.DefaultExt = ".xml";
            dlg.Filter = "Файли XML (*.xml)|*.xml|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                try
                {
                   // compilation.ReadSongs(dlg.FileName);
                }
                catch (Exception)
                {
                    MessageBox.Show("Помилка читання з файлу");
                }
                //InitGrid();
            }
        }

        //------------------------------\Обработчик меню\-------------------------------\\


        //------------------------------Обработчик wordquiz-------------------------------\\


        private void word_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void user_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void start_button_Click(object sender, RoutedEventArgs e)
        {

            word_textbox.Text = wordquiz.WordQuiz1();           
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            wordquiz.UserLetter = Convert.ToChar(user_textbox.Text);
            if (wordquiz.UserLetter != wordquiz.RemovedLetter)
            {
                count_labelF.Content = wordquiz.ScoreF++;
                MessageBox.Show("Не правильно. Отсутствующая буква - " + wordquiz.RemovedLetter);                 
                
            }
            else
            {
                count_labelT.Content = wordquiz.ScoreF++;
                MessageBox.Show("Правильно!");
                word_textbox.Text = wordquiz.WordQuiz1(); 
                
            }          
        }

        private void stop_button_Click(object sender, RoutedEventArgs e)
        {
            wordquiz.ScoreF = 0;
            wordquiz.ScoreT = 0;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

       

        //------------------------------\Обработчик wordquiz\-------------------------------\\
    }
}
