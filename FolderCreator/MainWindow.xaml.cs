using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;

using System.Windows.Forms;
using Path = System.IO.Path;

namespace FolderCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<string> PathsToNotMove = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
        }

        public enum ProcessType
        {
            Organise,
            Move
        }

        private void select_folder_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            //if folder has been selected
            if (openFolderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folder_name.Text = openFolderDialog.SelectedPath;
            }
        }

        private void organise_folder_Click(object sender, RoutedEventArgs e)
        {

            if(folder_name.Text != "")
            {
                try
                {
                    organise_folder.IsEnabled = false;
                    ProcessDirectory(folder_name.Text, ProcessType.Organise);
                    organise_folder.IsEnabled = true;
                    System.Windows.MessageBox.Show("Organisation Complete!");
                }catch(Exception ex)
                {
                    System.Windows.MessageBox.Show( ex.ToString(), "Organisation Failed!");
                }

            }
        }
        private void ProcessDirectory(string targetDirectory, ProcessType processType)
        {

            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            string[] directoryEntries = Directory.GetDirectories(targetDirectory);
            var filesAndDirs = new string[fileEntries.Length + directoryEntries.Length];
            fileEntries.CopyTo(filesAndDirs, 0);
            directoryEntries.CopyTo(filesAndDirs, fileEntries.Length);

            foreach (string fileName in filesAndDirs)
                ProcessFile(fileName, processType);

            if(processType == ProcessType.Organise)
            {
                ProcessDirectory( targetDirectory, ProcessType.Move);
            }
            else
            {
                PathsToNotMove.Clear();
            }
        }

        // Insert logic for processing found files here.
        private void ProcessFile(string path, ProcessType processType)
        {
            DateTime lastWrite = File.GetLastWriteTime(path);
            if (processType == ProcessType.Organise)
            {
                
                //if year folder does not exist
                if(!Directory.Exists(folder_name.Text + "/" + lastWrite.Year.ToString()))
                {
                    Directory.CreateDirectory(folder_name.Text + "/" + lastWrite.Year.ToString());
                }
                PathsToNotMove.Add(folder_name.Text + "/" + lastWrite.Year.ToString());


                if (!Directory.Exists(folder_name.Text + 
                    "/" + lastWrite.Year.ToString() + 
                    "/" + lastWrite.Month.ToString() + " - " + lastWrite.ToString("MMMM")))
                {
                    Directory.CreateDirectory(folder_name.Text +
                        "/" + lastWrite.Year.ToString() +
                        "/" + lastWrite.Month.ToString() + " - " + lastWrite.ToString("MMMM"));
                }
                PathsToNotMove.Add(folder_name.Text +
                        "/" + lastWrite.Year.ToString() +
                        "/" + lastWrite.Month.ToString() + " - " + lastWrite.ToString("MMMM"));
            }
            else
            {
                if (!PathsToNotMove.Contains(path))
                {

                    // get the file attributes for file or directory
                    FileAttributes attr = File.GetAttributes(path);

                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        String PathToMove = folder_name.Text +
                            "/" + lastWrite.Year.ToString() +
                            "/" + lastWrite.Month.ToString() + " - " + lastWrite.ToString("MMMM"); 

                        if(int.TryParse(Path.GetFileNameWithoutExtension(path), out int year ))
                        {
                            if (year > DateTime.Now.Year && year < 1900)
                            {
                                Console.WriteLine(PathToMove + "\\" + Path.GetFileNameWithoutExtension(path));
                                Directory.Move(path, PathToMove + "\\" + Path.GetFileNameWithoutExtension(path));
                            }

                        }
                        else
                        {
                            try
                            {
                                Directory.Move(path, PathToMove + "\\" + Path.GetFileNameWithoutExtension(path));
                            }
                            catch (Exception ex){
                                System.Windows.MessageBox.Show(path, PathToMove + "\\" + Path.GetFileNameWithoutExtension(path));
                            }
                           
                        }

                        

                    }
                    else
                    {
                        int count = 1;
                        while (true)
                        {
                            
                            try
                            { 
                                if(count == 1) {
                                    File.Move(path, folder_name.Text +
                                        "/" + lastWrite.Year.ToString() +
                                        "/" + lastWrite.Month.ToString() + " - " + lastWrite.ToString("MMMM") +
                                        "/" + Path.GetFileName(path));
                                }
                                else
                                {
                                    File.Move(path, folder_name.Text +
                                        "/" + lastWrite.Year.ToString() +
                                        "/" + lastWrite.Month.ToString() + " - " + lastWrite.ToString("MMMM") +
                                        "/"+ count.ToString() + "_" + Path.GetFileName(path));
                                }


                                break;
                            }
                            catch 
                            {
                                count++;
                            }
                        }

                }

                    
                }
            }
            
        }


    }
}
