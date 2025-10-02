using System.Collections.ObjectModel;

namespace UmaEventReaderV2.Website.Extensions;

public static class ObservableCollectionExtensions
{
    public static ObservableCollection<T> Fill<T>(this ObservableCollection<T> source, IEnumerable<T> content)
    {
        source.Clear();

        foreach (var item in content)
            source.Add(item);

        return source;
    }
}