
using DataProcessor;
using System.Collections.Concurrent;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Threading;

 class Program
{
    // private static ConcurrentDictionary<string, string> FilesToProcess =
    //   new ConcurrentDictionary<string, string>();

    private static MemoryCache FilesToProcess = MemoryCache.Default;


    private static void Main(string[] args)
    {
        Console.WriteLine("Parsing command line arguments");

        var directoryToWatch = args[0];

        if (!Directory.Exists(directoryToWatch))
        {
            Console.WriteLine($" Error: {directoryToWatch} does not exist");
        }
        else
        {
            Console.WriteLine($"Watching directory {directoryToWatch} for changes");
            ProcessExistingFiles(directoryToWatch);
            using var inputFileWatcher = new FileSystemWatcher(directoryToWatch);
           // using var timer = new Timer(ProcessFiles, null, 0, 1000);

            inputFileWatcher.IncludeSubdirectories = false;
            inputFileWatcher.InternalBufferSize = 32768; //32kb
            inputFileWatcher.Filter = "*.*"; //this is the default
            inputFileWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;

            inputFileWatcher.Created += FileCreated;
            inputFileWatcher.Changed += FileChanged;
            inputFileWatcher.Deleted += FileDeleted;
            inputFileWatcher.Renamed += FileRenamed;
            inputFileWatcher.Error += WatcherError;

            inputFileWatcher.EnableRaisingEvents = true;
            Console.WriteLine("Press enter to quit");
            Console.ReadLine();

        }

        static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"* File created {e.Name} - type: {e.ChangeType}");

            //FilesToProcess.TryAdd(e.FullPath, e.FullPath);
            AddToCache(e.FullPath);
        }

        static void FileChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"* File Changed {e.Name} - type: {e.ChangeType}");

            //FilesToProcess.TryAdd(e.FullPath, e.FullPath);
            AddToCache(e.FullPath);
        }

        static void FileDeleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"* File Deleted {e.Name} - type: {e.ChangeType}");
        }

        static void FileRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"* File Renamed {e.OldName} to {e.Name} - type: {e.ChangeType}");
        }

        static void WatcherError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"Error: file system watching may no longer be active: {e.GetException()}");
        }

         static void AddToCache(string fullPath)
        {
            var item = new CacheItem(fullPath, fullPath);
            var policy = new CacheItemPolicy
            {
                RemovedCallback = ProcessFile,
                SlidingExpiration = TimeSpan.FromSeconds(2),
            };

            FilesToProcess.Add(item, policy);
        }

        static void ProcessFile(CacheEntryRemovedArguments args)
        {
            Console.WriteLine($"* Cache item removed {args.CacheItem.Key} because {args.RemovedReason}");
            if(args.RemovedReason == CacheEntryRemovedReason.Expired)
            {
                var fileProcessor = new FileProcessor(args.CacheItem.Key);
                    fileProcessor.Process();
            }
            else
            {
                Console.WriteLine($"WARNing: {args.CacheItem.Key} was removed unexpectedly and may not be processed");
            }

        }

        static void ProcessExistingFiles(string inputDirectory)
        {
            Console.WriteLine($"Checking {inputDirectory} for existing files");
            foreach(var filePath in Directory.EnumerateFiles(inputDirectory))
            {
                Console.WriteLine($"   - FOUND {filePath}");
                AddToCache(filePath);
            }
        }


        // static void ProcessFiles(object stateInfo)
        //{ 
        //    foreach(var fileName in FilesToProcess.Keys)
        //    {
        //        if (FilesToProcess.TryRemove(fileName, out _))
        //        {
        //            var fileProcessor = new FileProcessor(fileName);
        //            fileProcessor.Process();
        //        }
        //    }
        //}
   
    }
}


