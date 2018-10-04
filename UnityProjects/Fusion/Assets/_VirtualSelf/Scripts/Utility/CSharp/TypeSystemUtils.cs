using System;
using System.Collections.Generic;
using System.Linq;


namespace VirtualSelf {

namespace Utility {

    
/// <summary>
/// A collection of static utility methods concerning the type system of C#, and generally C# types.
/// </summary>
public static class TypeSystemUtils {
    
    /* ---------- Public Methods ---------- */

    /// <summary>
    /// Returns a collection of all parent types of the type <paramref name="type"/>. This includes
    /// the entire hierarchy of base class types that <paramref name="type"/> inherits from, and all
    /// interface types that <paramref name="type"/> implements.
    /// </summary>
    /// <remarks>
    /// This method provides no guarantees about the order of the types that are returned. It should
    /// be assumed that the returned collection contains the types in a random order.<br/>
    /// This method was inspired by: https://stackoverflow.com/a/18375526/5183713
    /// </remarks>
    /// <param name="type">The type to collect and return all parent types of.</param>
    /// <param name="includeSelf">
    /// Whether to include the type of <paramref name="type"/> itself, as well. The default is
    /// <c>false</c>.
    /// </param>
    /// <returns>
    /// A collection of all parent types of <paramref name="type"/>. It should be assumed that the
    /// types are in a random order.
    /// </returns>
    public static IEnumerable<Type> GetAllParentTypesOf(Type type, bool includeSelf = false) {

        List<Type> types = new List<Type>();
        
        /* "null" is a special type - the "null type". It does not have any parent types. Since the
         * kind of types is not specified here, and "null" _is_ a type, it will be returned (as the
         * only type) if "includeSelf" is true. */
        
        if (type == null) {
            
            if (includeSelf == true) { types.Add(null); }
            return (types);
        }
        
        IList<Type> baseClasses = new List<Type>();
        IList<Type> interfaces = new List<Type>();
        
        GetBaseClassesAndInterfacesOf(type, ref baseClasses, ref interfaces, includeSelf);
        
        types.AddRange(baseClasses);
        types.AddRange(interfaces);

        return (types);
    }
    
    /// <summary>
    /// Collects the entire hierarchy of base class types that the type <paramref name="type"/>
    /// inherits from, and all interface types that <paramref name="type"/> implements.
    /// </summary>
    /// <remarks>
    /// This method provides no guarantees about the order of any types that are collected here. It
    /// should be assumed that the types are added to the provided collections in a random order.
    /// <br/>
    /// This method was inspired by: https://stackoverflow.com/a/18375526/5183713
    /// </remarks>
    /// <param name="type">The type to collect all base class types and interface types of.</param>
    /// <param name="baseClasses">
    /// A reference to a collection which will contain all base class types of
    /// <paramref name="type"/>. This should initially be empty (but existing content is not
    /// touched, the new values are just added).
    /// </param>
    /// <param name="interfaces">
    /// A reference to a collection which wiill contain all interface types of
    /// <paramref name="type"/>. This should initially be empty (but existing content is not
    /// touched, the new values are just added).
    /// </param>
    /// <param name="includeSelf">
    /// Whether to include the type of <paramref name="type"/> itself, as well. The default is
    /// <c>false</c>.
    /// </param>
    public static void GetBaseClassesAndInterfacesOf(
            Type type,
            ref IList<Type> baseClasses, ref IList<Type> interfaces,
            bool includeSelf = false) {

        /* "null" is a special type - the "null type". It is neither a class type, nor an interface
         * type, so it is not even covered by this method. It also does not have any base type, so
         * there is nothing to do here. */
        
        if (type == null) { return; }
        
        if (includeSelf == true) {

            if (type.IsInterface == true) { interfaces.Add(type); }
            else { baseClasses.Add(type); }
        }

        /* Add all interface types the given type implements. There is a method for this, so it's
         * easy. */
        
        foreach (Type interfaceType in type.GetInterfaces()) {

            interfaces.Add(interfaceType);
        }

        /* Add all base types the given type inherits from. For this, we recursively walk "upwards"
         * by repeatedly getting the base type, until we reached "null" - which means we arrived at
         * "System.object", which is at the top of the type hierarchy for this, and thus does not
         * have another base type again. */
        
        Type currentBaseType = type.BaseType;
        while (currentBaseType != null) {

            baseClasses.Add(currentBaseType);
            currentBaseType = currentBaseType.BaseType;
        }
    }

    /// <summary>
    /// Returns whether <paramref name="parentType"/> is a parent type of <paramref name="type"/>.
    /// <br/>
    /// A "parent type", in this case, is any base type of <paramref name="type"/> in the entire
    /// type hierarchy, or an interface type <paramref name="type"/> implements.
    /// </summary>
    /// <remarks>
    /// This method was inspired by: https://stackoverflow.com/a/18375526/5183713
    /// </remarks>
    /// <param name="type">
    /// The type to check whether <paramref name="parentType"/> is a parent type of.
    /// </param>
    /// <param name="parentType">
    /// The type to check for whether it is a parent type of <paramref name="type"/>.
    /// </param>
    /// <returns>
    /// Whether <paramref name="parentType"/> is a parent type of <paramref name="type"/>.
    /// </returns>
    public static bool IsParentTypeOf(Type type, Type parentType) {

        /* "null" is a special type, and cannot have any parent type. */
        
        if (type == null) { return (false); }

        /* Only interfaces can have "null" as their parent type. If "type" is an interface, then
         * "null" is a parent type of it - otherwise, it isn't. */
        
        if (parentType == null) { return (type.IsInterface); }

        /* If "parentType" is an interface, simply check all interfaces "type" implements. */
        
        if (parentType.IsInterface == true) {

            return (type.GetInterfaces().Contains(parentType));
        }
        
        /* If not, we check all base types of "type" recursively (until we reach "System.object",
         * the highest base type in the type hierarchy, which thus itself has no base type anymore),
         * and see if "parentType" is among them. */

        Type currentType = type;

        while (currentType != null) {

            if (currentType.BaseType == parentType) { return (true); }
            else { currentType = currentType.BaseType; }
        }
        
        /* If we found nothing up to here, then "parentType" is not a parent type. */

        return (false);
    }
}  

}

}


