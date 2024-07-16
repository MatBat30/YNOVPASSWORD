using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YnovPassword.general;
using YnovPassword.modele;
using System.Collections.ObjectModel;

namespace YnovPassword
{
    public partial class AddProfileWindow : Window
    {
        // Collection observable pour stocker les dossiers disponibles
        private ObservableCollection<Dossiers> _dossiers;
        // Identifiant de l'utilisateur actuel
        private Guid _userId;

        // Événement déclenché lorsqu'un profil est ajouté
        public event EventHandler<ProfilsData> ProfileAdded;

        // Constructeur de la fenêtre AddProfileWindow
        public AddProfileWindow(Guid userId)
        {
            InitializeComponent(); // Initialisation des composants de la fenêtre
            _userId = userId; // Assignation de l'ID utilisateur
            _dossiers = new ObservableCollection<Dossiers>(); // Initialisation de la collection des dossiers
            LoadDossiers(); // Chargement des dossiers disponibles
            InputFields_TextChanged(null, null); // Initialisation de l'état du bouton Ajouter
        }

        // Chargement des dossiers depuis la base de données
        private void LoadDossiers()
        {
            using (var context = new DataContext())
            {
                // Récupération des dossiers à l'exception de ceux nommés comme une constante spécifique
                _dossiers = new ObservableCollection<Dossiers>(
                    context.Dossiers.Where(d => d.Nom != classConstantes.sTypeprofilConnection_Nom_YnovPassword).ToList()
                );

                if (_dossiers.Count == 0)
                {
                    // Si aucun dossier n'est disponible, affichez un message et ouvrez une fenêtre pour en créer un
                    MessageBox.Show("Aucun dossier disponible. Veuillez créer un dossier d'abord.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CreateDossierWindow createDossierWindow = new CreateDossierWindow();
                    if (createDossierWindow.ShowDialog() == true && createDossierWindow.CreatedDossier != null)
                    {
                        _dossiers.Add(createDossierWindow.CreatedDossier);
                    }
                    else
                    {
                        // Si la création de dossier échoue, désactivez la ComboBox et affichez un message
                        DossierComboBox.Items.Add("Aucun dossier disponible");
                        DossierComboBox.IsEnabled = false;
                    }
                }
                else
                {
                    // Si des dossiers sont disponibles, les afficher dans la ComboBox
                    DossierComboBox.DisplayMemberPath = "Nom";
                    DossierComboBox.ItemsSource = _dossiers;
                    DossierComboBox.SelectedIndex = 0;
                }
            }
        }

        // Gestionnaire d'événements pour le bouton Ajouter
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupération des valeurs des champs de texte
            string nom = NomTextBox.Text.Trim();
            string url = UrlTextBox.Text.Trim();
            string login = LoginTextBox.Text.Trim();
            string password = PasswordTextBox.Text.Trim();

            // Vérification que les champs requis ne sont pas vides et qu'un dossier est sélectionné
            if (string.IsNullOrEmpty(nom) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password) || DossierComboBox.SelectedItem == null || DossierComboBox.SelectedItem.ToString() == "Aucun dossier disponible")
            {
                MessageBox.Show("Tous les champs doivent être remplis.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Sortie de la méthode si une validation échoue
            }

            var selectedDossier = DossierComboBox.SelectedItem as Dossiers;
            if (selectedDossier == null)
            {
                MessageBox.Show("Veuillez sélectionner un dossier.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Utilisation d'un contexte de base de données pour ajouter le nouveau profil
            using (var context = new DataContext())
            {
                string encryptedPassword = classFonctionGenerale.CrypterChaine(password); // Cryptage du mot de passe

                var newProfil = new ProfilsData
                {
                    ID = Guid.NewGuid(), // Génération d'un nouvel ID pour le profil
                    UtilisateursID = _userId, // Assignation de l'ID utilisateur
                    DossiersID = selectedDossier.ID, // Assignation de l'ID du dossier sélectionné
                    Nom = nom, // Assignation du nom
                    URL = url, // Assignation de l'URL
                    Login = login, // Assignation du login
                    EncryptedPassword = encryptedPassword // Assignation du mot de passe crypté
                };
                context.ProfilsData.Add(newProfil); // Ajout du profil au contexte
                context.SaveChanges(); // Sauvegarde des changements dans la base de données

                ProfileAdded?.Invoke(this, newProfil); // Déclenchement de l'événement ProfileAdded
            }

            // Affichage d'un message de succès et fermeture de la fenêtre
            MessageBox.Show("Profil ajouté avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        // Gestionnaire d'événements pour le bouton Annuler
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
                PasswordTextBox.Text = generatePasswordWindow.GeneratedPassword;
            }
        }

        // Gestionnaire d'événements pour le changement de texte dans les champs de saisie
        private void InputFields_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Activation ou désactivation du bouton Ajouter en fonction de la validité des champs
            AddButton.IsEnabled = !string.IsNullOrEmpty(NomTextBox.Text.Trim()) &&
                                  !string.IsNullOrEmpty(LoginTextBox.Text.Trim()) &&
                                  !string.IsNullOrEmpty(PasswordTextBox.Text.Trim()) &&
                                  DossierComboBox.SelectedItem != null &&
                                  DossierComboBox.SelectedItem.ToString() != "Aucun dossier disponible";

            AddButton.Opacity = AddButton.IsEnabled ? 1 : 0.5; // Changement de l'opacité pour indiquer visuellement l'état désactivé
        }

        // Gestionnaire d'événements pour le bouton d'aide
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            classFonctionGenerale.OuvrirAide(); // Appel de la méthode pour ouvrir l'aide
        }
    }
}
