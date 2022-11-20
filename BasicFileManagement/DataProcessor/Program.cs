
using DataProcessor;

Console.WriteLine("Parsing command line arguments");

//command line validation omitted for brevity

var command = args[0];

if (command == "--file")
{
    var filePath = args[1];
    //check if the file path is absolute or not
    if(!Path.IsPathFullyQualified(filePath))
    {
        Console.WriteLine($"Error: path {filePath} must be fully qualified");
        Console.ReadLine();
        return;
    }

    Console.WriteLine($"Single file {filePath} selected");
    ProcessSingleFile(filePath);
}
else if (command == "--dir")
{
    var directoryPath = args[1];
    var fileType = args[2];
    Console.WriteLine($"Directory {directoryPath} selected for {fileType} files");
    ProcessDirectory(directoryPath, fileType);
}
else
{
    Console.WriteLine("Invalid command line option");
}
Console.WriteLine("presss enter to quit");
Console.ReadLine();

 static void ProcessSingleFile(string filepath)
{
    var fileProcessor = new FileProcessor(filepath);
     fileProcessor.Process();

}

static void ProcessDirectory(string directoryPath, string fileType) 
{

    switch (fileType)
    {
        case "TEXT":
            string[] textFiles = Directory.GetFiles(directoryPath, "*.txt");
            foreach (var textFilePath in textFiles)
            {
                var fileProcessor = new FileProcessor(textFilePath);
                fileProcessor.Process();
            }   
            break;
        default: Console.WriteLine($"Error: {fileType} is not supported");
            return;
    }


}



