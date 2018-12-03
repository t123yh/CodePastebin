using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
    }

    public static class ClangFormat
    {
        public static async Task<string> Run(string orig)
        {
            ProcessStartInfo proc = new ProcessStartInfo("clang-format", "-assume-filename=1.cpp -style=file");
            Console.WriteLine(Environment.CurrentDirectory);
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