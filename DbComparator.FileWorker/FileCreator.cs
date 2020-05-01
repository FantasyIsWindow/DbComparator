using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace DbComparator.FileWorker
{
    public class FileCreator
    {
        private readonly string _folderName = "Collected Scripts";

        private readonly string _fileExtension = ".txt";

        private readonly string _newFolderPath;

        public FileCreator()
        {
            string _currentFolder = Environment.CurrentDirectory;
            _newFolderPath = Path.Combine(_currentFolder, _folderName);
        }

        /// <summary>
        /// The creation and maintenance of the transferred script
        /// </summary>
        /// <param name="name">File name</param>
        /// <param name="script">Text of the saved script</param>
        public void SaveFile(string name, string script)
        {
            string fileName = Path.Combine(_newFolderPath, name + _fileExtension);
            CreateFile(fileName, script);
            OpenFile(fileName);
        }

        /// <summary>
        /// Function for recording and saving the transmitted information to a file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="script">Text of the saved script</param>
        private void CreateFile(string fileName, string script)
        {
            CreateFolder(_newFolderPath);
            
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                byte[] arr = Encoding.Default.GetBytes(script);
                fileStream.Write(arr, 0, arr.Length);
            }
        }

        /// <summary>
        /// Function for checking the presence and creating a folder in case of its absence
        /// </summary>
        private void CreateFolder(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);

            if(!directory.Exists)
            {
                directory.Create();
            }
        }

        /// <summary>
        /// A function to open a file
        /// </summary>
        /// <param name="fileName">Name of the file to open</param>
        private void OpenFile(string fileName)
        {
            Process.Start(fileName);
        }
    }
}
