namespace Pg2Couch
{
    public enum Transformation
    {
        /// <summary>
        /// Converts field names to camel-case using lowercase-initial letter. (i.e. orderNumber style)
        /// </summary>
        CamelizeLower,

        /// <summary>
        /// Converts field names to camel-case uppercase-initial letter (also known as Pascal Case), i.e. OrderNumber
        /// style.
        /// </summary>
        CamelizeUpper,

        /// <summary>
        /// Converts field names to snake case, i.e. order_number style.
        /// </summary>
        SnakeCase,
    }
}
