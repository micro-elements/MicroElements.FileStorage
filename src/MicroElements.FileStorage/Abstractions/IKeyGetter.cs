using System;
using System.Linq.Expressions;

namespace MicroElements.FileStorage.Abstractions
{
    /// <summary>
    /// Key getter strategy.
    /// Uses for getting key from entity.
    /// </summary>
    /// <typeparam name="TValue">Type of entity/</typeparam>
    public interface IKeyGetter<TValue>
    {
        /// <summary>
        /// Gets expression for Id property access.
        /// </summary>
        /// <returns>Expression.</returns>
        Expression<Func<TValue, string>> GetIdExpression();

        /// <summary>
        /// Get Id property accessor function.
        /// </summary>
        /// <returns>Id function.</returns>
        Func<TValue, string> GetIdFunc();
    }
}