using Business.Domain.Enums;
using System.Text.RegularExpressions;

namespace Business.Domain.Extensions
{
    public static class StringExtensions
    {
        public static bool IsValidEmail(this string email)
        {
            Regex expression = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            return expression.IsMatch(email);
        }

        public static bool IsValidURL(this string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
        }

        public static bool IsStrongPassword(this string password)
        {
            PasswordScoreEnum score = CheckStrength(password);

            switch (score)
            {
                case PasswordScoreEnum.Blank:
                case PasswordScoreEnum.VeryWeak:
                    return false;
                case PasswordScoreEnum.Weak:
                case PasswordScoreEnum.Medium:
                case PasswordScoreEnum.Strong:
                case PasswordScoreEnum.VeryStrong:
                    return true;
            }

            return false;
        }

        private static PasswordScoreEnum CheckStrength(string password)
        {
            int score = 0;

            if (password.Length < 1)
                return PasswordScoreEnum.Blank;
            if (password.Length < 4)
                return PasswordScoreEnum.VeryWeak;

            if (password.Length >= 8)
                score++;
            if (password.Length >= 12)
                score++;
            if (Regex.Match(password, @"/\d+/", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"/[a-z]/", RegexOptions.ECMAScript).Success &&
                Regex.Match(password, @"/[A-Z]/", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"/.[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]/", RegexOptions.ECMAScript).Success)
                score++;

            return (PasswordScoreEnum)score;
        }
    }
}
