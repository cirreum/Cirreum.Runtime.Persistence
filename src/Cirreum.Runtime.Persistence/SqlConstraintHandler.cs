namespace Microsoft.Data.SqlClient;

using Cirreum;
using Cirreum.Exceptions;

/// <summary>
/// Helper methods for handling SQL constraint violations in a Result-oriented way.
/// </summary>
public static class SqlConstraintHandler {

	/// <summary>
	/// Executes an async action and converts constraint violations to appropriate Result failures.
	/// Unique constraint violations become AlreadyExists; foreign key violations become BadRequest.
	/// </summary>
	/// <typeparam name="T">The type of the result value.</typeparam>
	/// <param name="action">The async action that may throw a SqlException.</param>
	/// <param name="errorMessage">The error message to use if a constraint violation occurs.</param>
	/// <returns>The result of the action, or an appropriate failure if a constraint violation occurred.</returns>
	public static async Task<Result<T>> ExecuteAsync<T>(
		Func<Task<Result<T>>> action,
		string errorMessage) {
		try {
			return await action();
		} catch (SqlException ex) when (ex.IsUniqueConstraintViolation()) {
			return Result.AlreadyExist<T>(errorMessage);
		} catch (SqlException ex) when (ex.IsForeignKeyViolation()) {
			return Result.BadRequest<T>(errorMessage);
		}
	}

	/// <summary>
	/// Executes an async action and converts constraint violations to appropriate Result failures.
	/// Unique constraint violations become AlreadyExists; foreign key violations become BadRequest.
	/// </summary>
	/// <param name="action">The async action that may throw a SqlException.</param>
	/// <param name="errorMessage">The error message to use if a constraint violation occurs.</param>
	/// <returns>Success, or an appropriate failure if a constraint violation occurred.</returns>
	public static async Task<Result> ExecuteAsync(
		Func<Task<Result>> action,
		string errorMessage) {
		try {
			return await action();
		} catch (SqlException ex) when (ex.IsUniqueConstraintViolation()) {
			return Result.Fail(new AlreadyExistsException(errorMessage));
		} catch (SqlException ex) when (ex.IsForeignKeyViolation()) {
			return Result.Fail(new BadRequestException(errorMessage));
		}
	}

}