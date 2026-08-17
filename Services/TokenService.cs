using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzonReturnsManager1.Services
{
    public class TokenService
    {
        private const string TokenFileName = "token.txt";
        private string _tokenPath;

        public TokenService()
        {
            // Токен ищем в папке приложения
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _tokenPath = Path.Combine(baseDir, TokenFileName);
            
            // Если не нашли, пробуем в директории проекта (для отладки)
            if (!File.Exists(_tokenPath))
            {
                var projectDir = Directory.GetParent(baseDir)?.Parent?.Parent?.FullName;
                if (!string.IsNullOrEmpty(projectDir))
                {
                    _tokenPath = Path.Combine(projectDir, TokenFileName);
                }
            }
        }

        public string GetToken()
        {
            if (!File.Exists(_tokenPath))
            {
                throw new FileNotFoundException(
                    $"Файл с токеном '{TokenFileName}' не найден.\n" +
                    $"Создайте файл '{TokenFileName}' в папке приложения и поместите туда токен авторизации.");
            }

            return File.ReadAllText(_tokenPath).Trim();
        }

        public bool TokenExists()
        {
            return File.Exists(_tokenPath);
        }
    }
}
