namespace Business.Domain
{
    public class Throw
    {
        public static void IfIsNull(object value, string paramName = "", Exception? ex = null)
        {
            if (value == null)
                if (ex == null)
                    throw new ArgumentNullException(paramName);
                else
                    throw ex;
        }

        public static void IfEqZero(long value, string paramName = "", Exception? ex = null)
        {
            if (value == 0)
                if (ex == null)
                    throw new ArgumentOutOfRangeException(paramName);
                else
                    throw ex;
        }

        public static void IfLessThanZero(long value, string paramName = "", Exception? ex = null)
        {
            if (value < 0)
                if (ex == null)
                    throw new ArgumentOutOfRangeException(paramName);
                else
                    throw ex;
        }

        public static void IfLessThanOrEqZero(long value, string paramName = "", Exception? ex = null)
        {
            if (value <= 0)
                if (ex == null)
                    throw new ArgumentOutOfRangeException(paramName);
                else
                    throw ex;
        }

        public static void IfIsNullOrEmpty(string value, string paramName = "", Exception? ex = null)
        {
            if (string.IsNullOrEmpty(value))
                if (ex == null)
                    throw new ArgumentNullException(paramName);
                else
                    throw ex;
        }

        public static void IfIsNullOrWhiteSpace(string value, string paramName = "", Exception? ex = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                if (ex == null)
                    throw new ArgumentNullException(paramName);
                else
                    throw ex;
        }

        public static void IfStringLengthBiggerThan(string value, long length, string paramName = "", Exception? ex = null)
        {
            IfIsNullOrEmpty(value, paramName);

            if (value.Length > length)
                if (ex == null)
                    throw new ArgumentOutOfRangeException(paramName);
                else
                    throw ex;
        }

        public static void IfIsEmpty<T>(IEnumerable<T> value, string paramName = "", Exception? ex = null)
        {
            if (value == null || (value != null) && value.Count() == 0)
                if (ex == null)
                    throw new ArgumentOutOfRangeException(paramName);
                else
                    throw ex;
        }

        public static void IfIsFalse(bool value, string paramName = "", Exception? ex = null)
        {
            if (!value)
                if (ex == null)
                    throw new InvalidOperationException(paramName);
                else
                    throw ex;
        }

        public static void IfIsTrue(bool value, string paramName = "", Exception? ex = null)
        {
            if (value)
                if (ex == null)
                    throw new InvalidOperationException(paramName);
                else
                    throw ex;
        }
    }
}
