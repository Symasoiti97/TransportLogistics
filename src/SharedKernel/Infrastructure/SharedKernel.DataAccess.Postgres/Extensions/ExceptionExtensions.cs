using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace TL.SharedKernel.Infrastructure.DataAccess.Postgres.Extensions;

public static class ExceptionExtensions
{
    public static bool IsPostgresUniqueConstraintViolation(this DbUpdateException exception, string constraintName)
        => exception.InnerException is PostgresException postgresException
           && string.Equals(postgresException.ConstraintName, constraintName, StringComparison.OrdinalIgnoreCase);
}
