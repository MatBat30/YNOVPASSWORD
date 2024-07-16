using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using YnovPassword.modele;
using System.Collections.ObjectModel;
using System.Windows.Input;
using YnovPassword.general;
using System.Diagnostics;
using System.IO;

namespace YnovPassword
{
    public partial class MainWindow : Window
    {
        // Collections pour stocker les données des profils
        private ObservableCollection<ProfilsData> _profilsData = new ObservableCollection<ProfilsData>();
        private ObservableCollection<ProfilsData> _filteredProfilsData = new ObservableCollection<ProfilsData>();
        private ProfilsData _selectedProfile;

        public MainWindow()
        {
            InitializeComponent(); // Initialisation des composants de la fenêtre
            LoadProfilsData(); // Chargement des données des profils
        }

        // Chargement des données des profils depuis la base de données
        private void LoadProfilsData()
        {
            using (var context = new DataContext())
            {
                var userId = App.LoggedInUserId;
                // Récupération des profils de l'utilisateur connecté
                _profilsData = new ObservableCollection<ProfilsData>(context.ProfilsData.Where(p => p.UtilisateursID == userId).ToList());
                _filteredProfilsData = new ObservableCollection<ProfilsData>(_profilsData);
                dataGridProfils.ItemsSource = _filteredProfilsData; // Affectation des données au DataGrid
            }
        }

        // Ouverture de la fenêtre des paramètres
        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.Show();
        }

        // Suppression d'un profil
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ProfilsData item)
            {
                // Vérification que l'utilisateur ne supprime pas son propre profil
                if (item.UtilisateursID == App.LoggedInUserId)
                {
                    MessageBox.Show("Vous ne pouvez pas supprimer votre propre profil.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var context = new DataContext())
                {
                    // Suppression du profil de la base de données et des collections
                    context.ProfilsData.Remove(item);
                    context.SaveChanges();
                    _profilsData.Remove(item);
                    _filteredProfilsData.Remove(item);
                }
            }
        }

        // Modification d'un profil
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ProfilsData item)
            {
                EditProfileWindow editProfileWindow = new EditProfileWindow(item);
                // Rechargement des données des profils après la modification
                if (editProfileWindow.ShowDialog() == true)
                {
                    LoadProfilsData();
                }
            }
        }

        // Ajout d'un nouveau profil
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Guid userId = App.LoggedInUserId;
            AddProfileWindow addProfileWindow = new AddProfileWindow(userId);
            addProfileWindow.ProfileAdded += AddProfileWindow_ProfileAdded;
            addProfileWindow.ShowDialog();
        }

        // Ajout du nouveau profil aux collections après la création
        private void AddProfileWindow_ProfileAdded(object sender, ProfilsData e)
        {
            _profilsData.Add(e);
            _filteredProfilsData.Add(e);
            dataGridProfils.Items.Refresh();
        }

        // Fermeture de la fenêtre principale
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Affichage du mot de passe sélectionné
        private void ShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ProfilsData item)
            {
                _selectedProfile = item;
                // Décryptage et affichage du mot de passe
                PasswordTextBox.Text = classFonctionGenerale.DecrypterChaine(item.EncryptedPassword);
                PasswordLabel.Visibility = Visibility.Visible;
                PasswordTextBox.Visibility = Visibility.Visible;
            }
        }

        // Copie du mot de passe sélectionné dans le presse-papiers
        private void CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ProfilsData item)
            {
                Clipboard.SetText(classFonctionGenerale.DecrypterChaine(item.EncryptedPassword));
                MessageBox.Show("Mot de passe copié dans le presse-papiers.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Réinitialisation de l'affichage du mot de passe lors du changement de sélection dans le DataGrid
        private void DataGridProfils_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PasswordLabel.Visibility = Visibility.Collapsed;
            PasswordTextBox.Visibility = Visibility.Collapsed;
        }

        // Simulation d'un crash de l'application pour tester la gestion des erreurs
        private void CrashApi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int? divisor = null; // Définissez une variable nullable qui provoquera une division par zéro
                int result = 10 / divisor.Value; // Provoque une exception de division par zéro
            }
            catch (Exception ex)
            {
                classFonctionGenerale.GestionErreurLog(ex, "Erreur lors de la division par zéro dans CrashApi_Click", false);
            }
        }

        // Ouverture de l'aide
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                classFonctionGenerale.OuvrirAide();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible d'ouvrir le fichier d'aide.\nDétails : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Filtrage des profils affichés dans le DataGrid en fonction du texte de recherche
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string filter = SearchTextBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(filter))
            {
                _filteredProfilsData = new ObservableCollection<ProfilsData>(_profilsData);
            }
            else
            {
                _filteredProfilsData = new ObservableCollection<ProfilsData>(
                    _profilsData.Where(p => p.Nom.ToLower().Contains(filter)));
            }
            dataGridProfils.ItemsSource = _filteredProfilsData;
        }
    }
}
