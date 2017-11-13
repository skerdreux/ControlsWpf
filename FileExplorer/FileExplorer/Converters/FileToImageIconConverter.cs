using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using FileExplorer.Helpers;
using FileExplorer.Properties;

namespace FileExplorer.Converters
{
    internal class FileToImageIconConverter : System.Windows.Data.IValueConverter
    {

        /// <summary>
        /// The liste extension icone
        /// </summary>
        public Dictionary<string,BitmapImage> ListeExtensionIcone = new Dictionary<string, BitmapImage>();

        /// <summary>
        /// The instance
        /// </summary>
        public static readonly FileToImageIconConverter Instance =
            new FileToImageIconConverter();

        /// <summary>
        /// Convertit une valeur.
        /// </summary>
        /// <param name="value">Valeur produite par la source de liaison.</param>
        /// <param name="targetType">Type de la propriété de cible de liaison.</param>
        /// <param name="parameter">Paramètre de convertisseur à utiliser.</param>
        /// <param name="culture">Culture à utiliser dans le convertisseur.</param>
        /// <returns>
        /// Valeur convertie. Si la méthode retourne null, la valeur null valide est utilisée.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is FileInfo fileInfo))
            {
                return null;
            }

            if (this.ListeExtensionIcone.ContainsKey(fileInfo.Extension))
            {
                return this.ListeExtensionIcone[fileInfo.Extension];
            }
            using (var sysicon = Icon.ExtractAssociatedIcon(fileInfo.FullName))
            {
                if (sysicon != null)
                {
                    var icon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                        sysicon.Handle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                    return icon;
                }
            }

            // icon par défaut
            return BitmapHelpers.ToBitmapImage(ResourceImage.file);
        }

        /// <summary>
        /// Convertit une valeur.
        /// </summary>
        /// <param name="value">Valeur produite par la cible de liaison.</param>
        /// <param name="targetType">Type vers lequel effectuer la conversion.</param>
        /// <param name="parameter">Paramètre de convertisseur à utiliser.</param>
        /// <param name="culture">Culture à utiliser dans le convertisseur.</param>
        /// <returns>
        /// Valeur convertie. Si la méthode retourne null, la valeur null valide est utilisée.
        /// </returns>
        /// <exception cref="NotSupportedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }


    }
}
