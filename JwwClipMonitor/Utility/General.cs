namespace JwwClipMonitor.Utility
{
    internal static class General
    {
        /// <summary>
        /// int値を単純なキャストより安全にenum値にする。
        /// </summary>
        /// <typeparam name="T">enum型</typeparam>
        /// <param name="intValue">変換されるint値</param>
        /// <param name="defaultValue">enumに変換できなかった場合のデフォルト値</param>
        /// <returns>変換されたenum値</returns>
        public static T IntToEnum<T>(int intValue, T defaultValue) where T : Enum
        {
            return Enum.IsDefined(typeof(T), intValue) ? (T)Enum.ToObject(typeof(T), intValue) : defaultValue;
        }
    }
}
