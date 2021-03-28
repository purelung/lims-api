namespace ZeeReportingApi
{
    public static class Constants
    {
        /// <summary>
        ///     You can put this secret key in your config instead or read it from Key Vault
        /// </summary>
        public static readonly string SECRET_KEY = "Kr4OZhTrEk43XYo5yudqV4BbTf9keRBa5qZwP1qPaMdqe21s8TTa4DI86OEe2";
        public static readonly string DB_CONN_STRING = "Server=tcp:midnight-data.database.windows.net,1433;Initial Catalog=ODS;Persist Security Info=False;User ID=midnightData;Password=KRCxb24jLgzat279TgtYKPf4q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30";
    }
}