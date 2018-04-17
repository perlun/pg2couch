namespace Pg2Couch
{
    public class Table
    {
        public string Query { get; internal set; }
        public RowTransformer RowTransformer { get; internal set; }
    }
}
