using System;
using System.IO;

namespace DeeSynk.Components.Managers
{
    public sealed class FileLoader
    {

        private static readonly FileLoader fileLoader = new FileLoader();

        private FileLoader()
        {}

        public static FileLoader GetInstance()
        {
            return fileLoader;
        }

        public static void Load(String protocol)
        {

        }

        public static void Save()
        {

        }
    }
}
