using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YnovPassword.general
{
    internal class classConstantes
    {
        //enseble de constante servant notament a l'initialisation de l'admin user de la bdd aisi que dans les fonction de cryptage
        public const string sGeneral_CleCryptage = "Bk_Yk-#d859U3X=c";
        public const string sUtilisateur_Nom_Superadmin = "SUPERADMIN";
        public const string sUtilisateur_Login_Superadmin = "SUPERADMIN";
        public const string sUtilisateur_Password_Superadmin = "YNOV";

        public const string sTypeprofilConnection_Nom_YnovPassword = "YNOVPASSWORD";
        public const string sProfilConection_Nom_YnovPassword = "YNOVPASSWORD";

        public const int iBigNumVersion = 9;
        public const int iSmallNumVersion = 0;
    }
}
