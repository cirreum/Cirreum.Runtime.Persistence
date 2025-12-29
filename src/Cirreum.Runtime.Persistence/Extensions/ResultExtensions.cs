namespace Cirreum;

using Cirreum.Exceptions;

/// <summary>
/// Provides extension methods for creating standardized result objects from query operations, enabling consistent
/// handling of success and not found scenarios.
/// </summary>
/// <remarks>These methods simplify the conversion of potentially missing models or collections into result types,
/// streamlining error handling and response generation for query operations. Use these extensions to reduce boilerplate
/// code when mapping query results to result objects, especially in cases where a not found error should be returned if
/// the queried data is absent.</remarks>
public static class ResultExtensions {

	extension(Result) {

		/// <summary>
		/// Creates a <see cref="Result{T}"/> representing a successful result if the specified model is not null, or a
		/// failure result with a not found error if it is null.
		/// </summary>
		/// <remarks>Use this method to convert a potentially missing model into a standardized result object,
		/// simplifying error handling for not found cases.</remarks>
		/// <typeparam name="T">The type of the model to wrap in the result.</typeparam>
		/// <param name="responseOrNull">The model instance to wrap in the result. If null, the result will indicate a not found error.</param>
		/// <param name="key">The key associated with the model, used to construct the not found error if the model is null.</param>
		/// <returns>A <see cref="Result{T}"/> containing the model if it is not null; otherwise, a failure result with a <see
		/// cref="NotFoundException"/> referencing the specified key.</returns>
		public static Result<T> FromQuery<T>(T? responseOrNull, object key) {
			return responseOrNull is not null
				? Result.From(responseOrNull)
				: Result<T>.Fail(new NotFoundException(key));
		}

	}

}