using System.Runtime.InteropServices;
using System.Text;

namespace ProjectForTesting
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /// Метод игры 4 на 4
            Console.WriteLine("Игра 4 на 4");

            string baseDir = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            string username = "";
            string password = "";

            string usernamesPath = Path.Combine(baseDir, "username.txt");
            List<String> usernames = Read(usernamesPath);

            string passwordPath = Path.Combine(baseDir, "login.txt");
            List<String> passwords = Read(passwordPath);

            int index = -1;
            while (true)
            {
                username = GetInfo("Введите username:");
                password = GetInfo("Введите пароль:");

                index = isUser(usernames, username);

                if ((index > -1) && passwords[index] == password) break;
                else
                {
                    Console.WriteLine("Информация введена неверно. Попробуйте ещё раз");
                }
            }

            bool isActive = true;
            int stepCounter = 0;
            Random rnd = new Random();
            string guessedNumber = "";
            while (guessedNumber.Length < 4)
            {
                string elem = GenerateNumberToString(0, 10, rnd);
                if (!guessedNumber.Contains(elem))
                {
                    guessedNumber += elem;
                }

            }
            
            Console.WriteLine($"Число: {guessedNumber}");

            while (isActive)
            {
                string current = ReadFourDigitNumFromKeyboard("Введите число: ");

                isActive = CheckCorrect(guessedNumber, current, out int amountOfNumIncluded, out int positionAmount);
                Console.WriteLine();
                stepCounter++;

            }
            
            string combinedPath = Path.Combine(baseDir, "results.txt");

            WriteResult(stepCounter, username, combinedPath);

            Dictionary<string, List<int>> Results = new Dictionary<string, List<int>>();
            SortedSet<int> nums = new SortedSet<int>();
            ReadResult(combinedPath, out Results, out nums);

            Console.WriteLine($"Вы решили задачу за {stepCounter} шагов и находитесь в топ {MakeTop(stepCounter, nums.ToList() )}");
            Console.WriteLine($"Ваш лучший результат: {Results[username].Min().ToString()}");
        }

        static int isUser(List<String> elems, string elem)
        {
            /// вывод позиции пользователя в списке
            return elems.IndexOf(elem);
        }

        static string GetInfo(string text)
        {
            /// Получение информации от пользователя
            /// Считыванием данных из консоли
            Console.Write(text + " ");
            return Console.ReadLine();
        }

        static string GenerateNumberToString(int left, int right, Random rnd)
        {
            /// Генерация рандомных чисел
            return rnd.Next(left, right).ToString();
        }

        static int MakeTop(int num, List<int> results)
        {
            /// Функция составляет топ
            /// определяющий текущее положение
            int current_pos = results.IndexOf(num);

            if (current_pos <= 5)
            {
                return 5;
            }

            int secondDigit = (current_pos % 100) / 10;

            if (secondDigit < 5)
            {
                return (secondDigit + 1) * 10;
            }
            else
            {
                return 100;
            }
        }

        static List<String> Read(string filename)
        {
            /// Считывание из файла чисел
            /// из массива номеров
            List<String> elems = new List<String>();
            try
            {
                using (FileStream fstream = File.OpenRead(filename))
                {
                    byte[] buffer = new byte[fstream.Length];
                    fstream.Read(buffer, 0, buffer.Length);

                    string textFromFile = Encoding.Default.GetString(buffer);

                    elems.Add(textFromFile);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            return elems;
        }


        static int ReadResult(string compinedPath, out Dictionary<string, List<int>> userResults, out SortedSet<int> nums)
        {
            /// Считывание из файла результатов
            /// И дальнейшая сортировка
            userResults = new Dictionary<string, List<int>>();
            nums = new SortedSet<int>();
            using (FileStream fstream = File.OpenRead(compinedPath))
            {
                byte[] buffer = new byte[fstream.Length];
                fstream.Read(buffer, 0, buffer.Length);
                
                string textFromFile = Encoding.Default.GetString(buffer);

                string[] elems = textFromFile.Split("\n");

                for (int i =0; i < elems.Length; i++)
                {
                    string[] raw = elems[i].Split("|");

                    if (!userResults.ContainsKey(raw[0]))
                    {
                        userResults[raw[0]] = new List<int>() { Convert.ToInt32(raw[1]) };
                    }
                    else
                    {
                        userResults[raw[0]].Add(Convert.ToInt32(raw[1]));
                    }

                    nums.Add(Convert.ToInt32(raw[1]));
                }
            }
            return 0;
        }

        static bool WriteResult(int stepCounter, string login, string combinedPath)
        {
            /// Запись результата игры, привязанного к пользователю
            try
            {
                using (FileStream fstream = new FileStream(combinedPath, FileMode.Append))
                {
                    string baseString ="\n" + login + "|" + stepCounter.ToString();
                    byte[] input = Encoding.Default.GetBytes(baseString);
                    fstream.Write(input, 0, input.Length);
                }
            }

            catch (FileNotFoundException e)
            {
                return false;
            }

            return true;
        }

        static string ReadFourDigitNumFromKeyboard(string stroka)
        {
            /// Считывание числа из 4 чисел
            Console.Write(stroka);
            string guess = "";
            try
            {
                guess = Console.ReadLine();
                if (guess.Length != 4)
                {
                    throw new Exception("Ошибка ввода");
                }
                Convert.ToInt32(guess);
            }
            catch (Exception e)
            {
                guess = ReadFourDigitNumFromKeyboard("Вы ввели неверное число.\nВведите число: ");
            }
            return guess;
        }

        static bool CheckCorrect(string baseNum, string numForCheck, out int amountOfNumIncluded, out int positionAmount)
        {
            /// Проверка введённого числа
            /// С отгадываемым
            amountOfNumIncluded = 0;
            positionAmount = 0;

            for (int i = 0; i < baseNum.Length; ++i)
            {
                if (numForCheck.Contains(baseNum[i]))
                {
                    amountOfNumIncluded += 1;
                    if (baseNum[i] == numForCheck[i])
                    {
                        positionAmount += 1;
                    }
                }
            }
            Console.WriteLine($"Вы угадали {amountOfNumIncluded} номеров и {positionAmount} позиции");
            return !(baseNum == numForCheck);
        }

    }
}
