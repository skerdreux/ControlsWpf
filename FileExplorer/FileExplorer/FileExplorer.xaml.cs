namespace FileExplorer
{
    #region Usings

    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;

    #endregion

    /// <summary>
    ///     Logique d'interaction pour FileExplorer.xaml
    /// </summary>
    public partial class FileExplorer
    {
        #region Champs et constantes statiques

        /// <summary>
        /// The pattern file property
        /// </summary>
        public static readonly DependencyProperty PatternFileProperty =
            DependencyProperty.Register("PatternFile", typeof(string),
                                        typeof(FileExplorer), new PropertyMetadata(string.Empty));
                                        
        /// <summary>
        ///     The selected file property
        /// </summary>
        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register("SelectedFile", typeof(string),
                                        typeof(FileExplorer), new PropertyMetadata(string.Empty));

        /// <summary>
        ///     The selected file property
        /// </summary>
        public static readonly DependencyProperty DefaultDriveProperty =
            DependencyProperty.Register("DefaultDrive", typeof(string),
                                        typeof(FileExplorer), new PropertyMetadata(DefaultDrivePropertyChangedHandler));

        /// <summary>
        /// Defaults the drive property changed handler.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void DefaultDrivePropertyChangedHandler(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            dependencyObject.SetValue(DefaultDriveProperty, dependencyPropertyChangedEventArgs.NewValue);
            var owner = ((FileExplorer) dependencyObject);
            foreach (TreeViewItem treeViewItem in owner.TreeViewFolder.Items)
            {
                if (treeViewItem != null)
                {
                    treeViewItem.IsExpanded = string.Compare(treeViewItem.Tag.ToString(),
                                                             dependencyPropertyChangedEventArgs.NewValue.ToString(),
                                                             StringComparison.InvariantCultureIgnoreCase) == 0;
                }
            }
        }
        
        
        #endregion

        #region Champs

        private readonly object dummyNode = null;

        #endregion

        #region Constructeurs et destructeurs

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileExplorer" /> class.
        /// </summary>
        public FileExplorer()
        {
            this.InitializeComponent();
            this.ListDirectory();
            this.TreeViewFolder.SelectedItemChanged += this.TreeViewFolderOnSelectedItemChanged;
        }

        #endregion

        #region Propriétés et indexeurs

        /// <summary>
        ///     Gets or sets the default drive.
        /// </summary>
        /// <value>
        ///     The default drive.
        /// </value>
        public string DefaultDrive
        {
            get => (string) this.GetValue(DefaultDriveProperty);
            set => this.SetValue(DefaultDriveProperty, value);
        }


        /// <summary>
        ///     Gets or sets the pattern file.
        /// </summary>
        /// <value>
        ///     The pattern file.
        /// </value>
        public string PatternFile
        {
            get => (string)this.GetValue(PatternFileProperty);
            set => this.SetValue(PatternFileProperty, value);
        }

        /// <summary>
        ///     Gets or sets the selected file.
        /// </summary>
        /// <value>
        ///     The selected file.
        /// </value>
        public string SelectedFile
        {
            get => (string) this.GetValue(SelectedFileProperty);
            set => this.SetValue(SelectedFileProperty, value);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        ///     Creates the directory node.
        /// </summary>
        /// <param name="directoryInfo">The directory information.</param>
        /// <returns></returns>
        private TreeViewItem CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeViewItem
            {
                Header = directoryInfo.Name,
                Tag = directoryInfo.Name,
                FontWeight = FontWeights.Normal,
                Items = {this.dummyNode}
            };
            directoryNode.Expanded += this.FolderExpanded;

            return directoryNode;
        }

        /// <summary>
        ///     Folders the expanded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void FolderExpanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem) sender;
            if (item.Items.Count == 1 && item.Items[0] == this.dummyNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (var directoryPath in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        var subitem = new TreeViewItem
                        {
                            Header = directoryPath.Substring(
                                directoryPath.LastIndexOf("\\", StringComparison.Ordinal) + 1),
                            Tag = directoryPath,
                            FontWeight = FontWeights.Normal
                        };
                        if (Directory.GetDirectories(directoryPath).Length > 0)
                        {
                            subitem.Items.Add(this.dummyNode);
                        }
                        subitem.Expanded += this.FolderExpanded;
                        item.Items.Add(subitem);
                    }
                }
                catch (Exception)
                {
                    // ignored ==> dans le cas d'un accès impossible à un répertoire
                }
            }
        }

        /// <summary>
        ///     Lists the directory.
        /// </summary>
        private void ListDirectory()
        {
            this.TreeViewFolder.Items.Clear();

            var listDrives = Directory.GetLogicalDrives();
            foreach (var drive in listDrives)
            {
                var rootDirectoryInfo = new DirectoryInfo(drive);
                var myItem = this.CreateDirectoryNode(rootDirectoryInfo);
                this.TreeViewFolder.Items.Add(myItem);
            }
        }

        /// <summary>
        ///     TreeViews the folder on selected item changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="routedPropertyChangedEventArgs">The <see cref="object" /> instance containing the event data.</param>
        private void TreeViewFolderOnSelectedItemChanged(object sender,
                                                         RoutedPropertyChangedEventArgs<object>
                                                             routedPropertyChangedEventArgs)
        {
            // récupération du répertoire
            var fullPath = ((sender as TreeView)?.SelectedItem as TreeViewItem)?.Tag.ToString();
            if (!string.IsNullOrEmpty(fullPath))
            {
                // filtrage en fonction de la regex
                var regex = new Regex(this.PatternFile);
                var listFiles = Directory.GetFiles(fullPath).Where(path => regex.IsMatch(path))
                                         .Select(path => new FileInfo(path)).ToList();
                this.ListFichier.ItemsSource = listFiles;
            }
        }

        #endregion

        private void ListFichier_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedFile = ((sender as ListBox)?.SelectedItem as FileInfo)?.FullName;
        }
    }
}
