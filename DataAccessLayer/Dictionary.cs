using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using DTO = DictionaryApp.DataAccessLayer.DataTransferObjects;

namespace DictionaryApp.DataAccessLayer
{
    public class Dictionary
    {
        private String _dicName;

        public Dictionary(String dicName)
        {
            _dicName = dicName;
        }

        private String GetFullDictionaryPath()
        {
            String fullPath = CommonValues.DictionariesDirectoryPath + _dicName;
            return fullPath;
        }

        /// <summary>
        /// грузит все записи словаря из XML файла
        /// </summary>
        public List<DTO.DictionaryItem> LoadAll()
        {
            String fullPath = GetFullDictionaryPath();

            XDocument xDoc = XDocument.Load(fullPath);
            XElement rootElement = xDoc.Descendants("dictionary").Single();

            List<DTO.DictionaryItem> res = rootElement.Descendants("item").Select
            ( x=>
                new DTO.DictionaryItem()
                {
                    Id = Guid.Parse( x.Attribute("id").Value ),
                    Eng = x.Attribute("eng").Value,
                    Rus = x.Attribute("rus").Value
                }
            ).ToList();

            return res;
        }

        /// <summary>
        /// удаляет текущий файл словаря, записывает новый с тем же именем, содержащий записи пришедшие как параметр
        /// </summary>        
        public void SaveAll(List<DTO.DictionaryItem> items)
        {
            String fullPath = GetFullDictionaryPath();

            File.Delete(fullPath);

            // заполнение XDocument в стиле linq
            XDocument xDoc = new XDocument
            (
                new XDeclaration("1.0","utf-8","yes"),
                new XElement
                (
                    "dictionary",
                    items.Select
                    (
                        x => new XElement
                        (
                            "item",
                            new XAttribute("id",  x.Id.ToString() ),
                            new XAttribute("eng", x.Eng ),
                            new XAttribute("rus", x.Rus )
                        )
                    )
                )
            );

            xDoc.Save(fullPath);
        }
    }
}
