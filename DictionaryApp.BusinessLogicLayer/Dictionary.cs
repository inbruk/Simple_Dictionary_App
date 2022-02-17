using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// переобозначения используются для удобного именования представления одной и той же сущности на разных слоях
using DAL = DictionaryApp.DataAccessLayer;
using DTO = DictionaryApp.DataAccessLayer.DataTransferObjects;

namespace DictionaryApp.BusinessLogicLayer
{
    /// <summary>
    /// хранение и обработку одного словаря на уровне бизнес логики BLL, внутри использует классы из уровня доступа к данным DAL
    /// </summary>
    public class Dictionary
    {
        /// <summary>
        /// для хранения DAL представления словаря
        /// </summary>
        private DAL.Dictionary _dalDic;

        /// <summary>
        /// сырое представление данных словаря, используется не только внутри БЛ, но и для поставки данных визуальному контролу гриду в слое представления
        /// для ускорения поиска здесь можно было бы использовать Dictionary, но редактирование (при котором поиск) - редкая операция
        /// </summary>
        private List<DTO.DictionaryItem> _currData; 

        /// <summary>
        /// признак того что изменения в данных словаря ОЗУ должны быть перенесены в файл
        /// </summary>
        private Boolean _isChanged;

        /// <summary>
        /// признак того что изменения в данных словаря ОЗУ должны быть перенесены в файл
        /// </summary>
        public Boolean IsChanged
        {
            get
            {
                return _isChanged;
            }
        }

        /// <summary>
        /// Создает представление словаря на слое DAL, выполняет первичную загрузку данных из файла используя DAL
        /// </summary>
        public Dictionary(String name)
        {
            _dalDic = new DAL.Dictionary(name);
            _currData =  _dalDic.LoadAll();
            _isChanged = false;
        }

        /// <summary>
        /// возвращает случайную запись этого словаря
        /// </summary>
        public DTO.DictionaryItem GetRandomItem()
        {
            int itemsCount = _currData.Count;

            Random rnd = new Random();
            int randomIndex = rnd.Next(itemsCount);

            DTO.DictionaryItem randomItem = _currData[randomIndex];

            // делаем копию, чтобы обезопасить внутренее хранилище от внешних модификаций
            DTO.DictionaryItem res = new DTO.DictionaryItem()
            {
                Id = randomItem.Id,
                Eng = randomItem.Eng,
                Rus = randomItem.Rus
            };

            return res;
        }

        /// <summary>
        /// получить данные в сыром формате, для отображения в гриде редактора,
        /// для того чтобы полностью защитить данные от внешних действий, здесь можно создать копию возвращаемых данных
        /// </summary>
        public List<DTO.DictionaryItem> GetRawData()
        {
            // копирование для возврата копии данных, обеспечивает безопасность (инкапсуляцию) за счет небольшого ухудшения производительности
            List<DTO.DictionaryItem> res = _currData.Select
            (x =>
                new DTO.DictionaryItem()
                {
                    Id = x.Id,
                    Eng = x.Eng,
                    Rus = x.Rus
                }
            ).OrderBy(x => x.Eng).ToList();         

            // если размер словарей сильно вырастет, то лучше будет сделать пейджер по данным (так будет копироваться небольшая часть словаря)
            // но в качестве простого решения можно просто возвращять содержимое хранилища (это ухудшит безопасность, но увеличит производительность без дополнительного программирования)
            // List<DTO.DictionaryItem> res = _currData.OrderBy(x => x.Eng).ToList();

            return res;
        }

        /// <summary>
        /// добавляет запись в словарь только в ОЗУ, для переноса в файл нужно после всех операций изменения вызвать SubmitChanges()
        /// </summary>
        public void Add(DTO.DictionaryItem item)
        {
            _currData.Add(item);
            _isChanged = true;
        }

        /// <summary>
        /// обновляет запись в словаре только в ОЗУ, для переноса в файл нужно после всех операций изменения вызвать SubmitChanges()
        /// </summary>
        public void Update(DTO.DictionaryItem item)
        {
            DTO.DictionaryItem currItem = _currData.Single(x => x.Id == item.Id);
            currItem.Eng = item.Eng;
            currItem.Rus = item.Rus;
            _isChanged = true;
        }

        /// <summary>
        /// удаляе запись из словаря только в ОЗУ, для переноса в файл нужно после всех операций изменения вызвать SubmitChanges()
        /// </summary>
        public void Remove(DTO.DictionaryItem item)
        {
            DTO.DictionaryItem currItem = _currData.Single(x => x.Id == item.Id);
            _currData.Remove(currItem);
            _isChanged = true;
        }

        /// <summary>
        /// переносит данные из внутреннего хранилища в xml файл используя DAL, только если изменения были
        /// </summary>
        public void SubmitChanges()
        {
            if (_isChanged)
            {
                _dalDic.SaveAll(_currData);
                _isChanged = false;
            }
        }

        /// <summary>
        /// отменяет изменения внутреннего хранилища заново загружая данные из xml файла используя DAL, только если изменения были
        /// </summary>
        public void DiscardChanges()
        {
            if (_isChanged)
            {
                _currData = _dalDic.LoadAll();
                _isChanged = false;
            }
        }
    }
}
