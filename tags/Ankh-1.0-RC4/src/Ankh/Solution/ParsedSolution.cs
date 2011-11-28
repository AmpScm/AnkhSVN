using System;
using System.IO;
using System.Collections;
using System.Globalization;

namespace Ankh.Solution
{
    /// <summary>
    /// Parser for Visual Studio Solution & Project files
    /// </summary>
    public sealed class ParsedSolution
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ParsedSolution(string solutionFile, IContext context)
        {
            if(!File.Exists(solutionFile))
            {
                throw new ArgumentException("Solution file must exist.");
            }

            this.context=context;
            this.projects=new Hashtable();
            this.solutionFile=solutionFile;
        }

        public void Refresh()
        {
            this.solutionContents = null;
            this.projects.Clear();
        }

        /// <summary>
        /// Get the fileName for a given project
        /// </summary>
        /// <param name="projectName">Name of the project</param>
        /// <returns>fileName</returns>
        public string GetProjectFile(string projectName)
        {
            if(this.solutionContents==null)
            {
                StreamReader reader=new StreamReader(this.solutionFile);
                try
                {
                    this.solutionContents=reader.ReadToEnd();
                }
                finally
                {
                    reader.Close();
                }
            }

            try
            {
                //get the basic file name
                int startLocation=solutionContents.IndexOf("\""+projectName+"\",");
                startLocation+=projectName.Length+5;  //pass quotes, etc
                if(startLocation<0)
                {
                    return null; 
                }
                int endLocation=this.solutionContents.IndexOf(",", startLocation);
                endLocation-=1;  //pass quotes
                string file=this.solutionContents.Substring(startLocation, endLocation-startLocation);

                //put it all together
                return Path.Combine(Path.GetDirectoryName(this.solutionFile), file);
            }
            catch(Exception)
            {
                //behave somewhat nicely on solution files we don't understand
                return null;
            }
        }

        /// <summary>
        /// Get a top level item in a project
        /// </summary>
        /// <param name="projectName">name of the project to search</param>
        /// <param name="itemName">name of the item to search for</param>
        /// <returns>the item found</returns>
        public ParsedSolutionItem GetProjectItem(string projectName, string itemName)
        {
            ParsedSolutionItem project=this.GetProjectItems(projectName);
            if(project==null)
                return null;

            foreach(ParsedSolutionItem item in project.Children)
            {
                if(item.Name==itemName)
                {
                    return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the tree of items in a given project
        /// </summary>
        /// <param name="projectName">Project to retrieve items from</param>
        /// <returns>A tree of items from the project</returns>
        public ParsedSolutionItem GetProjectItems(string projectName)
        {
            try
            {
                ParsedSolutionItem project=(ParsedSolutionItem)projects[projectName];

                if(project==null)
                {
                    string projectFile=GetProjectFile(projectName);
                    if(projectFile==null)
                    {
                        return null;
                    }

                    project=new ParsedSolutionItem();
                    project.Name=projectName;
                    project.FileName=projectFile;

                    using(StreamReader reader=new StreamReader(projectFile))
                    {
                        string extension=Path.GetExtension(projectFile);
                        switch(extension)
                        {
                            case ".dbp":
                                this.ParseDatabaseProject(project, reader);
                                break;

                            default:
                                throw new ApplicationException("Trying to parse unknown project of type "+extension);
                        }
                    }

                    //don't save the parsed project until correct parsing is complete
                    this.projects[projectName]=project;
                }

                return project;
            }
            catch(Exception e)
            {
                context.OutputPane.Write("Parser Error: ");
                context.OutputPane.WriteLine(e.Message);
            }

            return null;
        }

        /// <summary>
        /// Recursively parse up the children of an item in a database project
        /// </summary>
        /// <param name="item">Parent item</param>
        /// <param name="reader">Open file being parsed</param>
        private void ParseDatabaseItemChildren(ParsedSolutionItem item, StreamReader reader)
        {
            for(string line=reader.ReadLine().Trim();
                string.Compare( DbStrings.FolderEnd, 0, line, 0, DbStrings.FolderEnd.Length, true, CultureInfo.InvariantCulture ) != 0; 
                line=reader.ReadLine().Trim())
            {
                ParsedSolutionItem child = new ParsedSolutionItem(item);

                if(string.Compare(DbStrings.FolderBegin, 0, line, 0, DbStrings.FolderBegin.Length, true, CultureInfo.InvariantCulture) == 0)
                {
                    int startIndex = line.IndexOf("= \"") + 3;
                    child.Name = line.Substring(startIndex, line.Length-startIndex-1);
                    child.FileName = Path.Combine( Path.GetDirectoryName( item.FileName ), child.Name ) + Path.DirectorySeparatorChar;
                    item.Children.Add(child);

                    this.ParseDatabaseItemChildren(child, reader);
                }
                else if(string.Compare(DbStrings.Script, 0, line, 0, DbStrings.Script.Length, true, CultureInfo.InvariantCulture) == 0 ||
                    string.Compare(DbStrings.Query, 0, line.Trim(), 0, DbStrings.Query.Length, true, CultureInfo.InvariantCulture) == 0)
                {
                    int startIndex=line.IndexOf("= \"") + 3;
                    child.Name = line.Substring(startIndex, line.Length-startIndex-1);
                    child.FileName = Path.Combine(item.FileName, child.Name);
                    item.Children.Add(child);
                }
                else if (string.Compare(DbStrings.DbRefFolderBegin, 0, line, 0, DbStrings.DbRefFolderBegin.Length, true, CultureInfo.InvariantCulture) == 0)
                {
                    //get the folder
                    int startIndex=line.IndexOf("= \"") + 3;
                    child.Name = line.Substring(startIndex, line.Length-startIndex-1);
                    child.FileName = Path.Combine(Path.GetDirectoryName(item.FileName), child.Name);
                    item.Children.Add(child);

                    //slurp db references
                    this.SlurpSection(reader, 1);
                }
                //ignore anything we don't recognize
            }
        }

        /// <summary>
        /// Slurp in and ignore a section of a file
        /// </summary>
        /// <param name="reader">Open file being parsed</param>
        /// <param name="startIndent">Number of section endings necessary to finish slurping</param>
        private void SlurpSection(StreamReader reader,int startIndent)
        {
            if (startIndent < 0)
                throw new ArgumentOutOfRangeException("startIndent");

            
            string begin = "begin ";

            while(startIndent > 0)
            {
                string line=reader.ReadLine();
                if(line == null)
                {
                    break;
                }
                else if(string.Compare(line.Trim(), "end", true, CultureInfo.InvariantCulture) == 0)
                {
                    startIndent--;
                }
                else if(string.Compare(line.Trim(), 0, begin, 0, begin.Length, true, CultureInfo.InvariantCulture) == 0)
                {
                    startIndent++;
                }
            }
        }

        /// <summary>
        /// Parse a project to read out the items contained
        /// </summary>
        /// <param name="project">Project to fill</param>
        /// <param name="reader">An open reader on the project file</param>
        private void ParseDatabaseProject(ParsedSolutionItem project, StreamReader reader)
        {
            //pass the header
            string line=reader.ReadLine();
            if(!line.StartsWith("# Microsoft Developer Studio Project File - Database Project"))
            {
                throw new ApplicationException("Malformed database project file");
            }
            line=reader.ReadLine();
            if(!line.StartsWith("Begin DataProject ="))
            {
                throw new ApplicationException("Malformed database project file");
            }

            this.ParseDatabaseItemChildren(project, reader);
        }

        class DbStrings
        {
            internal const string FolderBegin  = "begin folder";
            internal const string Script = "script ";
            internal const string Query = "query ";
            internal const string DbRefFolderBegin = "begin dbreffolder";
            internal const string FolderEnd = "end";
        }

        private IContext context;
        private Hashtable projects;
        private string solutionContents;
        private string solutionFile;
    }
}