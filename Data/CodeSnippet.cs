using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodePastebin.Data
{
    public class CodeSnippet
    {
        public string Id { get; set; }
        public string Poster { get; set; }
        public DateTime PostTime { get; set; }
        public string Language { get; set; }
        public string Content { get; set; }
    }
}