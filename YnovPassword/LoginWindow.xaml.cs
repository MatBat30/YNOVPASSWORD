using System.Linq;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;

namespace YnovPassword
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent(); // Initialisation des composants de la fenêtre
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupération des valeurs saisies par l'utilisateur
            string username = this.txtUsername.Text.Trim();
            string password = this.txtPassword.Password.Trim();

            // Vérification que les champs ne sont pas vides
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Le nom d'utilisateur et le mot de passe ne peuvent pas être vides.", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validation de l'utilisateur
            if (ValidateUser(username, password, out Guid userId))
            {
                App.LoggedInUserId = userId; // Stocker l'ID utilisateur après une connexion réussie
                MainWindow mainWindow = new MainWindow(); // Création de la fenêtre principale
                mainWindow.Show(); // Affichage de la fenêtre principale
                this.Close(); // Fermeture de la fenêtre de connexion
            }
            else
            {
                // Affichage d'un message d'erreur si les identifiants sont incorrects
                MessageBox.Show("Identifiant ou mot de passe incorrect.", "Erreur de connexion", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Méthode pour valider les informations de connexion de l'utilisateur
        private bool ValidateUser(string username, string password, out Guid userId)
        {
            userId = Guid.Empty; // Initialisation de l'ID utilisateur
            bool isUserValid;
            using (DataContext dataContext = new DataContext())
            {
                // Recherche de l'utilisateur dans la base de données
                var user = dataContext.Utilisateurs.FirstOrDefault(u =>
                    u.Login == username &&
                    u.ProfilsData.Single().EncryptedPassword == classFonctionGenerale.CrypterChaine(password));

                if (user != null)
                {
                    isUserValid = true;
                    userId = user.ID; // Récupération de l'ID utilisateur
                }
                else
                {
                    isUserValid = false;
                }
            }
            return isUserValid;
        }

        // Gestionnaire d'événements pour le bouton "Créer un utilisateur"
        private void CreateUserButton_Click(object sender, RoutedEventArgs e)
        {
            NewUserWindow newUserWindow = new NewUserWindow(); // Création de la fenêtre de création d'un nouvel utilisateur
            newUserWindow.ShowDialog(); // Affichage de la fenêtre de création d'un nouvel utilisateur
        }

        // Gestionnaire d'événements pour le bouton "Aide"
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OuvrirAide(); // Appel de la méthode pour ouvrir l'aide
        }

        private void OpenHelp_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OpenHelp();
        }

    }
}
