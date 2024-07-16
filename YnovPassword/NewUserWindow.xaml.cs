using System;
using System.Linq;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;

namespace YnovPassword
{
    public partial class NewUserWindow : Window
    {
        // Constructeur de la fenêtre NewUserWindow
        public NewUserWindow()
        {
            InitializeComponent(); // Initialisation des composants de la fenêtre
        }

        // Gestionnaire d'événements pour le bouton de sauvegarde
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupération et trim des valeurs des champs de texte
            string login = txtLogin.Text.Trim();
            string email = txtEmail.Text.Trim();
            string nom = txtNom.Text.Trim();
            string password = txtPassword.Password.Trim();
            string confirmPassword = txtConfirmPassword.Password.Trim();

            // Vérification que tous les champs sont remplis
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Tous les champs doivent être remplis.", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Sortie de la méthode si des champs sont vides
            }

            // Validation des mots de passe (doivent correspondre et être d'une longueur minimale de 8 caractères)
            if (!classFonctionGenerale.ValiderMotdepasse(password, confirmPassword))
            {
                MessageBox.Show("Les mots de passe ne correspondent pas ou ne respectent pas les critères de sécurité (minimum 8 caractères).", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Sortie de la méthode si la validation échoue
            }

            // Utilisation d'un contexte de base de données
            using (var context = new DataContext())
            {
                // Vérifiez si le dossier existe
                var dossier = context.Dossiers.FirstOrDefault(d => d.Nom == classConstantes.sTypeprofilConnection_Nom_YnovPassword);

                // Si le dossier n'existe pas, affichez un message d'erreur et arrêtez l'opération
                if (dossier == null)
                {
                    MessageBox.Show("Le dossier spécifié n'existe pas.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Sortie de la méthode si le dossier n'existe pas
                }

                // Création de l'utilisateur
                Guid userId = Guid.NewGuid(); // Génération d'un nouvel ID pour l'utilisateur
                string encryptedPassword = classFonctionGenerale.CrypterChaine(password); // Cryptage du mot de passe

                // Création d'une nouvelle instance de l'utilisateur
                var newUser = new Utilisateurs
                {
                    ID = userId,
                    Nom = nom,
                    Login = login,
                    Email = email,
                    // Ajoutez d'autres propriétés si nécessaire
                };

                // Ajout de l'utilisateur au contexte
                context.Utilisateurs.Add(newUser);

                // Création du profil associé à l'utilisateur
                var newProfilsData = new ProfilsData
                {
                    ID = Guid.NewGuid(), // Génération d'un nouvel ID pour le profil
                    UtilisateursID = userId, // Association de l'utilisateur
                    DossiersID = dossier.ID, // Utilisation de l'ID du dossier existant
                    Nom = nom, // Nom du profil (ajustez selon votre besoin)
                    URL = "", // URL du profil (ajustez selon votre besoin)
                    Login = login,
                    EncryptedPassword = encryptedPassword // Mot de passe crypté
                };

                // Ajout du profil au contexte
                context.ProfilsData.Add(newProfilsData);

                // Sauvegarde des changements dans la base de données
                context.SaveChanges();
            }

            // Affichage d'un message de succès et fermeture de la fenêtre
            MessageBox.Show("Utilisateur enregistré avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close(); // Fermeture de la fenêtre après succès
        }

        // Gestionnaire d'événements pour le bouton de fermeture
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Fermeture de la fenêtre sans effectuer d'actions supplémentaires
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OuvrirAide();
        }
        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }
    }
}
