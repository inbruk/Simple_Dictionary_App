using System;
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
using System.ComponentModel;

using BL = DictionaryApp.BusinessLogicLayer;
using DTO = DictionaryApp.DataAccessLayer.DataTransferObjects;

namespace DictionaryApp.PresentationLayer
{
    /// <summary>
    /// Interaction logic for DictionaryEditorWindow.xaml
    /// </summary>
    public partial class DictionaryEditorWindow : Window
    {
        public BL.Dictionary _currentDictionary;
        public BL.Dictionary CurrentDictionary 
        { 
            set
            {
                _currentDictionary = value;
            }
        }

        public DictionaryEditorWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// когда окно уже полностью отрисовано, можно все проинициализировать и заполнить
        /// к этому времени _currentDictionary уже должно быть установлено
        /// </summary>
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            RefreshGridAndButtons();
        }

        private void RefreshGridAndButtons()
        {
            List<DTO.DictionaryItem> rawData = _currentDictionary.GetRawData();
            dgrDictionaryItems.ItemsSource = rawData;

            RefreshButtons();
        }

        private void RefreshButtons()
        {
            if( _currentDictionary.IsChanged )
            {
                btnDiscardChanges.IsEnabled = true;
                btnSaveChangesAndExit.IsEnabled = true;
            }
            else
            {
                btnDiscardChanges.IsEnabled = false;
                btnSaveChangesAndExit.IsEnabled = false;
            }
        }

        private void btnAddDicItem_Click(object sender, RoutedEventArgs e)
        {
            DTO.DictionaryItem currItem = new DTO.DictionaryItem()
            {
                Id  = Guid.NewGuid(),
                Rus = "_новое значение",
                Eng = "_new value"
            };

            // добавим запись в хранилище на уровне БЛ
            _currentDictionary.Add(currItem);

            // обновим грид и кнопки
            RefreshGridAndButtons();
        }
        private void btnDeleteIndgrDictionaryItems_Click(object sender, RoutedEventArgs e)
        {
            // добудем из привязанных к гриду данных запись словаря
            Button currButton = (Button)sender;
            DTO.DictionaryItem currItem = (DTO.DictionaryItem)currButton.DataContext;

            // удалим запись из хранилища на уровне БЛ
            _currentDictionary.Remove(currItem);

            // обновим грид и кнопки
            RefreshGridAndButtons();
        }
        private void btnDiscardChanges_Click(object sender, RoutedEventArgs e)
        {
            _currentDictionary.DiscardChanges();
            RefreshGridAndButtons();
        }
        private void btnSaveChangesAndExit_Click(object sender, RoutedEventArgs e)
        {
            _currentDictionary.SubmitChanges();
            RefreshGridAndButtons();

            this.Close();
        }

        /// <summary>
        /// обработчик события возникающего во время изменения окончания редактирования ячейки таблицы данных
        /// переносит новые данные в БЛ, а потом обновляет грид
        /// </summary>
        protected void dgrDictionaryItems_CellEndEdit(object sender, DataGridCellEditEndingEventArgs cellEndEditEventArgs)
        {
            if( cellEndEditEventArgs.EditAction == DataGridEditAction.Commit )
            {
                // добудем новое введенное значение
                TextBox tb = (TextBox)cellEndEditEventArgs.EditingElement;
                String newValue = tb.Text;

                // добудем имя изменяемой колонки
                String columnName = cellEndEditEventArgs.Column.SortMemberPath;
                
                // добудем обновляемую запись
                DTO.DictionaryItem currItem = (DTO.DictionaryItem)cellEndEditEventArgs.Row.DataContext;

                // изменим соответствующее поле записи
                if (columnName == "Eng") currItem.Eng = newValue;
                if (columnName == "Rus") currItem.Rus = newValue;

                // сделаем обновление в БЛ
                _currentDictionary.Update(currItem);

                // обновим визуальные контролы
                RefreshGridAndButtons();
            }
        }

        /// <summary>
        /// перед закрытием окна нужно отменить изменения в словарях, если они были
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            if ( _currentDictionary.IsChanged )
            {
                _currentDictionary.DiscardChanges();
            }

            base.OnClosing(e);
        }

    }
}
