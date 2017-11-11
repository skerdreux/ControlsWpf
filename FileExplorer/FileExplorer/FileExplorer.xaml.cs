namespace FileExplorer
{
    #region Usings

    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
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

        public static readonly DependencyProperty PatternFileProperty =
            DependencyProperty.Register("PatternFile", typeof(string),
                                        typeof(FileExplorer), new PropertyMetadata(""));

        /// <summary>
        ///     The selected file property
        /// </summary>
        public static readonly DependencyProperty SelectedFileProperty =
            DependencyProperty.Register("SelectedFile", typeof(string),
                                        typeof(FileExplorer), new PropertyMetadata(""));

        /// <summary>
        ///     The selected file property
        /// </summary>
        public static readonly DependencyProperty DefaultDriveProperty =
            DependencyProperty.Register("DefaultDrive", typeof(string),
                                        typeof(FileExplorer), new PropertyMetadata(""));

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
            InitializeComponent();

            // on sélectionne le disque par défaut si non saisie
            if (string.IsNullOrEmpty(DefaultDrive))
            {
                DefaultDrive = Path.GetPathRoot(Assembly.GetExecutingAssembly().Location);
            }

            ListDirectory(TreeViewFolder, DefaultDrive);

            TreeViewFolder.SelectedItemChanged += TreeViewFolderOnSelectedItemChanged;
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
            get => (string) GetValue(DefaultDriveProperty);
            set => SetValue(DefaultDriveProperty, value);
        }


        /// <summary>
        ///     Gets or sets the pattern file.
        /// </summary>
        /// <value>
        ///     The pattern file.
        /// </value>
        public Regex PatternFile
        {
            get
            {
                var regexp = new Regex(GetValue(PatternFileProperty).ToString());
                return regexp;
            }
            set => SetValue(PatternFileProperty, value);
        }

        /// <summary>
        ///     Gets or sets the selected file.
        /// </summary>
        /// <value>
        ///     The selected file.
        /// </value>
        public string SelectedFile
        {
            get => (string) GetValue(SelectedFileProperty);
            set => SetValue(SelectedFileProperty, value);
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
                Items = {dummyNode}
            };
            directoryNode.Expanded += FolderExpanded;

            return directoryNode;
        }

        /// <summary>
        /// Folders the expanded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void FolderExpanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem) sender;
            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
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
                            subitem.Items.Add(dummyNode);
                        }
                        subitem.Expanded += FolderExpanded;
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
        /// <param name="treeView">The tree view.</param>
        /// <param name="path">The path.</param>
        private void ListDirectory(TreeView treeView, string path)
        {
            treeView.Items.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Items.Add(CreateDirectoryNode(rootDirectoryInfo));
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
                var listFiles = Directory.GetFiles(fullPath).Where(path => PatternFile.IsMatch(path))
                                         .Select(path => new FileInfo(path)).ToList();
                ListFichier.ItemsSource = listFiles;
            }
        }

        #endregion
    }
}
