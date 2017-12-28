using MicroElements.FileStorage.Abstractions;

namespace MicroElements.FileStorage
{
    public static class DocumentCollectionExtensions
    {
        public static string GetKey<T>(this IDocumentCollection<T> collection, T item) where T : class
        {
            return collection.ConfigurationTyped.KeyGetter.GetIdFunc()(item);
        }
    }
}