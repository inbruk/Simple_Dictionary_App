using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryApp.DataAccessLayer.DataTransferObjects
{
    /// <summary>
    /// DTO или POC объект для представления одной записи словаря, используется на всех слоях, на уровне BL создавать отдельный объект не стал, 
    /// так как для уровня представления все равно нужны POC c открытыми свойствами для отображения в гриде редактирования,
    /// при этом все операции делаются защищенно через объект словаря
    /// </summary>
    public class DictionaryItem
    {
        public Guid Id { set; get; }
        public String Eng { set; get; }
        public String Rus { set; get; }
    }
}
