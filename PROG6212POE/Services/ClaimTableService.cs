using Azure;
using Azure.Data.Tables;
using PROG6212POE.Models;

namespace PROG6212POE.Services
{
    public class ClaimTableService
    {
        private readonly TableClient _tableClient;

        public ClaimTableService(IConfiguration config)
        {
            var connectionString = config["AzureStorage:ConnectionString"];
            var tablename = "claims";
            _tableClient = new TableClient(connectionString, tablename);
            _tableClient.CreateIfNotExists();
        }

        //public async Task InsertClaimAsync(Claim claim)
        //{
        //    await _tableClient.AddEntityAsync(claim);
        //}

        //public async Task<List<Claim>> GetAllClaimsAsync()
        //{
        //    var claims = new List<Claim>();

        //    await foreach (Claim entity in _tableClient.QueryAsync<Claim>())
        //    {
        //        claims.Add(entity);
        //    }

        //    return claims;
        //}
    }
}
