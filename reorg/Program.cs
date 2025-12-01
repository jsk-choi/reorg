using System.Collections.Generic;
using System.IO;
using System.Reflection;

Console.OutputEncoding = System.Text.Encoding.UTF8;

string[] videoExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".m4v", ".mpeg", ".mpg", ".3gp", ".ts", ".mts", ".m2ts", ".vob" };






// NO PATH SUPPLIED, EXIT
if (args.Count() < 2) {
    Console.WriteLine("nothing supplied, exit");
    return;
}

foreach (string item in args.Skip(1))
{
    if (Path.Exists(item)) {
        Doreorg(item);
    }
    else
    {
        Console.WriteLine($"path not found : {item}");
    }
}







void Doreorg(string folderbase)
{
    //string folderbase = @"\\192.168.0.10\q\zz-org";

    var folders = Directory.GetDirectories(folderbase).Select(x => new DirectoryInfo(x));
    var filesall = Directory.GetFiles(folderbase, "*.*", SearchOption.AllDirectories);




    foreach (var folder in folders)
    {
        var dirinfo = new DirectoryInfo(folder.FullName);

        Console.WriteLine(folder);

        Console.WriteLine(@" ↓ ↓ ↓");

        var files = filesall.Where(x => x.StartsWith(folder.FullName));
        var filesinfos = files.Select(x => new FileInfo(x)).OrderByDescending(x => x.FullName.Length);


        int ctr = 1;
        foreach (var fileinfo in filesinfos)
        {
            // NOT VIDEO FILE > DELETE
            if (!videoExtensions.Contains(fileinfo.Extension))
            {
                File.Delete(fileinfo.FullName);
                Console.WriteLine($"del   : {fileinfo.FullName}");
            }
            // VIDEO FILE > MOVE/RENAME TO FOLDER NAME + EXTENSION
            else
            {
                // DELETE SAMPLES
                if (fileinfo.Name.ToLower().Contains("sample"))
                {
                    File.Delete(fileinfo.FullName);
                    Console.WriteLine($"del   : {fileinfo.FullName}");
                    continue;
                }


                // ADD CTR TO FILENAME IF ALREADY EXISTS
                string newfile = Path.Join(folderbase, folder.Name + fileinfo.Extension);

                while (File.Exists(newfile))
                {
                    newfile = Path.Join(folderbase, $"{folder.Name}-{ctr}.{fileinfo.Extension}");
                    ctr++;
                }

                File.Move(fileinfo.FullName, newfile);
                Console.WriteLine($"mov   : {fileinfo.FullName}\n        >> {newfile}");
            }


        }

        // FIND ALL DIRECTORIES IN FOLDER, DELETE IF EMPTY
        var subdirs = Directory.GetDirectories(folder.FullName, "*", SearchOption.AllDirectories)
            .Select(x => new DirectoryInfo(x)).OrderByDescending(x => x.FullName.Length);

        int filecount = 0;
        foreach (var subdir in subdirs)
        {
            filecount = Directory.GetFiles(subdir.FullName, "*.*", SearchOption.AllDirectories).Length;

            if (filecount == 0)
            {
                Directory.Delete(subdir.FullName, true);
                Console.WriteLine($"deldir: {subdir.FullName}");
            }

        }

        filecount = Directory.GetFiles(folder.FullName, "*.*", SearchOption.AllDirectories).Length;
        if (filecount == 0)
        {
            Directory.Delete(folder.FullName, true);
            Console.WriteLine($"deldir: {folder.FullName}");
        }


        Console.WriteLine("----------------------");
        Console.WriteLine("----------------------");
    }
}