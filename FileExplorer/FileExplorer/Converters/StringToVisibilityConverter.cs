namespace FileExplorer.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    /// <summary>
    /// string.empty = visibility.collapsed otherwhise visible
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class StringToVisibilityConverter : IValueConverter
    {
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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;

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
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
