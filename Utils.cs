using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CodePastebin
{
    public static class Utils
    {
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        public static Task WaitForExitAsync(this Process process, 
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);
            if(cancellationToken != default(CancellationToken))
                cancellationToken.Register(tcs.SetCanceled);

            return tcs.Task;
        }

        private const string WeChatUserAgent = "(iPhone; CPU iPhone OS 7_0_2 like Mac OS X) AppleWebKit/537.51.1 (KHTML, like Gecko) CriOS/";
        
        public static bool CheckIfWeChat(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("User-Agent", out var ua))
            {
                if (ua.Contains(WeChatUserAgent))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public static class ClangFormat
    {
        public static async Task<string> Run(string orig)
        {
            ProcessStartInfo proc = new ProcessStartInfo("clang-format", 
                "-assume-filename=1.cpp -style=file");
            proc.WorkingDirectory = Environment.CurrentDirectory;
            proc.RedirectStandardInput = true;
            proc.RedirectStandardOutput = true;
            using (Process process = new Process())
            {
                process.StartInfo = proc;
                process.Start();
                await process.StandardInput.WriteAsync(orig);
                process.StandardInput.Close();
                await process.WaitForExitAsync();
                return await process.StandardOutput.ReadToEndAsync();
            }
        }
    }
}
