using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace WordQuizClasses
{
    public class wordquiz
    {
        public static string Word;
        public static int ScoreT;
        public static int ScoreF;
        public static int PositionOfLetterToRemove;
        public static char RemovedLetter;
        public static string WordWithoutLetter;
        public static char UserLetter;

        public wordquiz(string word, int scoreT, int scoreF, int positionOfLetterToRemove, char removedLetter, string wordWithoutLetter, char userLetter)
        {
            Word = word;
            ScoreT = scoreT;
            ScoreF = scoreF;
            PositionOfLetterToRemove = positionOfLetterToRemove;
            RemovedLetter = removedLetter;
            WordWithoutLetter = wordWithoutLetter;
            UserLetter = userLetter;
        }
 
       public static string WordQuiz1()
        {
             
            var rnd = new Random((int)DateTime.Now.Ticks); // инициализируем генератор случайных чисел            
            var dictionary = new List<string>(); // создаем словарь в памяти            
            using (var dictionaryReader = new StreamReader(@"D:\\Words.txt")) // открыть файл словаря
            {

                while ((Word = dictionaryReader.ReadLine()) != null)
                {
                    dictionary.Add(Word);
                }
            }           
                     
            ScoreT = 0;// переменная для сохранения счета 
            ScoreF = 0;// переменная для сохранения счета           
            Word = dictionary.ElementAt(rnd.Next(dictionary.Count));// выбираем случайное слово из словаря                
            PositionOfLetterToRemove = rnd.Next(Word.Length);// выбираем случайную позицию буквы для удаления                
            RemovedLetter = Word.ToLower()[PositionOfLetterToRemove];// получаем удаляемую букву без учета регистра                
            WordWithoutLetter = Word.Remove(PositionOfLetterToRemove, 1);// получаем слово без удаленной буквы
            return WordWithoutLetter;            
       }     
        }
    }

