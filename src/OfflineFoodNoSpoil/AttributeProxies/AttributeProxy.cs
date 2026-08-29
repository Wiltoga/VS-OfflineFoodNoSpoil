using System.Diagnostics.CodeAnalysis;
using Vintagestory.API.Datastructures;

namespace Wiltoga.OfflineFoodNoSpoil;

/// <summary>
/// Class used to interact with an item's attributes through .NET properties
/// </summary>
/// <param name="attributes">The parent attributes</param>
public abstract class AttributeProxy(ITreeAttribute? attributes)
{
    /// <summary>
    /// Name of this attribute tree in the parent
    /// </summary>
    protected abstract string AttributeName { get; }

    /// <summary>
    /// The main tree of this attribute, indexed by <see cref="AttributeName" />
    /// </summary>
    protected ITreeAttribute? Tree => attributes?.GetTreeAttribute(AttributeName);

    /// <summary>
    /// Returns <c>true</c> when the attribute tree exists
    /// </summary>
    [MemberNotNullWhen(true, nameof(Tree))]
    public virtual bool Exists => Tree is not null;

    /// <summary>
    /// Get a double value. If the tree or the property does not exist, returns <c>null</c>
    /// </summary>
    /// <param name="name">Key of the property</param>
    /// <returns></returns>
    protected double? TryGetDouble(string name) => Tree?.TryGetDouble(name);
    
    /// <summary>
    /// Try to set a double value if the tree exists. Passing <c>null</c> removes the property.
    /// </summary>
    /// <param name="name">Key of the property</param>
    /// <param name="value">value to set</param>
    protected void TrySetDouble(string name, double? value)
    {
        if (Exists)
        {
            if (value is null && Tree.HasAttribute(name))
            {
                Tree.RemoveAttribute(name);
            }
            else if (value is not null)
            {
                Tree.SetDouble(name, value.Value);
            }
        }
    }

    /// <summary>
    /// Get a float array value. If the tree or the property does not exist, returns <c>null</c>
    /// </summary>
    /// <param name="name">Key of the property</param>
    /// <returns></returns>
    protected float[]? TryGetFloatArray(string name) => (Tree?[name] as FloatArrayAttribute)?.value;

    /// <summary>
    /// Try to set a float array value if the tree exists. Passing <c>null</c> removes the property.
    /// </summary>
    /// <param name="name">Key of the property</param>
    /// <param name="value">value to set</param>
    protected void TrySetFloatArray(string name, float[]? value)
    {
        if (Exists)
        {
            if (value is null && Tree.HasAttribute(name))
            {
                Tree.RemoveAttribute(name);
            }
            else if (value is not null)
            {
                Tree[name] = new FloatArrayAttribute(value);
            }
        }
    }
}
