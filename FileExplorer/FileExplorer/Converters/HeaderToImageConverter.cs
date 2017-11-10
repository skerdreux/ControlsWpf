namespace FileExplorer.Converters
{
    #region Usings

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;
    using global::FileExplorer.Properties;

    #endregion

    public class HeaderToImageConverter : IValueConverter
    {
        #region Champs et constantes statiques

        public static readonly HeaderToImageConverter Instance =
            new HeaderToImageConverter();

        #endregion

        #region Méthodes publiques

        /// <summary>
        ///     Convertit une valeur.
        /// </summary>
        /// <param name="value">Valeur produite par la source de liaison.</param>
        /// <param name="targetType">Type de la propriété de cible de liaison.</param>
        /// <param name="parameter">Paramètre de convertisseur à utiliser.</param>
        /// <param name="culture">Culture à utiliser dans le convertisseur.</param>
        /// <returns>
        ///     Valeur convertie. Si la méthode retourne null, la valeur null valide est utilisée.
        /// </returns>
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if (value != null && ((string) value).Contains(@"\"))
            {
                return ToBitmapImage(ResourceImage.drive);
            }
            return ToBitmapImage(ResourceImage.folder);
        }

        /// <summary>
        ///     Convertit une valeur.
        /// </summary>
        /// <param name="value">Valeur produite par la cible de liaison.</param>
        /// <param name="targetType">Type vers lequel effectuer la conversion.</param>
        /// <param name="parameter">Paramètre de convertisseur à utiliser.</param>
        /// <param name="culture">Culture à utiliser dans le convertisseur.</param>
        /// <returns>
        ///     Valeur convertie. Si la méthode retourne null, la valeur null valide est utilisée.
        /// </returns>
        /// <exception cref="System.NotSupportedException">Cannot convert back</exception>
        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture) =>
            throw new NotSupportedException("Cannot convert back");

        #endregion

        #region Méthodes privées

        /// <summary>
        ///     To the bitmap image.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns></returns>
        private static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        #endregion
    }
}
