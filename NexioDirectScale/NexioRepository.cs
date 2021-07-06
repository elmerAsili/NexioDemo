using Dapper;
using DirectScale.Disco.Extension.Services;
using Nexio.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;

namespace Nexio
{
    public interface INexioRepository
    {
        HttpStatusCode CreateNexioTables();
        NexioSettings GetNexioSettings();
        HttpStatusCode UpdateNexioSettings(NexioSettings settings);
        List<PendingOrder> GetPendingOrders(DateTime startDate, DateTime endDate);
    }

    public class NexioRepository : INexioRepository
    {
        private readonly IDataService _dataService;

        public NexioRepository(IDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }

        public HttpStatusCode CreateNexioTables()
        {
            CreateNexioSettingsTable();

            return HttpStatusCode.OK;
        }

        public NexioSettings GetNexioSettings()
        {
            using (var dbConnection = new SqlConnection(_dataService.ClientConnectionString.ConnectionString))
            {
                var query = $"SELECT * FROM Client.Nexio_Settings";
                var res = dbConnection.QueryFirstOrDefault<NexioSettings>(query);

                return res;
            }
        }

        public List<PendingOrder> GetPendingOrders(DateTime startDate, DateTime endDate)
        {
            using (var dbConnection = new SqlConnection(_dataService.ClientConnectionString.ConnectionString))
            {
                var query = $@"select DISTINCT o.recordnumber AS OrderId, p.recordnumber AS PaymentId, p.TransactionNumber from ORD_Order o
                OUTER APPLY (
	                SELECT TOP 1 * FROM ORD_Payments p 
	                WHERE o.recordnumber = p.OrderNumber
	                ORDER BY p.recordnumber DESC
                ) AS p
                WHERE p.recordnumber IS NOT NULL
                AND (p.PaymentStatus = 'Pending' OR p.PaymentStatus = 'PendingFraudReview') 
                AND p.Merchant in (9902, 9903)
                AND o.Void = 0
                AND (p.PaymentResponse LIKE '0: Pending' OR p.PaymentResponse LIKE 'F:%')
                AND CONVERT(date, p.last_modified) > '@startDate'
                AND CONVERT(date, p.last_modified) < '@endDate'";

                return dbConnection.Query<PendingOrder>(query).ToList();
            }
        }

        public HttpStatusCode UpdateNexioSettings(NexioSettings settings)
        {
            using (var dbConnection = new SqlConnection(_dataService.ClientConnectionString.ConnectionString))
            {
                var query = @"MERGE INTO Client.Nexio_Settings WITH (HOLDLOCK) AS TARGET 
                    USING (
                        SELECT @recordnumber AS 'recordnumber', @last_modified AS 'last_modified', @BaseApiUrl AS 'BaseApiUrl', @Username AS 'Username', @Password AS 'Password'
                    ) AS SOURCE 
                        ON SOURCE.recordnumber = TARGET.recordnumber
                    WHEN MATCHED THEN 
                        UPDATE SET TARGET.last_modified = SOURCE.last_modified, TARGET.BaseApiUrl = SOURCE.BaseApiUrl, TARGET.Username = SOURCE.Username, TARGET.Password = SOURCE.Password
                    WHEN NOT MATCHED BY TARGET THEN 
                        INSERT (BaseApiUrl, Username, Password) 
                        VALUES (SOURCE.BaseApiUrl, SOURCE.Username, SOURCE.Password);";

                dbConnection.Execute(query, settings);

                return HttpStatusCode.OK;
            }
        }

        private void CreateNexioSettingsTable()
        {
            try
            {
                using (var dbConnection = new SqlConnection(_dataService.ClientConnectionString.ConnectionString))
                {
                    var query = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = N'[Client].[Nexio_Settings]' AND type = 'U')
                    BEGIN
	                    CREATE TABLE [Client].[Nexio_Settings]
	                    (
		                    [recordnumber] int NOT NULL IDENTITY(1, 1),
		                    [last_modified] DATETIME CONSTRAINT DF_Nexio_Settings_last_modified DEFAULT (GETDATE()) NOT NULL,
		                    [BaseApiUrl] varchar(150) NOT NULL,
		                    [Username] varchar(50) NOT NULL,
		                    [Password] varchar(50) NOT NULL,
		                    CONSTRAINT [Nexio_Settings_PrimaryKey] PRIMARY KEY CLUSTERED 
			                    (
				                    [recordnumber]
			                    ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
	                    );
                    END";

                    dbConnection.Execute(query);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}