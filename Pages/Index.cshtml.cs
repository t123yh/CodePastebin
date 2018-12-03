using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodePastebin.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace CodePastebin.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public IndexModel(ApplicationDbContext db)
        {
            _dbContext = db;
        }

        public async Task OnGet()
        {
            ViewData["Count"] = await _dbContext.Snippets.CountAsync();
        }

        public async Task<IActionResult> OnPost(string poster, string content, string codeType)
        {
            string codeContent = content;
            if (codeType == "cpp")
            {
                codeContent = await ClangFormat.Run(content);
            }

            var newSnippet = new CodeSnippet()
            {
                Id = Utils.RandomString(10),
                PostTime = DateTime.Now,
                Content = codeContent,
                Poster = poster,
                Language = codeType
            };
            _dbContext.Snippets.Add(newSnippet);
            await _dbContext.SaveChangesAsync();
            return RedirectToPage("ViewCode", new {Id = newSnippet.Id});
        }
    }
}