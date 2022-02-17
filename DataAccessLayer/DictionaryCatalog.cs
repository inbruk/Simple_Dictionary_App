using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DictionaryApp.DataAccessLayer
{
    /// <summary>
    /// класс отвечает за каталог словарей (все словари в системе)
    /// </summary>
    public static class DictionaryCatalog
    {        
        /// <summary>
        /// загружает список названий словарей (названия совпадают с именами xml файлов, содержащих данные словарей)
        /// </summary>
        /// <returns> список названий словарей </returns>
        public static List<String> GetNames()
        {
            // сначала проверим существует ли каталог, со словорями
            if (Directory.Exists(CommonValues.DictionariesDirectoryPath) == false)
            {
                throw new Exception("Невозможно работать со словарями, так как папка с ними не существует.");
            }

            String[] fileNamesStrArr = Directory.GetFiles(CommonValues.DictionariesDirectoryPath, "*.xml");
            if( fileNamesStrArr.Count()<1 )
            {
                throw new Exception("Невозможно работать со словарями, так как не найдено ни одного xml файла в папке со словарями.");
            }

            List<String> res = fileNamesStrArr.Select( x => x.Remove( 0, CommonValues.DictionariesDirectoryPath.Length ) ).ToList();            
            return res;
        }
    }
}
