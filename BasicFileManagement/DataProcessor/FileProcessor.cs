using System;
using System.IO;
using System.Collections.Generic;


namespace DataProcessor
{
    public class FileProcessor
    {
        private const string BackupDirectoryName = "backup";
        private const string InProgressDirectoryName = "processing";
        private const string CompletedDirectoryName = "complete";
        

        public string InputFilePath { get; }

        public FileProcessor(string filepath) => InputFilePath = filepath;

        public void Process()
        {
            Console.WriteLine($"Begin process of {InputFilePath}");

            //check if the file exists
            if (!File.Exists(InputFilePath))
            {
                Console.WriteLine($"ERROR: File {InputFilePath} does not exist.");
                return;
            }

            //find the root directory of the current directory that you are in
            string rootDirectoryPath = new DirectoryInfo(InputFilePath).Parent.Parent.FullName;
            Console.WriteLine($"Root data path is {rootDirectoryPath}");


            //check if the backup directory exists 
            string backupDirectoryPath = Path.Combine(rootDirectoryPath, BackupDirectoryName);


            Console.WriteLine($" Attempting to Create {backupDirectoryPath}");
            //this is creating the backup directory for us if it does not exist
            Directory.CreateDirectory(backupDirectoryPath);


            //copy file to backup directory
            string inputFileName = Path.GetFileName(InputFilePath);
            string backupFilePath = Path.Combine(backupDirectoryPath, inputFileName);
            Console.WriteLine($" Copying {InputFilePath} to {backupFilePath}");
            File.Copy(InputFilePath, backupFilePath, true);


            //move file to in progress directory
            Directory.CreateDirectory(Path.Combine(rootDirectoryPath, InProgressDirectoryName));
            string inProgressFilePath = Path.Combine(rootDirectoryPath, InProgressDirectoryName, inputFileName);

            //check if the file exists because we do not want to move a file that may already be in process in this case.
            if (File.Exists(inProgressFilePath))
            {
                Console.WriteLine($"Error: a file with the name {inProgressFilePath} is already being processed");
                return;
            }

            //code block that will move the selected file to the path that we have set in this case.
            Console.WriteLine($"Moving {InputFilePath} to {inProgressFilePath}");
            File.Move(InputFilePath, inProgressFilePath);

            //determine type of file based on extenstion
            string extension = Path.GetExtension(InputFilePath);

            switch (extension)
            {
                case ".txt":
                    ProcessTextFile(InputFilePath);
                    break;
                default: Console.WriteLine($"{extension} is an unsupported file type.");
                    break;
            }

            //move file after processing is completed
            string completedDirectoryPath = Path.Combine (rootDirectoryPath, CompletedDirectoryName);
            Directory.CreateDirectory(completedDirectoryPath);
            Console.WriteLine($"Moving {inProgressFilePath} to {completedDirectoryPath}");
            // File.Move(inProgressFilePath, Path.Combine(completedDirectoryPath, inputFileName));

            string completedFileName =
                $"{Path.GetFileNameWithoutExtension(InputFilePath)}--{Guid.NewGuid()}{extension}";

            var completedFilePath = Path.Combine(completedDirectoryPath, completedFileName);

            File.Move(inProgressFilePath, completedFilePath);

            string inProgressDirectoryPath = Path.GetDirectoryName(inProgressFilePath);
            Directory.Delete(inProgressDirectoryPath, true);

        }


        private void ProcessTextFile(string inProgressFilePath)
        {
            Console.WriteLine($"Processing text file {inProgressFilePath}");
            //Read in and process

        }
    }
}
