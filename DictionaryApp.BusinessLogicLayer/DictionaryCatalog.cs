using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// переобозначения используются для удобного именования представления одной и той же сущности на разных слоях
using DAL = DictionaryApp.DataAccessLayer;
using DTO = DictionaryApp.DataAccessLayer.DataTransferObjects;
using BL = DictionaryApp.BusinessLogicLayer;

namespace DictionaryApp.BusinessLogicLayer
{
    /// <summary>
    /// реализует работу с каталогом словарей (набор всех словарей в системе + минимум общих операций)
    /// </summary>
    public class DictionaryCatalog
    {
        private List<DictionaryCatalogItem> _directoryItems;
        
        /// <summary>
        /// узнаем список xml файлов с содержимым словарей и грузим их, заполняя внутренее хранилище
        /// </summary>
        public DictionaryCatalog()
        {
            List<String> listOfNames = DAL.DictionaryCatalog.GetNames();
            if( listOfNames.Count<1 )
            {
                throw new Exception("В каталоге со словарями не найдено xml файлов. Для нормальной работы программы требуется хотя бы один.");
            }

            _directoryItems = listOfNames.Select
            (
                x => new DictionaryCatalogItem()
                {
                    Id = Guid.NewGuid(),
                    Name = x,
                    Dic = new BL.Dictionary(x),
                    IsSelected = true
                }              
            ).ToList();
        }

        /// <summary>
        /// получить данные в сыром формате, для отображения в гриде - списке словарей с чекбоксами
        /// возвращение безопасно, так как происходит копирование данных из внутреннего хранилища
        /// предполагается, что количество словарей не очень большое
        /// </summary>
        public List<DictionaryCatalogItem> GetRawData()
        {
            List<DictionaryCatalogItem> res = _directoryItems.Select
            ( x =>
                new DictionaryCatalogItem()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Dic = x.Dic,
                    IsSelected = x.IsSelected
                }
            ).ToList();         

            return res;
        }

        /// <summary>
        /// выбирает на уровне БЛ словарь для демонстрации, соответствует включению чекбокса в presentation layer
        /// </summary>
        public void SelectDictionary(DictionaryCatalogItem dicItem)
        {
            DictionaryCatalogItem currDicItem = _directoryItems.Single( x=> x.Id == dicItem.Id );
            if( currDicItem==null )
            {
                throw new Exception("Попытка выбора словаря (для демонстрации), не существующего в каталоге.");
            }
            currDicItem.IsSelected = true;
        }

        /// <summary>
        /// снимает выбор со словаря для демонстрации на уровне БЛ, соответствует выключению чекбокса в presentation layer
        /// </summary>
        public void UnselectDictionary(DictionaryCatalogItem dicItem)
        {
            DictionaryCatalogItem currDicItem = _directoryItems.Single(x => x.Id == dicItem.Id);
            if (currDicItem == null)
            {
                throw new Exception("Попытка снятия выбора со словаря (для демонстрации), не существующего в каталоге.");
            }
            currDicItem.IsSelected = false;
        }

        /// <summary>
        /// проверяет выбран ли хоть один словарь, используется в слое презентации (PL)
        /// </summary>
        public Boolean IsAnyDictionarySelected()
        {
            Boolean res = _directoryItems.Any(x => x.IsSelected == true);
            return res;
        }

        /// <summary>
        /// возвращает случайную запись, случайного словаря (только из выбранных словарей)
        /// </summary>
        /// <returns></returns>
        public DTO.DictionaryItem GetRandomDictionaryItem()
        {
            // сначала получим список выбранных словарей
            List<DictionaryCatalogItem> selectedDirectoryItems = _directoryItems.Where(x => x.IsSelected == true).ToList();            
            if (selectedDirectoryItems.Count < 1)
            {
                throw new Exception("Попытка получения случайного значения при отсутствии выбранных словарей");
            }

            // найдем случайный индекс выбранного словаря
            int itemsCount = selectedDirectoryItems.Count;
            Random rnd = new Random();
            int randomIndex = rnd.Next(itemsCount);

            // получим случайно выбранный словарь
            BL.Dictionary randomDictionary = selectedDirectoryItems[randomIndex].Dic;

            // получим случайный элемент случайного словаря
            DTO.DictionaryItem randomItem = randomDictionary.GetRandomItem();

            return randomItem;
        }
    }
}
