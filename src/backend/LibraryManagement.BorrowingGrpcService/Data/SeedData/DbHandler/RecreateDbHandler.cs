﻿using LibraryManagement.BorrowingGrpcService.Data.DataAccess.DbContexts;
using LibraryManagement.BorrowingGrpcService.Data.SeedData.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.BorrowingGrpcService.Data.SeedData.DbHandler
{

    public class RecreateDbHandler
    {
        private readonly BorrowingBaseDbContext _dbContext;

        private const string DROP_TABLES_SQL_SCRIPT = "DropSqlQuery.sql";

        public RecreateDbHandler(BorrowingBaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle()
        {
            _dbContext.Database.EnsureCreated();
            string generatedScript = _dbContext.Database.GenerateCreateScript();
            DatabaseUtils.DropTables(_dbContext, DROP_TABLES_SQL_SCRIPT);
            await _dbContext.Database.ExecuteSqlRawAsync(generatedScript.Replace("GO", string.Empty));
            _dbContext.Database.ExecuteSqlRaw("IF OBJECT_ID('dbo.__EFMigrationsHistory', 'U') IS NOT NULL  " +
                "                               BEGIN" +
                "                               TRUNCATE TABLE dbo.__EFMigrationsHistory;" +
                "                               END");

            return await Task.FromResult(Unit.Value);
        }
    }
}
