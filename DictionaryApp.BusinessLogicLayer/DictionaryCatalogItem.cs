using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// переобозначения используются для удобного именования представления одной и той же сущности на разных слоях
using BL = DictionaryApp.BusinessLogicLayer;

namespace DictionaryApp.BusinessLogicLayer
{
    /// <summary>
    /// DTO или POC объект для представления одной записи каталога словарей, используется на уровне BL и PL 
    /// для уровня представления нужны POC c открытыми свойствами для отображения в гриде словарей,
    /// /// при этом все операции делаются защищенно через объект каталог словарей
    /// </summary>
    public class DictionaryCatalogItem
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public BL.Dictionary Dic { get; set; }
        public Boolean IsSelected { get; set; }
    }
}
