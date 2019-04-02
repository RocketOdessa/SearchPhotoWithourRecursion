using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*Директива using static применяется к каждому типу, у которого есть статические члены (или вложенные типы), 
 * даже если при этом у него также имеются члены экземпляров. При этом для вызова членов экземпляров можно использовать только экземпляр типа.*/
using static System.Console;
/*Программа показывает работу с массивами файлов и небольшими возможностями класса Bitmap(пространства System.Drawing) и System.IO,
 Методы классов (FileInfo,DirectoryInfo), которые реализуют их экземпляры применяют станадртный поиск (с ним можно ознакомится на сайте MSDN)
 Постарайтесь вводить конкретные директории типа C:Users\{User_Name}\Desktop
 (причина этого связана с ограничениями по поиску, если углубиться то можно узнать предел (приблизительно 1 гб, однако у меня десктоп 2,38гб,
 зависит от железа и некоторых особенностях компьютера),не пробуйте вводить целый диск , схватите остановку главного потока и переполнение стека)
 https://docs.microsoft.com/ru-ru/dotnet/api/system.io.fileinfo?view=netframework-4.7.2
 Следующая модификация этой программы будет с рекурсивным обходом*/
namespace ArrayForFirstCourse
{
    class Program
    {   
        static void Main(string[] args)
        {
            ConsoleColor color = ForegroundColor;
            ForegroundColor = ConsoleColor.Red;
            WriteLine("Read text in PROGRAM class");

            ForegroundColor = ConsoleColor.Gray;

            WriteLine("Hello man, you can search all pictures (format *.png,*.jpeg,*.jpg) in your directories");
            WriteLine("What do you want to do with files ?\n1. Just save pictures\n2. Save and rotate on 180 flip");
            WriteLine("Write 1 or 2");

            // В коде ниже мы реализуем стандартную проверку на корректность ввода(многие аспекты не учтены, можете дополнить её сами)
            string choose = ReadLine();
            // метод Parse схож с Convert.ToInt32(имеет тот же функционал)
            while (int.Parse(choose) != 2 || int.Parse(choose) != 1)
            {
                if (choose == "1" || choose == "2")
                {
                    switch (choose)
                    {
                        case "1":
                            WriteLine("Enter the path");
                            string path = ReadLine();
                            // Здесь создается экземпляр класса DirectoryInfo(позволяет перемещаться по каталогам и подкаталогам) с применением одного из конструкторов, передавай точный путь в виде параметра
                            DirectoryInfo directory = new DirectoryInfo($"{path}");
                            GetAllPhotoOnWorkTable(directory);
                            break;
                        case "2":
                            WriteLine("Enter the path");
                            string secongPath = ReadLine();
                            // Здесь создается экземпляр класса DirectoryInfo(позволяет перемещаться по каталогам и подкаталогам) с применением одного из конструкторов, передавай точный путь в виде параметра 
                            DirectoryInfo secondDirectory = new DirectoryInfo($"{secongPath}");
                            ProcessFile(secondDirectory);
                            break;
                    }
                    break;
                }
                WriteLine("Write correct number");
                choose = ReadLine();
            }         
        }

        #region Search and convert Photo(plus save in the new Directory)
        private static void ProcessFile(DirectoryInfo directory)
        {
            //Класс StopWatch служит для замера времени выполнения (на MSDN так же можно найти примері замеров ФПС с помоцью єтого класса)
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //Набор массивов которые хранят все полученые файлы опредленного формата
            string[] photoJPG = Directory.GetFiles(directory.ToString(), "*.jpg", SearchOption.AllDirectories);

            string[] photoPNG = Directory.GetFiles(directory.ToString(), "*.png", SearchOption.AllDirectories);

            string[] photoJpeg = Directory.GetFiles(directory.ToString(), "*.jpeg", SearchOption.AllDirectories);

            //Новая папка хранится в том месте, где вы осуществляли поиск
            string newDirectory = $@"{directory}\newPhoto";
            //Создания папки
            Directory.CreateDirectory(newDirectory);

            DirectoryInfo info = new DirectoryInfo(directory.ToString());

            //Создание массива FileInfo для хранения и получения информации об файлах заданного формата
            FileInfo[] imageFileJpg = info.GetFiles("*.jpg", SearchOption.AllDirectories);
            //Вывод кол-ва файлов
            WriteLine("Found {0} *.jpg\n",imageFileJpg.Length);
            //Вывод сведений о фото
            foreach (var fileJpg in imageFileJpg)
            {
                WriteLine("************************");
                WriteLine($"File name: {fileJpg.Name}");
                WriteLine($"File size: {fileJpg.Length}");
                WriteLine($"Creation: {fileJpg.CreationTime}");
                WriteLine("************************");
            }
            //Создание массива FileInfo для хранения и получения информации об файлах заданного формата
            FileInfo[] imageFilePng = info.GetFiles("*.png", SearchOption.AllDirectories);
            WriteLine("Found {0} *.png\n", imageFilePng.Length);
            foreach (var filePng in imageFilePng)
            {
                WriteLine("************************");
                WriteLine($"File name: {filePng.Name}");
                WriteLine($"File size: {filePng.Length}");
                WriteLine($"Creation: {filePng.CreationTime}");
                WriteLine("************************");
            }
            //Создание массива FileInfo для хранения и получения информации об файлах заданного формата
            FileInfo[] imageFileJpeg = info.GetFiles("*.jpeg", SearchOption.AllDirectories);
            WriteLine("Found {0} *.jpeg\n", imageFilePng.Length);
            foreach (var fileJpeg in imageFilePng)
            {
                WriteLine("************************");
                WriteLine($"File name: {fileJpeg.Name}");
                WriteLine($"File size: {fileJpeg.Length}");
                WriteLine($"Creation: {fileJpeg.CreationTime}");
                WriteLine("************************");
            }
            //Создания блоков try/catch для отлова возможных ошибок в RunTime
            try
            {             
                foreach (var jpg in photoJPG)
                {
                    // В строку вы получаете имя файла
                    string fileName = Path.GetFileName(jpg);
                    //Применение ключевого слова using нужно не только для подключения пространства имен, а так же для реализации очистки ресурсов тех типов(классов, интерфейсов) которые реализуют интерфейс IDisposable
                    using(Bitmap bitmap = new Bitmap(jpg))
                    {
                        // Реализация вращения фотографии, тут можете покрутить самим как угодно, все методы довольно просты в понимание
                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        // Сохранение файла в новой папке, путем объеденения двух путей Path.Combine(new Path, Old path)
                        bitmap.Save(Path.Combine(newDirectory, fileName));
                    }
                    WriteLine("Operation with jpg done!");
                }
                foreach (var png in photoPNG)
                {
                    string fileName = Path.GetFileName(png);
                    using (Bitmap bitmap = new Bitmap(png))
                    {
                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        bitmap.Save(Path.Combine(newDirectory, fileName));
                    }
                    WriteLine("Operation with png done!");
                }
                foreach (var jpeg in photoJpeg)
                {
                    string fileName = Path.GetFileName(jpeg);
                    using (Bitmap bitmap = new Bitmap(jpeg))
                    {
                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        bitmap.Save(Path.Combine(newDirectory, fileName));
                    }
                    WriteLine("Operation with jpeg done!");
                }
                WriteLine("All operation done!");
            }
            //Блоки catch иду сверху вниз , с отлова универсальных ошибок к более общим
            catch(OperationCanceledException ex)
            {
                WriteLine(ex.Message);
            }
            catch(FileNotFoundException fileEx)
            {
                WriteLine(fileEx.Message);
            }
            //Выключаем таймеп
            stopwatch.Stop();
            //С помощью структуры TimeSpan записываем время потраченное на ранТайм действий 
            TimeSpan ts = stopwatch.Elapsed;
            // Вывод в нужном нам формате
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            WriteLine("RunTime " + elapsedTime);            
        }
        #endregion

        #region Method for search all files in different directories
        private static void GetAllPhotoOnWorkTable(DirectoryInfo directory)
        {
            //Отличие этого метода от предидущего в записи ифнормации с потока данных
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string[] allPhotoJpg = Directory.GetFiles($@"{directory}","*.jpg",SearchOption.AllDirectories);

            string[] allPhotoPng = Directory.GetFiles($@"{directory}", "*.png", SearchOption.AllDirectories);

            string[] allPhotoJpeg = Directory.GetFiles($@"{directory}", "*.jpeg", SearchOption.AllDirectories);
            // Напомню, сохранение идет в папку,которая создается в деректории , которую вы проверяли
            string newDirectoryToFile = $@"{directory}\allPhoto";
            Directory.CreateDirectory(newDirectoryToFile);
            // Здесь мы создаем экземпляр типа (класса) StreamWriter для записи данных с потока в определенное кодировке
            // File.CreateText() метод который создает текстовый файл в который будет записываться вся полученная информация с потока 
            using (StreamWriter stream = File.CreateText($@"{directory}\info.txt"))
            {
                DirectoryInfo info = new DirectoryInfo($@"{directory}");
                //Напонмю что, создание массива FileInfo для хранения и получения информации об файлах заданного формата
                FileInfo[] filesInfoJpg = info.GetFiles("*.jpg", SearchOption.AllDirectories);
                stream.WriteLine($"Found {filesInfoJpg.Length} *.jpg files");

                FileInfo[] filesInfoPng = info.GetFiles("*.png", SearchOption.AllDirectories);
                stream.WriteLine($"Found {filesInfoPng.Length} *.png files");

                FileInfo[] filesInfoJpeg = info.GetFiles("*.jpeg", SearchOption.AllDirectories);
                stream.WriteLine($"Found {filesInfoJpeg.Length} *.jpeg files");

                stream. WriteLine("\\\\\\\\\\Information about *.jpg files//////////");
                ConsoleColor color = ForegroundColor;
                ForegroundColor = ConsoleColor.Red;

                foreach (var jpg in filesInfoJpg)
                {
                    stream.WriteLine("*************************");
                    stream.WriteLine($"Root field of image {jpg.Name}:{jpg.Directory}");
                    stream.WriteLine($"File size: {jpg.Length}");
                    stream.WriteLine($"File creation Time {jpg.CreationTime}");
                    stream.WriteLine("**************************");
                }

                ForegroundColor = ConsoleColor.Green;

                stream.WriteLine("\\\\\\\\\\Information about *.png files///////////");

                foreach (var png in filesInfoPng)
                {
                    stream.WriteLine("*************************");
                    stream.WriteLine($"Root field of image {png.Name}:{png.Directory}");
                    stream.WriteLine($"File size: {png.Length}");
                    stream.WriteLine($"File creation Time {png.CreationTime}");
                    stream.WriteLine("**************************");
                }

                ForegroundColor = ConsoleColor.Yellow;
                stream.WriteLine("\\\\\\\\\\Information about *.jpeg files/////////////");

                foreach (var jpeg in filesInfoJpeg)
                {
                    stream.WriteLine("*************************");
                    stream.WriteLine($"Root field of image {jpeg.Name}:{jpeg.Directory}");
                    stream.WriteLine($"File size: {jpeg.Length}");
                    stream.WriteLine($"File creation Time {jpeg.CreationTime}");
                    stream.WriteLine("**************************");
                }
            }
            try
            {
                foreach (var item1 in allPhotoJpeg)
                {
                    string newFileJpeg = Path.GetFileName(item1);
                    using (Bitmap bitmap = new Bitmap(item1))
                    {
                        bitmap.Save(Path.Combine(newDirectoryToFile, newFileJpeg));
                    }
                    WriteLine("Operation with jpeg done!");
                }

                foreach (var item2 in allPhotoJpg)
                {
                    string newFileJpg = Path.GetFileName(item2);
                    using (Bitmap bitmap = new Bitmap(item2))
                    {
                        bitmap.Save(Path.Combine(newDirectoryToFile, newFileJpg));
                    }
                    WriteLine("Operation with jpg done!");
                }

                foreach (var item3 in allPhotoPng)
                {
                    string newFilePng = Path.GetFileName(item3);
                    using (Bitmap bitmap = new Bitmap(item3))
                    {
                        bitmap.Save(Path.Combine(newDirectoryToFile, newFilePng));
                    }
                    WriteLine("Operation with png done!");
                }
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
            }

            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            WriteLine("RunTime " + elapsedTime);
        }
        #endregion

    }
}
