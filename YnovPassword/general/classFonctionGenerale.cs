using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YnovPassword.general;
using DllYnov;
using LoggerLibrary;

namespace YnovPassword.general
{
    // Classe abstraite contenant des fonctions utilitaires générales
    public abstract class classFonctionGenerale
    {
        // Méthode pour ouvrir le fichier d'aide
        public static void OuvrirAide()
        {
            try
            {
                // Chemin du fichier d'aide
                string helpFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Aide.chm");
                if (File.Exists(helpFilePath))
                {
                    // Configuration des informations de démarrage du processus
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = helpFilePath,
                        UseShellExecute = true
                    };
                    Process.Start(psi); // Démarrage du processus
                }
                else
                {
                    // Affichage d'un message d'erreur si le fichier d'aide est introuvable
                    MessageBox.Show("Le fichier d'aide est introuvable.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                // Gestion des erreurs lors de l'ouverture du fichier d'aide
                GestionErreurLog(ex, "Erreur lors de l'ouverture du fichier d'aide", false);
            }
        }

        // Méthode pour gérer et enregistrer les erreurs
        public static void GestionErreurLog(Exception? ex, string? MessageLibre, bool boolFermeture)
        {
            string MessageFinal = "";
            if (!string.IsNullOrEmpty(MessageLibre))
            {
                MessageFinal += MessageLibre; // Ajout du message libre au message final
            }
            if (ex != null)
            {
                // Ajout des informations de l'exception au message final
                MessageFinal += "\r\n Une Erreur a eu lieu : " + ex.ToString();
                Logger.Error(MessageFinal); // Enregistrement de l'erreur dans les logs
            }

            // Affichage d'un message d'erreur
            MessageBox.Show(MessageFinal, "Erreur Rencontrée", MessageBoxButton.OK, MessageBoxImage.Error);

            // Fermeture de l'application si boolFermeture est vrai
            if (boolFermeture)
            {
                Application.Current.Shutdown();
            }
        }

        // Méthode pour crypter une chaîne de caractères
        public static string? CrypterChaine(string sPar_ChaineACrypter)
        {
            try
            {
                return Cryptage.DLLCrypterChaine(sPar_ChaineACrypter); // Utilisation d'une DLL pour crypter la chaîne
            }
            catch (Exception ex)
            {
                // Gestion des erreurs lors du cryptage de la chaîne
                GestionErreurLog(ex, "Erreur lors du cryptage de la chaîne", false);
                return null;
            }
        }

        // Méthode pour décrypter une chaîne de caractères
        public static string? DecrypterChaine(string sPar_ChaineCryptee)
        {
            try
            {
                return Cryptage.DLLDecrypterChaine(sPar_ChaineCryptee); // Utilisation d'une DLL pour décrypter la chaîne
            }
            catch (Exception ex)
            {
                // Gestion des erreurs lors du décryptage de la chaîne
                GestionErreurLog(ex, "Erreur lors du décryptage de la chaîne", false);
                return null;
            }
        }

        // Méthode pour valider les mots de passe
        public static bool ValiderMotdepasse(string motDePasse, string confirmerMotDePasse)
        {
            // Validation des mots de passe (doivent correspondre et être d'une longueur minimale de 8 caractères)
            return motDePasse == confirmerMotDePasse && motDePasse.Length >= 8;
        }

        // Méthode pour créer un super administrateur lors d'une migration
        public static void CreerSuperAdmin(MigrationBuilder oLocal_migrationBuilder)
        {
            // Génération de nouveaux GUID pour l'utilisateur, le dossier et les données de profil
            Guid gLocal_IdUtilisateur = Guid.NewGuid();
            Guid gLocal_IdDossier = Guid.NewGuid();
            Guid gLocal_IdProfilsData = Guid.NewGuid();
            string sLocal_PasswordSuperAdmin = "";

            // Cryptage du mot de passe de l'utilisateur super administrateur
            sLocal_PasswordSuperAdmin = classFonctionGenerale.CrypterChaine(classConstantes.sUtilisateur_Password_Superadmin);

            // Insertion des valeurs par défaut dans les tables Utilisateurs, Dossiers et ProfilsData
            oLocal_migrationBuilder.InsertData("Utilisateurs", new[] { "ID", "Nom", "Login", "Email" }, new object[] { gLocal_IdUtilisateur, classConstantes.sUtilisateur_Nom_Superadmin, classConstantes.sUtilisateur_Login_Superadmin, "" });

            oLocal_migrationBuilder.InsertData("Dossiers", new[] { "ID", "Nom" }, new object[] { gLocal_IdDossier, classConstantes.sTypeprofilConnection_Nom_YnovPassword });

            oLocal_migrationBuilder.InsertData("ProfilsData", new[] { "ID", "DossiersID", "UtilisateursID", "Nom", "URL", "Login", "EncryptedPassword" }, new object[] { gLocal_IdProfilsData, gLocal_IdDossier, gLocal_IdUtilisateur, classConstantes.sProfilConection_Nom_YnovPassword, "", classConstantes.sUtilisateur_Nom_Superadmin, sLocal_PasswordSuperAdmin });
        }
    }
}
