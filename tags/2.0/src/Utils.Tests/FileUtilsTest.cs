using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.IO;
using Utils;

namespace Utils.Tests
{
    [TestFixture]
    public class FileUtilsTest
    {

        [SetUp]
        public void SetUp()
        {
            this.random = new Random();
            this.dirStructure = CreateDirStructure();
        }

        [TearDown]
        public void TearDown()
        {
            RecursiveDelete( this.dirStructure );
        }

        [Test]
        public void CopyDirectory()
        {
            string target = Path.GetTempFileName();
            File.Delete( target );

            FileUtils.CopyDirectory( this.dirStructure, target );

            this.RecursiveCompareDirectory( new DirectoryInfo(this.dirStructure), new DirectoryInfo(target) );
        }

        private void RecursiveCompareDirectory( DirectoryInfo original, DirectoryInfo copy )
        {
            FileInfo[] originalFiles = original.GetFiles( );
            FileInfo[] copiedFiles = copy.GetFiles( );

            Assert.AreEqual( originalFiles.Length, copiedFiles.Length, "Number of files in copy differs." );

            for ( int i = 0; i < originalFiles.Length; i++ )
            {
                Assert.AreEqual( originalFiles[ i ].Name, copiedFiles[ i ].Name );
                Assert.AreEqual( originalFiles[ i ].Length, copiedFiles[i].Length, "Length differs for {0}", copiedFiles[ i ] );
            }

            DirectoryInfo[] originalDirs = original.GetDirectories( );
            DirectoryInfo[] copiedDirs = copy.GetDirectories();

            Assert.AreEqual( originalDirs.Length, copiedDirs.Length, "Number of dirs in copy differs" );
            for ( int i = 0; i < originalDirs.Length; i++ )
            {
                Assert.AreEqual( originalDirs[ i ].Name, copiedDirs[ i ].Name );
                RecursiveCompareDirectory( originalDirs[ i ], copiedDirs[ i ] );
            }
        }

        private string CreateDirStructure()
        {
            string dir = Path.GetTempFileName();
            File.Delete( dir );
            this.DoCreateDirStructure( dir, 0 );

            return dir;
        }

        private void DoCreateDirStructure( string dir, int depth )
        {
            if ( depth >= MaxDepth )
            {
                return;
            }

            Directory.CreateDirectory( dir );

            for ( int i = 0; i < random.Next(5) + 2; i++ )
            {
                string file = Path.Combine(dir, "file" + i);
                using ( StreamWriter writer = new StreamWriter( file ) )
                {
                    this.FillRandom( writer );
                }
            }

            for ( int i = 0; i < random.Next(3) + 2; i++ )
            {
                string subDir = Path.Combine( dir, "dir" + i );
                DoCreateDirStructure( subDir, depth + 1 );
            }
            
        }

        private void FillRandom( StreamWriter writer )
        {
            for ( int i = 0; i < random.Next(1000); i++ )
            {
                writer.Write( random.Next( 1000 ) );
            }
        }

        /// <summary>
        /// Recursively deletes a directory.
        /// </summary>
        /// <param name="path"></param>
        public static void RecursiveDelete(string path)
        {
            foreach (string dir in Directory.GetDirectories(path))
            {
                RecursiveDelete(dir);
            }

            foreach (string file in Directory.GetFiles(path))
                File.SetAttributes(file, FileAttributes.Normal);

            File.SetAttributes(path, FileAttributes.Normal);
            Directory.Delete(path, true);
        }

        private const int MaxDepth = 3;
        private string dirStructure;
        private Random random;
    }
}
