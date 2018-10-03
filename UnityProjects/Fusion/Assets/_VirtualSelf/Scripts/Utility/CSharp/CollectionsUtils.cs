using System.Collections.Generic;
using System.Linq;


namespace VirtualSelf.Utility {


/// <summary>
/// A collection of static utility methods for working with collections of different kinds, in ways
/// that do not have a simple, built-in solution.
/// </summary>
public static class CollectionsUtils {

    /* ---------- Public Methods ---------- */

    /// <summary>
    /// Finds all duplicates within the given collection, and returns the indices of all duplicate
    /// groups.
    /// </summary>
    /// <remarks>
    /// This method has a time complexity of O(n^2), plus additional (constant) "overhead" time.
    /// </remarks>
    /// <param name="collection">
    /// The collection to find duplicates in. Notice that this collection will not be modified by
    /// this method in any way.
    /// </param>
    /// <typeparam name="T">
    /// The type of the elements within <paramref name="collection"/>.
    /// </typeparam>
    /// <returns>
    /// A collection of all duplicates that have been found within <paramref name="collection"/>.
    /// <br/>
    /// The returned collection is a (sorted) mapping of the indices of all elements in
    /// <paramref name="collection"/> which have duplicates, to, for each one, a list of the indices
    /// of all elements which are duplicates of that particular element.<br/>
    /// (Of course, elements are not considered duplicates of themselves.)<br/>
    /// If no duplicates have been found, the collection returned will be empty.
    /// </returns>
    public static SortedDictionary<int, IList<int>> FindAllDuplicatesIn<T>(
                  IEnumerable<T> collection) {

        List<T> collectionList = collection.ToList();
        
        SortedDictionary<int, IList<int>> duplicatePositions = 
            new SortedDictionary<int, IList<int>>();
        
        for (int i = 0; i < collectionList.Count; i++) {

            T sourceElem = collectionList[i];
            bool foundAtLeastOne = false;
            
            for (int j = 0; j < collectionList.Count; j++) {

                T targetElem = collectionList[j];

                if (sourceElem.Equals(targetElem) && (i != j)) {

                    if (foundAtLeastOne == false) { 
                        
                        duplicatePositions.Add(i, new List<int>());
                        foundAtLeastOne = true;
                    }
                    
                    duplicatePositions[i].Add(j);
                }
            }
        }

        return (duplicatePositions);
    }
}

}