namespace LinFu.Persist.Metadata
{
    /// <summary>
    /// Describes the relationship between tables in the datastore.
    /// </summary>
    /// <remarks>
    /// The same <see cref="IRelationInfo"/> instance is used to describe both ends of the relation.
    /// </remarks>
    public interface IRelationInfo
    {        
        /// <summary>
        /// Points to the primary key of the primary table.
        /// </summary
        IKeyInfo PrimaryKey { get; set; }
        /// <summary>
        /// The foreign key that is used to refer the primary table.
        /// </summary>
        IKeyInfo ForeignKey { get; set; }
    }
}
