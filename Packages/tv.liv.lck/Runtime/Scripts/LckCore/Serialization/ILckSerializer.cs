namespace Liv.Lck.Core.Serialization
{
    /// <summary>
    /// Interface providing serialization capability
    /// </summary>
    internal interface ILckSerializer
    {
        /// <summary>
        /// The <see cref="SerializationType"/> used by the <see cref="ILckSerializer"/>
        /// </summary>
        SerializationType SerializationType { get; }
        
        /// <summary>
        /// Serialize the given <see cref="data"/> into a <see cref="byte"/> array
        /// </summary>
        /// <param name="data">The data to serialize</param>
        /// <returns>A <see cref="byte"/> array of serialized data</returns>
        byte[] Serialize(object data);
    }
}

