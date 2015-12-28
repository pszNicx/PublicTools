using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RtfCleanup
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var filePaths = Directory.GetFiles(Environment.CurrentDirectory, "*.rtf");

                foreach (var filePath in filePaths)
                {
                    Console.WriteLine(String.Format("Processing file: {0}", Path.GetFileName(filePath)));
                    var sourceFilePath = Path.Combine(@"C:\Users\Nicx\Dropbox\Transfer\", Path.GetFileName(filePath));
                    var destinationFilePath = Path.Combine(@"C:\Users\Nicx\Dropbox\Transfer\", Path.GetFileNameWithoutExtension(filePath) + ".txt");

                    var tempFilePath = Path.GetTempFileName();
                    var text = File.ReadAllText(sourceFilePath);

                    text = Regex.Replace(text, "\\\\ls-[0-9]+\\\\.*?\\{\\\\listtext\\t\\\\'95\\t\\}", "- "); // Swap out bullets
                    text = Regex.Replace(text, "\\\\ls-[0-9]+\\\\.*?\\{\\\\listtext\\t(.*)\\t\\}", "$1 "); // Swap out numeric lists
                    File.WriteAllText(tempFilePath, text);

                    var rtfControl = new RichTextBox();
                    rtfControl.LoadFile(tempFilePath);
                    File.Delete(tempFilePath);

                    var outputText = rtfControl.Text.Replace("\n", Environment.NewLine);
                    File.WriteAllText(destinationFilePath, outputText);
                    File.Delete(sourceFilePath);
                    Console.Write("...done.");
                }

                Console.WriteLine("File successfully processed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.ReadLine();
            }
        }
    }
}