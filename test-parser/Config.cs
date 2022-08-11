using System;

namespace test_parser
{
    public class Config
    {
        public string ConnectionString { get; set; }
        public float RequestDelay { get; set; } = 3;
        public int RecordCountPerRequest { get; set; } = 20;
        public int MaxRetries { get; set; } = 3;
        public int DealCount { get; set; } = 250000;

        public bool EnableStrictValidation { get; set; } = false;

        public bool UpdateRecords { get; set; } = true;
    }
}