﻿namespace WebAPI.Models
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }= null!;
        public string Message { get; set; } = null!;
    }
}
