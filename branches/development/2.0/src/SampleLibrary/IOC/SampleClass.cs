namespace SampleLibrary
{
    public class SampleClass : ISampleService, ISampleGenericService<int>
    {
        public bool Called
        {
            get; set;
        }
    }
}