using System;
using System.Windows;
using YnovPassword.modele;
using YnovPassword.general;

namespace YnovPassword
{
    public partial class EditProfileWindow : Window
    {
        // Instance de ProfilsData pour stocker le profil à éditer
        private ProfilsData _profil;

        // Constructeur de la fenêtre EditProfileWindow
        public EditProfileWindow(ProfilsData profil)
        {
            InitializeComponent(); // Initialisation des composants de la fenêtre
            _profil = profil; // Assignation du profil à éditer
            LoadProfileData(); // Chargement des données du profil dans les champs
        }

        // Méthode pour charger les données du profil dans les champs de texte
        private void LoadProfileData()
        {
            // Remplir les champs avec les données du profil
            NomTextBox.Text = _profil.Nom; // Chargement du nom
            URLTextBox.Text = _profil.URL; // Chargement de l'URL
            LoginTextBox.Text = _profil.Login; // Chargement du login
            PasswordTextBox.Password = classFonctionGenerale.DecrypterChaine(_profil.EncryptedPassword); // Décryptage et chargement du mot de passe
        }

        // Gestionnaire d'événements pour le bouton de sauvegarde
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Mise à jour des données du profil avec les valeurs des champs
                _profil.Nom = NomTextBox.Text;
                _profil.URL = URLTextBox.Text;
                _profil.Login = LoginTextBox.Text;
                _profil.EncryptedPassword = classFonctionGenerale.CrypterChaine(PasswordTextBox.Password); // Cryptage du mot de passe

                // Utilisation d'un contexte de base de données pour mettre à jour le profil
                using (var context = new DataContext())
                {
                    context.ProfilsData.Update(_profil); // Mise à jour du profil dans le contexte
                    context.SaveChanges(); // Sauvegarde des changements dans la base de données
                }

                // Affichage d'un message de succès et fermeture de la fenêtre
                MessageBox.Show("Profil mis à jour avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true; // Réglage de DialogResult à true pour indiquer une sauvegarde réussie
                this.Close(); // Fermeture de la fenêtre
            }
            catch (Exception ex)
            {
                // Gestion des erreurs
                classFonctionGenerale.GestionErreurLog(ex, "Erreur lors de la mise à jour du profil", false);
            }
        }

        // Gestionnaire d'événements pour le bouton d'annulation
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Réglage de DialogResult à false pour indiquer une annulation
            this.Close(); // Fermeture de la fenêtre
        }

        // Gestionnaire d'événements pour le bouton de génération de mot de passe
        private void GeneratePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // Création et affichage de la fenêtre de génération de mot de passe
            GeneratePasswordWindow generatePasswordWindow = new GeneratePasswordWindow();
            if (generatePasswordWindow.ShowDialog() == true)
            {
                // Si un mot de passe est généré, le charger dans le champ de mot de passe
                PasswordTextBox.Password = generatePasswordWindow.GeneratedPassword;
            }
        }

        // Gestionnaire d'événements pour le bouton d'aide
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OuvrirAide(); // Appel de la méthode pour ouvrir l'aide
        }
    }
}
