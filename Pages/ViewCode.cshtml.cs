using System.Text;
using System.Threading.Tasks;
using CodePastebin.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CodePastebin.Pages
{
    public class ViewCodeModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        public ViewCodeModel(ApplicationDbContext db)
        {
            _dbContext = db;
        }

        public async Task OnGet(string id)
        {
            ViewData["Id"] = id;
            ViewData["Code"] = await _dbContext.FindAsync<CodeSnippet>(id);
        }
        
        public async Task<ActionResult> OnGetDownload(string id)
        {
            var code = await _dbContext.FindAsync<CodeSnippet>(id);
            var type = code.Language == "cpp" ? "cpp" : "txt";
            return File(Encoding.UTF8.GetBytes(code.Content), "text/plain", $"{code.Id}.{type}");
        }
    }
}