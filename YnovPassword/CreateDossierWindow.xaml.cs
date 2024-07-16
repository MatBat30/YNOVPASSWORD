using System;
using System.Windows;
using YnovPassword.general;
using YnovPassword.modele;

namespace YnovPassword
{
    public partial class CreateDossierWindow : Window
    {
        // Propriété pour stocker le dossier créé
        public Dossiers? CreatedDossier { get; private set; }

        // Constructeur de la fenêtre CreateDossierWindow
        public CreateDossierWindow()
        {
            InitializeComponent(); // Initialisation des composants de la fenêtre
        }

        // Gestionnaire d'événements pour le bouton de création de dossier
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupération et trim du nom du dossier
            string dossierName = DossierNameTextBox.Text.Trim();

            // Vérification que le nom du dossier n'est pas vide
            if (string.IsNullOrEmpty(dossierName))
            {
                MessageBox.Show("Le nom du dossier ne peut pas être vide.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Sortie de la méthode si le nom est vide
            }

            // Utilisation d'un contexte de base de données pour ajouter le nouveau dossier
            using (var context = new DataContext())
            {
                var newDossier = new Dossiers
                {
                    ID = Guid.NewGuid(), // Génération d'un nouvel ID pour le dossier
                    Nom = dossierName // Assignation du nom du dossier
                };
                context.Dossiers.Add(newDossier); // Ajout du dossier au contexte
                context.SaveChanges(); // Sauvegarde des changements dans la base de données
                CreatedDossier = newDossier; // Stockage du dossier créé dans la propriété CreatedDossier
            }

            // Affichage d'un message de succès et fermeture de la fenêtre
            MessageBox.Show("Dossier créé avec succès.", "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true; // Réglage de DialogResult à true pour indiquer une création réussie
            this.Close(); // Fermeture de la fenêtre
        }

        // Gestionnaire d'événements pour le bouton d'annulation
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Réglage de DialogResult à false pour indiquer une annulation
            this.Close(); // Fermeture de la fenêtre
        }

        // Gestionnaire d'événements pour le bouton d'aide
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
