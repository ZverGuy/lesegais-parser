namespace test_parser
{
    public class Config
    {
        public string ConnectionString { get; set; }
        public float RequestDelay { get; set; }
        public int RecordCountPerRequest { get; set; } = 20;
        public int MaxRetries { get; set; } = 3;
    }
}