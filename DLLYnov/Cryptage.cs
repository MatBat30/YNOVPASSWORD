using System.Security.Cryptography;
using System.Text;

namespace DllYnov
{
    public class Cryptage
    {
        public const string sGeneral_CleCryptage = "Bk_Yk-#d859U3X=c";

        /// <summary>
        /// Crypte la chaine passée en paramètre avec la méthode AES et retourne la chaine ainsi cryptée
        /// </summary>
        /// <param name="sPar_ChaineACrypter">Chaine à crypter</param>
        /// <returns>Chaine passée en paramètre cryptée, null en cas d'erreur</returns>
        public static string? DLLCrypterChaine(string sPar_ChaineACrypter)
        {
            byte[] aLocal_TableauVecteur = new byte[16]; //Vecteur d'initialisation nécessaire au cryptage
            byte[] aLocal_ResultatCryptage; //Le tableau de byte contenant le résultat du cryptage
            ICryptoTransform oLocal_Encrypteur; // Objet réalisant le cryptage
            string? sLocal_ValeurRetour = null; // Valeur retournée par la fonction, en cas de succès

            // Si la chaine passée en paramètre est vide ou nulle
            if (sPar_ChaineACrypter == null || sPar_ChaineACrypter == "")
            {
              //  GestionErreurLog(null, "La chaine passée en paramètre est vide", false);-----------------------------------------------------------------------------------------------------
            }
            else
            {
                //A l'aide d'un objet AES
                using (Aes oLocal_AES = Aes.Create())
                {
                    //On initialise la clé en l'encodant en byte grâce au UTF8.
                    oLocal_AES.Key = Encoding.UTF8.GetBytes(sGeneral_CleCryptage);
                    //On initialise le vecteur d'initialisation.
                    oLocal_AES.IV = aLocal_TableauVecteur;
                    //On créé un encodeur AES avec la clé et le vecteur d'initialisation en paramètre.
                    oLocal_Encrypteur = oLocal_AES.CreateEncryptor(oLocal_AES.Key, oLocal_AES.IV);

                    // A l'aide de l'objet memory stream
                    using (MemoryStream oLocal_MemoryStream = new MemoryStream())
                    {
                        // A l'aide d'un objet Crypto stream, créé à partir du memory stream et de l'objet d'encryption
                        using (CryptoStream oLocal_CryptoStream = new CryptoStream((Stream)oLocal_MemoryStream, oLocal_Encrypteur, CryptoStreamMode.Write))
                        {
                            // A l'aide d'un objet "streamwriter"
                            using (StreamWriter oLocal_StreamWriter = new StreamWriter((Stream)oLocal_CryptoStream))
                            {
                                // On stocke dans un objet streamwriter la chaine cryptée
                                oLocal_StreamWriter.Write(sPar_ChaineACrypter);
                            }
                            // On récupère, dans un tableau de bytes, la chaine cryptée
                            aLocal_ResultatCryptage = oLocal_MemoryStream.ToArray();
                        }
                    }
                }
                // On converti le tableau de byte en chaine et on retourne cet élément
                sLocal_ValeurRetour = Convert.ToBase64String(aLocal_ResultatCryptage);
            }
            // Si la chaine cryptée est nulle ou vide
            if (sLocal_ValeurRetour == null || sLocal_ValeurRetour == "")
                return null;
            else
                return sLocal_ValeurRetour;
        }


        /// <summary>
        /// Décrypte la chaine passée en paramètre avec la méthode AES et retourne la chaine ainsi décryptée
        /// </summary>
        /// <param name="sPar_ChaineCryptee">Chaine à décrypter</param>
        /// <returns>Chaine passée en paramètre décryptée, null en cas d'erreur</returns>
        public static string? DLLDecrypterChaine(string sPar_ChaineCryptee)
        {

            byte[] aLocal_iv = new byte[16]; // Tableau de bytes servant de vecteur d'initialisation
            byte[] aLocal_Buffer = Convert.FromBase64String(sPar_ChaineCryptee); //Tableau de bytes contenant la chaine à décrypter
            ICryptoTransform oLocal_Decrypteur;
            string? sLocal_ValeurRetour = null; // Valeur retournée par la fonction, en cas de succès

            // Si la chaine passée en paramètre est vide ou nulle
            if (sPar_ChaineCryptee == null || sPar_ChaineCryptee == "")
            {
             //   GestionErreurLog(null, "La chaine passée en paramètre est vide", false);-------------------------------------------------------------------------------------------------------------------
            }
            else
            {
                // A l'aide d'un objet AES
                using (Aes oLocal_AES = Aes.Create())
                {
                    // Définition des paramètres de cryptage AES
                    oLocal_AES.Key = Encoding.UTF8.GetBytes(sGeneral_CleCryptage);
                    oLocal_AES.IV = aLocal_iv;
                    // Création de l'objet "décrypteur" à partir des paramètres définis
                    oLocal_Decrypteur = oLocal_AES.CreateDecryptor(oLocal_AES.Key, oLocal_AES.IV);
                    // A l'aide d'un objet "memory stream"
                    using (MemoryStream oLocal_ms = new MemoryStream(aLocal_Buffer))
                    {
                        // A l'aide d'un objet Crypto stream
                        using (CryptoStream oLocal_cryptoStream = new CryptoStream((Stream)oLocal_ms, oLocal_Decrypteur, CryptoStreamMode.Read))
                        {
                            // A l'aide d'un objet streamreader
                            using (StreamReader oLocal_streamReader = new StreamReader((Stream)oLocal_cryptoStream))
                            {
                                //Récupération de la chaine cryptée
                                sLocal_ValeurRetour = oLocal_streamReader.ReadToEnd();
                            }

                        }
                    }
                }
            }
            // Si la chaine cryptée est nulle ou vide
            if (sLocal_ValeurRetour == null || sLocal_ValeurRetour == "")
                return null;
            else
                return sLocal_ValeurRetour;

        }
    }
}
