using System;
using System.Diagnostics.CodeAnalysis;

public static class Ensure
{
    /// <summary>
    /// Shortcut method for throwing an exception if the tested argument was null. Ideally, this should be used for
    /// testing if the passed field or a property had something assigned in the scene, if exported. It was made
    /// especially for those members, because they often lose reference if there were any changes in the scene.
    /// </summary>
    /// <remarks>
    /// This method will resolve "dereference of possible null reference".
    /// </remarks>
    /// <param name="argument">Any object to test for null reference, preferably a scene component.</param>
    /// <typeparam name="T">Type of the argument - its full type name will be printed on exception.</typeparam>
    public static void IsNotNull<T>([NotNull] T? argument)
    {
        if (argument is null)
        {
            throw new ArgumentNullException(
                nameof(argument),
                $"{typeof(T).FullName} was null. \nIf this was an inspector component, check if it still has an object assigned."
            );
        }
    }
}
