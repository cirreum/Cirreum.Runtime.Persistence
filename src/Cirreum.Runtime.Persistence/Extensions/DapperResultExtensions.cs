namespace System.Data;

using Cirreum;
using Cirreum.Persistence;
using Dapper;

/// <summary>
/// Provides extension methods for executing SQL queries using Dapper and returning results wrapped in Result types,
/// including support for single, multiple, and paginated query results with optional mapping functions.
/// </summary>
/// <remarks>These extension methods simplify common query patterns by integrating Dapper query execution with the
/// Result and PagedResult types. They support asynchronous operations, parameterized queries, and mapping between data
/// and domain models. Methods are designed to handle not-found cases and pagination scenarios, and to promote
/// consistent result handling across data access layers.</remarks>
public static class DapperResultExtensions {

	extension(IDbConnection conn) {

		/// <summary>
		/// Executes the specified SQL query asynchronously and returns a single result wrapped in a <see cref="Result{T}"/>
		/// object.
		/// </summary>
		/// <typeparam name="T">The type of the object to be returned from the query.</typeparam>
		/// <param name="sql">The SQL query to execute. Should be a statement that returns a single row.</param>
		/// <param name="param">An object containing the parameters to be passed to the SQL query, or <see langword="null"/> if no parameters are
		/// required.</param>
		/// <param name="key">A key associated with the query result, used to identify or correlate the returned value.</param>
		/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
		/// <returns>A task representing the asynchronous operation. The result contains a <see cref="Result{T}"/> object with the
		/// queried value, or a NotFound result if no row is found.</returns>
		public async Task<Result<T>> QuerySingleAsync<T>(
			string sql,
			object? param,
			object key,
			CancellationToken cancellationToken = default) {
			var result = await conn.QuerySingleOrDefaultAsync<T>(new CommandDefinition(
				sql,
				param,
				cancellationToken: cancellationToken));
			return Result.FromQuery(result, key);
		}

		/// <summary>
		/// Executes the specified SQL query asynchronously and returns a single result wrapped in a <see cref="Result{T}"/>
		/// object, applying a mapping function to transform the item.
		/// </summary>
		/// <typeparam name="TData">The type of the object returned by the SQL query (data layer).</typeparam>
		/// <typeparam name="TModel">The type of the object in the final result (domain layer).</typeparam>
		/// <param name="sql">The SQL query to execute. Should be a statement that returns a single row.</param>
		/// <param name="param">An object containing the parameters to be passed to the SQL query, or <see langword="null"/> if no parameters are required.</param>
		/// <param name="key">A key associated with the query result, used to identify or correlate the returned value.</param>
		/// <param name="mapper">A function to transform the data item to the domain model.</param>
		/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
		/// <returns>A task representing the asynchronous operation. The result contains a <see cref="Result{T}"/> object with the
		/// mapped value, or a NotFound result if no row is found.</returns>
		public async Task<Result<TModel>> QuerySingleAsync<TData, TModel>(
			string sql,
			object? param,
			object key,
			Func<TData, TModel> mapper,
			CancellationToken cancellationToken = default) {
			var result = await conn.QuerySingleOrDefaultAsync<TData>(new CommandDefinition(
				sql,
				param,
				cancellationToken: cancellationToken));
			if (result is null) {
				return Result.NotFound<TModel>(key);
			}
			return mapper(result);
		}

		/// <summary>
		/// Executes the specified SQL query asynchronously and returns zero or more results as a read-only list
		/// wrapped in a successful Result.
		/// </summary>
		/// <typeparam name="T">The type of the elements to be returned in the result list.</typeparam>
		/// <param name="sql">The SQL query to execute against the database.</param>
		/// <param name="param">An object containing the parameters to be passed to the SQL query, or null if no parameters are required.</param>
		/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a successful Result
		/// object wrapping a read-only list of items (which may be empty).</returns>
		public async Task<Result<IReadOnlyList<T>>> QueryAnyAsync<T>(
			string sql,
			object? param,
			CancellationToken cancellationToken = default) {
			var result = await conn.QueryAsync<T>(new CommandDefinition(
				sql,
				param,
				cancellationToken: cancellationToken));
			return Result.From<IReadOnlyList<T>>([.. result]);
		}

		/// <summary>
		/// Executes the specified SQL query asynchronously and returns zero or more results as a read-only list
		/// wrapped in a successful Result, applying a mapping function to transform each item.
		/// </summary>
		/// <typeparam name="TData">The type of the elements returned by the SQL query (data layer).</typeparam>
		/// <typeparam name="TModel">The type of the elements in the final result list (domain layer).</typeparam>
		/// <param name="sql">The SQL query to execute against the database.</param>
		/// <param name="param">An object containing the parameters to be passed to the SQL query, or null if no parameters are required.</param>
		/// <param name="mapper">A function to transform each data item to the domain model.</param>
		/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a successful Result
		/// object wrapping a read-only list of mapped items (which may be empty).</returns>
		public async Task<Result<IReadOnlyList<TModel>>> QueryAnyAsync<TData, TModel>(
			string sql,
			object? param,
			Func<TData, TModel> mapper,
			CancellationToken cancellationToken = default) {
			var result = await conn.QueryAsync<TData>(new CommandDefinition(
				sql,
				param,
				cancellationToken: cancellationToken));
			return Result.From<IReadOnlyList<TModel>>([.. result.Select(mapper)]);
		}

		/// <summary>
		/// Executes the specified SQL query asynchronously and returns the results as a paginated result wrapped in a
		/// <see cref="Result{T}"/> object.
		/// </summary>
		/// <remarks>
		/// This method expects an SQL query that includes OFFSET/FETCH clauses for pagination. The total count must be
		/// obtained separately before calling this method, typically via a COUNT(*) query.
		/// </remarks>
		/// <typeparam name="T">The type of the elements to be returned in the paged result.</typeparam>
		/// <param name="sql">The SQL query to execute against the database. Should include OFFSET/FETCH for pagination.</param>
		/// <param name="param">An object containing the parameters to be passed to the SQL query, or <see langword="null"/> if no parameters are required.</param>
		/// <param name="totalCount">The total number of records matching the query criteria (before pagination).</param>
		/// <param name="pageSize">The number of items per page.</param>
		/// <param name="page">The current page number (1-based).</param>
		/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T}"/>
		/// wrapping a <see cref="PagedResult{T}"/> with the queried items and pagination metadata.</returns>
		public async Task<Result<PagedResult<T>>> QueryPagedAsync<T>(
			string sql,
			object? param,
			int totalCount,
			int pageSize,
			int page,
			CancellationToken cancellationToken = default) {
			var items = await conn.QueryAsync<T>(new CommandDefinition(
				sql,
				param,
				cancellationToken: cancellationToken));
			var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
			return new PagedResult<T>(
				[.. items],
				totalCount,
				pageSize,
				page,
				totalPages
			);
		}

		/// <summary>
		/// Executes the specified SQL query asynchronously and returns the results as a paginated result wrapped in a
		/// <see cref="Result{T}"/> object, applying a mapping function to transform each item.
		/// </summary>
		/// <typeparam name="TData">The type of the elements returned by the SQL query (data layer).</typeparam>
		/// <typeparam name="TModel">The type of the elements in the final paged result (domain layer).</typeparam>
		/// <param name="sql">The SQL query to execute against the database. Should include OFFSET/FETCH for pagination.</param>
		/// <param name="param">An object containing the parameters to be passed to the SQL query, or <see langword="null"/> if no parameters are required.</param>
		/// <param name="totalCount">The total number of records matching the query criteria (before pagination).</param>
		/// <param name="pageSize">The number of items per page.</param>
		/// <param name="page">The current page number (1-based).</param>
		/// <param name="mapper">A function to transform each data item to the domain model.</param>
		/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T}"/>
		/// wrapping a <see cref="PagedResult{T}"/> with the mapped items and pagination metadata.</returns>
		public async Task<Result<PagedResult<TModel>>> QueryPagedAsync<TData, TModel>(
			string sql,
			object? param,
			int totalCount,
			int pageSize,
			int page,
			Func<TData, TModel> mapper,
			CancellationToken cancellationToken = default) {
			var items = await conn.QueryAsync<TData>(new CommandDefinition(
				sql,
				param,
				cancellationToken: cancellationToken));
			var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
			return new PagedResult<TModel>(
				[.. items.Select(mapper)],
				totalCount,
				pageSize,
				page,
				totalPages
			);
		}

	}

}