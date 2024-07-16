using System;
using System.Linq;
using System.Text;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;

namespace YnovPassword
{
    public partial class GeneratePasswordWindow : Window
    {
        // Propriété pour stocker le mot de passe généré
        public string GeneratedPassword { get; private set; }

        // Constructeur de la fenêtre GeneratePasswordWindow
        public GeneratePasswordWindow()
        {
            InitializeComponent(); // Initialisation des composants de la fenêtre
            GeneratedPassword = string.Empty; // Initialisation de GeneratedPassword
            LengthSlider.ValueChanged += LengthSlider_ValueChanged; // Attachement du gestionnaire d'événements pour le slider de longueur
            LengthSlider_ValueChanged(null, null); // Initialisation du TextBlock avec la valeur par défaut du slider
        }

        // Gestionnaire d'événements pour l'activation de la génération de passphrase
        private void GeneratePassphrase_Checked(object sender, RoutedEventArgs e)
        {
            LengthSlider.Minimum = 3; // Réglage de la longueur minimale pour les passphrases
            LengthSlider.Maximum = 16; // Réglage de la longueur maximale pour les passphrases
            LengthSlider.Value = 4; // Longueur par défaut pour les passphrases
        }

        // Gestionnaire d'événements pour la désactivation de la génération de passphrase
        private void GeneratePassphrase_Unchecked(object sender, RoutedEventArgs e)
        {
            LengthSlider.Minimum = 8; // Réglage de la longueur minimale pour les mots de passe
            LengthSlider.Maximum = 256; // Réglage de la longueur maximale pour les mots de passe
            LengthSlider.Value = 16; // Longueur par défaut pour les mots de passe
        }

        // Gestionnaire d'événements pour le bouton de génération de mot de passe
        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (GeneratePassphrase.IsChecked == true)
            {
                // Génération de passphrase
                GeneratedPassword = GeneratePassphraseMethod((int)LengthSlider.Value);
            }
            else
            {
                // Génération de mot de passe
                GeneratedPassword = GeneratePassword((int)LengthSlider.Value, IncludeSpecialChars.IsChecked == true, IncludeNumbers.IsChecked == true);
            }

            // Affichage du mot de passe généré dans la TextBox
            GeneratedPasswordTextBox.Text = GeneratedPassword;
        }

        // Gestionnaire d'événements pour le bouton d'utilisation du mot de passe
        private void UsePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true; // Réglage de DialogResult à true pour indiquer que le mot de passe est utilisé
            this.Close(); // Fermeture de la fenêtre
        }

        // Méthode pour générer un mot de passe
        private string GeneratePassword(int length, bool includeSpecialChars, bool includeNumbers)
        {
            // Définition des ensembles de caractères
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&*()_-+=<>?";

            StringBuilder charSet = new StringBuilder(letters);
            if (includeNumbers)
            {
                charSet.Append(numbers); // Ajout des chiffres si nécessaire
            }
            if (includeSpecialChars)
            {
                charSet.Append(specialChars); // Ajout des caractères spéciaux si nécessaire
            }

            Random random = new Random();
            // Génération du mot de passe en sélectionnant des caractères aléatoires
            return new string(Enumerable.Repeat(charSet.ToString(), length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Méthode pour générer une passphrase
        private string GeneratePassphraseMethod(int wordCount)
        {
            using (var context = new DataContext())
            {
                // Récupération des mots du dictionnaire
                var words = context.Dictionnaires.Select(d => d.Mot).ToList();
                if (words.Count < wordCount)
                {
                    throw new InvalidOperationException("Pas assez de mots dans le dictionnaire pour générer une passphrase.");
                }

                Random random = new Random();
                // Sélection de mots aléatoires
                var selectedWords = words.OrderBy(x => random.Next()).Take(wordCount).ToArray();
                // Génération de la passphrase en joignant les mots avec un "+"
                var passphrase = string.Join("+", selectedWords) + random.Next(0, 10); // Ajout d'un chiffre à la fin pour plus de sécurité

                return passphrase;
            }
        }

        // Gestionnaire d'événements pour le changement de valeur du slider de longueur
        private void LengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PasswordLengthTextBlock != null)
            {
                // Mise à jour du TextBlock avec la valeur actuelle du slider
                PasswordLengthTextBlock.Text = LengthSlider.Value.ToString("N0");
            }
        }

        // Gestionnaire d'événements pour le bouton d'aide
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OuvrirAide(); // Appel de la méthode pour ouvrir l'aide
        }
    }
}
